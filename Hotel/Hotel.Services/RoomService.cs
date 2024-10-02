using Hotel.Data;
using Hotel.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IRoomService 
    {
        Task Add(Room room);
        Task Delete(int id);
        Task Update(Room room);
        Task<List<Room>> GetAll();
    }

    public class RoomService : IRoomService
    {
        private readonly HotelContext _context;
        public RoomService(HotelContext context)
        {
            _context = context;
        }

        public async Task Add(Room room)
        {
            _context.Rooms.Add(room);
          await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Id == id);
           if (room != null)
            {
                _context.Remove(room);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Room>> GetAll()
        {
            return await _context.Rooms.ToListAsync();
        }

        public Task Update(Room room)
        {
            throw new NotImplementedException();
        }
    }
}
