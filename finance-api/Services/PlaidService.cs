using System;
using System.Text.Json;
using finance_api.Data;
using finance_api.Plaid;
using Going.Plaid;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace finance_api.Services;

public class PlaidService : IPlaidService
{
    private readonly AppDbContext _context;

    public PlaidService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string> CreateLinkToken(PlaidClient plaid, CreateLinkTokenRequest req)
    {
        var request = new Going.Plaid.Link.LinkTokenCreateRequest
        {
            ClientName = "Your App",
            Language = Going.Plaid.Entity.Language.English,
            CountryCodes = new[] { Going.Plaid.Entity.CountryCode.Us },
            User = new() { ClientUserId = req.UserId },
            Products = new[] { Going.Plaid.Entity.Products.Transactions }
        };

        var response = await plaid.LinkTokenCreateAsync(request);

        return response.LinkToken;
    }

    public async Task<Going.Plaid.Item.ItemPublicTokenExchangeResponse> ExchangePublicToken(PlaidClient plaid, ExchangePublicTokenRequest req)
    {
        var exchange = await plaid.ItemPublicTokenExchangeAsync(
        new Going.Plaid.Item.ItemPublicTokenExchangeRequest
        {
            PublicToken = req.PublicToken
        });

        return exchange;
    }

    public async Task AddPlaidItem(PlaidItem item)
    {
        _context.PlaidItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Going.Plaid.Entity.Account>> GetAccounts(string userId, PlaidClient plaid, PlaidItem item)
    {
        var response = await plaid.AccountsGetAsync(
        new Going.Plaid.Accounts.AccountsGetRequest
        {
            AccessToken = item.AccessToken
        });

        return response.Accounts;
    }

    public async Task<PlaidItem?> GetPlaidItem(string userId)
    {
        var item = await _context.PlaidItems
            .Where(x => x.UserId == userId)
            .FirstOrDefaultAsync();

        return item;
    }

    public async Task<List<PlaidTransaction>> GetPlaidTransactions(PlaidClient plaid, PlaidItem item, GetPlaidTransactionsRequest req)
    {
        var numberOfTransactions = req.Page * req.PageSize;

        var lastTrans = await _context.PlaidTransactions.OrderByDescending(x => x.Date).FirstOrDefaultAsync();

        if (lastTrans is null)
        {
            await SeedDb();
        }
        // else
        // {
        //     // Determine Time Since Last Transaction
        //     var dateOfLastTrans = lastTrans.Date;
        //     var timeSinceLastTrans = DateTime.UtcNow - dateOfLastTrans?.ToDateTime(TimeOnly.MinValue);
        //     var numMonthsToFetch = (int)Math.Ceiling(timeSinceLastTrans?.TotalDays ?? 3);

        //     // Fetch transactions from plaid
        //     var request = new Going.Plaid.Transactions.TransactionsGetRequest
        //     {
        //         AccessToken = item.AccessToken,
        //         StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(numMonthsToFetch * -1)),
        //         EndDate = DateOnly.FromDateTime(DateTime.UtcNow)
        //     };

        //     var response = await plaid.TransactionsGetAsync(request);

        //     // Filter only new transactions
        //     var newTransactions = response.Transactions.ToList();
        //     var lastTransIdx = newTransactions.FindIndex(x => x.TransactionId == lastTrans.Id);
        //     newTransactions = newTransactions[0..lastTransIdx];

        //     // Map and update category for new transactions - add to database
        //     var mapped = MapPlaidTransactions(newTransactions);
        //     await UpdateCategory(mapped);
        //     mapped.Reverse();
        //     _context.PlaidTransactions.AddRange(mapped);
        //     await _context.SaveChangesAsync();
        // }

        var transactions = await _context.PlaidTransactions.Include(x => x.Category).OrderByDescending(x => x.Date).ToListAsync();
        return transactions;
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

    private async Task SeedDb()
    {
        string json = File.ReadAllText("../finance-api/Plaid/data.json");
        List<Going.Plaid.Entity.Transaction> data = JsonSerializer.Deserialize<IReadOnlyList<Going.Plaid.Entity.Transaction>>(json)?.ToList()!;

        var mapped = MapPlaidTransactions(data);
        await UpdateCategory(mapped);
        mapped.Reverse();
        _context.PlaidTransactions.AddRange(mapped);
        await _context.SaveChangesAsync();
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
            Date = x.Date,
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

    private async Task UpdateCategory(List<PlaidTransaction> transactions)
    {
        var categoryRules = await _context.CategoryRules.ToListAsync();
        foreach (var t in transactions)
        {
            var name = String.IsNullOrEmpty(t.MerchantName

            ) ? t.Name : t.MerchantName;
            var rule = categoryRules.FirstOrDefault(x => x.Name == name);
            if (rule is not null)
            {
                t.CategoryId = rule.CategoryId;
            }
        }
    }
}