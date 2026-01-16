using System;

namespace finance_api.Plaid;

public class PlaidTransactionCategory
{
    public string? Primary { get; set; }

    public string? Detailed { get; set; }

    public string? ConfidenceLevel { get; set; }
}
