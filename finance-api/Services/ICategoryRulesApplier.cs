using finance_api.Plaid;

namespace finance_api.Services;

public interface ICategoryRulesApplier
{
    Task ApplyCategoryRules();

    Task ApplyCategoryRules(List<PlaidTransaction> transactions);
}
