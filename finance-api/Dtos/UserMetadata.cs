using System.Text.Json.Serialization;

namespace finance_api.Dtos;

public class UserMetadata
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("phone_verified")]
    public bool PhoneVerified { get; set; }

    [JsonPropertyName("sub")]
    public string Sub { get; set; } = "";
}
