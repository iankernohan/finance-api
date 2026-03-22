using AutoMapper;
using finance_api.Data;
using finance_api.Dtos;
using finance_api.Models;
using finance_api.Plaid;
using Going.Plaid;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace finance_api.Services;

public class TransactionsService(AppDbContext context, PlaidClient plaid, ICategoryRulesApplier applier, EncryptionService encryptionService, IMapper mapper) : ITransactionsService
{
    private readonly AppDbContext _context = context;
    private readonly PlaidClient _plaid = plaid;
    private readonly ICategoryRulesApplier _applier = applier;
    private readonly EncryptionService _encryptionService = encryptionService;
    private readonly IMapper _mapper = mapper;

    public async Task<List<Transaction>> GetTransactions(PlaidItem item, GetTransactionsRequest req, string userId)
    {
        var lastTrans = await _context.Transactions.Where(t => t.UserId == userId).OrderByDescending(x => x.Date).FirstOrDefaultAsync();

        IQueryable<Transaction> query = _context.Transactions.Where(t => t.UserId == userId).Include(x => x.Category).Include(x => x.SubCategory);
        if (lastTrans is null)
        {
            await SeedDb(userId);
        }
        else
        {
            if (DateTime.Now.ToString("MM:dd:yyyy") != lastTrans.Date?.ToString("MM:dd:yyyy") && req.ShouldFetch)
                await SyncTransactions(item, lastTrans);
        }

        if (req.Filters is not null)
        {
            query = ApplyFilters(query, req.Filters);
            return await query.OrderByDescending(x => x.Date).ToListAsync();
        }

        return await _context.Transactions
            .Where(t => t.UserId == userId)
            .Include(x => x.Category)
            .Include(x => x.SubCategory)
            .OrderByDescending(x => x.Date)
            .Skip((req.Page - 1) * req.PageSize + 1)
            .Take(req.PageSize)
            .ToListAsync();
    }

    public async Task<Transaction> UpdateTransaction(UpdateTransactionRequest updatedTransaction)
    {
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == updatedTransaction.Id) ?? throw new Exception($"No transaction found with id {updatedTransaction.Id}");

        if (!string.IsNullOrEmpty(updatedTransaction.Name))
            transaction.Name = updatedTransaction.Name;

        if (!string.IsNullOrEmpty(updatedTransaction.MerchantName))
            transaction.MerchantName = updatedTransaction.MerchantName;

        if (updatedTransaction.Date.HasValue)
            transaction.Date = updatedTransaction.Date;

        if (updatedTransaction.Amount != null && updatedTransaction.Amount > 0)
            transaction.Amount = (decimal)updatedTransaction.Amount;

        if (updatedTransaction.TransactionType is not null)
            transaction.TransactionType = updatedTransaction.TransactionType;

        if (!string.IsNullOrEmpty(updatedTransaction.AccountId))
            transaction.AccountId = updatedTransaction.AccountId;

        if (!string.IsNullOrEmpty(updatedTransaction.CurrencyCode))
            transaction.CurrencyCode = updatedTransaction.CurrencyCode;

        if (updatedTransaction.PlaidCategory is not null)
            transaction.PlaidCategory = updatedTransaction.PlaidCategory;

        if (updatedTransaction.Location is not null)
            transaction.Location = updatedTransaction.Location;

        if (!string.IsNullOrEmpty(updatedTransaction.LogoUrl))
            transaction.LogoUrl = updatedTransaction.LogoUrl;

        if (!string.IsNullOrEmpty(updatedTransaction.MerchantEntityId))
            transaction.MerchantEntityId = updatedTransaction.MerchantEntityId;

        if (!string.IsNullOrEmpty(updatedTransaction.Website))
            transaction.Website = updatedTransaction.Website;

        if (!string.IsNullOrEmpty(updatedTransaction.CategoryIconUrl))
            transaction.CategoryIconUrl = updatedTransaction.CategoryIconUrl;

        if (updatedTransaction.CategoryId.HasValue)
            transaction.CategoryId = updatedTransaction.CategoryId;

        if (updatedTransaction.SubCategoryId.HasValue)
            transaction.SubCategoryId = updatedTransaction.SubCategoryId;

        return transaction;
    }

    public async Task<Transaction> UpdateTransactionAndSave(UpdateTransactionRequest updatedTransaction)
    {
        var transaction = await UpdateTransaction(updatedTransaction);
        await _context.SaveChangesAsync();
        return transaction;
    }

    public async Task<int> GetTransactionsCount(TransactionsCountRequest req, string userId)
    {
        IQueryable<Transaction> query = _context.Transactions.Where(t => t.UserId == userId);
        if (req.Filters is not null)
        {
            query = ApplyFilters(query, req.Filters);
        }
        return await query.CountAsync();
    }

    public async Task<List<Transaction>> GetUncategorizedTransactions(string userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .Where(x => x.CategoryId == null)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        return transactions;
    }

    public async Task<List<Transaction>> GetCategorizedTransactions(string userId)
    {
        var transactions = await _context.Transactions
            .Where(t => t.UserId == userId)
            .Where(x => x.CategoryId != null)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        return transactions;
    }

    public async Task<Dictionary<string, List<Transaction>>> GetTransactionsByCategory(List<string>? categoryNames, string userId)
    {
        List<string> categories;

        if (categoryNames is null || categoryNames.Count == 0)
        {
            categories = await _context.Category.Where(c => c.UserId == userId).Select(c => c.Name).ToListAsync();
        }
        else
        {
            categories = categoryNames;
        }

        Dictionary<string, List<Transaction>> result = new();

        foreach (var category in categories)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Where(t => t.Category != null && t.Category.Name == category)
                .ToListAsync();

            if (transactions.Count > 0)
            {
                result[category] = transactions;
            }
        }

        return result.OrderByDescending(kvp => kvp.Value.Count)
                     .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public async Task<Transaction> UpdateCategory(string transactionId, int categoryId)
    {
        var transaction = await _context.Transactions.FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction == null)
        {
            throw new Exception($"No transaction found with id {transactionId}");
        }

        if (categoryId == -1)
        {
            transaction.CategoryId = null;
        }
        else
        {
            transaction.CategoryId = categoryId;
        }

        await _context.SaveChangesAsync();

        var updated = await _context.Transactions.Include(t => t.Category).FirstAsync(t => t.Id == transaction.Id);
        return updated;
    }

    public async Task<MonthlySummary> GetMonthlySummary(MonthlySummaryRequest req, string userId)
    {
        var monthName = GetMonthName(req.Month);
        var income = await _context.Transactions
            .Where(t => t.UserId == userId)
            .Where(t => t.Date.HasValue && t.Date.Value.Month == req.Month && t.Date.Value.Year == req.Year)
            .Where(t => t.Amount < 0).
            SumAsync(t => t.Amount);
        var expenses = await _context.Transactions
            .Where(t => t.UserId == userId)
            .Where(t => t.Date.HasValue && t.Date.Value.Month == req.Month && t.Date.Value.Year == req.Year)
            .Where(t => t.Amount > 0)
            .SumAsync(t => t.Amount);

        var categories = await _context.Category.Where(t => t.UserId == userId).OrderBy(c => c.Name).Select(c => c.Name).ToListAsync();
        Dictionary<string, List<Transaction>> transactionsByCategory = new();

        foreach (var category in categories)
        {
            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId)
                .Where(t => t.Date.HasValue && t.Date.Value.Month == req.Month && t.Date.Value.Year == req.Year)
                .Where(t => t.Category != null && t.Category.Name == category)
                .Include(t => t.SubCategory)
                .ToListAsync();

            if (transactions.Count > 0)
            {
                transactionsByCategory[category] = transactions;
            }
        }
        transactionsByCategory["Uncategorized"] = await _context.Transactions
            .Where(t => t.UserId == userId)
            .Where(t => t.Date.HasValue && t.Date.Value.Month == req.Month && t.Date.Value.Year == req.Year)
            .Where(t => t.Category == null).ToListAsync();

        return new MonthlySummary
        {
            MonthName = monthName,
            Year = req.Year,
            IncomeTotal = income,
            ExpenseTotal = expenses,
            Categories = transactionsByCategory
        };
    }

    private async Task SeedDb(string userId)
    {
        string json = File.ReadAllText("../finance-api/Plaid/data.json");
        List<Transaction> data = JsonSerializer.Deserialize<List<Transaction>>(json)?.ToList()!;

        foreach (var t in data)
        {
            t.Date = DateTime.SpecifyKind(t.Date ?? DateTime.Now, DateTimeKind.Utc);
            t.Category = null;
            t.SubCategory = null;
            t.UserId = userId;
        }

        await _applier.ApplyCategoryRules(data, userId);
        data.Reverse();
        _context.Transactions.AddRange(data);
        await _context.SaveChangesAsync();
    }

    private async Task SyncTransactions(PlaidItem item, Transaction lastTrans)
    {
        // Determine Time Since Last Transaction
        var dateOfLastTrans = lastTrans.Date;
        var timeSinceLastTrans = DateTime.UtcNow - dateOfLastTrans;
        var numDaysToFetch = (int)Math.Ceiling(timeSinceLastTrans?.TotalDays ?? 30);

        // Fetch transactions from plaid
        var request = new Going.Plaid.Transactions.TransactionsGetRequest
        {
            AccessToken = _encryptionService.Decrypt(item.EncryptedAccessToken),
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(numDaysToFetch * -1)),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow)
        };

        // var response = new Going.Plaid.Transactions.TransactionsGetResponse { Transactions = new List<Going.Plaid.Entity.Transaction> { } };
        var response = await _plaid.TransactionsGetAsync(request);

        var newTransactions = response.Transactions.ToList();

        var alreadyExist = await _context.Transactions
            .Where(t => newTransactions
                .Select(n => n.TransactionId).Contains(t.Id))
            .ToListAsync();

        var alreadyExistIds = alreadyExist.Select(t => t.Id);

        var mapped = MapTransactions(newTransactions, item.UserId);

        // Update transactions that already exist and remove them from 'mapped'
        List<Transaction> mappedCopy = [.. mapped];
        foreach (var transaction in mappedCopy)
        {
            if (alreadyExistIds.Contains(transaction.Id))
            {
                if (JsonSerializer.Serialize(transaction) != JsonSerializer.Serialize(alreadyExist.FirstOrDefault(t => t.Id == transaction.Id)))
                {
                    var req = _mapper.Map<UpdateTransactionRequest>(transaction);
                    await UpdateTransaction(req);
                }
                mapped.RemoveAll(t => t.Id == transaction.Id);
            }
        }

        await _applier.ApplyCategoryRules(mapped, item.UserId);
        mapped.Reverse();
        // Write 'data' to newTransactions.json
        // var json = System.Text.Json.JsonSerializer.Serialize(mapped);
        // var filePath = Path.Combine(Directory.GetCurrentDirectory(), "newTransactions.json");
        // await System.IO.File.WriteAllTextAsync(filePath, json);
        _context.Transactions.AddRange(mapped);
        await _context.SaveChangesAsync();
    }

    private IQueryable<Transaction> ApplyFilters(IQueryable<Transaction> query, Filters filters)
    {
        if (filters.NumberOfMonths is not null && filters.NumberOfMonths.Value > 0)
        {
            var today = DateTime.UtcNow;
            query = query.Where(t => t.Date >= today.AddMonths(filters.NumberOfMonths.Value * -1));
        }
        else
        {
            if (filters.StartDate.HasValue)
                query = query.Where(t => t.Date >= filters.StartDate.Value);

            if (filters.EndDate.HasValue)
                query = query.Where(t => t.Date <= filters.EndDate.Value);
        }
        if (filters.StartDate.HasValue)
            query = query.Where(t => t.Date >= filters.StartDate.Value);

        if (filters.EndDate.HasValue)
            query = query.Where(t => t.Date <= filters.EndDate.Value);

        if (filters.MinAmount.HasValue)
            query = query.Where(t => t.Amount >= filters.MinAmount.Value);

        if (filters.MaxAmount.HasValue)
            query = query.Where(t => t.Amount <= filters.MaxAmount.Value);

        if (!string.IsNullOrEmpty(filters.Description))
            query = query.Where(t => t.Name.ToLower().Contains(filters.Description.ToLower()) || t.MerchantName.ToLower().Contains(filters.Description.ToLower()));

        if (filters.Category?.Length > 0)
            query = query.Where(t => t.Category != null && filters.Category.Contains(t.Category.Name));

        if (filters.TransactionType != "")
            if (filters.TransactionType == "Income")
                query = query.Where(t => t.Category != null && t.Category.TransactionType == Enums.TransactionType.Income);
            else if (filters.TransactionType == "Expense")
                query = query.Where(t => t.Category != null && t.Category.TransactionType == Enums.TransactionType.Expense);
        return query;
    }

    private List<Transaction> MapTransactions(IReadOnlyList<Going.Plaid.Entity.Transaction> transactions, string userId)
    {
        var mapped = transactions.Select(x => new Transaction
        {
            Id = x.TransactionId ?? "",
            UserId = userId,
            AccountId = x.AccountId ?? "",
            Amount = x.Amount ?? 0,
            PlaidCategory = new PlaidTransactionCategory
            {
                ConfidenceLevel = x.PersonalFinanceCategory?.ConfidenceLevel,
                Detailed = x.PersonalFinanceCategory?.Detailed,
                Primary = x.PersonalFinanceCategory?.Primary,
            },
            CategoryIconUrl = x.PersonalFinanceCategoryIconUrl,
            CurrencyCode = x.IsoCurrencyCode,
            Date = DateTime.SpecifyKind(x.Date?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Now, DateTimeKind.Utc),
            Location = new PlaidTransactionLocation
            {
                Address = x.Location?.Address,
                City = x.Location?.City,
                Country = x.Location?.Country,
                PostalCode = x.Location?.PostalCode,
                Region = x.Location?.Region,
            },
            LogoUrl = x.LogoUrl,
            MerchantEntityId = x.MerchantEntityId,
            MerchantName = x.MerchantName ?? "",
            Name = x.Name ?? "",
            TransactionType = x.TransactionType,
            Website = x.Website,
        }).ToList();

        return mapped;
    }

    private string GetMonthName(int month)
    {
        return month switch
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12")
        };
    }
}
