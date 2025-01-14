using Hotel.Data.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public interface IAnalyticService 
    {
        Task<ApiResponse> GetAnalytic();
    }

    public class AnalyticService : IAnalyticService
    {
        public Task<ApiResponse> GetAnalytic()
        {
            throw new NotImplementedException();
        }
    }
}
