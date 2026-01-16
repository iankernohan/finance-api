using System;

namespace finance_api.Plaid;

public class PlaidTransactionLocation
{
      public string? Address {get; set;}

      public string? City {get; set;}

      public string? Region {get; set;}

      public string? PostalCode {get; set;}

      public string? Country {get; set;}
}
