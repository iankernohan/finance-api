using System;
using finance_api.Dtos;
using finance_api.Models;

namespace finance_api.Services;

public interface ICategoryRulesService
{
    Task<List<CategoryRules>> GetCategoryRules(string userId);
    Task AddCategoryRule(string ruleName, int categoryId, int? subCategoryId, decimal? amount, string userId);
    Task<CategoryRules> UpdateCategoryRule(UpdateCategoryRuleRequest request, string userId);
    Task<CategoryRules> DeleteCategoryRule(int ruleId);
}
