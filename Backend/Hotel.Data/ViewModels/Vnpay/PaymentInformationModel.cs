﻿using Hotel.Data.Models;
using Hotel.Data.ViewModels.Reservations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.Vnpay
{
    public class PaymentInformationModel
    {
        public string? OrderType { get; set; }
        public double? Amount { get; set; }
        public string? OrderDescription { get; set; }
        public string? Name { get; set; }
        public Booking? Booking {  get; set; }
    }
}
