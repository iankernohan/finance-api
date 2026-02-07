using finance_api.Data;
using finance_api.Plaid;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class CategoryRulesApplier(AppDbContext context) : ICategoryRulesApplier
{
    private readonly AppDbContext _context = context;

    public async Task ApplyCategoryRules()
    {
        var plaidTransactions = await _context.PlaidTransactions.ToListAsync();
        await ApplyCategoryRules(plaidTransactions);
        await _context.SaveChangesAsync();
    }

    public async Task ApplyCategoryRules(List<PlaidTransaction> transactions)
    {
        var categoryRules = await _context.CategoryRules.ToListAsync();
        foreach (var t in transactions)
        {
            var name = String.IsNullOrEmpty(t.MerchantName) ? t.Name : t.MerchantName;
            var rule = categoryRules.FirstOrDefault(r => name.Contains(r.Name));
            if (rule is not null)
            {
                t.CategoryId = rule.CategoryId;
                t.SubCategoryId = rule.SubCategoryId;
            }
        }
    }
}
