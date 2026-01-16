using System;
using finance_api.Plaid;
using Going.Plaid;

namespace finance_api.Services;

public interface IPlaidService
{
    Task<string> CreateLinkToken(PlaidClient plaid, CreateLinkTokenRequest req);

    Task<Going.Plaid.Item.ItemPublicTokenExchangeResponse> ExchangePublicToken(PlaidClient plaid, ExchangePublicTokenRequest req);

    Task<PlaidItem?> GetPlaidItem(string userId);

    Task<IReadOnlyList<Going.Plaid.Entity.Account>> GetAccounts(string userId, PlaidClient plaidClient, PlaidItem item);

    Task AddPlaidItem(PlaidItem item);

    Task<List<PlaidTransaction>> GetPlaidTransactions(PlaidClient plaid, PlaidItem item, GetPlaidTransactionsRequest req);

    Task<List<PlaidTransaction>> GetUncategorizedTransactions(string userId);

    Task<List<PlaidTransaction>> GetCategorizedTransactions(string userId);

    Task<Dictionary<string, List<PlaidTransaction>>> GetTransactionsByCategory(string userId, List<string>? categoryNames);
}
