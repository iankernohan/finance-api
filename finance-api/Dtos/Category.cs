using System;
using finance_api.Models;

namespace finance_api.Dtos;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    public string TransactionType { get; set; } = string.Empty;
}
