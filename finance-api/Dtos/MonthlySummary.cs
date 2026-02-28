using System;
using finance_api.Models;

namespace finance_api.Dtos;

public class MonthlySummary
{
    public required string MonthName { get; set; }
    public int Year { get; set; }
    public decimal IncomeTotal { get; set; }
    public decimal ExpenseTotal { get; set; }
    public Dictionary<string, List<Transaction>> Categories { get; set; } = [];
}
