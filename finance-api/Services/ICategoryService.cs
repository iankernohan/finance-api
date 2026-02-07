using System;
using finance_api.Dtos;
using finance_api.Models;

namespace finance_api.Services;

public interface ICategoryService
{
    Task<List<CategoryDtoResponse>> GetCategories();
    Task<CategoryDtoResponse> GetCategory(int Id);
    Task<CategoryDtoResponse> GetCategory(string Name);
    Task AddCategory(CategoryDtoRequest category);
    Task<Category> UpdateCategory(UpdateCategoryRequest req);
    Task AddSubCategory(SubCategoryDtoRequest subCategory);
    Task<SubCategory> UpdateSubCategory(UpdateSubCategoryRequest req);
    Task DeleteSubCategory(int subCategoryId);
}
