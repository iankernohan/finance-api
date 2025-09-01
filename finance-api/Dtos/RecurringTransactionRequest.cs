using System;

namespace finance_api.Dtos;

public class RecurringTransactionRequest
{
    public double Amount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? DateCreated { get; set; }
    public string? Description { get; set; }
    public int IntervalDays { get; set; }
    public int CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
}
