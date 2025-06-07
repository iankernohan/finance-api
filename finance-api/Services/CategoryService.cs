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

    public async Task<List<CategoryDtoResponse>> GetCategories()
    {
        var categories = await _context.Category
            .Select(c => _mapper.Map<CategoryDtoResponse>(c))
            .ToListAsync();

        foreach (var category in categories)
        {
            var subcategories = await _context.SubCategory
                .Where(s => s.CategoryId == category.Id)
                .ToListAsync();
            category.SubCategories = subcategories;
        }

        return categories;
    }

    public async Task<CategoryDtoResponse> GetCategory(int Id)
    {
        var category = await _context.Category
            .FindAsync(Id) ?? throw new Exception($"No category found with the id of {Id}");

        var model = _mapper.Map<CategoryDtoResponse>(category);

        var subcategories = await _context.SubCategory
            .Where(s => s.CategoryId == Id)
            .ToListAsync();
        model.SubCategories = subcategories;

        return model;
    }

    public async Task<CategoryDtoResponse> GetCategory(string Name)
    {
        var category = await _context.Category
            .FirstOrDefaultAsync(c => c.Name == Name) ?? throw new Exception($"No category found with the id of {Name}");

        var model = _mapper.Map<CategoryDtoResponse>(category);

        var subcategories = await _context.SubCategory
            .Where(s => s.CategoryId == category.Id)
            .ToListAsync();
        model.SubCategories = subcategories;

        return model;
    }

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
