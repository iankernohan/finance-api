using finance_api.Dtos;
using finance_api.Plaid;
using finance_api.Services;
using Going.Plaid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace finance_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsServiceV2 _service;
        private readonly IPlaidService _plaidService;

        public TransactionsController(ITransactionsServiceV2 service, IPlaidService plaidService)
        {
            _service = service;
            _plaidService = plaidService;
        }

        [HttpPost("Transactions")]
        [Authorize]
        public async Task<IActionResult> GetTransactions(GetPlaidTransactionsRequest req)
        {
            var item = await _plaidService.GetPlaidItem(req.UserId);

            if (item == null)
                return NotFound("No Plaid item found");

            var data = await _service.GetTransactions(item, req);

            return Ok(data);
        }

        [HttpPost("UncategorizedTransactions")]
        [Authorize]
        public async Task<IActionResult> GetUncategorizedTransactions(GetPlaidTransactionsRequest req)
        {
            var transactions = await _service.GetUncategorizedTransactions(req.UserId);
            return Ok(transactions);
        }

        [HttpPost("CategorizedTransactions")]
        [Authorize]
        public async Task<IActionResult> GetCategorizedTransactions(GetPlaidTransactionsRequest req)
        {
            var transactions = await _service.GetCategorizedTransactions(req.UserId);
            return Ok(transactions);
        }

        [HttpPost("TransactionsByCategory")]
        [Authorize]
        public async Task<IActionResult> GetTransactionsByCategory(TransactionsByCategoryRequest req)
        {
            var transactions = await _service.GetTransactionsByCategory(req.CategoryNames);

            return Ok(transactions);
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
            await _service.AddCategoryRule(req.Name, req.CategoryId);
            return Ok("Category Rule Successfully Added.");
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

        [HttpPost("UpdateCategory")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(UpdateCategoryRequest req)
        {
            var updated = await _service.UpdateCategory(req.TransactionId, req.CategoryId);

            return Ok(updated);
        }
    }
}
