﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Enum 
{
    public static class BookingStatus
   {
        public const string CheckIn = "CheckIn";
        public const string CheckOut = "CheckOut";
        public const string Pending = "Pending";
        public const string Confirmed = "Confirmed";
        public const string Cancelled = "Cancelled";
    }
}
