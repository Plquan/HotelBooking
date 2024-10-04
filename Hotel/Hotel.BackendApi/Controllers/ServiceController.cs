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
    public class ServiceController : ControllerBase
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;
        public ServiceController(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [Route("GetAllSupply")]
        public async Task<ActionResult<List<ServiceDTO>>> GetAllSupply()
        {
            var supplys = await _serviceRepository.GetAll();
            return _mapper.Map<List<ServiceDTO>>(supplys);
        }
        [HttpPost]
        [Route("AddSupply")]
        public async Task AddSupply(ServiceDTO supplyDTO)
        {
            var Supply = _mapper.Map<Service>(supplyDTO);
            await _serviceRepository.Add(Supply);
        }
        [HttpDelete]
        [Route("DeleteSupply/{id}")]
        public async Task DeleleSupply(int id)
        {
            await _serviceRepository.Delete(id);
        }
        [HttpPut]
        [Route("UpdateSupply")]
        public async Task UpdateSupply(ServiceDTO supplyDTO)
        {
            var supply = _mapper.Map<Service>(supplyDTO);
            await _serviceRepository.Update(supply);
        }
    }
}
