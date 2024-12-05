using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Ultils
{
    public class ApiResponse
    {    
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Data { get; set; }
    }
}
