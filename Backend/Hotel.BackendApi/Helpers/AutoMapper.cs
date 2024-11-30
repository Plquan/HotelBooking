using AutoMapper;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.BackendApi.Helpers
{
	public class AutoMapper: Profile
	{
		public AutoMapper()
		{
			CreateMap<Room, RoomDTO>().ReverseMap();
			CreateMap<RoomType, RoomTypeDTO>().ReverseMap();
			CreateMap<RoomImage, RoomImageDTO>().ReverseMap();
			CreateMap<RoomFacility, RoomFacilityDTO>().ReverseMap();
            CreateMap<Booking, BookingDTO>().ReverseMap();


        }
	}
}
