using System;
using Going.Plaid.Entity;

namespace finance_api.Dtos;

public class TransactionsCountRequest
{
    public Filters? Filters { get; set; }
}
