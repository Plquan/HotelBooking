using Hotel.Data;
using Hotel.Data.Enum;
using Hotel.Data.Libraries;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Data.ViewModels.Vnpay;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IVnPayService
    {
        Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        Task<ApiResponse> RefundAsync(RefundModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);
    }

    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;
        private readonly HotelContext _context;

        public VnPayService(IConfiguration configuration, HotelContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
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
            pay.AddRequestData("vnp_TxnRef", model.Booking.Id.ToString());
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

        public async Task<ApiResponse> RefundAsync(RefundModel model, HttpContext context)
        {
            try
            {
                var transaction = _context.Transactions.FirstOrDefault(x => x.BookingId == model.BookingId)
                    ?? throw new Exception($"Mã giao dịch {model.BookingId} không được tìm thấy.");
                if (model.TransactionType == "03")
                {                 
                    if((transaction.Amount - transaction.RefundAmount) < model.RefundAmount)
                    {
                        return new ApiResponse
                        {
                            IsSuccess = false,
                            Message = "Số tiền hoàn trả lớn hơn số tiền khả dụng"
                        };

                    }
                }
                if(transaction.Amount == transaction.RefundAmount)
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Message = "Đã hoàn trả hết số tiền cho đơn hàng này"
                    };
                }

                var pay = new VnPayLibrary();
                // Dữ liệu đầu vào
                string vnp_RequestId = Guid.NewGuid().ToString("N").Substring(0, 8);
                string vnp_Version = _configuration["Vnpay:Version"]!;
                string vnp_Command = "refund";
                string vnp_TmnCode = _configuration["Vnpay:TmnCode"]!;
                string vnp_TransactionType = model.TransactionType!;
                string vnp_TxnRef = transaction.BookingId.ToString()!;
                string amount = ((int)model.RefundAmount! * 100).ToString();
                string vnp_OrderInfo = $"Hoan tien GD OrderId: {vnp_TxnRef}";
                string vnp_TransactionDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                string vnp_CreateBy = "quan";

                // Lấy thời gian hiện tại theo múi giờ Việt Nam
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]!);
                string vnp_CreateDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone).ToString("yyyyMMddHHmmss");

                // Địa chỉ IP người dùng
                string vnp_IpAddr = pay.GetIpAddress(context);

                // Tạo hash dữ liệu
                string hashData = string.Join("|", vnp_RequestId, vnp_Version, vnp_Command, vnp_TmnCode,
                                               vnp_TransactionType, vnp_TxnRef, amount, "", vnp_TransactionDate,
                                               vnp_CreateBy, vnp_CreateDate, vnp_IpAddr, vnp_OrderInfo);

                string vnp_SecureHash = pay.HmacSha512(_configuration["Vnpay:HashSecret"]!, hashData);

                // Dữ liệu gửi đi
                var vnp_ParamsWithHash = new
                {
                    vnp_RequestId,
                    vnp_Version,
                    vnp_Command,
                    vnp_TmnCode,
                    vnp_TransactionType,
                    vnp_TxnRef,
                    vnp_Amount = amount.ToString(),
                    vnp_OrderInfo,
                    vnp_TransactionNo = "",
                    vnp_TransactionDate,
                    vnp_CreateBy,
                    vnp_CreateDate,
                    vnp_IpAddr,
                    vnp_SecureHash
                };

                // Gửi yêu cầu POST
                string apiUrl = _configuration["Vnpay:ApiUrl"]!;
                var responseContent = await PostJsonAsync(apiUrl, vnp_ParamsWithHash);

                var responseJson = JsonSerializer.Deserialize<Dictionary<string, string>>(responseContent);

                if (responseJson == null || !responseJson.ContainsKey("vnp_ResponseCode"))
                {
                    throw new Exception("Phản hồi không hợp lệ từ VNPAY.");
                }

                string responseCode = responseJson["vnp_ResponseCode"];
                if (responseCode == "00")
                {
                    var newRefund = new Refund()
                    {
                        TransactionId = transaction.Id,
                        RefundAmount = model.RefundAmount,
                        RefundReason = model.RefundReason,
                        CreatedDate = DateTime.Now,
                    };

                    _context.Refunds.Add(newRefund);
                    var booking = await _context.Bookings.FirstOrDefaultAsync(i => i.Id == transaction.BookingId)
                          ?? throw new Exception($" Không tìm thấy đơn {model.BookingId}");
                    booking.PaymentStatus = PaymentStatus.Refund;
                   
                    if(model.TransactionType == "02")
                    {
                        transaction.RefundAmount = transaction.Amount;
                    }
                    else
                    {
                        transaction.RefundAmount += model.RefundAmount;
                    }

                    await _context.SaveChangesAsync();

                    // Hoàn tiền thành công
                    return new ApiResponse { 
                     StatusCode = 200,
                     IsSuccess = true,
                     Message = $"Hoàn tiền thành công: {responseJson["vnp_ResponseCode"]}"
                    }; 
                    
                }
                else
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        Message = $"Hoàn tiền thất bại: {responseJson["vnp_ResponseCode"]}"
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Refund error: {ex.Message}");
            }
        }

        private async Task<string> PostJsonAsync(string url, object data)
        {
            using var client = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(data);
            var httpResponse = await client.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, "application/json"));

            httpResponse.EnsureSuccessStatusCode();
            return await httpResponse.Content.ReadAsStringAsync();
        }



    }
}
