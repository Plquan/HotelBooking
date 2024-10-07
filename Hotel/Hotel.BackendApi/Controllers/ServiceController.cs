using AutoMapper;
using Hotel.Data.Dtos;
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
        [Route("GetAll")]
        public async Task<ActionResult<List<ServiceDTO>>> GetAllSupply()
        {
            var supplys = await _serviceRepository.GetAll();
            return _mapper.Map<List<ServiceDTO>>(supplys);
        }
        [HttpPost]
        [Route("AddService")]
        public async Task AddSupply(ServiceDTO supplyDTO)
        {
            var Supply = _mapper.Map<Service>(supplyDTO);
            await _serviceRepository.Add(Supply);
        }
        [HttpDelete]
        [Route("DeleteService/{id}")]
        public async Task DeleleSupply(int id)
        {
            await _serviceRepository.Delete(id);
        }
        [HttpPut]
        [Route("UpdateService")]
        public async Task UpdateSupply(ServiceDTO supplyDTO)
        {
            var supply = _mapper.Map<Service>(supplyDTO);
            await _serviceRepository.Update(supply);
        }
    }
}
