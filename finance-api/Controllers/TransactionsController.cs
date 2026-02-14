using finance_api.Dtos;
using finance_api.Plaid;
using finance_api.Services;
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

        [HttpPost("UpdateCategory")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(UpdateTransactionCategoryRequest req)
        {
            var updated = await _service.UpdateCategory(req.TransactionId, req.CategoryId);

            return Ok(updated);
        }

        [HttpPost("MonthlySummary")]
        [Authorize]
        public async Task<IActionResult> GetMonthlySummary(MonthlySummaryRequest req)
        {
            var summary = await _service.GetMonthlySummary(req);
            return Ok(summary);
        }
    }
}
