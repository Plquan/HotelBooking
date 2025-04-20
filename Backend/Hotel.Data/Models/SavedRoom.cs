using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class SavedRoom
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }
        public AppUser AppUser { get; set; }
    }
}
