using finance_api.Plaid;

namespace finance_api.Services;

public interface ITransactionsServiceV2
{
    Task<List<PlaidTransaction>> GetTransactions(PlaidItem item, GetPlaidTransactionsRequest req);
    Task<List<PlaidTransaction>> GetUncategorizedTransactions(string userId);
    Task<List<PlaidTransaction>> GetCategorizedTransactions(string userId);
    Task<Dictionary<string, List<PlaidTransaction>>> GetTransactionsByCategory(List<string>? categoryNames);
    Task<PlaidTransaction> UpdateCategory(string transactionId, int categoryId);
}
