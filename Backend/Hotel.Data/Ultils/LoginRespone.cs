using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Ultils
{
    public class LoginRespone
    {
        public bool Successful {  get; set; }
        public string Error { get; set; }
        public string Token { get; set; }
    }
}
