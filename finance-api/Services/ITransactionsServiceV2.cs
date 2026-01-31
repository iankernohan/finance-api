using finance_api.Dtos;
using finance_api.Models;
using finance_api.Plaid;

namespace finance_api.Services;

public interface ITransactionsServiceV2
{
    Task<List<PlaidTransaction>> GetTransactions(PlaidItem item, GetPlaidTransactionsRequest req);
    Task<List<PlaidTransaction>> GetUncategorizedTransactions(string userId);
    Task<List<PlaidTransaction>> GetCategorizedTransactions(string userId);
    Task<Dictionary<string, List<PlaidTransaction>>> GetTransactionsByCategory(List<string>? categoryNames);
    Task<List<CategoryRules>> GetCategoryRules();
    Task AddCategoryRule(string ruleName, int categoryId);
    Task<CategoryRules> UpdateCategoryRule(UpdateCategoryRuleRequest request);
    Task<CategoryRules> DeleteCategoryRule(int ruleId);
    Task<PlaidTransaction> UpdateCategory(string transactionId, int categoryId);
}
