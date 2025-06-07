using System;
using finance_api.Dtos;

namespace finance_api.Services;

public interface ICategoryService
{
    Task AddCategory(CategoryDtoRequest category);
    Task AddSubCategory(SubCategoryDtoRequest subCategory);
}
