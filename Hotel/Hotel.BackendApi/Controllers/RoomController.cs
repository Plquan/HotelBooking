using Hotel.Data;
using Hotel.Data.Models;
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
        public RoomController(HotelContext Context) { 
            _context = Context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetAllRoom()
        {
            var rooms = await _context.Rooms.ToListAsync();
            return rooms;
        }
        [HttpPost]
        public async Task<ActionResult<int>> AddRoom(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room.Id;
        }
    }
}
