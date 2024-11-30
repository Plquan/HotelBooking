using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Dtos
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public decimal Totalprice { get; set; }
        public int TotalPerson {  get; set; }
        public DateOnly FromDate { get; set; }
        public DateOnly ToDate { get; set; }
        public string? PaymentMethod { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public string? Status { get; set; }
    }
}
