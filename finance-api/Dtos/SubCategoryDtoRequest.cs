using System;

namespace finance_api.Dtos;

public class SubCategoryDtoRequest
{
    public string Name { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
}
