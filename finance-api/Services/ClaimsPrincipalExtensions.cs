using System.Security.Claims;
using System.Text.Json;
using finance_api.Dtos;

namespace finance_api.Services;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        var raw = user.Claims.FirstOrDefault(c => c.Type == "user_metadata")?.Value 
            ?? throw new Exception("No User Metadata Found.");
        
        var metadata = JsonSerializer.Deserialize<UserMetadata>(raw) 
            ?? throw new Exception("Failed to deserialize user metadata.");
        
        return metadata.Sub ?? throw new Exception("No userId (sub) found in user metadata.");
    }
}
