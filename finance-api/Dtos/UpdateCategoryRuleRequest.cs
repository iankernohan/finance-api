using System;

namespace finance_api.Dtos;

public class UpdateCategoryRuleRequest
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required int CategoryId { get; set; }
}
