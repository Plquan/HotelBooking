using Hotel.Data.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels
{
    public class CheckRoomVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Content { get; set; }
        public string? Slug { get; set; }
        public int Capacity { set; get; }
        public decimal Price { set; get; }
        public string? View { get; set; }
        public string? BedType { get; set; }
        public string? Size { get; set; }
        public string? Thumb { set; get; }
        public List<RoomDTO>? Rooms { get; set; }
        public List<RoomImageDTO>? RoomImages { get; set; }
    }
}
