using Hotel.Data.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;

namespace Hotel.Data.Dtos
{
    public class RoomTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
		public string? Content { get; set; }
		public int Capacity { set; get; }
        public decimal Price { set; get; }
        public string? View { get; set; }
        public string BedType { get; set; }
		public string Size { get; set; }
		public string? Thumb { set; get; }
        public string? Status { set; get; }
        public List<string>? RoomImages { set; get; }
        public List<string>? RoomFacilitys { set; get; }
        public List<string>? RoomServices { set; get; }

    }
}
