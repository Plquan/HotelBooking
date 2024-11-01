using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class RoomType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Content  { get; set; }
        public int Capacity { set; get; }
        public decimal Price { set; get; }
        public string? View {  get; set; }
        public string BedType { set; get; }
		public string Size { get; set; }
		public string? Thumb { set; get; }
        public string? Status { set; get; }
        public ICollection<Room>? Rooms { get; set; }
		public ICollection<RoomImage>? RoomImages { get; set; }
		public ICollection<RoomFacility>? RoomFacilitys { get; set; }
        public ICollection<RoomService>? RoomServices { get; set; }
	}
}
