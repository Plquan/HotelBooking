using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.Contacts;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        [Route("CreateContact")]
        public async Task<IActionResult> CreateContact(ContactVM contact) 
        {
            try
            {
                var newcontact = await _contactService.CreateContact(contact);
                return Ok(new { message = "Thêm thành công", data = newcontact });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpDelete]
        [Route("DeleteContact/{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            try
            {
                var delcontact = await _contactService.DeleteContact(id);
                return Ok(new { message = "thành công", data = delcontact });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPost]
        [Route("GetListPaging")]
        public async Task<IActionResult> GetListPaging(PagingModel model)
        {
            try
            {
                var list = await _contactService.GetListPaging(model);
                return Ok(new { message = "thành công", data = list });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPost]
        [Route("ReplyEmail")]
        public async Task<IActionResult> ReplyEmail(ReplyModel model) {
            try
            {
                var resp = await _contactService.ReplyEmail(model);
                return Ok(new { message = "thành công", data = resp });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }

    }
}
