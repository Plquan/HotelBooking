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
        //public string Slug { get; set; }
        public string Content  { get; set; }
        public int Capacity { set; get; }
        public decimal Price { set; get; }
        public string View {  get; set; }
        public string BedType { set; get; }
		public string Size { get; set; }
		public string Thumb { set; get; }
        public string Status { set; get; }
        public List<Room> Rooms { get; set; }
		public List<RoomImage> RoomImages { get; set; }
		public List<RoomFacility> RoomFacilitys { get; set; }
        public List<RoomService> RoomServices { get; set; }
	}
}
