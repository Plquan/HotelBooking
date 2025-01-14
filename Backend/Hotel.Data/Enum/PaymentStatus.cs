using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Enum
{
    public static class PaymentStatus
    {     
        public const string Paid = "Paid";
        public const string Unpaid = "Unpaid";
        public const string CheckedIn = "CheckedIn";
        public const string Online = "Online";
        public const string PaymentPending = "PaymentPending";
        public const string Pending = "Pending";
        public const string COD = "COD";
        public const string OP = "OP";
        public const string Refund = "Refund";
    }
}
