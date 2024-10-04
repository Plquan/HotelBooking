using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class BookingService
    {
        public int Id { get; set; }
        public int BookingId { get; set; }

        public int ServiceId { get; set; }
        public Booking Booking { get; set; }
        public Service Service { get; set; }
    }
}
