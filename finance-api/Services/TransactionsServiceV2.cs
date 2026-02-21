using finance_api.Data;
using finance_api.Dtos;
using finance_api.Enums;
using finance_api.Models;
using finance_api.Plaid;
using Going.Plaid;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace finance_api.Services;

public class TransactionsServiceV2(AppDbContext context, PlaidClient plaid, ICategoryRulesApplier applier, EncryptionService encryptionService) : ITransactionsServiceV2
{
    private readonly AppDbContext _context = context;
    private readonly PlaidClient _plaid = plaid;
    private readonly ICategoryRulesApplier _applier = applier;
    private readonly EncryptionService _encryptionService = encryptionService;

    public async Task<List<PlaidTransaction>> GetTransactions(PlaidItem item, GetPlaidTransactionsRequest req)
    {
        var lastTrans = await _context.PlaidTransactions.OrderByDescending(x => x.Date).FirstOrDefaultAsync();

        IQueryable<PlaidTransaction> query = _context.PlaidTransactions.Include(x => x.Category).Include(x => x.SubCategory);
        if (lastTrans is null)
        {
            await SeedDb();
        }
        else
        {
            // if (DateTime.Now.ToString("MM:dd:yyyy") != lastTrans.Date?.ToString("MM:dd:yyyy"))
            //     await SyncTransactions(item, lastTrans);
        }

        if (req.Filters is not null)
        {
            query = ApplyFilters(query, req.Filters);
            return await query.OrderByDescending(x => x.Date).ToListAsync();
        }

        return await _context.PlaidTransactions.Include(x => x.Category).Include(x => x.SubCategory).OrderByDescending(x => x.Date).ToListAsync();
    }

    public async Task<List<PlaidTransaction>> GetUncategorizedTransactions(string userId)
    {
        var transactions = await _context.PlaidTransactions
            .Where(x => x.CategoryId == null)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        return transactions;
    }

    public async Task<List<PlaidTransaction>> GetCategorizedTransactions(string userId)
    {
        var transactions = await _context.PlaidTransactions
            .Where(x => x.CategoryId != null)
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        return transactions;
    }

    public async Task<Dictionary<string, List<PlaidTransaction>>> GetTransactionsByCategory(List<string>? categoryNames)
    {
        List<string> categories;

        if (categoryNames is null || categoryNames.Count == 0)
        {
            categories = await _context.Category.Select(c => c.Name).ToListAsync();
        }
        else
        {
            categories = categoryNames;
        }

        Dictionary<string, List<PlaidTransaction>> result = new();

        foreach (var category in categories)
        {
            var transactions = await _context.PlaidTransactions
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

    public async Task<PlaidTransaction> UpdateCategory(string transactionId, int categoryId)
    {
        var transaction = await _context.PlaidTransactions.FirstOrDefaultAsync(t => t.Id == transactionId);

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

        var updated = await _context.PlaidTransactions.Include(t => t.Category).FirstAsync(t => t.Id == transaction.Id);
        return updated;
    }

    public async Task<MonthlySummary> GetMonthlySummary(MonthlySummaryRequest req)
    {
        var monthName = GetMonthName(req.Month);
        var income = await _context.PlaidTransactions
            .Where(t => t.Date.HasValue && t.Date.Value.Month == req.Month && t.Date.Value.Year == req.Year)
            .Where(t => t.Amount < 0).
            SumAsync(t => t.Amount);
        var expenses = await _context.PlaidTransactions
            .Where(t => t.Date.HasValue && t.Date.Value.Month == req.Month && t.Date.Value.Year == req.Year)
            .Where(t => t.Amount > 0)
            .SumAsync(t => t.Amount);

        var categories = await _context.Category.Select(c => c.Name).ToListAsync();
        Dictionary<string, List<PlaidTransaction>> transactionsByCategory = new();

        foreach (var category in categories)
        {
            var transactions = await _context.PlaidTransactions
                .Where(t => t.Date.HasValue && t.Date.Value.Month == req.Month && t.Date.Value.Year == req.Year)
                .Where(t => t.Category != null && t.Category.Name == category)
                .Include(t => t.SubCategory)
                .ToListAsync();

            if (transactions.Count > 0)
            {
                transactionsByCategory[category] = transactions;
            }
        }
        transactionsByCategory["Uncategorized"] = await _context.PlaidTransactions
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

    private async Task SeedDb()
    {
        string json = File.ReadAllText("../finance-api/Plaid/data.json");
        List<Going.Plaid.Entity.Transaction> data = JsonSerializer.Deserialize<IReadOnlyList<Going.Plaid.Entity.Transaction>>(json)?.ToList()!;

        var mapped = MapPlaidTransactions(data);
        await _applier.ApplyCategoryRules(mapped);
        mapped.Reverse();
        _context.PlaidTransactions.AddRange(mapped);
        await _context.SaveChangesAsync();
    }

    private async Task SyncTransactions(PlaidItem item, PlaidTransaction lastTrans)
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

        // Filter only new transactions
        var newTransactions = response.Transactions.ToList();
        var lastTransIdx = newTransactions.FindIndex(x => x.Date?.ToDateTime(TimeOnly.MinValue) == lastTrans.Date && x.Amount == lastTrans.Amount && x.Name == lastTrans.Name && x.TransactionType == lastTrans.TransactionType);
        newTransactions = newTransactions[0..lastTransIdx];

        // Map and update category for new transactions - add to database
        var mapped = MapPlaidTransactions(newTransactions);
        await _applier.ApplyCategoryRules(mapped);
        mapped.Reverse();
        _context.PlaidTransactions.AddRange(mapped);
        await _context.SaveChangesAsync();
    }

    private IQueryable<PlaidTransaction> ApplyFilters(IQueryable<PlaidTransaction> query, Filters filters)
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
            query = query.Where(t => t.Name.Contains(filters.Description) || t.MerchantName.Contains(filters.Description));

        if (filters.Category?.Length > 0)
            query = query.Where(t => t.Category != null && filters.Category.Contains(t.Category.Name));

        if (filters.TransactionType != "")
            if (filters.TransactionType == "Income")
                query = query.Where(t => t.Category != null && t.Category.TransactionType == Enums.TransactionType.Income);
            else if (filters.TransactionType == "Expense")
                query = query.Where(t => t.Category != null && t.Category.TransactionType == Enums.TransactionType.Expense);
        return query;
    }

    private List<PlaidTransaction> MapPlaidTransactions(IReadOnlyList<Going.Plaid.Entity.Transaction> transactions)
    {
        var mapped = transactions.Select(x => new PlaidTransaction
        {
            Id = x.TransactionId ?? "",
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
            Date = x.Date?.ToDateTime(TimeOnly.MinValue),
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
