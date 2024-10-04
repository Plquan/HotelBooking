using AutoMapper;
using Hotel.BackendApi.Dtos;
using Hotel.Data.Models;

namespace Hotel.BackendApi.Mappings
{
    public class AutoMapperConfig: Profile
    {
       public AutoMapperConfig() {        
        CreateMap<Room,RoomDTO>().ReverseMap();
        CreateMap<RoomType, RoomTypeDTO>().ReverseMap();
        CreateMap<Supply, SupplyDTO>().ReverseMap();

        }
    }
}
