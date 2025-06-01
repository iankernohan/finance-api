using System;
using finance_api.Dtos;

namespace finance_api.Services;

public interface ITransactionsService
{
    Task<List<Transaction>> GetAllTransactions();
    Task<List<Transaction>> GetAllExpenses();
    Task<List<Transaction>> GetAllIncome();
}
