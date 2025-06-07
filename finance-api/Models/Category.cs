using System;
using finance_api.Enums;

namespace finance_api.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<SubCategory> SubCategories { get; set; } = [];
    public TransactionType transactionType { get; set; }
}
