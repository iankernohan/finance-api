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
    private readonly PlaidClient _plaid;

    public PlaidService(AppDbContext context, PlaidClient plaid)
    {
        _context = context;
        _plaid = plaid;
    }

    public async Task<string> CreateLinkToken(CreateLinkTokenRequest req)
    {
        var request = new Going.Plaid.Link.LinkTokenCreateRequest
        {
            ClientName = "Your App",
            Language = Going.Plaid.Entity.Language.English,
            CountryCodes = new[] { Going.Plaid.Entity.CountryCode.Us },
            User = new() { ClientUserId = req.UserId },
            Products = new[] { Going.Plaid.Entity.Products.Transactions }
        };

        var response = await _plaid.LinkTokenCreateAsync(request);

        return response.LinkToken;
    }

    public async Task<Going.Plaid.Item.ItemPublicTokenExchangeResponse> ExchangePublicToken(ExchangePublicTokenRequest req)
    {
        var exchange = await _plaid.ItemPublicTokenExchangeAsync(
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

    public async Task<IReadOnlyList<Going.Plaid.Entity.Account>> GetAccounts(string userId, PlaidItem item)
    {
        var response = await _plaid.AccountsGetAsync(
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
}