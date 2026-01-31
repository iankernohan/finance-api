using System;

namespace finance_api.Dtos;

public class CategoryRuleRequest
{
    public required string Name { get; set; }

    public required int CategoryId { get; set; }
}
