using System;
using finance_api.Enums;
using finance_api.Models;
using finance_api.Dtos;
using AutoMapper;
using finance_api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

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
            .OrderBy(t => t.DateCreated)
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

    public async Task<TransactionDtoResponse?> DeleteTransaction(int id)
    {
        var transaction = await _context.Transactions.FindAsync(id);

        if (transaction is not null)
        {
            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return _mapper.Map<TransactionDtoResponse>(transaction);
        }

        return null;
    }

    public async Task<IActionResult> AddRecurringTransaction(RecurringTransactionRequest request)
    {
        try
        {
            var model = _mapper.Map<RecurringTransactions>(request);

            _context.RecurringTransactions.Add(model);
            await _context.SaveChangesAsync();

            await ProcessRecurringTransactions();

            return new OkObjectResult("Recurring transaction added successfully.");
        }
        catch (Exception ex)
        {
            return new BadRequestObjectResult($"Error adding recurring transaction: {ex.Message}");
        }
    }

    public async Task<List<RecurringTransactionResponse>> GetAllRecurringTransactions()
    {
        var recurringTransactions = await _context.RecurringTransactions
            .Select(rt => _mapper.Map<RecurringTransactionResponse>(rt))
            .ToListAsync();

        return recurringTransactions;
    }

    public async Task<RecurringTransactionResponse?> DeleteRecurringTransaction(int id)
    {
        var recurringTransaction = await _context.RecurringTransactions.FindAsync(id);

        if (recurringTransaction is not null)
        {
            _context.RecurringTransactions.Remove(recurringTransaction);
            await _context.SaveChangesAsync();

            return _mapper.Map<RecurringTransactionResponse>(recurringTransaction);
        }

        return null;
    }

    public async Task<RecurringTransactionResponse?> UpdateRecurringTransaction(int id, RecurringTransactionUpdateRequest updatedTransaction)
    {
        var recurringTransaction = await _context.RecurringTransactions.FindAsync(id) ?? throw new Exception("No recurring transaction found with that id.");

        recurringTransaction.Amount = updatedTransaction.Amount ?? recurringTransaction.Amount;
        recurringTransaction.Description = updatedTransaction.Description ?? recurringTransaction.Description;
        recurringTransaction.EndDate = updatedTransaction.EndDate ?? recurringTransaction.EndDate;
        recurringTransaction.IntervalDays = updatedTransaction.IntervalDays ?? recurringTransaction.IntervalDays;

        await _context.SaveChangesAsync();
        var updated = await GetAllRecurringTransactions();

        return _mapper.Map<RecurringTransactionResponse>(updated.FirstOrDefault(rt => rt.Id == id));
    }

    public async Task ProcessRecurringTransactions()
    {
        var today = DateTime.UtcNow.Date;
        var recurringTransactions = await _context.RecurringTransactions.ToListAsync();

        foreach (var rt in recurringTransactions)
        {
            if (rt.StartDate.Date > today) continue;
            if (rt.EndDate.HasValue && rt.EndDate.Value.Date < today) continue;

            var lastTransaction = await _context.Transactions
                .Where(t => t.Description == rt.Description && t.Amount == rt.Amount && t.CategoryId == rt.CategoryId && t.SubCategoryId == rt.SubCategoryId)
                .OrderByDescending(t => t.DateCreated)
                .FirstOrDefaultAsync();

            var nextDate = lastTransaction?.DateCreated.AddDays(rt.IntervalDays) ?? rt.StartDate;

            while (nextDate.Date <= today)
            {
                var newTransaction = new Transaction
                {
                    Amount = rt.Amount,
                    DateCreated = nextDate,
                    Description = rt.Description,
                    IsRecurring = true,
                    CategoryId = rt.CategoryId,
                    SubCategoryId = rt.SubCategoryId
                };

                _context.Transactions.Add(newTransaction);
                nextDate = nextDate.AddDays(rt.IntervalDays);
            }
        }

        await _context.SaveChangesAsync();
    }
}