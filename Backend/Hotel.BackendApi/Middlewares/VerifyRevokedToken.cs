using Hotel.Data;
using Hotel.Data.Ultils;
using Hotel.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.BackendApi.Middlewares
{
    public class VerifyRevokedToken
    {
        private readonly RequestDelegate _next;

        public VerifyRevokedToken(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, HotelContext dbContext,IJwtService jwtServices)
        {
            var accessToken = await context.GetTokenAsync("accessToken");
            if (accessToken is null)
            {
                await _next(context);
                return;
            }
            var tokenClaims = jwtServices.GetTokenClaims(accessToken);
            var tokenId = tokenClaims?.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
            var isRevoked = dbContext.InvalidatedTokens.Any(t => t.TokenId == tokenId);
            if (isRevoked)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsJsonAsync(new ApiResponse
                {
                    StatusCode = 401,
                    IsSuccess = false,
                    Message = "Bạn chưa xác thực, làm ơn đăng nhập để truy cập"
                });
                return;
            }
            await _next(context);
        }
    }
}
