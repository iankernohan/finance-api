using System;
using finance_api.Plaid;
using Going.Plaid;

namespace finance_api.Services;

public interface IPlaidService
{
    Task<string> CreateLinkToken(CreateLinkTokenRequest req);

    Task<Going.Plaid.Item.ItemPublicTokenExchangeResponse> ExchangePublicToken(ExchangePublicTokenRequest req);

    Task<PlaidItem?> GetPlaidItem(string userId);

    Task<IReadOnlyList<Going.Plaid.Entity.Account>> GetAccounts(string userId, PlaidItem item);

    Task AddPlaidItem(PlaidItem item);

    Task<List<PlaidTransaction>> GetPlaidTransactions(PlaidItem item, GetPlaidTransactionsRequest req);

    Task<List<PlaidTransaction>> GetUncategorizedTransactions(string userId);

    Task<List<PlaidTransaction>> GetCategorizedTransactions(string userId);

    Task<Dictionary<string, List<PlaidTransaction>>> GetTransactionsByCategory(List<string>? categoryNames);
}
