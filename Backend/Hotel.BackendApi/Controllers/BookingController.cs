using AutoMapper;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        public BookingController(IBookingService bookingService) 
        {
          _bookingService = bookingService;
        }
        [HttpPost]
        [Route("CheckRoom")]
        public async Task<List<CheckRoomVM>> CheckRoom(CheckDate date)
        {
            return await _bookingService.CheckRoom(date);
           
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
        [HttpPost]
        [Route("PlaceOrder")]
        public async Task<IActionResult> PlaceOrder(BookingVM bookingVM)
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

    }
}
