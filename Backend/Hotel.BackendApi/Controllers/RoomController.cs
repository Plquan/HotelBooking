using AutoMapper;

using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.Rooms;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        public RoomController(IRoomService roomService, IMapper mapper) { 
            _roomService = roomService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<List<RoomVM>>> GetAll()
        {
            var rooms = await  _roomService.GetAll();
            return  rooms;
        }
        [HttpPost]
        [Route("Add")]
        public async Task Add(RoomDTO room)
        {     
          await  _roomService.Add(room);
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task Delete(int id)
        {
            await _roomService.Delete(id);
        }
        [HttpPut]
        [Route("Update")]
        public async Task Update(RoomDTO roomdto)
        {
            var room = _mapper.Map<Room>(roomdto);
            await _roomService.Update(room);
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<RoomDTO> GetById(int id)
        {
			var room = await _roomService.GetById(id);
			return  _mapper.Map<RoomDTO>(room);
		}
        [HttpPost]
        [Route("ChangeStatus/{id}")]
        public async Task<string> ChangeStatus(int id)
        {
            return await _roomService.ChangeStatus(id);
        }
        [HttpGet]
        [Route("GetListPaging")]
        public async Task<IActionResult> GetListPaging(PagingModel model)
        {
            try
            {
                var roomTypes = await _roomService.GetListPaging(model);
                return Ok(new { message = "Lấy thành công", data = roomTypes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
    }
}
