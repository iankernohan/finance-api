using finance_api.Models;

namespace finance_api.Services;

public interface ICategoryRulesApplier
{
    Task ApplyCategoryRules(string userId);

    Task ApplyCategoryRules(List<Transaction> transactions, string userId);
}
