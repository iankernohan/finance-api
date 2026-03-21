using System.Text.Json;
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
        private readonly ICategoryRulesApplier _applier;

        public CategoryRulesController(ICategoryRulesService service, ICategoryRulesApplier applier)
        {
            _service = service;
            _applier = applier;
        }

        [HttpPost("GetCategoryRules")]
        [Authorize]
        public async Task<IActionResult> GetCategoryRules()
        {
            var userId = User.GetUserId();
            var rules = await _service.GetCategoryRules(userId);
            return Ok(rules);
        }

        [HttpPost("AddCategoryRule")]
        [Authorize]
        public async Task<IActionResult> AddCategoryRule(CategoryRuleRequest req)
        {
            var userId = User.GetUserId();
            await _service.AddCategoryRule(req.Name, req.CategoryId, req.SubCategoryId, req.Amount, userId);
            return Ok();
        }

        [HttpPost("UpdateCategoryRule")]
        [Authorize]
        public async Task<IActionResult> UpdateCategoryRule(UpdateCategoryRuleRequest req)
        {
            var userId = User.GetUserId();
            var rule = await _service.UpdateCategoryRule(req, userId);
            return Ok(rule);
        }

        [HttpPost("DeleteCategoryRule")]
        [Authorize]
        public async Task<IActionResult> DeleteCategoryRule(int ruleId)
        {
            var rule = await _service.DeleteCategoryRule(ruleId);
            return Ok(rule);
        }

        [HttpPost("ApplyCategoryRules")]
        [Authorize]
        public async Task<IActionResult> ApplyCategoryRules()
        {
            var userId = User.GetUserId();
            await _applier.ApplyCategoryRules(userId);
            return Ok();
        }
    }
}
