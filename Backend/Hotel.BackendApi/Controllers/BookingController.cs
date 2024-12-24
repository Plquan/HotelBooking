using AutoMapper;
using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Enum;
using Hotel.Data.Libraries;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _configuration;
        private readonly IMailService _mailService;
        private readonly HotelContext _hotelContext;
        private readonly ILogger<BookingController> _logger;
        private const int DEFAULT_PAGE_INDEX = 1;
        private const int DEFAULT_PAGE_SIZE = 1;

        public BookingController(IBookingService bookingService, IVnPayService vnPayService, IConfiguration configuration, IMailService mailService, HotelContext hotelContext, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _vnPayService = vnPayService;
            _configuration = configuration;
            _mailService = mailService;
            _hotelContext = hotelContext;
            _logger = logger;
        }

        [HttpPost]
        [Route("CheckRoom")]
        public async Task<List<CheckRoomVM>> CheckRoom(CheckDate date)
        {
            return await _bookingService.CheckRoom(date);
           
        }
        [HttpPost]
        [Route("CheckRoomById")]
        public async Task<IActionResult> CheckRoomById(CheckDate date)
        {          
            try
            {
                var availableRoom = await _bookingService.CheckRoomById(date);
                return Ok(new { message = "Lấy dữ liệu thành công", data = availableRoom });
            }
            catch (Exception ex)
            {
                return BadRequest(new { statusCode = 500, message = "Lỗi thực thi", error = ex.Message });
            }

        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
               var bookings =  await _bookingService.GetAll();
                return Ok(new { message = "Lấy thành công", data =  bookings});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetListPaging")]
        public async Task<IActionResult> GetListPaging(int pageIndex = DEFAULT_PAGE_INDEX, int pageSize = DEFAULT_PAGE_SIZE)
        {
            try
            {
                var bookings = await _bookingService.GetListPaging(pageIndex,pageSize);
                return Ok(new { message = "Lấy thành công", data = bookings });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetBookingDetail/{id}")]
        public async Task<IActionResult> GetBookingDetail(int id) {
            try
            {
                var response = await _bookingService.GetBookingDetail(id);
                return Ok(new { message = "Lấy thành công", data = response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPost]
        [Route("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(BookingModel bookingVM)
        {
            try
            {         
               await _bookingService.PlaceOrder(bookingVM);
                return Ok(new { message = "Lấy thành công", data = bookingVM });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("del")]
        public IActionResult Delete(int id) {
            try
            {
                 _bookingService.DeleteBooking(id);
                return Ok(new { message = "Lấy thành công", });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("SendMail")]
        public IActionResult SendMail(string email,string code)
        {
            try
            {
                var bodyHtml  = EmailMessage.EmailBody(code);
                var Title = "Mã hóa đơn";
                _mailService.SendEmailAsync(email,Title,bodyHtml);
                return Ok(new { message = "Lấy thành công", });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("PaymentExecute")]
        public async Task<IActionResult> PaymentCallbackVnpay()
        {
            try
            {
                var response = _vnPayService.PaymentExecute(Request.Query);
                var bookingDetail = _hotelContext.Bookings.FirstOrDefault(s => s.Id == int.Parse(response.OrderId)) 
                    ?? throw new Exception("Lỗi không tìm thấy đơn");
                
                if(bookingDetail.PaymentStatus == PaymentStatus.Paid)
                {
                    return Ok(new { message = "Đã thanh toán", statusCode = response.VnPayResponseCode });
                }
                if (response.VnPayResponseCode == "00")
                {                 
                   await  _bookingService.UpdatePaymentStatus(int.Parse(response.OrderId),PaymentStatus.Paid);
                    var bodyHtml = EmailMessage.EmailBody(bookingDetail.Code!);
                    var Title = "Mã hóa đơn";
                    await _mailService.SendEmailAsync(bookingDetail.Email!, Title, bodyHtml);
                    return Ok(new { message = "Thanh toán thành công", data = bookingDetail, statusCode = response.VnPayResponseCode });
                }
                else
                {
                   await _bookingService.DeleteBooking(int.Parse(response.OrderId));
                    return Ok(new { message = "Thanh toán thất bại", statusCode = response.VnPayResponseCode });
                }              
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }

        //[HttpPost]
        //[Route("vnpay_ipn")]
        //public IActionResult ReceiveIPN()
        //{
        //    try
        //    {
        //        var vnpayData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());

        //        var vnp_SecureHash = vnpayData["vnp_SecureHash"];
        //        string vnp_HashSecret = _configuration["Vnpay:HashSecret"]; 


        //        vnpayData.Remove("vnp_SecureHash");


        //        VnPayLibrary vnpay = new VnPayLibrary();

        //        foreach (var item in vnpayData)
        //        {
        //            vnpay.AddResponseData(item.Key, item.Value);
        //        }

        //        // Xác thực chữ ký từ VNPAY
        //        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

        //        // Kiểm tra nếu chữ ký hợp lệ
        //        if (checkSignature)
        //        {
        //            long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
        //            long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;  // Số tiền thanh toán từ VNPAY
        //            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
        //            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        //            string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");

        //            _logger.LogInformation("test 12345667");
        //            return Ok(new { message = "Xác nhận thành công", status = "00" });
        //        }

                
        //        else
        //        {
        //            _logger.LogError("Chữ ký không hợp lệ, InputData={0}");
        //            return BadRequest(new { message = "Chữ ký không hợp lệ", status = "97" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Xử lý lỗi
        //        _logger.LogError($"Lỗi trong quá trình xử lý IPN: {ex.Message}");
        //        return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
        //    }
        //}


    }
}
