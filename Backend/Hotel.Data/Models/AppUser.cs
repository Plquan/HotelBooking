using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
      public class AppUser : IdentityUser 
      {
        public string? Status { get; set; }
        public string? VerifyCode { get; set; }
        public DateTime? CodeExpireTime { get; set; }
        public string? Avatar {  get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
      }
}
