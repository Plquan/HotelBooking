using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class BookingDetail
    {
        [Key]
        public int Id { get; set; }
        public int BookingId { get; set; }
        public int RoomId { get; set; }
        public Booking Booking { get; set; }
        public Room Room { get; set; }
    }
}
