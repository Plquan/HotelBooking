using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public decimal Totalprice { get; set; }
        public string PaymentMethod { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public int Status { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
    }
}
