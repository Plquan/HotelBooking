using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Dtos
{
	public class RoomImageDTO
	{
		public int Id { get; set; }
		public int RoomTypeId { get; set; }
		public string Url { get; set; }

	}
}
