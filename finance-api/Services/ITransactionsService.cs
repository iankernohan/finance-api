using finance_api.Dtos;
using finance_api.Plaid;
using finance_api.Models;

namespace finance_api.Services;

public interface ITransactionsService
{
    Task<List<Transaction>> GetTransactions(PlaidItem item, GetTransactionsRequest req, string userId);
    Task<Transaction> UpdateTransactionAndSave(UpdateTransactionRequest updatedTransaction);
    Task<int> GetTransactionsCount(TransactionsCountRequest req, string userId);
    Task<List<Transaction>> GetUncategorizedTransactions(string userId);
    Task<List<Transaction>> GetCategorizedTransactions(string userId);
    Task<Dictionary<string, List<Transaction>>> GetTransactionsByCategory(List<string>? categoryNames, string userId);
    Task<Transaction> UpdateCategory(string transactionId, int categoryId);
    Task<MonthlySummary> GetMonthlySummary(MonthlySummaryRequest req, string userId);
}
