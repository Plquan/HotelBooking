using AutoMapper;
using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Enum;
using Hotel.Data.Models;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Data.ViewModels.RoomTypes;
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
        Task<CheckRoomVM> CheckRoomById(CheckDate date);
        Task<List<BookingVM>> GetAll();
        Task<int> PlaceOrder(BookingVM bookingVM);
        Task<int> ConfirmBooking(int bookingId);
        Task<bool> PaymentSuccess(int BookingId);
    }

    public class CheckDate
    {
        public int? RoomTypeId { get; set; }
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
                                       RoomImages = _mapper.Map<List<RoomImageModel>>(rt.RoomImages.ToList()),
                                       Rooms = (from r in _context.Rooms
                                                where r.RoomTypeId == rt.Id && r.Status == RoomStatus.Active
                                                select new RoomDTO()
                                                {
                                                    Id = r.Id,
                                                    RoomNumber = r.RoomNumber!,
                                                }).ToList()
                                   }).ToListAsync();
            return roomTypes;                  
        }

        public async Task<List<BookingVM>> GetAll()
        {
            var bookings = await _context.Bookings.ToListAsync();
            return _mapper.Map<List<BookingVM>>(bookings);
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
                UserName = bookingVM.UserName,
                Email = bookingVM.Email,
                Phone = bookingVM.Phone,
                Note = bookingVM.Note,
                Code = GenerateCode(),
                TotalPrice = bookingVM.TotalPrice,
                TotalPerson = bookingVM.TotalPerson,
                FromDate = bookingVM.FromDate,
                ToDate = bookingVM.ToDate,
                CreatedDate = DateTime.Now,
                PaymentMethod = bookingVM.PaymentMethod,
                PaymentStatus = bookingVM.PaymentMethod == PaymentStatus.CheckedIn ? PaymentStatus.Unpaid : PaymentStatus.PaymentPending,
                Status = PaymentStatus.Pending
            };
             _context.Bookings.Add(newBooking);
             _context.SaveChanges();
            return newBooking.Id;
        }
        public async Task<int> PlaceOrder(BookingVM bookingVM)
        {
            var BookingId = CreateBooking(bookingVM);
            foreach (var book in bookingVM.ChooseRooms!) {

                var rooms = (from r in _context.Rooms
                             where r.RoomTypeId == book.RoomTypeId && r.Status == RoomStatus.Active                                                                
                             select new RoomDTO()
                             {
                                 Id = r.Id,
                                 RoomNumber = r.RoomNumber,
                             }).Take(book.Number).ToList();

                    foreach (var room in rooms) {
                    var bookingRoom = new BookingDetail()
                    {
                        BookingId = BookingId,
                        RoomId = room.Id
                    };
                    _context.BookingDetails.Add(bookingRoom);
                }
            }
             await _context.SaveChangesAsync();
            return BookingId;
        }

        public async Task<CheckRoomVM> CheckRoomById(CheckDate date)
        {
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
                                       RoomImages = _mapper.Map<List<RoomImageModel>>(rt.RoomImages.ToList()),
                                       Rooms = (from r in _context.Rooms
                                                where r.RoomTypeId == rt.Id && r.Status == RoomStatus.Active
                                                select new RoomDTO()
                                                {
                                                    Id = r.Id,
                                                    RoomNumber = r.RoomNumber!,
                                                }).ToList()
                                   }).FirstOrDefaultAsync(r => r.Id == date.RoomTypeId);
            return roomTypes;
        }

        public Task<int> ConfirmBooking(int bookingId)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> PaymentSuccess(int BookingId)
        {
            var booking  = await _context.Bookings.Where(r => r.Id == BookingId).FirstOrDefaultAsync();
           if (booking == null)
            {
                return false;
            }
            booking.PaymentStatus = PaymentStatus.Paid;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
