using System;
using System.Text.Json.Serialization;
using finance_api.Plaid;

namespace finance_api.Models;

public class Transaction
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("userId")]
    public string UserId { get; set; } = "";

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("currencyCode")]
    public string? CurrencyCode { get; set; } = "";

    [JsonPropertyName("merchantName")]
    public string MerchantName { get; set; } = "";

    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = "";

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("date")]
    public DateTime? Date { get; set; }

    [JsonPropertyName("location")]
    public PlaidTransactionLocation? Location { get; set; }

    [JsonPropertyName("transactionType")]
    public Going.Plaid.Entity.TransactionTransactionTypeEnum? TransactionType { get; set; }

    [JsonPropertyName("logoUrl")]
    public string? LogoUrl { get; set; }

    [JsonPropertyName("website")]
    public string? Website { get; set; }

    [JsonPropertyName("plaidCategory")]
    public PlaidTransactionCategory? PlaidCategory { get; set; }

    [JsonPropertyName("categoryIconUrl")]
    public string? CategoryIconUrl { get; set; }

    [JsonPropertyName("merchantEntityId")]
    public string? MerchantEntityId { get; set; }

    [JsonPropertyName("categoryId")]
    public int? CategoryId { get; set; } = null;

    [JsonPropertyName("category")]
    public Category? Category { get; set; } = null;

    [JsonPropertyName("subCategoryId")]
    public int? SubCategoryId { get; set; } = null;

    [JsonPropertyName("subCategory")]
    public SubCategory? SubCategory { get; set; } = null;
}
