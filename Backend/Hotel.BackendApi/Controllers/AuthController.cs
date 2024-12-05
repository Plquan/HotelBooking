using Hotel.Data.Ultils;
using Hotel.Services;
using Microsoft.AspNetCore.Authentication;
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
        public async Task<IActionResult> LoginAsync(LoginRequest login)
        {

            try
            {
                var respone = await _authService.LoginAsync(login);
                if (!respone.IsSuccess) { 
                return BadRequest($"Lỗi khi đăng nhập {respone.Message}");
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
                var respone = await _authService.RegisterAsync(register);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {          
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken)){
                    return Unauthorized(
                        new ApiResponse()
                        {
                            StatusCode = 500,
                            Message = "Bạn chưa được xác thực, làm ơn đăng nhập lại",
                            IsSuccess = false
                        }
                        );
                }
                var respone = await _authService.RefreshTokenAsync(refreshToken);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPost]
        [Route("Logout")]

        public async Task<IActionResult> Logout()
        {
            var accessToken = await Request.HttpContext.GetTokenAsync("access_token");
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authService.LogoutAsync(accessToken, refreshToken);
            return StatusCode(response.StatusCode, response);

        }
    }
}
