using Hotel.Data.Dtos;
using Hotel.Data.Libraries;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Data.ViewModels.Vnpay;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }
        [HttpPost]
        [Route("CreatePaymentUrlVnpay")]
        public  async Task<IActionResult> CreatePaymentUrlVnpay(PaymentInformationModel model) 
        {
            try
            {
                var url = await _vnPayService.CreatePaymentUrl(model, HttpContext);
                return Ok(new { message = "Update successful", data = url });
            }
            catch (Exception ex) {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
        [HttpPost]
        [Route("RefundAsync")]
        public async Task<IActionResult> RefundAsync(RefundModel model)
        {
            try
            {
                var url = await  _vnPayService.RefundAsync(model, HttpContext);
                return Ok(new { message = "Hoàn tiền thành công", data = url });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi thực thi", error = ex.Message });
            }
        }
      

    }
}
