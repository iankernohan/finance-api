using System;
using finance_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<SubCategory> SubCategory { get; set; }
}
