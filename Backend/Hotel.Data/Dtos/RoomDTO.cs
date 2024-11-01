using Hotel.Data.Models;

namespace Hotel.Data.Dtos
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomNumber { get; set; }
		public string Status { get; set; }
	}
}
