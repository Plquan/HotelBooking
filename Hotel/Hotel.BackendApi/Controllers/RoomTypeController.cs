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
        [Route("Add")]
        public  async Task Add(RoomTypeDTO typeDTO)
        {
            var roomtype = _mapper.Map<RoomType>(typeDTO);
              await _roomTypeRepository.Add(roomtype);           
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<List<RoomTypeDTO>>> GetAll()
        {
            var roomtype = await _roomTypeRepository.GetAll();
            return _mapper.Map<List<RoomTypeDTO>>(roomtype);
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task Delete(int id)
        {
            await _roomTypeRepository.Delete(id);
        }
        [HttpPut]
        [Route("Update")]
        public async Task Update(RoomTypeDTO roomTypeDTO)
        {
            var room = _mapper.Map<RoomType>(roomTypeDTO);
            await _roomTypeRepository.Update(room);
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<RoomTypeDTO> GetById(int id)
        {
            var roomtype = await _roomTypeRepository.GetById(id);
            return _mapper.Map<RoomTypeDTO>(roomtype);
        }
    }
}
