using System;

namespace finance_api.Dtos;

public class UpdateSubCategoryRequest
{
    public required int Id { get; set; }
    public string? Name { get; set; }
    public int? CategoryId { get; set; }
}
