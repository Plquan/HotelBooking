using Hotel.Data.Models;

namespace Hotel.Data.Dtos
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public int RoomNumber { get; set; }
        public int Capacity { set; get; }
        public string Image { get; set; }
		public decimal Price { set; get; }
		public bool Status { get; set; }
	}
}
