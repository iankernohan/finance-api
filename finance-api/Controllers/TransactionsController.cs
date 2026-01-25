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
        private readonly IPlaidService _service;

        public TransactionsController(IPlaidService service)
        {
            _service = service;
        }

        [HttpPost("Transactions")]
        [Authorize]
        public async Task<IActionResult> GetTransactions(GetPlaidTransactionsRequest req)
        {
            var item = await _service.GetPlaidItem(req.UserId);

            if (item == null)
                return NotFound("No Plaid item found");

            var data = await _service.GetPlaidTransactions(item, req);

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
            var userId = User.FindFirst("sub")?.Value;

            Console.WriteLine(User);
            var transactions = await _service.GetTransactionsByCategory(req.CategoryNames);

            return Ok(transactions);
        }
    }
}
