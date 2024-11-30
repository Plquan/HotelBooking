using Hotel.Data.Ultils;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase      
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new LoginRespone
                    {
                        Successful = false,
                        Error = "Thông tin đăng nhập không hợp lệ"
                    });
            }
            try
            {
                var respone = await _authService.Login(login);
                if (!respone.Successful) { 
                return BadRequest($"Lỗi khi đăng nhập {respone.Error}");
                }
                return Ok(respone);
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequest register) {
            if (!ModelState.IsValid) { 
            return BadRequest("Vui lòng nhập đầy đủ thông tin");
            }
            try
            {
                var respone = await _authService.Register(register);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
    }
}
