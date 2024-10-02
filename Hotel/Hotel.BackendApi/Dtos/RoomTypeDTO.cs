using Hotel.Data.Models;

namespace Hotel.BackendApi.Dtos
{
    public class RoomTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public ICollection<Room> Rooms { get; set; }
    }
}
