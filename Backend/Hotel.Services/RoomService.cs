using AutoMapper;
using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.ViewModels.Rooms;
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
        Task Add(RoomDTO roomDTO);
        Task Delete(int id);
        Task Update(Room room);
        Task<List<RoomVM>> GetAll();
        Task<Room> GetById(int id);
        Task<string> ChangeStatus(int id);
        Task<Paging<RoomVM>> GetListPaging(int pageIndex, int pageSize);
    }

    public class RoomService : IRoomService
    {
        private readonly HotelContext _context;
		private readonly IMapper _mapper;
        private readonly IPagingService _pagingService;
		public RoomService(HotelContext context, IMapper mapper , IPagingService pagingService)
        {
            _context = context;
            _mapper = mapper;
            _pagingService = pagingService;
        }

        public async Task Add(RoomDTO roomDTO)
        {
            var room = new Room() { 
            RoomNumber = roomDTO.RoomNumber,
            RoomTypeId = roomDTO.RoomTypeId,
            Status = roomDTO.Status
            };
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
                            Status = r.Status,
                            TypeName = t.Name
						}).Distinct().ToListAsync();
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
                room.Status = newroom.Status;
                await _context.SaveChangesAsync();
            }
                
        }

        public async Task<string> ChangeStatus(int id)
        {
            var room = _context.Rooms.FirstOrDefault(x => x.Id == id);

            if (room != null)
            {
                room.Status = room.Status == "active" ? "inActive" : "active";
                await _context.SaveChangesAsync();
                return room.Status;
            }
            else
            {
                throw new Exception("không tìm thấy phòng");
            }

        }

        public async Task<Paging<RoomVM>> GetListPaging(int pageIndex, int pageSize)
        {
            var query = from r in _context.Rooms
                              join t in _context.RoomTypes on r.RoomTypeId equals t.Id
                              select new RoomVM
                              {
                                  Id = r.Id,
                                  RoomNumber = r.RoomNumber,
                                  Status = r.Status,
                                  TypeName = t.Name
                              };
            return await _pagingService.GetPagedAsync<RoomVM>(query, pageIndex, pageSize);
        }
    }



}

