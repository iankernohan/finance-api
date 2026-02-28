using System;

namespace finance_api.Dtos;

public record ExchangePublicTokenRequest(string PublicToken, string UserId);

public record CreateLinkTokenRequest(string UserId);

