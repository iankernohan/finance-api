using System;
using System.Text.Json.Serialization;

namespace finance_api.Plaid;

public class PlaidTransactionLocation
{
      [JsonPropertyName("address")]
      public string? Address { get; set; }

      [JsonPropertyName("city")]
      public string? City { get; set; }

      [JsonPropertyName("region")]
      public string? Region { get; set; }

      [JsonPropertyName("postalCode")]
      public string? PostalCode { get; set; }

      [JsonPropertyName("country")]
      public string? Country { get; set; }
}
