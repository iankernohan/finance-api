using System;

namespace finance_api.Dtos;

public class CategoryDtoRequest
{
    public string Name { get; set; } = string.Empty;
    public string transactionType { get; set; } = string.Empty;
}
