using System;
using Going.Plaid.Entity;

namespace finance_api.Dtos;

public class TransactionsCountRequest
{
    public required string UserID { get; set; }

    public Filters? Filters { get; set; }
}
