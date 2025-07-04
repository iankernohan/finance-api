using System;
using finance_api.Models;

namespace finance_api.Dtos;

public class BudgetDtoResponse
{
    public int id { get; set; }
    public int limit { get; set; }
    public Category category { get; set; }
}
