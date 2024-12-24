using Hotel.Data.Libraries;
using Hotel.Data.ViewModels.Vnpay;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }

    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly IBookingService _bookingService;

        public VnPayService(IConfiguration configuration, IBookingService bookingService)
        {
            _configuration = configuration;
            _bookingService = bookingService;
        }

        public async Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var bookingId = await _bookingService.PlaceOrder(model.Booking);

            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]!);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]!);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]!);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]!);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount! * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]!);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]!);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.Amount} {model.OrderDescription}");
            pay.AddRequestData("vnp_OrderType", model.OrderType!);
            pay.AddRequestData("vnp_ReturnUrl", _configuration["Vnpay:ReturnUrl"]!);
            pay.AddRequestData("vnp_TxnRef", bookingId.ToString());
            var paymentUrl =
                pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"]!, _configuration["Vnpay:HashSecret"]!);

            return paymentUrl;

        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]!);
            return response;
        }
    }
}
