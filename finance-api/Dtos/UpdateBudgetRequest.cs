using System;

namespace finance_api.Dtos;

public class UpdateBudgetRequest
{
    public int id { get; set; }
    public int limit { get; set; }
}