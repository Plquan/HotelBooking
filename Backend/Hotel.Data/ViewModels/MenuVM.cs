using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels
{
    public class MenuVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Ordering { get; set; }
        public string? Link { get; set; }
        public int? ParentId { get; set; }
        public string? TypeOpen { get; set; }
        public string? Status { get; set; }
        public List<MenuVM>? MenuChild { get; set; }
    }
}
