using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{

    public class LoginRequest {
    public string? UserName { get; set; }
    public string? Password { get; set; }
    }
    public class RegisterRequest {
        public string? UserName { get; set;}
        public string? Email { get; set; }
        public string? Phone {  get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }

    public interface IAuthService 
    {
        Task<LoginRespone> Login(LoginRequest login);
        Task<IdentityResult> Register(RegisterRequest register);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IConfiguration configuration,
                                RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }
        public async Task<LoginRespone> Login(LoginRequest login)
        {
            var user = await _userManager.FindByEmailAsync(login.UserName) ?? await _userManager.FindByNameAsync(login.UserName);
            if (user == null) {
                return new LoginRespone
                {
                    Successful = false,
                    Error = "Sai tên đăng nhập hoặc mật khẩu"
                };
            }
            var loginResult = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, false, false);
            if (!loginResult.Succeeded)
            {
                return new LoginRespone
                {
                    Successful = false,
                    Error = "Sai tên đăng nhập hoặc mật khẩu"
                };
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var creds = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature);
            var expiry = DateTime.Now.AddDays(Convert.ToInt32(_configuration["ExpiryDay"]));
            var token = new JwtSecurityToken(
              issuer: _configuration["JWT:ValidIssuer"],
              audience: _configuration["JWT:ValidAudience"],
              expires: expiry,
              claims: claims,
              signingCredentials: creds
             );
            return new LoginRespone
            {
                Successful = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }

        public async Task<IdentityResult> Register(RegisterRequest register)
        {
            var user = new AppUser 
            { 
              UserName = register.UserName,
              Email = register.Email,
              PhoneNumber = register.Phone,     
            };
            var result = await _userManager.CreateAsync(user,register.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync(AppRole.Customer))
                {
                    await _roleManager.CreateAsync(new IdentityRole(AppRole.Customer));
                }
                await _userManager.AddToRoleAsync(user, AppRole.Customer);
            }
            return result;
        }

    }

}
