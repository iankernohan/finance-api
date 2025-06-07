using System;

namespace finance_api.Dtos;

public class CategoryDtoRequest
{
    public string Name { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
}
