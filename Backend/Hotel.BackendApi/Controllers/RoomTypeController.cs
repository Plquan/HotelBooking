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
    public class RoomTypeController : ControllerBase
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IMapper _mapper;

        public RoomTypeController(IRoomTypeRepository roomTypeRepository, IMapper mapper) 
        { 
            _roomTypeRepository = roomTypeRepository;
            _mapper = mapper;

        }


        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(RoomTypeDTO roomTypeDTO)
        {
            try
            {
                await _roomTypeRepository.Add(roomTypeDTO);
                return Ok(new { message = "Update successful", data = roomTypeDTO });
            }
            catch (Exception ex)
            {              
                return BadRequest(new { message = "Lỗi thêm phòng", error = ex.Message });
            }
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<List<RoomType>>> GetAll()
        {
            var roomtype = await _roomTypeRepository.GetAll();
            return roomtype;
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task Delete(int id)
        {
            await _roomTypeRepository.Delete(id);
        }
        [HttpPut]
        [Route("Update")]
        public async Task Update( RoomTypeDTO model)
        {
            await _roomTypeRepository.Update(model);
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<RoomTypeVM> GetById(int id)
        {
            var roomtype = await _roomTypeRepository.GetById(id);
            return roomtype;
        }
        [HttpPost]
		[Route("ChangeStatus/{id}")]
		public async Task<string> ChangeStatus(int id)
        {      
            return await _roomTypeRepository.ChangeStatus(id);
		}
        
    }
}
