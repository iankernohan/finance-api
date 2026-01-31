using System;

namespace finance_api.Models;

public class CategoryRules
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }
}
