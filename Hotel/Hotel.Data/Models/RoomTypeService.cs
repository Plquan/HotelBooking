using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Models
{
	public class RoomTypeService
	{
		public int Id { get; set; }
		public int RoomTypeId { get; set; }
		public int ServiceId { get; set; }
		public RoomType RoomType { get; set; }
		public Service Service { get; set; }
	}
}
