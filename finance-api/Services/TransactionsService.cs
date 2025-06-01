using System;
using finance_api.Enums;
using finance_api.Models;

namespace finance_api.Services;

public class TransactionsService : ITransactionsService
{
    List<Transaction> transactions = [
        new Transaction()
    {
        Id = 0,
        Amount = 20.00,
        Category = { Id = 0, Name = "Transportation", transactionType = TransactionType.Expense },
        SubCategory = { Id = 0, Name = "Gas", CategoryId = 0} ,
    },
    new Transaction()
    {
        Id = 1,
        Amount = 68.23,
        Category = { Id = 1, Name = "Food", transactionType = TransactionType.Expense },
        SubCategory = { Id = 1, Name = "Groceries", CategoryId = 1 },
    },
    new Transaction()
    {
        Id = 2,
        Amount = 364.78,
        Category = { Id = 3, Name = "Salary", transactionType = TransactionType.Income },
    }
    ];

    public Task<List<Transaction>> GetAllTransactions()
    {
        return Task.FromResult(transactions);
    }
    public Task<List<Transaction>> GetAllExpenses()
    {
        var expenses = transactions.FindAll(t => t.Category.transactionType == TransactionType.Expense);
        return Task.FromResult(expenses);
    }

    public Task<List<Transaction>> GetAllIncome()
    {
        var income = transactions.FindAll(t => t.Category.transactionType == TransactionType.Income);
        return Task.FromResult(income);
    }
}
