using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.AppUsers
{
    public class AppUserModel
    {
        public string? UserName { get; set; }    
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public string? Status { get; set; }
        public string? VerifyCode { get; set; }
        public DateTime? CodeExpireTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public List<string>? Roles { get; set; }
    }
}
