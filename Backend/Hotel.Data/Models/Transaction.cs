using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string? OrderDescription { get; set; }
        public string? TransactionId { get; set; }
        public int? BookingId { get; set; }   
        public string? PaymentMethod { get; set; }
        public string? PaymentId { get; set; }
        public DateTime? DateCreated { get; set; }
        public Booking? Booking { get; set; }
    }
}
