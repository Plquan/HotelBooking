using Hotel.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Dtos
{
	public class RoomServiceDTO
	{
		public int Id { get; set; }
		public int RoomTypeId { get; set; }
		public string Name { get; set; }	
	}
}
