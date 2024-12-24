using AutoMapper;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.ViewModels.AppUsers;
using Hotel.Data.ViewModels.Auth;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Data.ViewModels.RoomTypes;
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
			CreateMap<RoomType, RoomType>().ReverseMap();
			CreateMap<RoomImage, RoomImageModel>().ReverseMap();
			CreateMap<RoomFacility, RoomFacilityModel>().ReverseMap();
            CreateMap<Booking, BookingModel>().ReverseMap();
            CreateMap<AppUser, PublicUserVM>().ReverseMap();
            CreateMap<AppUser, AppUserModel>().ReverseMap();

        }
	}
}
