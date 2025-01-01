using AutoMapper;
using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Enum;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Data.ViewModels.Rooms;
using Hotel.Data.ViewModels.RoomTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Hotel.Services
{
    public interface IBookingService
    {
        Task<List<CheckRoomVM>> CheckRoom(CheckDate date);
        Task<CheckRoomVM> CheckRoomById(CheckDate date);
        Task<ApiResponse> GetAll();
        Task<Paging<BookingVM>> GetListPaging(PagingModel model);
        Task<int> PlaceOrder(BookingModel bookingVM);
        Task<ApiResponse> DeleteBooking(int bookingId);
        Task<ApiResponse> UpdatePaymentStatus(int BookingId,string status);
        Task <ApiResponse> GetBookingDetail(int bookingId);
        Task <ApiResponse> UpdateStatus(int bookingId,string status);

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
        private readonly IPagingService _pagingService;
        private readonly IConfiguration _configuration;
        public BookingService( HotelContext context, IMapper mapper, IPagingService pagingService,IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _pagingService = pagingService;
            _configuration = configuration;
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
                                       RoomImages = _mapper.Map<List<RoomImageModel>>(rt.RoomImages!.ToList()),
                                       Rooms = (from r in _context.Rooms
                                                where r.RoomTypeId == rt.Id && r.Status == RoomStatus.Active &&
                                                      !_context.Bookings.Any(b =>
                                                          _context.BookingDetails.Any(br =>
                                                              br.RoomId == r.Id &&
                                                              br.BookingId == b.Id &&
                                                              (
                                                                  (b.FromDate <= date.FromDate && date.FromDate <= b.ToDate) ||
                                                                  (b.FromDate <= date.ToDate && date.ToDate <= b.ToDate) ||
                                                                  (b.FromDate >= date.FromDate && date.ToDate >= b.ToDate)
                                                              )
                                                              && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckIn)
                                                          )
                                                      )                                        
                                                select new RoomDTO()
                                                {
                                                    Id = r.Id,
                                                    RoomNumber = r.RoomNumber!,
                                                }).ToList()
                                   }).ToListAsync();
            return roomTypes;                  
        }
        public async Task<ApiResponse> GetAll()
        {
            var bookings = await ( from b in _context.Bookings.Where(s => s.Status != BookingStatus.CheckOut && s.Status != BookingStatus.Cancelled)
                           select new BookingVM
                           {
                               Id = b.Id,
                               UserName = b.UserName,
                               Email = b.Email,
                               Phone = b.Phone,
                               Code = b.Code,
                               Note = b.Note,
                               TotalPerson = b.TotalPerson,
                               TotalPrice = b.TotalPrice,
                               FromDate = b.FromDate,
                               ToDate = b.ToDate,
                               PaymentMethod = b.PaymentMethod,
                               PaymentStatus = b.PaymentStatus,
                               CreatedDate = b.CreatedDate,
                               ConfirmBy = b.ConfirmBy,
                               Status = b.Status,
                               Rooms = (from r in _context.Rooms
                                        join
                                            bd in _context.BookingDetails on r.Id equals bd.RoomId
                                        where bd.BookingId == b.Id
                                        select new RoomVM
                                        {
                                            Id = r.Id,
                                            RoomNumber = r.RoomNumber
                                        }).ToList()
                           }).ToListAsync();

            return new ApiResponse { 
            StatusCode = 200,
            IsSuccess = true,
            Data = bookings,
            Message = "Lấy dữ liệu thành công"
            };

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
        public int CreateBooking(BookingModel bookingVM)
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
        public async Task<int> PlaceOrder(BookingModel bookingVM)
        {
            var BookingId = CreateBooking(bookingVM);
            foreach (var book in bookingVM.ChooseRooms!)
            {
                var rooms = (from r in _context.Rooms
                             where r.RoomTypeId == book.RoomTypeId && r.Status == RoomStatus.Active &&
                                   !_context.Bookings.Any(b =>
                                       _context.BookingDetails.Any(br =>
                                           br.RoomId == r.Id &&
                                           br.BookingId == b.Id &&
                                       (
                                          (b.FromDate <= bookingVM.FromDate && bookingVM.FromDate <= b.ToDate) ||
                                          (b.FromDate <= bookingVM.ToDate && bookingVM.ToDate <= b.ToDate) ||
                                          (b.FromDate >= bookingVM.FromDate && bookingVM.ToDate >= b.ToDate)
                                       )
                                           && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckIn)
                                       )
                                   )
                             select new BookingDetail()
                             {
                                 BookingId = BookingId,
                                 RoomId = r.Id
                             }).Take(book.Number).ToList();
                if(rooms.Count < 1)
                {
                    throw new Exception("Không tìm thấy phòng");
                }
                _context.BookingDetails.AddRange(rooms);
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
                                       RoomImages = _mapper.Map<List<RoomImageModel>>(rt.RoomImages!.ToList()),
                                       Rooms = (from r in _context.Rooms
                                                where r.RoomTypeId == rt.Id && r.Status == RoomStatus.Active &&
                                                      !_context.Bookings.Any(b =>
                                                          _context.BookingDetails.Any(br =>
                                                              br.RoomId == r.Id &&
                                                              br.BookingId == b.Id &&
                                                              (
                                                                  (b.FromDate <= date.FromDate && date.FromDate <= b.ToDate) ||
                                                                  (b.FromDate <= date.ToDate && date.ToDate <= b.ToDate) ||
                                                                  (b.FromDate >= date.FromDate && date.ToDate >= b.ToDate)
                                                              )
                                                              && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.CheckIn)
                                                          )
                                                      )
                                                select new RoomDTO()
                                                {
                                                    Id = r.Id,
                                                    RoomNumber = r.RoomNumber!,
                                                }).ToList()
                                   }).FirstOrDefaultAsync(r => r.Id == date.RoomTypeId);
            return roomTypes;
        }
        public async Task<ApiResponse> UpdatePaymentStatus(int BookingId, string status)
        {
            var booking  = await _context.Bookings.FirstOrDefaultAsync(r => r.Id == BookingId);
           if (booking == null)
            {
                return new ApiResponse { 
                 StatusCode = 400,
                 IsSuccess = false,
                 Message = "Cập nhật trạng thái thanh toán không thành công"
                };

            }
            booking.PaymentStatus = status;
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Cập nhật thành công"
            };

        }

        public async Task<ApiResponse> DeleteBooking(int bookingId)
        {
           var booking = await _context.Bookings.FirstOrDefaultAsync(r => r.Id == bookingId);
            if (booking == null)
            {
                throw new Exception("Không tìm thấy phòng");
            }
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Xóa thành công"
            };
        }

        public async Task<Paging<BookingVM>> GetListPaging(PagingModel model)
        {
            var query = from b in _context.Bookings
                        select new BookingVM
                        {
                            Id = b.Id,
                            UserName = b.UserName,
                            Email = b.Email,
                            Phone = b.Phone,
                            Code = b.Code,
                            Note = b.Note,
                            TotalPerson = b.TotalPerson,
                            TotalPrice = b.TotalPrice,
                            FromDate = b.FromDate,
                            ToDate = b.ToDate,
                            PaymentMethod = b.PaymentMethod,
                            PaymentStatus = b.PaymentStatus,
                            CreatedDate = b.CreatedDate,
                            ConfirmBy = b.ConfirmBy,
                            Status = b.Status                       
                        };
            return await _pagingService.GetPagedAsync<BookingVM>(query, model.PageIndex,model.PageSize);
        }

        public async Task<ApiResponse> GetBookingDetail(int bookingId)
        {
            var bookingDetail = await (from b in _context.Bookings                               
                                 select new BookingVM
                                 {
                                     Id = b.Id,
                                     UserName = b.UserName,
                                     Email = b.Email,
                                     Phone = b.Phone,
                                     Code = b.Code,
                                     Note = b.Note,
                                     TotalPerson = b.TotalPerson,
                                     TotalPrice = b.TotalPrice,
                                     FromDate = b.FromDate,
                                     ToDate = b.ToDate,
                                     PaymentMethod = b.PaymentMethod,
                                     PaymentStatus = b.PaymentStatus,
                                     CreatedDate = b.CreatedDate,
                                     ConfirmBy = b.ConfirmBy,
                                     Status = b.Status,
                                     Rooms = (from r in _context.Rooms join
                                              bd in _context.BookingDetails on r.Id equals bd.RoomId 
                                              where bd.BookingId == bookingId
                                              select new RoomVM { 
                                               Id = r.Id,
                                               RoomNumber = r.RoomNumber
                                              }).ToList()
                                 }).FirstOrDefaultAsync(i => i.Id == bookingId);
            if (bookingDetail == null)
            {
                return new ApiResponse
                {
                    StatusCode = 404,
                    IsSuccess = false,
                    Message = "Không tìm thấy phòng"
                };

            }
            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Lấy thành công",
                Data = bookingDetail
            };
        }

        public async Task<ApiResponse> UpdateStatus(int bookingId, string status)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(r => r.Id == bookingId);
            if (booking == null)
            {
                return new ApiResponse
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    Message = "Cập nhật trạng thái đơn không thành công"
                };

            }
            booking.Status = status;
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Cập nhật thành công"
            };
        }

    }
}
