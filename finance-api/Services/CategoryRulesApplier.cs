using finance_api.Data;
using finance_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class CategoryRulesApplier(AppDbContext context) : ICategoryRulesApplier
{
    private readonly AppDbContext _context = context;

    public async Task ApplyCategoryRules(string userId)
    {
        var userRules = await _context.CategoryRules
            .Where(r => r.UserId == userId)
            .ToListAsync();

        const int batchSize = 100;
        int skip = 0;

        while (true)
        {
            var batch = await _context.Transactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Skip(skip)
                .Take(batchSize)
                .ToListAsync();

            if (batch.Count == 0) break;

            foreach (var t in batch)
            {
                var name = String.IsNullOrEmpty(t.MerchantName) ? t.Name : t.MerchantName;
                var rule = userRules.FirstOrDefault(r => name.ToLower().Contains(r.Name.ToLower()) && r.Amount == t.Amount) ??
                           userRules.FirstOrDefault(r => name.ToLower().Contains(r.Name.ToLower()) && r.Amount is null);

                if (rule is not null)
                {
                    t.CategoryId = rule.CategoryId;
                    t.SubCategoryId = rule.SubCategoryId;
                }
            }

            await _context.SaveChangesAsync();
            skip += batchSize;
        }
    }

    public async Task ApplyCategoryRules(List<Transaction> transactions, string userId)
    {
        var userRules = await _context.CategoryRules.Where(r => r.UserId == userId).ToListAsync();
        foreach (var t in transactions)
        {
            var name = String.IsNullOrEmpty(t.MerchantName) ? t.Name : t.MerchantName;
            var rule = userRules.FirstOrDefault(r => name.ToLower().Contains(r.Name.ToLower()) && r.Amount == t.Amount) ??
                       userRules.FirstOrDefault(r => name.ToLower().Contains(r.Name.ToLower()) && r.Amount is null);

            if (rule is not null)
            {
                t.CategoryId = rule.CategoryId;
                t.SubCategoryId = rule.SubCategoryId;
            }
        }
    }
}
