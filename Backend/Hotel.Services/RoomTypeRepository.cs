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

namespace Hotel.Services
{
    public interface IRoomTypeRepository
    {      
        Task Add(RoomTypeDTO roomType);
        Task Delete(int id);
        Task Update(RoomTypeDTO room);
        Task<string> ChangeStatus(int id);
        Task<List<RoomType>> GetAll();
        Task<RoomTypeVM> GetById(int id);
    }


    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly HotelContext _context;
        private readonly IMapper _mapper;
        private readonly IFileServices _fileServices;
        public RoomTypeRepository(HotelContext context, IMapper mapper,IFileServices fileServices)
        {
            _context = context;
            _mapper = mapper;
            _fileServices = fileServices;
            
        }

        public int GetRoomId(RoomTypeDTO roomType)
        {
            var newroomtype = new RoomType() {
                Name = roomType.Name,
                Content = roomType.Content,
                Capacity = roomType.Capacity,
                Price = roomType.Price,
                View = roomType.View,
                BedType = roomType.BedType,
                Size = roomType.Size,
                Thumb = roomType.Thumb != null ? _fileServices.Upload(roomType.Thumb) : null,              
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
            if(roomType.RoomServices != null)
            {
                foreach(var service in roomType.RoomServices)
                {
                    var newService = new RoomService() 
                    {
                        RoomTypeId = Id,
                        Name = service
                    };
                    _context.RoomServices.Add(newService);
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

        public async Task<List<RoomType>> GetAll()
        {

        //    var roomType = _context.RoomTypes
        // .Include(r => r.RoomImages)
        //  .Include(r => r.RoomServices)
        //.Include(r => r.RoomFacilitys)
        // .Select(r => new RoomTypeVM
        // {
        //     Id = r.Id,
        //     Name = r.Name,
        //     Capacity = r.Capacity,
        //     View = r.View,
        //     BedType = r.BedType,
        //     Price = r.Price,
        //     Size = r.Size,
        //     Status = r.Status,
        //     Content = r.Content,
        //     Thumb = r.Thumb,
        //     RoomImages = _mapper.Map<List<RoomImageDTO>>(r.RoomImages.ToList()),
        //     RoomServices = _mapper.Map<List<RoomServiceDTO>>(r.RoomServices.ToList()),
        //     RoomFacilitys = _mapper.Map<List<RoomFacilityDTO>>(r.RoomFacilitys.ToList())

        // }).ToList();


            return await _context.RoomTypes.ToListAsync();
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
			var service = await _context.RoomServices.Where(x => x.RoomTypeId == id).ToListAsync();
                     
			var roomtype = new RoomTypeVM()
			{
				Name = room.Name,
                Capacity = room.Capacity,
                View = room.View,
                BedType = room.BedType,
                Price = room.Price,
                Status = room.Status,
                Content = room.Content,
                Size = room.Size,
                Thumb = room.Thumb,
                RoomServices = _mapper.Map<List<RoomServiceDTO>>(service),
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
                roomtype.Capacity = roomTypeDTO.Capacity;
                roomtype.View = roomTypeDTO.View;
                roomtype.BedType = roomTypeDTO.BedType;
                roomtype.Price = roomTypeDTO.Price;

               if(roomTypeDTO.Thumb != null)
                {
					var newthumb = _fileServices.Upload(roomTypeDTO.Thumb);
					_fileServices.Delete(roomtype.Thumb);
                    roomtype.Thumb = newthumb;
				}
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
                if (roomTypeDTO.RoomServices != null)
                {
                   var roomService = _context.RoomServices.Where(x => x.RoomTypeId == roomtype.Id).ToList();
                    _context.RemoveRange(roomService);
                    foreach(var service in roomTypeDTO.RoomServices)
                    {
                        var newRoomService = new RoomService() { 
                            RoomTypeId = roomtype.Id,
                            Name = service
                        };
                        _context.RoomServices.Add(newRoomService);

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

  



	}

  }

