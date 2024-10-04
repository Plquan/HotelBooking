using AutoMapper;
using Hotel.BackendApi.Dtos;
using Hotel.Data.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeService _roomTypeService;
        private readonly IMapper _mapper;
        public RoomTypeController(IRoomTypeService roomTypeService,IMapper mapper) 
        { 
            _roomTypeService = roomTypeService;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("AddRoomType")]
        public  async Task AddRoomType(RoomTypeDTO typeDTO)
        {
            var roomtype = _mapper.Map<RoomType>(typeDTO);
              await _roomTypeService.Add(roomtype);           
        }
        [HttpGet]
        [Route("GetAllRoomType")]
        public async Task<ActionResult<List<RoomTypeDTO>>> GetAllRoomType()
        {
            var roomtype = await _roomTypeService.GetAll();
            return _mapper.Map<List<RoomTypeDTO>>(roomtype);
        }
        [HttpDelete]
        [Route("DeleteRoomType/{id}")]
        public async Task DeleteRoomType(int id)
        {
            await _roomTypeService.Delete(id);
        }
        [HttpPut]
        [Route("UpdateRoomType")]
        public async Task UpdateRoomType(RoomTypeDTO roomTypeDTO)
        {
            var room = _mapper.Map<RoomType>(roomTypeDTO);
            await _roomTypeService.Update(room);
        }

    }
}
