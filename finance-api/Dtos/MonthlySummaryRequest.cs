using System;

namespace finance_api.Dtos;

public class MonthlySummaryRequest
{
    public int Month { get; set; }
    public int Year { get; set; }
}
