using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string? OrderDescription { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentMethod { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? RefundAmount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Booking Booking { get; set; }

    }
}
