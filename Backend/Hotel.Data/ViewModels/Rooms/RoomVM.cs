using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.Rooms
{
    public class RoomVM
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }
        public string? RoomNumber { get; set; }
        public string? Status { get; set; }
    }
}
