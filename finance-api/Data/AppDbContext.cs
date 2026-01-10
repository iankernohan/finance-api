using System;
using finance_api.Enums;
using finance_api.Models;
using finance_api.Plaid;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<SubCategory> SubCategory { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<RecurringTransactions> RecurringTransactions { get; set; }
    public DbSet<PlaidItem> PlaidItems => Set<PlaidItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
        .HasData(
            new Category { Id = 1, Name = "Bills", TransactionType = TransactionType.Expense },
            new Category { Id = 2, Name = "Transport", TransactionType = TransactionType.Expense },
            new Category { Id = 3, Name = "Pleasure", TransactionType = TransactionType.Expense },
            new Category { Id = 4, Name = "Food", TransactionType = TransactionType.Expense },
            new Category { Id = 5, Name = "Shopping", TransactionType = TransactionType.Expense },
            new Category { Id = 6, Name = "Salary", TransactionType = TransactionType.Income },
            new Category { Id = 7, Name = "Investment", TransactionType = TransactionType.Income },
            new Category { Id = 8, Name = "Pets", TransactionType = TransactionType.Expense }
        );

        modelBuilder.Entity<SubCategory>()
        .HasData(
            new SubCategory { Id = 1, Name = "Electricity", CategoryId = 1 },
            new SubCategory { Id = 2, Name = "Water", CategoryId = 1 },
            new SubCategory { Id = 3, Name = "Gas", CategoryId = 1 },
            new SubCategory { Id = 4, Name = "Public Transport", CategoryId = 2 },
            new SubCategory { Id = 5, Name = "Uber", CategoryId = 2 },
            new SubCategory { Id = 6, Name = "Gas", CategoryId = 2 },
            new SubCategory { Id = 7, Name = "Cinema", CategoryId = 3 },
            new SubCategory { Id = 8, Name = "Golf", CategoryId = 3 },
            new SubCategory { Id = 9, Name = "Skateboard", CategoryId = 3 },
            new SubCategory { Id = 10, Name = "Soccer", CategoryId = 3 },
            new SubCategory { Id = 11, Name = "Computer", CategoryId = 3 },
            new SubCategory { Id = 12, Name = "Concerts", CategoryId = 3 },
            new SubCategory { Id = 13, Name = "Groceries", CategoryId = 4 },
            new SubCategory { Id = 14, Name = "Dining Out", CategoryId = 4 },
            new SubCategory { Id = 15, Name = "Clothes", CategoryId = 5 },
            new SubCategory { Id = 16, Name = "Stocks", CategoryId = 7 },
            new SubCategory { Id = 17, Name = "Interest", CategoryId = 7 },
            new SubCategory { Id = 18, Name = "Food", CategoryId = 8 },
            new SubCategory { Id = 19, Name = "Toys", CategoryId = 8 },
            new SubCategory { Id = 20, Name = "Litter", CategoryId = 8 },
            new SubCategory { Id = 21, Name = "Vet", CategoryId = 8 }
        );
    }
}
