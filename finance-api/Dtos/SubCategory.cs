using System;

namespace finance_api.Dtos;

public class SubCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
}
