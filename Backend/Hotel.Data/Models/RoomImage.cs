using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class RoomImage
    {
        [Key]
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public string Url { get; set; }
        public RoomType RoomType { get; set; }
    }
}
