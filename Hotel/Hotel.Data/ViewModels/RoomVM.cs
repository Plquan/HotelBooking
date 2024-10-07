using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.ViewModels
{
    public class RoomVM
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public int RoomNumber { get; set; }
        public int Capacity { set; get; }
		public decimal Price { set; get; }
		public bool Status { get; set; }
    }
}
