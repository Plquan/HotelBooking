using AutoMapper;
using Hotel.BackendApi.Dtos;
using Hotel.Data;
using Hotel.Data.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<List<Room>>> GetAllRoom()
        {
            var rooms = await  _roomService.GetAll();
            return  _mapper.Map<List<Room>>(rooms);
        }
        [HttpPost]
        public async Task AddRoom(RoomDTO roomdto)
        {
            var room = _mapper.Map<Room>(roomdto);
          await  _roomService.Add(room);
        }
    }
}
