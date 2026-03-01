using finance_api.Dtos;
using finance_api.Plaid;
using finance_api.Models;

namespace finance_api.Services;

public interface ITransactionsService
{
    Task<List<Transaction>> GetTransactions(PlaidItem item, GetTransactionsRequest req);
    Task<int> GetTransactionsCount(TransactionsCountRequest req);
    Task<List<Transaction>> GetUncategorizedTransactions(string userId);
    Task<List<Transaction>> GetCategorizedTransactions(string userId);
    Task<Dictionary<string, List<Transaction>>> GetTransactionsByCategory(List<string>? categoryNames);
    Task<Transaction> UpdateCategory(string transactionId, int categoryId);
    Task<MonthlySummary> GetMonthlySummary(MonthlySummaryRequest req);
}
