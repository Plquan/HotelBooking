using AutoMapper;
using Hotel.BackendApi.Dtos;
using Hotel.Data;
using Hotel.Data.Models;
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
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        public RoomController(HotelContext Context , IRoomService roomService,IMapper mapper) { 
            _context = Context;
            _roomService = roomService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<List<RoomDTO>>> GetAllRoom()
        {
            var rooms = await  _roomService.GetAll();
            return  _mapper.Map<List<RoomDTO>>(rooms);
        }
        [HttpPost]
        [Route("Add")]
        public async Task AddRoom(RoomDTO roomdto)
        {
            var room = _mapper.Map<Room>(roomdto);
          await  _roomService.Add(room);
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task DeleteRoom(int id)
        {
            await _roomService.Delete(id);
        }
        [HttpPut]
        [Route("Update")]
        public async Task UpdateRoom(RoomDTO roomdto)
        {
            var room = _mapper.Map<Room>(roomdto);
            await _roomService.Update(room);
        }

    }
}
