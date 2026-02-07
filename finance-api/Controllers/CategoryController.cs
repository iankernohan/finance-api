using Microsoft.AspNetCore.Mvc;
using finance_api.Dtos;
using finance_api.Services;
using Microsoft.AspNetCore.Authorization;

namespace finance_api.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost("GetAllCategories")]
    [Authorize]
    public async Task<ActionResult<List<CategoryDtoResponse>>> GetAllCategories()
    {
        var categories = await _categoryService.GetCategories();
        return Ok(categories);
    }

    [HttpGet("GetCategoryByName/{name}")]
    [Authorize]
    public async Task<ActionResult<CategoryDtoResponse>> GetCategoryByName(string name)
    {
        var category = await _categoryService.GetCategory(name);
        return Ok(category);
    }

    [HttpGet("GetCategoryById/{id}")]
    [Authorize]
    public async Task<ActionResult<CategoryDtoResponse>> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategory(id);
        return Ok(category);
    }

    [HttpPost("AddCategory")]
    [Authorize]
    public async Task<IActionResult> AddCategory([FromBody] CategoryDtoRequest category)
    {
        await _categoryService.AddCategory(category);
        return Ok();
    }

    [HttpPost("UpdateCategory")]
    [Authorize]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequest req)
    {
        var category = await _categoryService.UpdateCategory(req);
        return Ok(category);
    }

    [HttpPost("AddSubCategory")]
    [Authorize]
    public async Task<IActionResult> AddSubCategory([FromBody] SubCategoryDtoRequest subCategory)
    {
        await _categoryService.AddSubCategory(subCategory);
        return Ok();
    }

    [HttpPost("UpdateSubCategory")]
    [Authorize]
    public async Task<IActionResult> UpdateSubCategory([FromBody] UpdateSubCategoryRequest subCategory)
    {
        var subcategory = await _categoryService.UpdateSubCategory(subCategory);
        return Ok(subcategory);
    }

    [HttpPost("DeleteSubCategory")]
    [Authorize]
    public async Task<IActionResult> DeleteSubCategory([FromBody] int subCategoryId)
    {
        await _categoryService.DeleteSubCategory(subCategoryId);
        return Ok();
    }
}
