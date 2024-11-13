using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
    public class Menu
    {
        public  int Id { get; set; }
        public string Name { get; set; }
        public int Ordering {  get; set; }
        public string Link { get; set; }
        public int Parent_Id { get; set; }
        public int Lft {  get; set; }
        public int Rgt { get; set; }
        public string Type_Menu { get; set; }
        public string Type_Open { get; set; }
        public string Status { get; set; }
    }
}
