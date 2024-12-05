using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels
{
    public class ChooseRoom
    {
        public int Id { get; set; }
        public int Number { get; set; }
    }
    public class BookingVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public decimal? Totalprice { get; set; }
        public int? TotalPerson { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Status { get; set; }
        public List<ChooseRoom> ChooseRooms { get; set; }
         
    }
}
