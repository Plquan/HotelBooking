using AutoMapper;

using Hotel.Data.Dtos;
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
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IMapper _mapper;
        public RoomTypeController(IRoomTypeRepository roomTypeRepository, IMapper mapper) 
        { 
            _roomTypeRepository = roomTypeRepository;
            _mapper = mapper;
        }
        [HttpPost]
        [Route("AddRoomType")]
        public  async Task AddRoomType(RoomTypeDTO typeDTO)
        {
            var roomtype = _mapper.Map<RoomType>(typeDTO);
              await _roomTypeRepository.Add(roomtype);           
        }
        [HttpGet]
        [Route("GetAllRoomType")]
        public async Task<ActionResult<List<RoomTypeDTO>>> GetAllRoomType()
        {
            var roomtype = await _roomTypeRepository.GetAll();
            return _mapper.Map<List<RoomTypeDTO>>(roomtype);
        }
        [HttpDelete]
        [Route("DeleteRoomType/{id}")]
        public async Task DeleteRoomType(int id)
        {
            await _roomTypeRepository.Delete(id);
        }
        [HttpPut]
        [Route("UpdateRoomType")]
        public async Task UpdateRoomType(RoomTypeDTO roomTypeDTO)
        {
            var room = _mapper.Map<RoomType>(roomTypeDTO);
            await _roomTypeRepository.Update(room);
        }

    }
}
