using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class RefreshToken
    {
        [Key]
        public string? JwtId { get; set; }
        public string? AppUserId { get; set; }
        public string? TokenRefresh { get; set; }
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiredAt { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
