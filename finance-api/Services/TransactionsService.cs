using System;
using finance_api.Enums;
using finance_api.Models;
using finance_api.Dtos;
using AutoMapper;
using finance_api.Data;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class TransactionsService(IMapper mapper, AppDbContext context) : ITransactionsService
{
    private readonly IMapper _mapper = mapper;
    private readonly AppDbContext _context = context;

    public async Task<List<TransactionDtoResponse>> GetAllTransactions()
    {
        var transactions = await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.SubCategory)
            .Select(t => _mapper.Map<TransactionDtoResponse>(t))
            .ToListAsync();

        return transactions;
    }
    public async Task<List<TransactionDtoResponse>> GetAllExpenses()
    {
        var expenses = await _context.Transactions
            .Where(t => t.Category != null && t.Category.TransactionType == TransactionType.Expense)
            .Select(t => _mapper.Map<TransactionDtoResponse>(t))
            .ToListAsync();

        return expenses;
    }

    public async Task<List<TransactionDtoResponse>> GetAllIncome()
    {
        var income = await _context.Transactions
            .Where(t => t.Category != null && t.Category.TransactionType == TransactionType.Income)
            .Select(t => _mapper.Map<TransactionDtoResponse>(t))
            .ToListAsync();

        return income;
    }

    public async Task AddTransaction(TransactionDtoRequest transaction)
    {
        var model = _mapper.Map<Transaction>(transaction);
        model.DateCreated = DateTime.UtcNow;

        _context.Transactions.Add(model);
        await _context.SaveChangesAsync();
    }
}
