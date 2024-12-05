using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Ultils
{
    public class TokenResponse
    {
        public string? TokenId { get; set; }
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
