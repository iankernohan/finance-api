using System;

namespace finance_api.Dtos;

public class BudgetDtoRequest
{
    public int limit { get; set; }
    public int categoryId { get; set; }
}
