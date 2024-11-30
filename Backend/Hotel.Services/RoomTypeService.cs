using AutoMapper;
using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Models;
using Hotel.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Hotel.Services
{
    public interface IRoomTypeService
    {      
        Task Add(RoomTypeDTO roomType);
        Task Delete(int id);
        Task Update(RoomTypeDTO room);
        Task<string> ChangeStatus(int id);
        Task<List<RoomTypeVM>> GetAll();
        Task<RoomTypeVM> GetById(int id);
        Task<Paging<RoomTypeVM>> GetListPaging(int pageIndex, int pageSize);
    }


    public class RoomTypeService : IRoomTypeService
    {
        private readonly HotelContext _context;
        private readonly IMapper _mapper;
        private readonly IFileServices _fileServices;
        private readonly IPagingService _pagingService;
        public RoomTypeService(HotelContext context, IMapper mapper,IFileServices fileServices,IPagingService pagingService)
        {
            _context = context;
            _mapper = mapper;
            _fileServices = fileServices;
            _pagingService = pagingService;
            
        }

        public int GetRoomId(RoomTypeDTO roomType)
        {
            var newroomtype = new RoomType() {
                Name = roomType.Name,
                Content = roomType.Content,
                Slug = roomType.Slug,
                Capacity = roomType.Capacity,
                Price = roomType.Price,
                View = roomType.View,
                BedType = roomType.BedType,
                Size = roomType.Size,         
                Status = "inactive"
            };

          _context.RoomTypes.Add(newroomtype);
			 _context.SaveChanges();
            return newroomtype.Id;
		}

        public async Task Add(RoomTypeDTO roomType)
        {
            var Id = GetRoomId(roomType); 

            if (roomType.RoomFacilitys != null) {
                foreach (var facility in roomType.RoomFacilitys)
                {
                    var newTag = new RoomFacility()
                    {
                        RoomTypeId = Id,
                        Name = facility
                    };
                    _context.RoomFacilitys.Add(newTag);
                }
            }
            if (roomType.RoomImages != null)
            {
                foreach(var image in roomType.RoomImages)
                {
                    var img = _fileServices.Upload(image);
                    var newImage = new RoomImage()
                    {
                        RoomTypeId = Id,
                        Url = img                    
                    };
                    _context.RoomImages.Add(newImage);
                }
            }
            await _context.SaveChangesAsync();
        }


        public async Task Delete(int id)
        {
            var roomtype = _context.RoomTypes.FirstOrDefault(r => r.Id == id);
            if (roomtype != null)
            {
                _context.Remove(roomtype);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<RoomTypeVM>> GetAll()
        {
            var roomTypes = await (from rt in _context.RoomTypes                            
                              select new RoomTypeVM
                              {
                                  Id = rt.Id,
                                  Name = rt.Name,
                                  Capacity = rt.Capacity,
                                  Slug = rt.Slug,
                                  View = rt.View,
                                  BedType = rt.BedType,
                                  Price = rt.Price,
                                  Status = rt.Status,
                                  Content = rt.Content,
                                  Size = rt.Size,
                                  RoomImages = _mapper.Map<List<RoomImageDTO>>(rt.RoomImages),
                                  RoomFacilitys = _mapper.Map<List<RoomFacilityDTO>>(rt.RoomFacilitys)
                              }).ToListAsync();
      
            return roomTypes;
        }

		public async Task<RoomTypeVM> GetById(int id)
		{
			var room = await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (room == null)
            {
                throw new Exception("Không tìm thấy phòng!");
            }
            var facility = await _context.RoomFacilitys.Where(x => x.RoomTypeId == id).ToListAsync();
			var image = await _context.RoomImages.Where(x => x.RoomTypeId == id).ToListAsync();
                     
			var roomtype = new RoomTypeVM()
			{
				Name = room.Name,
                Capacity = room.Capacity,
                View = room.View,
                Slug = room.Slug,
                BedType = room.BedType,
                Price = room.Price,
                Status = room.Status,
                Content = room.Content,
                Size = room.Size,
                RoomImages = _mapper.Map<List<RoomImageDTO>>(image),
                RoomFacilitys = _mapper.Map<List<RoomFacilityDTO>>(facility)
			};
            return roomtype;
		}

        public async Task Update(RoomTypeDTO roomTypeDTO)
        {
            var roomtype = _context.RoomTypes.FirstOrDefault(x => x.Id == roomTypeDTO.Id);

            if (roomtype != null)
            { 
                roomtype.Name = roomTypeDTO.Name;
                roomtype.Content = roomTypeDTO.Content;
                roomtype.Slug = roomTypeDTO.Slug;
                roomtype.Size = roomTypeDTO.Size;
                roomtype.Capacity = roomTypeDTO.Capacity;
                roomtype.View = roomTypeDTO.View;
                roomtype.BedType = roomTypeDTO.BedType;
                roomtype.Price = roomTypeDTO.Price;
                if (roomTypeDTO.RoomImages != null)
                {
                    var roomImg = _context.RoomImages.Where(x => x.RoomTypeId == roomtype.Id).ToList();

                    var deleteImage = roomImg
                          .Where(img => !roomTypeDTO.RoomImages.Any(dtoImg => dtoImg == img.Url))
                     .ToList();
                    foreach(var img in deleteImage)
                    {
                        _fileServices.Delete(img.Url);
                    }
                    _context.RemoveRange(roomImg);

                    foreach (var img in roomTypeDTO.RoomImages)
                    {
                        var imgUpload =  _fileServices.Upload(img);
                        var newImg = new RoomImage()
                        {
                            RoomTypeId = roomtype.Id,
                            Url = imgUpload
						};
                        _context.RoomImages.Add(newImg);

                    }
                }

                if (roomTypeDTO.RoomFacilitys != null)
                {
                    var roomTag = _context.RoomFacilitys.Where(x => x.RoomTypeId == roomtype.Id).ToList();
                       _context.RemoveRange(roomTag);
                       foreach (var facility in roomTypeDTO.RoomFacilitys)
                        {
                        var newtag = new RoomFacility()
                        {
                            RoomTypeId = roomtype.Id,
                            Name = facility
                        };
                        _context.RoomFacilitys.Add(newtag);                                                     
                        }
                }
                    await _context.SaveChangesAsync();

                }
            }

		public async Task<string> ChangeStatus(int id)
		{
			var roomType = _context.RoomTypes.FirstOrDefault(x => x.Id == id);

           
				if (roomType != null)
				{
					roomType.Status = roomType.Status == "active" ? "inActive" : "active";
				await _context.SaveChangesAsync();
				return roomType.Status;
			}
            else
            {
                throw new Exception("không tìm thấy phòng");
            }
					
		}
        public async Task<Paging<RoomTypeVM>> GetListPaging(int pageIndex, int pageSize)
        {
            var query = from rt in _context.RoomTypes
                                 select new RoomTypeVM
                                 {
                                     Id = rt.Id,
                                     Name = rt.Name,
                                     Capacity = rt.Capacity,
                                     Slug = rt.Slug,
                                     View = rt.View,
                                     BedType = rt.BedType,
                                     Price = rt.Price,
                                     Status = rt.Status,
                                     Content = rt.Content,
                                     Size = rt.Size,
                                     RoomImages = _mapper.Map<List<RoomImageDTO>>(rt.RoomImages),
                                     RoomFacilitys = _mapper.Map<List<RoomFacilityDTO>>(rt.RoomFacilitys)
                                 };
            return await _pagingService.GetPagedAsync<RoomTypeVM>(query, pageIndex, pageSize);
        }
    }

  }

