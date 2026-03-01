using finance_api.Data;
using finance_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class CategoryRulesApplier(AppDbContext context) : ICategoryRulesApplier
{
    private readonly AppDbContext _context = context;

    public async Task ApplyCategoryRules()
    {
        var plaidTransactions = await _context.Transactions.ToListAsync();
        await ApplyCategoryRules(plaidTransactions);
        await _context.SaveChangesAsync();
    }

    public async Task ApplyCategoryRules(List<Transaction> transactions)
    {
        var categoryRules = await _context.CategoryRules.ToListAsync();
        foreach (var t in transactions)
        {
            var name = String.IsNullOrEmpty(t.MerchantName) ? t.Name : t.MerchantName;
            var rule = categoryRules.FirstOrDefault(r => name.ToLower().Contains(r.Name.ToLower()) && r.Amount is null) ??
            categoryRules.FirstOrDefault(r => name.ToLower().Contains(r.Name.ToLower()) && r.Amount == t.Amount);
            if (rule is not null)
            {
                t.CategoryId = rule.CategoryId;
                t.SubCategoryId = rule.SubCategoryId;
            }
        }
    }
}
