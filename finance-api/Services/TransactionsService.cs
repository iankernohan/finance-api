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

    private static List<Dtos.Transaction> MapTransaction(List<Transaction> transactions)
    {
        List<Dtos.Transaction> transactionsDto = [];
        foreach (Transaction t in transactions)
        {
            transactionsDto.Add(new Dtos.Transaction()
            {
                Id = t.Id,
                Amount = t.Amount,
                Category = new()
                {
                    Id = t.Category.Id,
                    Name = t.Category.Name,
                    TransactionType = t.Category.transactionType.ToString(),
                    SubCategories = t.Category.SubCategories.Select(sc => new Dtos.SubCategory()
                    {
                        Id = sc.Id,
                        Name = sc.Name,
                        CategoryId = sc.CategoryId
                    }).ToList()
                }
            });
        }
        return transactionsDto;
    }

    public Task<List<Dtos.Transaction>> GetAllTransactions()
    {
        List<Dtos.Transaction> transactionsDto = MapTransaction(transactions);
        return Task.FromResult(transactionsDto);
    }
    public Task<List<Dtos.Transaction>> GetAllExpenses()
    {
        var expenses = transactions.FindAll(t => t.Category.transactionType == TransactionType.Expense);

        List<Dtos.Transaction> mappedExpenses = MapTransaction(expenses);

        return Task.FromResult(mappedExpenses);
    }

    public Task<List<Dtos.Transaction>> GetAllIncome()
    {
        var income = transactions.FindAll(t => t.Category.transactionType == TransactionType.Income);

        List<Dtos.Transaction> mappedIncome = MapTransaction(income);

        return Task.FromResult(mappedIncome);
    }
}
