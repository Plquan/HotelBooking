using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Ordering {  get; set; }
        public string? Link { get; set; }
        public int? ParentId { get; set; }
        public string? TypeOpen { get; set; }
        public string? Status { get; set; }
    }
}
