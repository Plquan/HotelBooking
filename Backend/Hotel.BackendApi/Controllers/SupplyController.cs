using AutoMapper;
using Hotel.BackendApi.Dtos;
using Hotel.Data.Models;
using Hotel.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel.BackendApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplyController : ControllerBase
    {
        private readonly ISupplyService _supplyService;
        private readonly IMapper _mapper;
        public SupplyController(ISupplyService supplyService,IMapper mapper)
        {
            _supplyService = supplyService;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAllSupply")]
        public async Task<ActionResult<List<SupplyDTO>>> GetAllSupply()
        {
            var supplys = await _supplyService.GetAll();
            return _mapper.Map<List<SupplyDTO>>(supplys);
        }
        [HttpPost]
        [Route("AddSupply")]
        public async Task AddSupply(SupplyDTO supplyDTO)
        {
            var Supply = _mapper.Map<Supply>(supplyDTO);
            await _supplyService.Add(Supply);
        }
        [HttpDelete]
        [Route("DeleteSupply/{id}")]
        public async Task DeleleSupply(int id)
        {
            await _supplyService.Delete(id);
        }
        [HttpPut]
        [Route("UpdateSupply")]
        public async Task UpdateSupply(SupplyDTO supplyDTO)
        {
            var supply = _mapper.Map<Supply>(supplyDTO);
            await _supplyService.Update(supply);
        }
    }
}
