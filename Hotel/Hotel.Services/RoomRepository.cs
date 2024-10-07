using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.ViewModels;
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
        Task<List<RoomVM>> GetAll();
        Task<Room> GetById(int id);  
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
        public async Task<List<RoomVM>> GetAll()
        {
			var query = await ( from r in _context.Rooms
						join t in _context.RoomTypes on r.RoomTypeId equals t.Id
						select new RoomVM
						{
							Id = r.Id,
                            RoomNumber = r.RoomNumber,
                            Capacity = r.Capacity,
                            Price = r.Price,
                            Status = r.Status,
                            TypeName = t.Name
						}).ToListAsync();
            return query;
		}

		public async Task<Room> GetById(int id)
		{
			var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id);
            if(room == null)
            {
                throw new Exception("Không tìm thấy phòng!");
            }
                return room;
         
			      
		}

		public  async Task Update(Room newroom)
        {
            var room = _context.Rooms.FirstOrDefault(x => x.Id == newroom.Id);
            if (room != null)
            {
                room.RoomNumber = newroom.RoomNumber;
                room.Capacity = newroom.Capacity;           
                room.Price = newroom.Price;
                room.Status = newroom.Status;
                await _context.SaveChangesAsync();
            }
                
        }
    }
}
