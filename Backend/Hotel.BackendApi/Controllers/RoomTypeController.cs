using AutoMapper;

using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.RoomTypes;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomTypeController : ControllerBase
    {
        private const int DEFAULT_PAGE_INDEX = 1;
        private const int DEFAULT_PAGE_SIZE = 1;
        private readonly IRoomTypeService _roomTypeService;
        private readonly IMapper _mapper;

        public RoomTypeController(IRoomTypeService roomTypeService, IMapper mapper) 
        { 
            _roomTypeService = roomTypeService;
            _mapper = mapper;

        }


        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(RoomTypeDTO roomTypeDTO)
        {
            try
            {
                await _roomTypeService.Add(roomTypeDTO);
                return Ok(new { message = "Update successful", data = roomTypeDTO });
            }
            catch (Exception ex)
            {              
                return BadRequest(new { message = "Lỗi thêm phòng", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<ApiResponse>> GetAll()
        {
            var roomtype = await _roomTypeService.GetAll();
            return roomtype;
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task Delete(int id)
        {
            await _roomTypeService.Delete(id);
        }
        [HttpPut]
        [Route("Update")]
        public async Task Update( RoomTypeDTO model)
        {
            await _roomTypeService.Update(model);
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ApiResponse> GetById(int id)
        {
            var roomtype = await _roomTypeService.GetById(id);
            return roomtype;
        }
        [HttpPost]
		[Route("ChangeStatus/{id}")]
		public async Task<string> ChangeStatus(int id)
        {      
            return await _roomTypeService.ChangeStatus(id);
		}
        [HttpGet]
        [Route("GetListPaging")]
        public async Task<IActionResult> GetListPaging(int pageIndex = DEFAULT_PAGE_INDEX, int pageSize = DEFAULT_PAGE_SIZE)
        {
            try
            {
               var roomTypes =  await _roomTypeService.GetListPaging( pageIndex, pageSize);
                return Ok(new { message = "Lấy thành công", data = roomTypes });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPost("SaveRoom")]
        [Authorize]
        public async Task<ApiResponse> SaveRoom([FromBody] int roomId)
        {
            var res = await _roomTypeService.SaveRoom(roomId);
            return res;
        }
        [HttpGet("GetListSavedRoom")]
        [Authorize]
        public async Task<ApiResponse> GetListSavedRoom()
        {
            var res = await _roomTypeService.GetListSavedRoom();
            return res;
        }
    }
}
