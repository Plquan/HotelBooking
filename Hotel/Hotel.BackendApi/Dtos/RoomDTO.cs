using Hotel.Data.Models;

namespace Hotel.BackendApi.Dtos
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public string Code { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Status { get; set; }
        public RoomType RoomType { get; set; }
    }
}
