using System;

namespace finance_api.Models;

public class Budget
{
    public int id { get; set; }
    public int limit { get; set; }
    public int categoryId { get; set; }
    public Category? category { get; set; }
}
