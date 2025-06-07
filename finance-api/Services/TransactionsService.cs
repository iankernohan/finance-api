using System;
using finance_api.Enums;
using finance_api.Models;
using finance_api.Dtos;
using AutoMapper;
using finance_api.Data;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class TransactionsService : ITransactionsService
{
    private readonly IMapper _mapper;
    private readonly AppDbContext _context;

    public TransactionsService(IMapper mapper, AppDbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<List<TransactionDtoResponse>> GetAllTransactions()
    {
        var transactions = await _context.Transactions
            .Select(t => _mapper.Map<TransactionDtoResponse>(t))
            .ToListAsync();

        return transactions;
    }
    public async Task<List<TransactionDtoResponse>> GetAllExpenses()
    {
        var expenses = await _context.Transactions
            .Where(t => t.Category.transactionType == TransactionType.Expense)
            .Select(t => _mapper.Map<TransactionDtoResponse>(t))
            .ToListAsync();

        return expenses;
    }

    public async Task<List<TransactionDtoResponse>> GetAllIncome()
    {
        var income = await _context.Transactions
            .Where(t => t.Category.transactionType == TransactionType.Income)
            .Select(t => _mapper.Map<TransactionDtoResponse>(t))
            .ToListAsync();

        return income;
    }
}
