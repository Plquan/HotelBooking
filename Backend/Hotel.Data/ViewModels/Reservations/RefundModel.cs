using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.Reservations
{
    public class RefundModel
    {
        public int? TransactionId { get; set; }
        public int? BookingId { get; set; }
        public decimal? RefundAmount { get; set; } = 0;
        public string? RefundReason { get; set; }
        public string? TransactionType { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
