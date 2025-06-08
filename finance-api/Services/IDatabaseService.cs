using System;

namespace finance_api.Services;

public interface IDatabaseService
{
    Task ResetDatabaseAsync();
}
