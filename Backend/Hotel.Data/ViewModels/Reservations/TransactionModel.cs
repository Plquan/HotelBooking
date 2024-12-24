using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.Reservations
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public string? OrderDescription { get; set; }
        public string? TransactionId { get; set; }
        public int? BookingId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentId { get; set; }
        public DateTime? DateCreated { get; set; }
    }
}
