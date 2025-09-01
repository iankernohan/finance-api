using System;

namespace finance_api.Dtos;

public class RecurringTransactionUpdateRequest
{
    public double? Amount { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Description { get; set; }
    public int? IntervalDays { get; set; }
}
