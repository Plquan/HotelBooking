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
        public async Task<ActionResult<List<ServiceDTO>>> GetAll()
        {
            var services = await _serviceRepository.GetAll();
            return _mapper.Map<List<ServiceDTO>>(services);
        }
        [HttpPost]
        [Route("Add")]
        public async Task Add(ServiceDTO serviceDTO)
        {
            var service = _mapper.Map<Service>(serviceDTO);
            await _serviceRepository.Add(service);
        }
        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task Delele(int id)
        {
            await _serviceRepository.Delete(id);
        }
        [HttpPut]
        [Route("Update")]
        public async Task Update(ServiceDTO serviceDTO)
        {
            var service = _mapper.Map<Service>(serviceDTO);
            await _serviceRepository.Update(service);
        }
        [HttpGet]
        [Route("GetById/{id}")]
        public async Task<ServiceDTO> GetById(int id)
        {
            var service = await _serviceRepository.GetById(id);
            return _mapper.Map<ServiceDTO>(service);
        }
    }
}
