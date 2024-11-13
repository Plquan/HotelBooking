using AutoMapper;
using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IBookingService
    {
        Task<List<CheckRoomVM>> CheckRoom(CheckDate date);
    }

    public class CheckDate
    {
        public DateOnly? FromDate {  get; set; }
        public DateOnly? ToDate { get; set; }
    }
    public class BookingService : IBookingService
    {
        private readonly  HotelContext _context;
        private readonly IMapper _mapper;
        public BookingService( HotelContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<CheckRoomVM>> CheckRoom(CheckDate date)
        {
            
            if(date.FromDate == null || date.ToDate == null)
            {          
                return [];
            }
        
                var rooms = await (from rt in _context.RoomTypes
                                   join r in _context.Rooms on rt.Id equals r.RoomTypeId
                                   join pr in _context.PaymentRooms on r.Id equals pr.RoomId into prGroup
                                   from pr in prGroup.DefaultIfEmpty()
                                   join p in _context.Payments on pr.PaymentId equals p.Id into pGroup
                                   from p in pGroup.DefaultIfEmpty()
                                   where p == null || p.ToDate < date.FromDate || p.FromDate > date.ToDate
                                   select new CheckRoomVM()
                                   {
                                       Id = rt.Id,
                                       Name = rt.Name,
                                       Content = rt.Content,
                                       View = rt.View,
                                       Size = rt.Size,
                                       BedType = rt.BedType,
                                       Capacity = rt.Capacity,
                                       Thumb = rt.Thumb,
                                       Price = rt.Price,
                                       RoomCount = rt.Rooms.Count()
                                   }).Distinct().ToListAsync();

                return rooms;
            
            
      
        }

    }
}
