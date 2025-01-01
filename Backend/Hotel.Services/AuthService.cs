using Hotel.Data;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.Ultils.Email;
using Hotel.Data.ViewModels.AppUsers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using static Org.BouncyCastle.Math.EC.ECCurve;

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
    }

    public interface IAuthService 
    {
        Task<ApiResponse> LoginAsync(LoginRequest login);
        Task<ApiResponse> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse> RegisterAsync(RegisterRequest register);
        Task<ApiResponse> LogoutAsync(string? accessToken, string? refreshToken);
        Task<ApiResponse> ConfirmEmailAsync(ConfirmEmailModel model);
        Task<ApiResponse> ResendCodeConfirmEmailAsync(string email);

    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HotelContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IMailService _mailService;
        public AuthService(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IConfiguration configuration,
                                RoleManager<IdentityRole> roleManager,
                                IJwtService jwtService,
                                IHttpContextAccessor httpContextAccessor,
                                HotelContext context,
                                ILogger<AuthService> logger,
                                IMailService mailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
            _logger = logger;
            _mailService = mailService;
        }

        public async Task<ApiResponse> LoginAsync(LoginRequest login)
        {
            var user = await _userManager.FindByEmailAsync(login.UserName) ?? await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Sai tên đăng nhập hoặc mật khẩu"
                };
            }
            var checkPass = await _userManager.CheckPasswordAsync(user, login.Password!);
            if (!checkPass)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Sai tên đăng nhập hoặc mật khẩu"
                };
            }
            try
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name,user.UserName ?? " "),

            };
                var userRoles = await _userManager.GetRolesAsync(user);
                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }
                var accessToken = _jwtService.GenerateAccessToken(claims);
                var refreshToken = _jwtService.GenerateRefreshToken(claims);
                if (accessToken == null || refreshToken == null)
                {
                    throw new InvalidOperationException("Access token or refresh tooken invalid");
                }
                var userRefreshToken = new RefreshToken()
                {
                    JwtId = refreshToken.TokenId,
                    TokenRefresh = refreshToken.Token ?? "",
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = refreshToken.Expiration,
                    IsUsed = false,
                    IsRevoked = false,
                    AppUserId = user.Id
                };

                _context.RefreshTokens.Add(userRefreshToken);
                await _context.SaveChangesAsync();
                _httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken?.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = refreshToken?.Expiration
                });

                return new ApiResponse
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Đăng nhập thành công",
                    Data = new
                    {
                        accessToken = accessToken?.Token,
                        refreshToken = refreshToken?.Token,
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while logging in: {ex.Message} at {DateTime.UtcNow}");
                return new ApiResponse
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "Lỗi máy chủ, vui lòng thử lại sau"
                };
            }
        }
        public async Task<ApiResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var partsClaim = _jwtService.GetTokenClaims(refreshToken);
                var refreshTokenId = partsClaim.FirstOrDefault(r => r.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var userId = partsClaim.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;

                if (!_jwtService.CheckValidateToken(refreshToken, _configuration["JWT:RefreshTokenSecret"]!) || userId == null || refreshTokenId == null)
                {
                    throw new InvalidOperationException("Token invalid");
                }
                var refreshTokenDb = _context.RefreshTokens.FirstOrDefault(r => r.JwtId == refreshTokenId);
                var partClaimsDb = _jwtService.GetTokenClaims(refreshTokenDb?.TokenRefresh);
                var isClaimValid = partClaimsDb.All(c => partsClaim.Any(r => r.Type == c.Type && r.Value == c.Value));
                if (refreshTokenDb.IsUsed || !isClaimValid || !_jwtService.CheckValidateToken(refreshTokenDb.TokenRefresh, _configuration["JWT:AccessTokenSecret"]!))
                {
                    throw new InvalidOperationException("Token invalid");
                }
                var newAccessTokenResponse = _jwtService.GenerateAccessToken(partClaimsDb);
                var newRefreshTokenResponse = _jwtService.GenerateRefreshToken(partClaimsDb);
                if (newRefreshTokenResponse == null || newAccessTokenResponse == null)
                {
                    throw new InvalidOperationException("Cannot generate token");
                }
                var NewRefreshToken = new RefreshToken
                {
                    JwtId = newRefreshTokenResponse.TokenId,
                    TokenRefresh = newRefreshTokenResponse.Token ?? "",
                    IssuedAt = DateTime.UtcNow,
                    ExpiredAt = newRefreshTokenResponse.Expiration,
                    IsUsed = false,
                    IsRevoked = false,
                    AppUserId = userId
                };
                _context.RefreshTokens.Remove(refreshTokenDb);
                _context.RefreshTokens.Add(NewRefreshToken);
                await _context.SaveChangesAsync();
                _httpContextAccessor?.HttpContext?.Response.Cookies.Append("refreshToken", newRefreshTokenResponse.Token!, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = newRefreshTokenResponse.Expiration
                });

                return new ApiResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Token đã được cập nhật",
                    Data = new
                    {
                        accessToken = newAccessTokenResponse.Token,
                        refreshToken = newRefreshTokenResponse.Token
                    }
                };

            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException && ex.Message == "Token invalid")
                {
                    return new ApiResponse
                    {
                        StatusCode = 401,
                        IsSuccess = false,
                        Message = "Token không hợp lệ"
                    };
                }

                return new ApiResponse
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
        public async Task<ApiResponse> RegisterAsync(RegisterRequest register)
        {
            if (await _userManager.FindByEmailAsync(register.Email ?? "") != null)
                return new ApiResponse() { StatusCode = 409, Message = "Email này đã được đăng ký" };

            string code = GenerateVerificationCode();
            var user = new AppUser
            {
                UserName = register.UserName,
                Email = register.Email,
                PhoneNumber = register.Phone,
                VerifyCode = code,
                CodeExpireTime = DateTime.UtcNow.AddMinutes(2),
            };
            var result = await _userManager.CreateAsync(user, register.Password);


            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    _logger.LogError($"There are an error for userManager: {item.Description} at {DateTime.UtcNow}");
                }

            }
            string title = "Xác thực email của bạn";        
            string html = AuthCodeMessage.EmailBody(code, title, 2);
            await _mailService.SendEmailAsync(register.Email ?? "", title, html);
            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Đăng kí thành công"
            };
            
        }
        public async Task<ApiResponse> LogoutAsync(string? accessToken, string? refreshToken)
        {        
            try
            {
                var accessTokenClaims = new List<Claim>();
                var refreshTokenClaims = new List<Claim>();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    accessTokenClaims = _jwtService.GetTokenClaims(accessToken);
                }
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    refreshTokenClaims = _jwtService.GetTokenClaims(refreshToken);
                }
                var accessTokenId = accessTokenClaims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;
                var refreshTokenId = refreshTokenClaims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (!string.IsNullOrEmpty(accessTokenId) && !string.IsNullOrEmpty(accessToken))
                {
                    var newInvalidToken = new InvalidatedToken()
                    {
                        Id = accessTokenId,
                        expiryTime = _jwtService.GetTokenExpiration(accessToken)
                    };
                }
                _httpContextAccessor?.HttpContext?.Response.Cookies.Delete("refreshToken");
                _context.RefreshTokens.RemoveRange(_context.RefreshTokens.Where(r => r.JwtId == refreshTokenId));
                await _context.SaveChangesAsync();

                return new ApiResponse()
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Message = "Đăng xuất thành công"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while logging out: {ex.Message} at {DateTime.UtcNow}");
                return new ApiResponse()
                {
                    StatusCode = 500,
                    IsSuccess = false,
                    Message = "Lỗi máy chủ"
                };

            }

        }

        public async Task<ApiResponse> ConfirmEmailAsync(ConfirmEmailModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ApiResponse()
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Message = "Không tìm thấy email này"
                };
            }
            if (user.VerifyCode != model.Code || user.CodeExpireTime < DateTime.UtcNow)
            {
                return new ApiResponse()
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    Message = "Mã xác thực không hợp lệ"
                };
            }
            user.EmailConfirmed = true;
            user.CodeExpireTime = null;
            user.VerifyCode = null;

            var roleExists = await _roleManager.RoleExistsAsync(AppRole.Member);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(AppRole.Member));
            }

            await _userManager.AddToRoleAsync(user, AppRole.Member);

            await _userManager.UpdateAsync(user);

            return new ApiResponse()
            {
                StatusCode = 200,
                Message = "Xác thực tài khoản thành công",
                IsSuccess = true
            };
        }

        private static string GenerateVerificationCode()
        {
            return new Random().Next(1000, 9999).ToString();
        }

        public async Task<ApiResponse> ResendCodeConfirmEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new ApiResponse()
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Message = "Không tìm thấy email"
                };
            }

            string code = GenerateVerificationCode();

            user.CodeExpireTime = DateTime.UtcNow.AddMinutes(2);
            user.VerifyCode = code;

            string title = "Xác thực email của bạn";
         
            string html = AuthCodeMessage.EmailBody(code, title, 2);
            await _mailService.SendEmailAsync(email ?? "", title, html);

            await _userManager.UpdateAsync(user);

            return new ApiResponse()
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Gửi lại mã code thành công",
            };
        }
    }


}
