using Hotel.Data.ViewModels.RoomTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.Reservations
{

    public class BookedModel
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public int? TotalPerson { get; set; }
        public decimal? TotalPrice { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ConfirmBy { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public string? Status { get; set; }
        public List<RoomTypeVM>? RoomTypes { get; set; }

    }
}
