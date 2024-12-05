
using Hotel.Data.Ultils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IJwtService
    {
        TokenResponse GenerateAccessToken(List<Claim> claims);
        TokenResponse GenerateRefreshToken(List<Claim> claims);
        List<Claim> GetTokenClaims(string? token);
        bool CheckValidateToken(string? token, dynamic op);
        DateTime? GetTokenExpiration(string? token);
    }
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration) 
        {
            _configuration = configuration;     
        }
        public  TokenResponse GenerateAccessToken(List<Claim> claims)
        {
            try
            {
                var idToken = Guid.NewGuid().ToString();
                var isIdTokenExist = claims.Exists(c => c.Type == JwtRegisteredClaimNames.Jti);
                if (isIdTokenExist)
                {
                    claims.Remove(claims.First(c => c.Type == JwtRegisteredClaimNames.Jti));
                }

                var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:AccessTokenSecret"])
                                                         ?? throw new InvalidOperationException("Không tìm thấy access token"));
                var creds = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature);
                var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:AccessTokenExpireMinutes"]));
                var token = new JwtSecurityToken(
                  issuer: _configuration["JWT:Issuer"],
                  audience: _configuration["JWT:Audience"],
                  expires: expiry,
                  claims: claims,
                  signingCredentials: creds
                 );
                return new TokenResponse
                {
                    TokenId = idToken,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expiry
                };
            }
            catch(Exception ex) {
                throw new Exception("Lỗi khởi tạo token",ex);

            }
        }

        public TokenResponse GenerateRefreshToken(List<Claim> claims)
        {
            try
            {
                var idToken = Guid.NewGuid().ToString();
                var isIdTokenExist = claims.Exists(c => c.Type == JwtRegisteredClaimNames.Jti);
                if (isIdTokenExist)
                {
                    claims.Remove(claims.First(c => c.Type == JwtRegisteredClaimNames.Jti));
                }

                var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:RefreshTokenSecret"])
                                                         ?? throw new InvalidOperationException("Không tìm thấy access token"));
                var creds = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature);
                var expiry = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:RefreshTokenExpireMinutes"]));
                var token = new JwtSecurityToken(
                  issuer: _configuration["JWT:Issuer"],
                  audience: _configuration["JWT:Audience"],
                  expires: expiry,
                  claims: claims,
                  signingCredentials: creds
                 );
                return new TokenResponse
                {
                    TokenId = idToken,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Expiration = expiry
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khởi tạo token", ex);

            }
        }
        public List<Claim> GetTokenClaims(string? token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.ReadToken(token) is JwtSecurityToken securityToken)
                return securityToken.Claims.ToList();
            return new List<Claim>();
        }
        public bool CheckValidateToken(string? token, dynamic op)
        {
            var issues = _configuration["JWT:Issuer"];
            var audience = _configuration["JWT:Audience"];
            var secret = _configuration[op];

            var tokenHandler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "")),
                ValidateIssuer = true,
                ValidIssuer = issues,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            return validatedToken != null;
        }
        public DateTime? GetTokenExpiration(string? token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var checkToken = tokenHandler.ReadToken(token) as JwtSecurityToken;
                if (checkToken != null)
                {
                    return checkToken.ValidTo;
                }
                return default;
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
