using System;
using AutoMapper;
using finance_api.Data;
using finance_api.Dtos;
using finance_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class BudgetService(AppDbContext context, IMapper mapper) : IBudgetService
{
    private readonly AppDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task AddBudget(BudgetDtoRequest budget)
    {
        var model = _mapper.Map<Budget>(budget);

        _context.Budgets.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task<List<BudgetDtoResponse>> GetAllBudgets()
    {
        var allBudgets = await _context.Budgets.Include(b => b.category).Select((b) => _mapper.Map<BudgetDtoResponse>(b)).ToListAsync();

        return allBudgets;
    }
}
