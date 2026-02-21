using System.Text.Json.Serialization;

namespace finance_api.Plaid;

public class PlaidItem
{
    public int Id { get; set; }

    public string UserId { get; set; } = default!;

    public string ItemId { get; set; } = default!;

    [JsonIgnore]
    public string EncryptedAccessToken { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}