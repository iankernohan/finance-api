using System;
using finance_api.Dtos;

namespace finance_api.Services;

public interface IBudgetService
{
    Task AddBudget(BudgetDtoRequest budget);
    Task<List<BudgetDtoResponse>> GetAllBudgets();
}
