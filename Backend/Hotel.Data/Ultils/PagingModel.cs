using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Ultils
{
    public class PagingModel
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public string? KeyWord { get; set; } = null;
        public string? Order {  get; set; } = null;
        public string? FilterType { get; set; } = null;
    }
}
