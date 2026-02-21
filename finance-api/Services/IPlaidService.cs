using System;
using finance_api.Plaid;
using Going.Plaid;

namespace finance_api.Services;

public interface IPlaidService
{
    Task<string> CreateLinkToken(CreateLinkTokenRequest req);

    Task<PlaidItem> ExchangePublicToken(ExchangePublicTokenRequest req);

    Task<PlaidItem?> GetPlaidItem(string userId);

    Task<IReadOnlyList<Going.Plaid.Entity.Account>> GetAccounts(string userId, PlaidItem item);

    Task AddPlaidItem(PlaidItem item);
}
