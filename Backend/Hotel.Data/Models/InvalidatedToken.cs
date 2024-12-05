using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class InvalidatedToken
    {
        [Key]
        public string Id { get; set; }
        public DateTime? expiryTime { get; set; }
    }
}
