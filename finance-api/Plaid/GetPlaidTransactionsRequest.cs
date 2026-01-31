using System;
using finance_api.Dtos;

namespace finance_api.Plaid;

public class GetPlaidTransactionsRequest
{
    public required string UserId { get; set; }

    public int? PageSize { get; set; } = 15;

    public int? Page { get; set; } = 1;

    public Filters? Filters { get; set; }
}
