using finance_api.Dtos;
using finance_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace finance_api.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class CategoryRulesController : ControllerBase
    {
        private readonly ICategoryRulesService _service;

        public CategoryRulesController(ICategoryRulesService service)
        {
            _service = service;
        }

        [HttpPost("GetCategoryRules")]
        [Authorize]
        public async Task<IActionResult> GetCategoryRules()
        {
            var rules = await _service.GetCategoryRules();
            return Ok(rules);
        }

        [HttpPost("AddCategoryRule")]
        [Authorize]
        public async Task<IActionResult> AddCategoryRule(CategoryRuleRequest req)
        {
            await _service.AddCategoryRule(req.Name, req.CategoryId, req.SubCategoryId);
            return Ok();
        }

        [HttpPost("UpdateCategoryRule")]
        [Authorize]
        public async Task<IActionResult> UpdateCategoryRule(UpdateCategoryRuleRequest req)
        {
            var rule = await _service.UpdateCategoryRule(req);
            return Ok(rule);
        }

        [HttpPost("DeleteCategoryRule")]
        [Authorize]
        public async Task<IActionResult> DeleteCategoryRule(int ruleId)
        {
            var rule = await _service.DeleteCategoryRule(ruleId);
            return Ok(rule);
        }
    }
}
