using System;
using finance_api.Dtos;

namespace finance_api.Services;

public interface ITransactionsService
{
    Task<List<TransactionDtoResponse>> GetAllTransactions();
    Task<List<TransactionDtoResponse>> GetAllExpenses();
    Task<List<TransactionDtoResponse>> GetAllIncome();
}
