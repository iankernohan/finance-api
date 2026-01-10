using System;
using System.Text.Json;
using finance_api.Data;
using finance_api.Plaid;
using Going.Plaid;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IReadOnlyList<Going.Plaid.Entity.Transaction>?> GetPlaidTransactions(PlaidClient plaid, PlaidItem item)
    {
        string json = File.ReadAllText("..\\finance-api\\Plaid\\data.json");

        IReadOnlyList<Going.Plaid.Entity.Transaction>? data = JsonSerializer.Deserialize<IReadOnlyList<Going.Plaid.Entity.Transaction>>(json);

        return data;

        // var request = new Going.Plaid.Transactions.TransactionsGetRequest
        // {
        //     AccessToken = item.AccessToken,
        //     StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-3)),
        //     EndDate = DateOnly.FromDateTime(DateTime.UtcNow)
        // };

        // var response = await plaid.TransactionsGetAsync(request);

        // return response.Transactions;
    }
}
