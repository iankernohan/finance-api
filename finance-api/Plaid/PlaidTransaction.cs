using System;
using finance_api.Models;

namespace finance_api.Plaid;

public class PlaidTransaction
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public string? CurrencyCode = "";

    public string MerchantName { get; set; } = "";
    
    public string AccountId { get; set; } = "";

    public decimal Amount { get; set; }

    public DateOnly? Date { get; set; }

    public PlaidTransactionLocation? Location { get; set; }

    public Going.Plaid.Entity.TransactionTransactionTypeEnum? TransactionType { get; set; }

    public string? LogoUrl { get; set; }

    public string? Website { get; set; }

    public PlaidTransactionCategory? PlaidCategory { get; set; }

    public string? CategoryIconUrl { get; set; }

    public string? MerchantEntityId { get; set; }

    public int? CategoryId { get; set; }

    public Category? Category{ get; set; }
}
