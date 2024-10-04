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

    public interface ISupplyService
    {
        Task Add(Supply supply);
        Task Delete(int id);
        Task Update(Supply supply);
        Task<List<Supply>> GetAll();
    }
    public class SupplyService : ISupplyService
    {
        private readonly HotelContext _context;

        public SupplyService(HotelContext context)
        {
            _context = context;
        }

        public async Task Add(Supply supply)
        {
            _context.Supply.Add(supply);
          await  _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var Supply = _context.Supply.FirstOrDefault(x => x.Id == id);
            if (Supply != null)
            {
                _context.Supply.Remove(Supply);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Supply>> GetAll()
        {
           return await _context.Supply.ToListAsync();
        }

        public async Task Update(Supply newsupply)
        {
            var supply = _context.Supply.FirstOrDefault(x => x.Id == newsupply.Id);
            if (supply != null)
            {
                supply.Name = newsupply.Name;
                supply.Price = newsupply.Price;
                supply.Description = newsupply.Description;
              
                await _context.SaveChangesAsync();
            }
        }
    }
}
