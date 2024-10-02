using Hotel.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IRoomTypeService 
    {
        Task Add(RoomType room);
        Task Delete(int id);
        Task Update(RoomType room);
        Task<List<RoomType>> GetAll();
    }


    public class RoomTypeService : IRoomTypeService
    {
        public Task Add(RoomType room)
        {
            throw new NotImplementedException();
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<RoomType>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task Update(RoomType room)
        {
            throw new NotImplementedException();
        }
    }
}
