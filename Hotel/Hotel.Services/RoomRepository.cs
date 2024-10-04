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
    public interface IRoomRepository
    {
        Task Add(Room room);
        Task Delete(int id);
        Task Update(Room room);
        Task<List<Room>> GetAll();
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly HotelContext _context;
        public RoomRepository(HotelContext context)
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

        public  async Task Update(Room newroom)
        {
            var room = _context.Rooms.FirstOrDefault(x => x.Id == newroom.Id);
            if (room != null)
            {
                room.RoomTypeId = newroom.RoomTypeId;
                room.Code = newroom.Code;
                room.Image = newroom.Image;
                room.CreatedAt = newroom.CreatedAt;
                room.UpdatedAt = newroom.UpdatedAt;               
                await _context.SaveChangesAsync();
            }
                
        }
    }
}
