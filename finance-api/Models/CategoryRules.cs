using System;

namespace finance_api.Models;

public class CategoryRules
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";

    public required string Name { get; set; }

    public decimal? Amount { get; set; }

    public int CategoryId { get; set; }

    public Category? Category { get; set; }

    public int? SubCategoryId { get; set; }

    public SubCategory? SubCategory { get; set; }
}
