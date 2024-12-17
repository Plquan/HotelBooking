using Azure.Core;
using Hotel.Data;
using Hotel.Data.Models;
using Hotel.Data.ViewModels.Menus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Services
{

    public class Request
    { 
     public string? Task {  get; set; }
     public string? Button { get; set; }
     public int? Id { get; set; }
    }

    public interface IMenuServices
    {
        Task Add(Menu menu);
        Task Delete(int id);
        Task Update(Menu menu);
        Task<MenuVM> GetById(int id);
        Task<List<MenuVM>> GetAll(int? parentId = null);
        Task<List<MenuVM>> GetMenuSelect(int id, int? parent = null);
        Task SaveItem(Request request);
    }

    public class MenuService : IMenuServices
    {
        private readonly HotelContext _context;
        public MenuService(HotelContext context) 
        {
         _context = context;
        }    
        public async Task Add(Menu menu)
        {
            int newOrdering = 0;
            if (menu.ParentId == null)
            {
                int ordering = await _context.Menus.Where(x => x.ParentId == null).MaxAsync(m => m.Ordering) ?? 0;
                newOrdering = ordering == 0 ? 1 : ordering + 1;
            }
            else {
                int ordering = await _context.Menus.Where(x => x.ParentId == menu.ParentId).MaxAsync(m => m.Ordering) ?? 0;
                newOrdering = ordering == 0 ? 1 : ordering + 1;
            }
            var newMenu = new Menu() {
             Name = menu.Name,
             Link = menu.Link,
             Ordering = newOrdering,
             ParentId = menu.ParentId,
             TypeOpen = menu.TypeOpen == null ? "_self" : menu.TypeOpen,
             Status = menu.Status == null ? "inactive" : menu.Status,
            };

            _context.Menus.Add(newMenu);
          await  _context.SaveChangesAsync();
        }

        public async Task<List<MenuVM>> GetAll(int? parentId = null)
        {
            var menus = await _context.Menus.Where(x => x.ParentId == parentId).OrderBy(m => m.Ordering)
                  .Select(m => new MenuVM
                  {
                      Id = m.Id,
                      Name = m.Name,
                      Link = m.Link,
                      Ordering= m.Ordering,
                      ParentId = m.ParentId,
                      TypeOpen = m.TypeOpen,
                      Status = m.Status
                  }).ToListAsync();
            foreach (var menu in menus)
            {
                menu.MenuChild = await GetAll(menu.Id); 
            }
            return menus;
        }

        public async Task Delete(int id)
        {
            var Cmenu = await _context.Menus.FirstOrDefaultAsync(x => x.Id == id);
            if (Cmenu != null)
            { 
                var ordering = _context.Menus.Where(m => m.ParentId == (Cmenu.ParentId == null ? null : Cmenu.ParentId))
                                 .Where(y => y.Ordering > Cmenu.Ordering).ToList();
                foreach (var order in ordering)
                {
                    order.Ordering = order.Ordering - 1;
                } 
                await DeleteMenuChild(Cmenu.Id);
                _context.Menus.Remove(Cmenu); 
              await _context.SaveChangesAsync(); 
            } 
        }

        public async Task DeleteMenuChild(int id)
        {
            var menus = await _context.Menus.Where(x => x.ParentId == id).ToListAsync();
            foreach (var menu in menus) {
                await DeleteMenuChild(menu.Id);
            }
           _context.Menus.RemoveRange(menus);
        }

        public async Task Update(Menu menu)
        {
           var Cmenu = await _context.Menus.FirstOrDefaultAsync(x => x.Id == menu.Id);
            if (Cmenu != null) { 
                if(Cmenu.ParentId != menu.ParentId)
                {
                    var refreshOrder = await _context.Menus.Where(x => x.ParentId == Cmenu.ParentId).Where(m => m.Ordering > Cmenu.Ordering).ToListAsync();
                    foreach (var order in refreshOrder)
                    {
                        order.Ordering = order.Ordering - 1;
                    }
                    var ordering = await _context.Menus.Where(x => x.ParentId == menu.ParentId).MaxAsync(x => x.Ordering);
                    Cmenu.Ordering = ordering == null ? 1 : ordering + 1;
                }            
                    Cmenu.Name = menu.Name;
                    Cmenu.Link = menu.Link;
                    Cmenu.ParentId = menu.ParentId;
                    Cmenu.TypeOpen = menu.TypeOpen;
                    Cmenu.Status = menu.Status;        
                await _context.SaveChangesAsync();
            }
            
        }
        public async Task<List<MenuVM>> GetMenuSelect(int id,int? parentId = null)
        {          
            var menus = await _context.Menus.Where(x => x.ParentId == parentId && x.Id != id).OrderBy(m => m.Ordering)
                 .Select(m => new MenuVM
                 {
                     Id = m.Id,
                     Name = m.Name,
                     Link = m.Link,
                     Ordering = m.Ordering,
                     ParentId = m.ParentId,
                     TypeOpen = m.TypeOpen,
                     Status = m.Status
                 }).ToListAsync();
            foreach (var menu in menus)
            {
                menu.MenuChild = await GetMenuSelect(id,menu.Id);
            }
            return menus;
        }
       
        public async Task<MenuVM> GetById(int id)
        {
            var Cmenu = await _context.Menus.FirstOrDefaultAsync(x => x.Id == id);
            if (Cmenu != null) {
                var menu = new MenuVM()
                {
                    Id = id,
                    Name = Cmenu.Name,
                    Link = Cmenu.Link,
                    ParentId = Cmenu.ParentId,
                    Ordering=Cmenu.Ordering,
                    TypeOpen = Cmenu.TypeOpen,
                    Status = Cmenu.Status,             
                };
                return menu;
            }
            else
            {
                throw new NotImplementedException("Không tìm thấy menu");
            }
        }
        public async Task SaveItem(Request request)
        {
            if(request.Task == "ordering")
            {
                var Cmenu = _context.Menus.FirstOrDefault(x => x.Id == request.Id);
                if (Cmenu != null)
                {
                    if (request.Button == "up")
                    {
                        var Nmenu = _context.Menus.Where(m => m.ParentId == (Cmenu.ParentId == null ? null : Cmenu.ParentId))
                                     .FirstOrDefault(y => y.Ordering == Cmenu.Ordering - 1);
                        if (Nmenu != null)
                        {
                            var c = Cmenu.Ordering;
                            Cmenu.Ordering = Nmenu.Ordering;
                            Nmenu.Ordering = c;
                        }
                    }
                    else
                    {
                        var Nmenu = _context.Menus.Where(m => m.ParentId == (Cmenu.ParentId == null ? null : Cmenu.ParentId))
                                     .FirstOrDefault(y => y.Ordering == Cmenu.Ordering + 1);
                        if (Nmenu != null)
                        {
                            var c = Cmenu.Ordering;
                            Cmenu.Ordering = Nmenu.Ordering;
                            Nmenu.Ordering = c;
                        }
                    }
                    await _context.SaveChangesAsync();
                }

            }
            if (request.Task == "status")
            {
                var Cmenu = _context.Menus.FirstOrDefault(x => x.Id == request.Id);
                if (Cmenu != null)
                {
                    Cmenu.Status = request.Button == "inactive" ? "active" : "inactive";
                    await _context.SaveChangesAsync();
                }

            }
            if(request.Task == "typeOpen")
            {
                var Cmenu = _context.Menus.FirstOrDefault(x => x.Id == request.Id);
                if (Cmenu != null)
                {
                    Cmenu.TypeOpen = request.Button;
                    await _context.SaveChangesAsync();
                }
            }
        }
    }

}
