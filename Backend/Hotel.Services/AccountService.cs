using AutoMapper;
using Hotel.Data;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IAccountService 
    {
     Task<ApiResponse> GetCurrentUserAsync();
    }

    public class AccountService : IAccountService
    {
        private readonly HotelContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AccountService(HotelContext context, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor,IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<ApiResponse> GetCurrentUserAsync()
        {
            try
            {
                var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? throw new Exception("User not found");
                var currentUser = await _userManager.FindByIdAsync(userId);
                if (currentUser == null)
                {
                    return new ApiResponse
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        IsSuccess = false,
                        Message = "Không tìm thấy thông tin người dùng"
                    };
                }
                var userRoles = await _userManager.GetRolesAsync(currentUser);
                var userInfo = _mapper.Map<PublicUserVM>(currentUser);
                userInfo.Roles = userRoles;
                return new ApiResponse
                {
                    StatusCode = 200,
                    IsSuccess = true,
                    Data = userInfo
                };
            }
            catch (Exception ex) {
                
                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status500InternalServerError,
                    IsSuccess = false,
                    Message = "Lỗi hệ thống, vui lòng thử lại sau"
                };
            }

        }
    }
}
