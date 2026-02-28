using System;

namespace finance_api.Dtos;

public class TransactionsByCategoryRequest
{
    public List<string>? CategoryNames { get; set; } = new();
}
