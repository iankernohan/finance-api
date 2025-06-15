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

    public async Task<TransactionDtoResponse> GetTransaction(int id)
    {
        var transaction = await _context.Transactions
            .Include(t => t.Category)
            .Include(t => t.SubCategory)
            .FirstOrDefaultAsync(t => t.Id == id);
        var transactionDto = _mapper.Map<TransactionDtoResponse>(transaction);

        return transactionDto;
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

    public async Task<TransactionDtoResponse> AddTransaction(TransactionDtoRequest transaction)
    {
        try
        {
            var model = _mapper.Map<Transaction>(transaction);
            model.DateCreated = DateTime.UtcNow;

            var added = _context.Transactions.Add(model);
            await _context.SaveChangesAsync();

            var response = await GetTransaction(added.Entity.Id);

            return response;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<TransactionDtoResponse> UpdateTransaction(int id, TransactionDtoRequest updatedtransaction)
    {
        var transaction = await _context.Transactions.FindAsync(id) ?? throw new Exception("No transaction found with that id.");

        transaction.Amount = updatedtransaction.Amount;
        transaction.Description = updatedtransaction.Description;
        if (updatedtransaction.DateCreated is not null) transaction.DateCreated = (DateTime)updatedtransaction.DateCreated;
        transaction.CategoryId = updatedtransaction.CategoryId;
        transaction.SubCategoryId = updatedtransaction.SubCategoryId;

        await _context.SaveChangesAsync();
        var updated = await GetTransaction(id);

        return _mapper.Map<TransactionDtoResponse>(updated);
    }
}