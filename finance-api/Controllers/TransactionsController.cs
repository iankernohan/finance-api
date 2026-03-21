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
        private readonly ITransactionsService _service;
        private readonly IPlaidService _plaidService;

        public TransactionsController(ITransactionsService service, IPlaidService plaidService)
        {
            _service = service;
            _plaidService = plaidService;
        }

        [HttpPost("Transactions")]
        [Authorize]
        public async Task<IActionResult> GetTransactions(GetTransactionsRequest req)
        {
            var userId = User.GetUserId();
            var item = await _plaidService.GetPlaidItem(userId);

            if (item == null)
                return NotFound("No Plaid item found");

            var data = await _service.GetTransactions(item, req, userId);

            return Ok(data);
        }

        [HttpPost("TransactionsCount")]
        [Authorize]
        public async Task<IActionResult> GetTransactionsCount(TransactionsCountRequest req)
        {
            var userId = User.GetUserId();
            var data = await _service.GetTransactionsCount(req, userId);

            return Ok(data);
        }

        [HttpPost("UncategorizedTransactions")]
        [Authorize]
        public async Task<IActionResult> GetUncategorizedTransactions()
        {
            var userId = User.GetUserId();
            var transactions = await _service.GetUncategorizedTransactions(userId);
            return Ok(transactions);
        }

        [HttpPost("CategorizedTransactions")]
        [Authorize]
        public async Task<IActionResult> GetCategorizedTransactions()
        {
            var userId = User.GetUserId();
            var transactions = await _service.GetCategorizedTransactions(userId);
            return Ok(transactions);
        }

        [HttpPost("TransactionsByCategory")]
        [Authorize]
        public async Task<IActionResult> GetTransactionsByCategory(TransactionsByCategoryRequest req)
        {
            var userId = User.GetUserId();
            var transactions = await _service.GetTransactionsByCategory(req.CategoryNames, userId);

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
            var userId = User.GetUserId();
            var summary = await _service.GetMonthlySummary(req, userId);
            return Ok(summary);
        }
    }
}
