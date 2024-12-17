using Azure.Core;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.ViewModels;
using Hotel.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Request = Hotel.Services.Request;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MenuController : ControllerBase
    {
        private readonly IMenuServices _menuServices;
        public MenuController(IMenuServices menuServices)
        {
            _menuServices = menuServices;
        }
        [HttpGet]
        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var menus = await _menuServices.GetAll();
                return Ok(new { message = "Lấy dữ liệu thành công", data = menus });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
           
        }
        [HttpPost]
        [Route("Add")]
        public async Task<IActionResult> Add(Menu menu)
        { 
            try
            {
                await _menuServices.Add(menu);
                return Ok(new { message = "Thêm thành công", data = menu });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
               await _menuServices.Delete(id);
                return Ok(new { message = $"Xóa thành công{id}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(Menu menu)
        {
            try
            {
                await _menuServices.Update(menu);
                return Ok(new { message = "Thêm thành công", data = menu });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
            
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
               var menu = await _menuServices.GetById(id);
                return Ok(new { message = "Lấy thành công",data = menu });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpGet]
        [Route("GetMenuSelect/{id}")]
        public async Task<IActionResult> GetMenuSelect(int id)
        {
            try
            {
                var menu = await _menuServices.GetMenuSelect(id);
                return Ok(new { message = "Lấy thành công", data = menu });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPut]
        [Route("SaveItem")]
        public async Task<IActionResult> SaveItem(Request request)
        {
            try
            {
                await _menuServices.SaveItem(request);
                return Ok(new { message = "Cập nhật thành công"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
    }
}
