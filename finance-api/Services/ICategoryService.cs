using System;
using finance_api.Dtos;

namespace finance_api.Services;

public interface ICategoryService
{
    Task<List<CategoryDtoResponse>> GetCategories();
    Task<CategoryDtoResponse> GetCategory(int Id);
    Task<CategoryDtoResponse> GetCategory(string Name);
    Task AddCategory(CategoryDtoRequest category);
    Task AddSubCategory(SubCategoryDtoRequest subCategory);
}
