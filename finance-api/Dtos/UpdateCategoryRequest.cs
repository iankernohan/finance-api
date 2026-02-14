using System;
using finance_api.Enums;

namespace finance_api.Dtos;

public class UpdateCategoryRequest
{
    public required int Id { get; set; }
    public string? Name { get; set; } = string.Empty;
    public decimal? BudgetLimit { get; set; }
    public TransactionType? TransactionType { get; set; }
}
