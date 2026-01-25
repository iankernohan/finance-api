using finance_api.Plaid;
using finance_api.Services;
using Going.Plaid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace finance_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlaidController : ControllerBase
    {
        private readonly IPlaidService _service;

        public PlaidController(IPlaidService service)
        {
            _service = service;
        }

        [HttpPost("CreateLinkToken")]
        [Authorize]
        public async Task<IActionResult> CreateLinkToken(CreateLinkTokenRequest req)
        {
            var LinkToken = await _service.CreateLinkToken(req);
            return Ok(new { link_token = LinkToken });
        }

        [HttpPost("ExchangePublicToken")]
        [Authorize]
        public async Task<IActionResult> ExchangePublicToken(ExchangePublicTokenRequest req)
        {
            var exchange = await _service.ExchangePublicToken(req);

            var item = new PlaidItem
            {
                UserId = req.UserId,
                ItemId = exchange.ItemId,
                AccessToken = exchange.AccessToken
            };

            await _service.AddPlaidItem(item);

            return Ok(new { itemId = exchange.ItemId });
        }
    }
}
