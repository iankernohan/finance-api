using System;
using finance_api.Data;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class DatabaseService : IDatabaseService
{
    private readonly AppDbContext _context;
    public DatabaseService(AppDbContext context)
    {
        _context = context;
    }
    public async Task ResetDatabaseAsync()
    {
        await _context.SubCategory.ExecuteDeleteAsync();
        await _context.Category.ExecuteDeleteAsync();
        await _context.Transactions.ExecuteDeleteAsync();
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Transactions', RESEED, 0)");
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Category', RESEED, 0)");
        await _context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('SubCategory', RESEED, 0)");
    }
}
