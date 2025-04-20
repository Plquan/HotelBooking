using Hotel.Data.Dtos;
using Hotel.Data.ViewModels.RoomTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.Reservations
{
    public class CheckRoomVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public string? Slug { get; set; }
        public int? Capacity { set; get; }
        public decimal? Price { set; get; }
        public string? View { get; set; }
        public string? BedType { get; set; }
        public string? Size { get; set; }
        public int? AvailableRooms { get; set; }
        public bool? IsSaved { set; get; }
        public List<RoomImageModel>? RoomImages { get; set; }
    }
}
