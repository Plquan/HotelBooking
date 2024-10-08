using Hotel.Data;
using Hotel.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{

    public interface IServiceRepository
    {
        Task Add(Service Services);
        Task Delete(int id);
        Task Update(Service Services);
        Task<Service> GetById(int id);
        Task<List<Service>> GetAll();
    }
    public class ServiceRepository : IServiceRepository
    {
        private readonly HotelContext _context;

        public ServiceRepository(HotelContext context)
        {
            _context = context;
        }

        public async Task Add(Service Services)
        {
            _context.Services.Add(Services);
          await  _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var Services = _context.Services.FirstOrDefault(x => x.Id == id);
            if (Services != null)
            {
                _context.Services.Remove(Services);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Service>> GetAll()
        {
           return await _context.Services.ToListAsync();
        }

		public async Task<Service> GetById(int id)
		{
			var service = await _context.Services.FirstOrDefaultAsync(x => x.Id == id);
			if (service == null)
			{
				throw new Exception("Không tìm thấy loại dịch vụ!");
			}
			return service;
		}

		public async Task Update(Service newServices)
        {
            var Services = _context.Services.FirstOrDefault(x => x.Id == newServices.Id);
            if (Services != null)
            {
                Services.Name = newServices.Name;
                Services.Price = newServices.Price;
                Services.Description = newServices.Description;
              
                await _context.SaveChangesAsync();
            }
        }
    }
}
