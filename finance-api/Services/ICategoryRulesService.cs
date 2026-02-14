using System;
using finance_api.Dtos;
using finance_api.Models;

namespace finance_api.Services;

public interface ICategoryRulesService
{
    Task<List<CategoryRules>> GetCategoryRules();
    Task AddCategoryRule(string ruleName, int categoryId, int? subCategoryId, decimal? amount);
    Task<CategoryRules> UpdateCategoryRule(UpdateCategoryRuleRequest request);
    Task<CategoryRules> DeleteCategoryRule(int ruleId);
}
