using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Model
{
    public class Room
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int TypeId { get; set; }
        public string Code { get; set; }
        public string Image {  get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [ForeignKey("TypeId")]
        public RoomType RoomType { get; set; }

    }
}
