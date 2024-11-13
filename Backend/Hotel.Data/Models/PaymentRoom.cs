using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class PaymentRoom
    {
        [Key]
        public int Id { get; set; }
        public int PaymentId { get; set; }
        public int RoomId { get; set; }
        public Payment Payment { get; set; }
        public Room Room { get; set; }
    }
}
