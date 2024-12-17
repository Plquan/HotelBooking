using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels.RoomTypes
{
    public class RoomFacilityModel
    {
        public int? Id { get; set; }
        public int? RoomTypeId { get; set; }
        public string? Name { get; set; }
    }
}
