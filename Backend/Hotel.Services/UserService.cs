using AutoMapper;
using Hotel.Data;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.AppUsers;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Data.ViewModels.Rooms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Hotel.Services
{
    public interface IUserService 
    {
        Task<Paging<AppUserVM>> GetListPaging(PagingModel model);
        Task<ApiResponse> CreateUser(AppUserModel model);
      //Task<ApiResponse> UpdateUser(AppUserModel model);
      //  Task<ApiResponse> DeleteUser(AppUserModel model);
    }


    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IPagingService _pagingService;
        private readonly HotelContext _context;

        public UserService(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IPagingService pagingService, HotelContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _pagingService = pagingService;
            _context = context;
        }

        public async Task<ApiResponse> CreateUser(AppUserModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                return new ApiResponse
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Email này đã tồn tại",
                };
            }
            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Status = model.Status,
                CreatedAt = DateTime.Now,
            };
            if (model.Roles != null)
            {
                foreach (var role in model.Roles)
                {
                    if (!await _roleManager.RoleExistsAsync(role)) continue;

                    await _userManager.AddToRoleAsync(user, role);
                }
            }
            var newUser = await _userManager.CreateAsync(user, model.PhoneNumber ?? "");
            var userReturn = _mapper.Map<AppUserModel>(newUser);
            userReturn.Roles = model.Roles;

            return new ApiResponse
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Thêm người dùng thành công",
                IsSuccess = true,
                Data = userReturn
            };
        }

        public async Task<Paging<AppUserVM>> GetListPaging(PagingModel model)
        {
            var query = from u in _userManager.Users
                        select new AppUserVM { 
                         UserName=u.UserName,
                         Email=u.Email,
                         PhoneNumber=u.PhoneNumber,
                         Status = u.Status,
                         CreatedAt = DateTime.Now,
                         UpdateAt = DateTime.Now,
                         Avatar = u.Avatar
                        };
            return await _pagingService.GetPagedAsync<AppUserVM>(query, model.PageIndex, model.PageSize);
        }
    }
}
