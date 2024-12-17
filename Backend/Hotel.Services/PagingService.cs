using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{
    public class Paging<T>
    {
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public List<T> Items { get; set; } = [];

        public Paging(List<T> items, int count, int pageIndex, int pageSize)
        {
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
            Items.AddRange(items);
        }
    }

    public interface IPagingService
    {
        Task<Paging<T>> GetPagedAsync<T>(IQueryable<T> list, int pageIndex, int pageSize);
    }

    public class PagingService : IPagingService
    {
        public async Task<Paging<T>> GetPagedAsync<T>(IQueryable<T> list, int pageIndex, int pageSize)
        {
            var count = await list.CountAsync();
            var items = await list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();

            return new Paging<T>(items, count, pageIndex, pageSize);
        }
    }
}
