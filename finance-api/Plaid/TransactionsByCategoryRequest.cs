using System;

namespace finance_api.Plaid;

public class TransactionsByCategoryRequest
{
    public List<string>? CategoryNames { get; set; } = new();
}
