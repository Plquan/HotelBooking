﻿using Hotel.Data.Models;

namespace Hotel.Data.Dtos
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public int RoomNumber { get; set; }
        public int Capacity { set; get; }
		public decimal Price { set; get; }
		public int Status { get; set; }
	}
}
