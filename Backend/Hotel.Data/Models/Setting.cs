using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class Setting
    {
        [Key]
        public int Id { get; set; }
        public string? Thumb { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? Instagram { get; set; }
        public string? Address { get; set; }
        public int? TaxRate { get; set; }
        public int? RefundFee { get; set; }
    }
}
