using finance_api.Dtos;
using finance_api.Services;
using Going.Plaid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace finance_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IPlaidService _service;

        public UserController(IPlaidService service)
        {
            _service = service;
        }


        [HttpGet("Accounts/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetAccounts(string userId)
        {
            var item = await _service.GetPlaidItem(userId);

            if (item == null)
                return NotFound("No Plaid item found");

            var accounts = await _service.GetAccounts(userId, item);

            return Ok(accounts);
        }

        [HttpPost("UserHasConnection")]
        [Authorize]
        public async Task<IActionResult> UserHasBankConnection(UserHasBankConnectionRequest req)
        {
            var item = await _service.GetPlaidItem(req.UserId);
            return Ok(!string.IsNullOrEmpty(item?.ItemId));
        }
    }
}
