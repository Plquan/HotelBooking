using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpGet]
        [Route("MyInfo")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await _accountService.GetCurrentUserAsync();
            return StatusCode(response.StatusCode, response);
        }
    }
}
