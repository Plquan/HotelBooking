using AutoMapper;
using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Enum;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.Ultils.Email;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Data.ViewModels.Rooms;
using Hotel.Data.ViewModels.RoomTypes;
using Hotel.Data.ViewModels.Vnpay;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

using static System.Runtime.InteropServices.JavaScript.JSType;


namespace Hotel.Services
{
    public interface IBookingService
    {
        Task<ApiResponse> CheckRoom(CheckDate date);
        Task<CheckRoomVM> CheckRoomById(CheckDate date);
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetBookingById();
        Task<Paging<BookingVM>> GetListPaging(PagingModel model);
        Task<ApiResponse> PlaceOrder(BookingModel bookingVM,HttpContext context);
        Task<ApiResponse> DeleteBooking(int bookingId);
        Task<ApiResponse> UpdatePaymentStatus(UpdateBookingStatus model);
        Task <ApiResponse> GetBookingDetail(int bookingId);
        Task <ApiResponse> UpdateStatus(UpdateBookingStatus model);
        Task<Paging<BookingVM>> GetPaymentHistory(PagingModel model);
        Task<ApiResponse> GetTransactionDetail(int bookingId);
        Task<ApiResponse> GetBooked();

    }

    public class CheckDate
    {
        public int? RoomTypeId { get; set; }
        public DateOnly? FromDate {  get; set; }
        public DateOnly? ToDate { get; set; }
    }
    public class UpdateBookingStatus 
    {
      public int? BookingId { get; set; }
      public string? Status { get; set; }
    }
    public class BookingService : IBookingService
    {
        private readonly  HotelContext _context;
        private readonly IMapper _mapper;
        private readonly IPagingService _pagingService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMailService _mailService;
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<BookingService> _logger;

        public BookingService(HotelContext context, 
            IMapper mapper, 
            IPagingService pagingService, 
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IMailService mailService,
            IVnPayService vnPayService,
            ILogger<BookingService> logger)
        {
            _context = context;
            _mapper = mapper;
            _pagingService = pagingService;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mailService = mailService;
            _vnPayService = vnPayService;
            _logger = logger;

        }

        public async Task<bool> CheckSaved(int roomId)
        {
            var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return false;
            }

            var check = await _context.SavedRooms.Where(x => x.AppUserId == userId).Where(x => x.RoomTypeId == roomId).CountAsync();
            if (check > 0) {
                return true;
            }
            return false;
                  
        }
        public async Task<ApiResponse> CheckRoom(CheckDate date)
        {

            if (date.FromDate == null || date.ToDate == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = 200,
                    Message = "Lỗi ngày tháng"
                }; ;
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
                                       AvailableRooms = (from r in _context.Rooms
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
                                                              && (b.Status != BookingStatus.CheckOut && b.Status != BookingStatus.Cancelled)
                                                          )
                                                      )                                        
                                                select new RoomDTO()
                                                {
                                                    Id = r.Id,
                                                    RoomNumber = r.RoomNumber!,
                                                }).Count()
                                   }).ToListAsync();
            foreach (var roomType in roomTypes)
            {
                roomType.IsSaved = await CheckSaved(roomType.Id);
            }
            return new ApiResponse { 
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "Lấy dữ liệu thành công",
                        Data = roomTypes
                    };

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
        public Booking CreateBooking(BookingModel bookingVM)
        {
                DateOnly fromDate = bookingVM.FromDate;
                DateOnly toDate = bookingVM.ToDate;

                int totalDays = toDate.DayNumber - fromDate.DayNumber;
                Decimal totalPrice = 0;

                foreach (var choosed in bookingVM.ChooseRooms!)
                {
                    totalPrice += (choosed.Price * totalDays) * choosed.Number;
                }
                var newBooking = new Booking
                {
                    Note = bookingVM.Note,
                    Code = GenerateCode(),
                    TotalPrice = totalPrice,
                    TotalPerson = bookingVM.TotalPerson,
                    FromDate = bookingVM.FromDate,
                    ToDate = bookingVM.ToDate,
                    CreatedDate = DateTime.Now,
                    PaymentMethod = bookingVM.PaymentMethod,
                    PaymentStatus = PaymentStatus.Unpaid,
                    Status = PaymentStatus.Pending,
                    AppUserId = bookingVM?.AppUserId,
                    UserName = bookingVM!.UserName,
                    Email = bookingVM!.Email,
                    Phone = bookingVM!.Phone
                };
                _context.Bookings.Add(newBooking);
                _context.SaveChanges();
                return newBooking;
        }
        public async Task<ApiResponse> PlaceOrder(BookingModel bookingVM, HttpContext context)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var booking = CreateBooking(bookingVM);
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
                                               && (b.Status != BookingStatus.CheckOut && b.Status != BookingStatus.Cancelled)
                                           )
                                       )
                                 select new BookingDetail
                                 {
                                     BookingId = booking.Id,
                                     RoomId = r.Id
                                 }).Take(book.Number).ToList();

                    if (rooms.Count < book.Number)
                    {
                        await DeleteBooking(booking.Id);
                        throw new Exception($"Không tìm đủ {book.Number} phòng cho loại {book.RoomTypeId}");
                    }

                    _context.BookingDetails.AddRange(rooms);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                if(booking.PaymentMethod == "OP")
                {
                    var paymentModel = new PaymentInformationModel {
                        OrderType = "other",
                        Amount = (double)booking.TotalPrice!,
                        Name = booking.UserName,
                        OrderDescription = booking.Note,
                        Booking = booking
                    };

                    var url = await _vnPayService.CreatePaymentUrl(paymentModel, context);
                    return new ApiResponse
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "Đặt phòng thành công",
                        Data = url
                    };

                }

                return new ApiResponse { 
                      IsSuccess = true,
                      StatusCode = 200,
                      Message = "Đặt phòng thành công",
                      Data = booking
                };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while logging in: {ex.Message} at {DateTime.UtcNow}");
                await transaction.RollbackAsync();
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi tạo đơn đặt phòng",             
                };
            }
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
                                       AvailableRooms = (from r in _context.Rooms
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
                                                              && (b.Status != BookingStatus.CheckOut && b.Status != BookingStatus.Cancelled)
                                                          )
                                                      )
                                                select new RoomDTO()
                                                {
                                                    Id = r.Id,
                                                    RoomNumber = r.RoomNumber!,
                                                }).Count()
                                   }).FirstOrDefaultAsync(r => r.Id == date.RoomTypeId);
            return roomTypes;
        }
        public async Task<ApiResponse> UpdatePaymentStatus(UpdateBookingStatus model)
        {
            var booking  = await _context.Bookings.FirstOrDefaultAsync(r => r.Id == model.BookingId);
           if (booking == null)
            {
                return new ApiResponse { 
                 StatusCode = 400,
                 IsSuccess = false,
                 Message = "Cập nhật trạng thái thanh toán không thành công"
                };

            }
            booking.PaymentStatus = model.Status;
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
                        orderby b.CreatedDate descending
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
                            ConfirmDate = b.ConfirmDate,
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
        public async Task<ApiResponse> UpdateStatus(UpdateBookingStatus model)
        {
            var cUserName = _httpContextAccessor?.HttpContext?.User.FindFirst(ClaimTypes.Name)?.Value;
            if (cUserName == null)
            {
                throw new Exception("Không tim thấy người dùng");
            }
            var booking = await _context.Bookings.FirstOrDefaultAsync(r => r.Id == model.BookingId);
            if (booking == null)
            {
                return new ApiResponse
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    Message = "Không tìm thấy đơn phòng"
                };
            }

            booking.Status = model.Status;
            if (model.Status == BookingStatus.Confirmed) {
                // ktr p trống
                var cRooms = from r in _context.Rooms
                             where r.Status == RoomStatus.Active &&
                                   !_context.Bookings.Any(b =>
                                       _context.BookingDetails.Any(br =>
                                           br.RoomId == r.Id &&
                                           br.BookingId == b.Id &&
                                           (
                                               (b.FromDate <= booking.FromDate && booking.FromDate <= b.ToDate) ||
                                               (b.FromDate <= booking.ToDate && booking.ToDate <= b.ToDate) ||
                                               (b.FromDate >= booking.FromDate && booking.ToDate >= b.ToDate)
                                           )
                                           && (b.Status != BookingStatus.CheckOut && b.Status != BookingStatus.Cancelled)
                                       ))
                             select r.Id;
                var checkRoom = (from r in _context.Rooms
                                 join bd in _context.BookingDetails on r.Id equals bd.RoomId
                                 where bd.BookingId == booking.Id
                                 select r.Id).Any(i => cRooms.Contains(i));
                if (checkRoom)
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Không có phòng trống"
                    };
                }
                // gửi mail
                booking.PaymentStatus = PaymentStatus.Paid;
                var bodyHtml = EmailMessage.EmailBody(booking.Code!);
                var Title = "Đơn phòng của bạn đã được xác nhận";
                await _mailService.SendEmailAsync(booking.Email!, Title, bodyHtml);
                //
                booking.ConfirmBy = cUserName;
                booking.ConfirmDate = DateTime.Now;
            }
            if(model.Status == BookingStatus.Cancelled)
            {
                var bodyHtml = EmailMessage.EmailBody(booking.Code!);
                var Title = "Đơn phòng của bạn đã được hủy";
                await _mailService.SendEmailAsync(booking.Email!, Title, bodyHtml);
            }
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Cập nhật thành công"
            };
        }
        public async Task<Paging<BookingVM>> GetPaymentHistory(PagingModel model)
        {
            var query = from b in _context.Bookings.Where(i => i.PaymentMethod == PaymentStatus.OP)              
                        orderby b.CreatedDate descending
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
                            ConfirmDate = b.ConfirmDate,
                            Status = b.Status
                        };
            return await _pagingService.GetPagedAsync<BookingVM>(query, model.PageIndex, model.PageSize);
        }
        public async Task<ApiResponse> GetTransactionDetail(int bookingId)
        {
            var transaction = await _context.Transactions.FirstOrDefaultAsync(x => x.BookingId == bookingId);
            if (transaction == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy giao dịch"

                };
            }

            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Lấy giao dịch thành công",
                Data = transaction

            };
        }
        public async Task<ApiResponse> GetBookingById()
        {
            var bookings = await(from b in _context.Bookings.Where(s => s.Status != BookingStatus.CheckOut && s.Status != BookingStatus.Cancelled)
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

            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Data = bookings,
                Message = "Lấy dữ liệu thành công"
            };
        }

        public async Task<ApiResponse> GetBooked()
        {
            try
            {
                var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new Exception("User not found");
                var bookingsRaw = await (
                    from b in _context.Bookings.Where(x => x.AppUserId == userId)
                    join bd in _context.BookingDetails on b.Id equals bd.BookingId
                    join r in _context.Rooms on bd.RoomId equals r.Id
                    join rt in _context.RoomTypes.Include(rt => rt.RoomImages) on r.RoomTypeId equals rt.Id
                    group new { b, rt } by b into g
                    select new
                    {
                        Booking = g.Key,
                        RoomTypes = g.Select(x => x.rt).ToList()
                    }
                ).ToListAsync();

                var bookings = bookingsRaw.Select(item => new BookedModel
                {
                    Id = item.Booking.Id,
                    UserName = item.Booking.UserName,
                    Email = item.Booking.Email,
                    Phone = item.Booking.Phone,
                    Code = item.Booking.Code,
                    Note = item.Booking.Note,
                    TotalPerson = item.Booking.TotalPerson,
                    TotalPrice = item.Booking.TotalPrice,
                    FromDate = item.Booking.FromDate,
                    ToDate = item.Booking.ToDate,
                    PaymentMethod = item.Booking.PaymentMethod,
                    PaymentStatus = item.Booking.PaymentStatus,
                    CreatedDate = item.Booking.CreatedDate,
                    ConfirmBy = item.Booking.ConfirmBy,
                    Status = item.Booking.Status,
                    RoomTypes = item.RoomTypes
                        .GroupBy(rt => rt.Id)
                        .Select(g => {
                            var rt = g.First();
                            return new RoomTypeVM
                            {
                                Id = rt.Id,
                                Name = rt.Name,
                                Content = rt.Content,
                                Slug = rt.Slug,
                                Capacity = rt.Capacity,
                                Price = rt.Price,
                                View = rt.View,
                                BedType = rt.BedType,
                                Size = rt.Size,
                                Status = rt.Status,
                                RoomImages = rt.RoomImages?.Select(img => new RoomImageModel
                                {
                                    Id = img.Id,
                                    Url = img.Url,
                                    RoomTypeId = img.RoomTypeId
                                }).ToList()
                            };
                        }).ToList()
                }).ToList();

                return new ApiResponse
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lấy danh sách thành công",
                    Data = bookings
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while logging in: {ex.Message} at {DateTime.UtcNow}");
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Lỗi khi lấy danh sách đã đặt",
                };
            }

        }
    }
}
