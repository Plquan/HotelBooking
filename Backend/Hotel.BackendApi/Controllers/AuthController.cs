﻿using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.AppUsers;
using Hotel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<ApiResponse> LoginAsync(LoginRequest login)
        {
                var respone = await _authService.LoginAsync(login);
                return respone;     
        }
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterRequest register) {
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
                    return StatusCode(401, "Người dùng chưa xác thực");
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
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var accessToken = await Request.HttpContext.GetTokenAsync("access_token");
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _authService.LogoutAsync(accessToken, refreshToken);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPost]
        [Route("ConfirmEmailAsync")]
        public async Task<IActionResult> ConfirmEmailAsync(ConfirmEmailModel model)
        {
            try
            {
                var respone = await _authService.ConfirmEmailAsync(model);              
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("ResendCodeConfirmEmailAsync")]
        public async Task<IActionResult> ResendCodeConfirmEmailAsync(string email)
        {
            try
            {
                var respone = await _authService.ResendCodeConfirmEmailAsync(email);
                return Ok(respone);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }

        [HttpGet("GetMe")]
        [Authorize]
        public async Task<ApiResponse> GetMe()
        {
            var response = await _authService.GetCurrentUserAsync();
            return response;
        }
    }
}
