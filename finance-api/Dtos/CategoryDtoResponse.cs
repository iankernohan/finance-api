using System;
using finance_api.Models;

namespace finance_api.Dtos;

public class CategoryDtoResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal? BudgetLimit { get; set; }
    public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    public string TransactionType { get; set; } = string.Empty;
}
