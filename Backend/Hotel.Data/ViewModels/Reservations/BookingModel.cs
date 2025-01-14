using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.Reservations
{
    public class ChooseRoom
    {
        public int RoomTypeId { get; set; }
        public int Number { get; set; }
    }
    public class BookingModel
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Code { get; set; }
        public string? Note { get; set; }
        public int? TotalPerson { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalPrice { get; set; }
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? ConfirmBy { get; set; }
        public string? Status { get; set; }
        public List<ChooseRoom>? ChooseRooms { get; set; }
    }
}
