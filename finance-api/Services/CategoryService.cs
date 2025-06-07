using System;
using AutoMapper;
using finance_api.Data;
using finance_api.Dtos;
using finance_api.Models;
using Microsoft.EntityFrameworkCore;

namespace finance_api.Services;

public class CategoryService(IMapper mapper, AppDbContext context) : ICategoryService
{
    private readonly IMapper _mapper = mapper;
    private readonly AppDbContext _context = context;

    public async Task AddCategory(CategoryDtoRequest category)
    {
        var doesExist = await _context.Category.FirstOrDefaultAsync(c =>
            c.Name == category.Name && c.TransactionType.ToString() == category.TransactionType);

        if (doesExist != null)
        {
            throw new Exception("Category already exists.");
        }

        var model = _mapper.Map<Category>(category);

        _context.Category.Add(model);
        await _context.SaveChangesAsync();
    }

    public async Task AddSubCategory(SubCategoryDtoRequest subCategory)
    {
        var doesExist = await _context.SubCategory.FirstOrDefaultAsync(s =>
            s.Name == subCategory.Name && s.CategoryId == subCategory.CategoryId);

        if (doesExist != null)
        {
            throw new Exception("SubCategory already exists.");
        }

        var category = await _context.Category.FindAsync(subCategory.CategoryId);

        if (category == null)
        {
            throw new Exception($"Category with the id of {subCategory.CategoryId} does not exist.");
        }

        var model = _mapper.Map<SubCategory>(subCategory);

        _context.SubCategory.Add(model);
        await _context.SaveChangesAsync();
    }
}
