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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hotel.Services
{
    public interface IBookingService
    {
        Task<List<CheckRoomVM>> CheckRoom(CheckDate date);
        Task<List<BookingDTO>> GetAll();
        Task PlaceOrder(BookingVM bookingVM);
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

            if (date.FromDate == null || date.ToDate == null)
            {
                return [];
            }
            var roomTypes = await (from rt in _context.RoomTypes
                                   select new CheckRoomVM()
                                   {
                                       Id = rt.Id,
                                       Name = rt.Name,
                                       Content = rt.Content,
                                       Slug = rt.Slug,
                                       View = rt.View,
                                       Size = rt.Size,
                                       BedType = rt.BedType,
                                       Capacity = rt.Capacity,
                                       Price = rt.Price,
                                       RoomImages = _mapper.Map<List<RoomImageDTO>>(rt.RoomImages.ToList()),
                                       Rooms = (from r in _context.Rooms
                                                where r.RoomTypeId == rt.Id &&
                                                      !_context.Bookings.Any(b =>  
                                                          _context.BookingRooms.Any(br =>  
                                                              br.RoomId == r.Id &&        
                                                              br.BookingId == b.Id &&      
                                                              (
                                                                  (b.FromDate <= date.FromDate && date.FromDate <= b.ToDate) || 
                                                                  (b.FromDate <= date.ToDate && date.ToDate <= b.ToDate) ||     
                                                                  (b.FromDate >= date.FromDate && date.ToDate >= b.ToDate)  
                                                              )
                                                          )
                                                      )
                                                select new RoomDTO()
                                                {
                                                    Id = r.Id,
                                                    RoomNumber = r.RoomNumber,
                                                }).ToList()
                                   }).ToListAsync();
            return roomTypes;
                        
        }

        public async Task<List<BookingDTO>> GetAll()
        {
            var bookings = await _context.Bookings.ToListAsync();
            return _mapper.Map<List<BookingDTO>>(bookings);
        }

        public  string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder code = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < 8; i++)
            {
                code.Append(chars[random.Next(chars.Length)]);
            }
            string generatedCode = code.ToString();         
            return generatedCode; 
        }
    public int CreateBooking(BookingVM bookingVM)
        {
            var newBooking = new Booking()
            {
                Name = bookingVM.Name,
                Email = bookingVM.Email,
                Phone = bookingVM.Phone,
                Note = bookingVM.Note,
                Code = GenerateCode(),
                TotalPrice = bookingVM.Totalprice,
                TotalPerson = bookingVM.TotalPerson,
                FromDate = bookingVM.FromDate,
                ToDate = bookingVM.ToDate,
                CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                Status = "booked"
            };
            _context.Bookings.Add(newBooking);
             _context.SaveChanges();
            return newBooking.Id;
        }
        public async Task PlaceOrder(BookingVM bookingVM)
        {
            var BookingId = CreateBooking(bookingVM);
            foreach (var book in bookingVM.ChooseRooms) {

                var rooms = (from r in _context.Rooms
                             where r.RoomTypeId == book.Id &&
                                   !_context.Bookings.Any(b =>
                                       _context.BookingRooms.Any(br =>
                                           br.RoomId == r.Id &&
                                           br.BookingId == b.Id &&
                                           (
                                               (b.FromDate <= bookingVM.FromDate && bookingVM.FromDate <= b.ToDate) ||
                                               (b.FromDate <= bookingVM.ToDate && bookingVM.ToDate <= b.ToDate) ||
                                               (b.FromDate >= bookingVM.FromDate && bookingVM.ToDate >= b.ToDate)
                                           )
                                       )
                                   )
                             select new RoomDTO()
                             {
                                 Id = r.Id,
                                 RoomNumber = r.RoomNumber,
                             }).Take(book.Number).ToList();

                    foreach (var room in rooms) {
                    var bookingRoom = new BookingRoom()
                    {
                        BookingId = BookingId,
                        RoomId = room.Id
                    };
                    _context.BookingRooms.Add(bookingRoom);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
