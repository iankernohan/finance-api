using System;
using finance_api.Data;
using finance_api.Dtos;
using finance_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class CategoryRulesService(AppDbContext context, ICategoryRulesApplier applier) : ICategoryRulesService
{
    private readonly AppDbContext _context = context;
    private readonly ICategoryRulesApplier _applier = applier;

    public async Task<List<CategoryRules>> GetCategoryRules()
    {
        var rules = await _context.CategoryRules
        .OrderBy(x => x.Name)
        .Include(x => x.Category)
        .Include(x => x.SubCategory)
        .ToListAsync();
        return rules;
    }

    public async Task AddCategoryRule(string ruleName, int categoryId, int? subCategoryId, decimal? amount)
    {
        _context.CategoryRules.Add(new CategoryRules { Name = ruleName, Amount = amount, CategoryId = categoryId, SubCategoryId = subCategoryId });
        await _context.SaveChangesAsync();
        await _applier.ApplyCategoryRules();
        return;
    }

    public async Task<CategoryRules> UpdateCategoryRule(UpdateCategoryRuleRequest request)
    {
        if (string.IsNullOrEmpty(request.Name))
        {
            throw new Exception($"Rule Name Must Not Be Empty.");
        }

        var rule = await _context.CategoryRules.FirstOrDefaultAsync(r => r.Id == request.Id) ?? throw new Exception($"No rule found with id: {request.Id}");

        var category = await _context.Category.FirstOrDefaultAsync(c => c.Id == request.CategoryId) ?? throw new Exception($"No category found with id: {request.CategoryId}");

        var subCategory = await _context.SubCategory.FirstOrDefaultAsync(s => s.Id == request.SubCategoryId) ?? throw new Exception($"No Sub Category found with id: {request.SubCategoryId}");

        if (rule.Name != request.Name && !string.IsNullOrEmpty(request.Name))
        {
            rule.Name = request.Name;
        }

        if (rule.CategoryId != request.CategoryId && request.CategoryId is not null)
        {
            rule.CategoryId = request.CategoryId ?? 0;
        }

        if (rule.SubCategoryId != request.SubCategoryId)
        {
            rule.SubCategoryId = request.SubCategoryId;
        }

        await _context.SaveChangesAsync();
        await _applier.ApplyCategoryRules();

        return rule;
    }

    public async Task<CategoryRules> DeleteCategoryRule(int ruleId)
    {
        var rule = await _context.CategoryRules.FirstOrDefaultAsync(r => r.Id == ruleId) ?? throw new Exception($"No Rule Found With Id:  {ruleId}");

        _context.CategoryRules.Remove(rule);

        await _context.SaveChangesAsync();

        return rule;
    }
}
