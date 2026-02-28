using System;
using finance_api.Enums;
using finance_api.Models;
using finance_api.Plaid;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Category> Category { get; set; }
    public DbSet<SubCategory> SubCategory { get; set; }
    public DbSet<PlaidItem> PlaidItems => Set<PlaidItem>();
    public DbSet<CategoryRules> CategoryRules => Set<CategoryRules>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Transaction>().OwnsOne(p => p.PlaidCategory);
        modelBuilder.Entity<Transaction>().OwnsOne(p => p.Location);

        modelBuilder.Entity<Category>()
        .HasData(
            new Category { Id = 1, Name = "Bills", TransactionType = TransactionType.Expense },
            new Category { Id = 2, Name = "Transport", TransactionType = TransactionType.Expense },
            new Category { Id = 3, Name = "Pleasure", TransactionType = TransactionType.Expense },
            new Category { Id = 4, Name = "Food", TransactionType = TransactionType.Expense },
            new Category { Id = 5, Name = "Shopping", TransactionType = TransactionType.Expense },
            new Category { Id = 6, Name = "Salary", TransactionType = TransactionType.Income },
            new Category { Id = 7, Name = "Investment", TransactionType = TransactionType.Income },
            new Category { Id = 8, Name = "Pets", TransactionType = TransactionType.Expense },
            new Category { Id = 9, Name = "Healthcare", TransactionType = TransactionType.Expense }
        );

        modelBuilder.Entity<CategoryRules>()
            .HasData(
                new CategoryRules { Id = 1, Name = "Taco Bell", CategoryId = 4, SubCategoryId = 1004, Amount = null },
                new CategoryRules { Id = 2, Name = "DEPOSIT ACH UNITED WHOLESALE TYPE: PAYROLL ID: 9990001601", CategoryId = 6, SubCategoryId = null, Amount = null },
                new CategoryRules { Id = 3, Name = "Acorns", CategoryId = 7, SubCategoryId = 17, Amount = null },
                new CategoryRules { Id = 4, Name = "Royal Oak Family Dentistry", CategoryId = 9, SubCategoryId = 1003, Amount = null },
                new CategoryRules { Id = 5, Name = "Trader Joe's", CategoryId = 4, SubCategoryId = 13, Amount = null },
                new CategoryRules { Id = 6, Name = "DTE Energy", CategoryId = 1, SubCategoryId = 1, Amount = null },
                new CategoryRules { Id = 1001, Name = "Ustore North", CategoryId = 4, SubCategoryId = null, Amount = null },
                new CategoryRules { Id = 1005, Name = "Aldi", CategoryId = 4, SubCategoryId = 13, Amount = null },
                new CategoryRules { Id = 2001, Name = "Venmo", CategoryId = 1, SubCategoryId = 1005, Amount = (decimal)699.00 },
                new CategoryRules { Id = 2002, Name = "Costco", CategoryId = 4, SubCategoryId = 13, Amount = null }
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
                new SubCategory { Id = 21, Name = "Vet", CategoryId = 8 },
                new SubCategory { Id = 1001, Name = "Prescriptions", CategoryId = 9 },
                new SubCategory { Id = 1003, Name = "Copay", CategoryId = 9 },
                new SubCategory { Id = 1004, Name = "Fast Food", CategoryId = 4 },
                new SubCategory { Id = 1005, Name = "Rent", CategoryId = 1 }
            );
    }
}
