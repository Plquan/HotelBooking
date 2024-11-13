using AutoMapper;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.ViewModels;
using Hotel.Services;
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

    }
}
