using System;

namespace finance_api.Plaid;

public class PlaidItem
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public string ItemId { get; set; } = default!;
    public string AccessToken { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}