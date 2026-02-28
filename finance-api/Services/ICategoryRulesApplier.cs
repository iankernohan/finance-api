using finance_api.Models;

namespace finance_api.Services;

public interface ICategoryRulesApplier
{
    Task ApplyCategoryRules();

    Task ApplyCategoryRules(List<Transaction> transactions);
}
