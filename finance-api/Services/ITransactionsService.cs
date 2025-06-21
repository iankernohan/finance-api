using System;
using finance_api.Dtos;
using finance_api.Models;

namespace finance_api.Services;

public interface ITransactionsService
{
    Task<List<TransactionDtoResponse>> GetAllTransactions();
    Task<TransactionDtoResponse> GetTransaction(int id);
    Task<List<TransactionDtoResponse>> GetAllExpenses();
    Task<List<TransactionDtoResponse>> GetAllIncome();
    Task<TransactionDtoResponse> AddTransaction(TransactionDtoRequest transaction);
    Task<TransactionDtoResponse> UpdateTransaction(int id, TransactionDtoRequest updatedtransaction);
    Task<TransactionDtoResponse?> DeleteTransaction(int id);
}
