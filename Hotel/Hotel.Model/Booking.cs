using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Model
{
    public class Booking
    {
        public int Id { get; set; }
        public int UserId {  get; set; }
        public int PaymentId { get; set; }
        public int RoomId { get; set; }
        public int ServiceId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal Price { get; set; }

        [ForeignKey("PaymentId")]
        public Payment Payment { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

    }
}
