using System;
using finance_api.Enums;

namespace finance_api.Dtos;

public class Filters
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int? NumberOfMonths { get; set; }
    public string[]? Category { get; set; }
    public string[]? SubCategory { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? TransactionType { get; set; } = "";
    public string? Description { get; set; }
}
