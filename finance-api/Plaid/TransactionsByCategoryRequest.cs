using System;

namespace finance_api.Plaid;

public class TransactionsByCategoryRequest
{
    public required string UserId { get; set; }
    public List<string>? CategoryNames { get; set; } = new();
}
