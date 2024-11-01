using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public int? PaymentId { get; set; }
        public int RoomId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }       
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public decimal Price { get; set; }
        public Payment Payment { get; set; }
        public AppUser User { get; set; }
        public Room Room { get; set; }
    }
}
