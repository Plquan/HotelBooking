﻿using Hotel.Data;
using Hotel.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IRoomTypeRepository
    {
        Task Add(RoomType room);
        Task Delete(int id);
        Task Update(RoomType room);
        Task<List<RoomType>> GetAll();
        Task<RoomType> GetById(int id);
    }


    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly HotelContext _context;
        public RoomTypeRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task Add(RoomType room)
        {
           _context.RoomTypes.Add(room);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var roomtype = _context.RoomTypes.FirstOrDefault(r => r.Id == id);
            if (roomtype != null)
            {
                _context.Remove(roomtype);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<RoomType>> GetAll()
        {
            return await _context.RoomTypes.ToListAsync();
        }

		public async Task<RoomType> GetById(int id)
		{
			var room = await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == id);
			if (room == null)
			{
				throw new Exception("Không tìm thấy loại phòng!");
			}
			return room;
		}

		public async Task Update(RoomType newtype)
        {
            var roomtype = _context.RoomTypes.FirstOrDefault(x => x.Id == newtype.Id);
            if (roomtype != null)
            {
                roomtype.Name = newtype.Name;
                roomtype.Description = newtype.Description;
                await _context.SaveChangesAsync();
            }         

        }



    }
}
