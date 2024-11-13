using AutoMapper;

using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly HotelContext _context;
        private readonly IRoomRepository _roomRepository;
        private readonly IMapper _mapper;
        public RoomController(HotelContext Context , IRoomRepository roomRepository, IMapper mapper) { 
            _context = Context;
            _roomRepository = roomRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<List<RoomVM>>> GetAll()
        {
            var rooms = await  _roomRepository.GetAll();
            return  rooms;
        }
        [HttpPost]
        [Route("Add")]
        public async Task Add(RoomDTO room)
        {     
          await  _roomRepository.Add(room);
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task Delete(int id)
        {
            await _roomRepository.Delete(id);
        }
        [HttpPut]
        [Route("Update")]
        public async Task Update(RoomDTO roomdto)
        {
            var room = _mapper.Map<Room>(roomdto);
            await _roomRepository.Update(room);
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<RoomDTO> GetById(int id)
        {
			var room = await _roomRepository.GetById(id);
			return  _mapper.Map<RoomDTO>(room);
		}
        [HttpPost]
        [Route("ChangeStatus/{id}")]
        public async Task<string> ChangeStatus(int id)
        {
            return await _roomRepository.ChangeStatus(id);
        }
    }
}
