using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.AppUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IUserService 
    {
      Task<ApiResponse> CreateUser(AppUserModel model);
      //Task<ApiResponse> UpdateUser(AppUserModel model);
      //  Task<ApiResponse> DeleteUser(AppUserModel model);
    }


    public class UserService : IUserService
    {
        public Task<ApiResponse> CreateUser(AppUserModel model)
        {
            throw new NotImplementedException();
        }
    }
}
