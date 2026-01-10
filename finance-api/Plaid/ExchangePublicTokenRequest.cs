using System;

namespace finance_api.Plaid;

public record ExchangePublicTokenRequest(string PublicToken, string UserId);

public record CreateLinkTokenRequest(string UserId);

