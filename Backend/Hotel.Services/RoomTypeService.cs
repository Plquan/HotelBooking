﻿using AutoMapper;
using Hotel.Data;
using Hotel.Data.Dtos;
using Hotel.Data.Enum;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.ViewModels.Reservations;
using Hotel.Data.ViewModels.RoomTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hotel.Services
{
    public interface IRoomTypeService
    {      
        Task Add(RoomTypeDTO roomType);
        Task Delete(int id);
        Task Update(RoomTypeDTO room);
        Task<string> ChangeStatus(int id);
        Task<ApiResponse> GetAll();
        Task<ApiResponse> GetById(int id);
        Task<Paging<RoomTypeVM>> GetListPaging(int pageIndex, int pageSize);
        Task<ApiResponse> SaveRoom(int roomId);
        Task<ApiResponse> GetListSavedRoom();
    }


    public class RoomTypeService : IRoomTypeService
    {
        private readonly HotelContext _context;
        private readonly IMapper _mapper;
        private readonly IFileServices _fileServices;
        private readonly IPagingService _pagingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RoomTypeService(HotelContext context, IMapper mapper, IFileServices fileServices, IPagingService pagingService,
             IHttpContextAccessor httpContextAccessor
            )
        {
            _context = context;
            _mapper = mapper;
            _fileServices = fileServices;
            _pagingService = pagingService;
            _httpContextAccessor = httpContextAccessor;
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
                foreach (var image in roomType.RoomImages)
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

        public async Task<ApiResponse> GetAll()
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
                                       RoomImages = _mapper.Map<List<RoomImageModel>>(rt.RoomImages),
                                       RoomFacilitys = _mapper.Map<List<RoomFacilityModel>>(rt.RoomFacilitys)
                                   }).ToListAsync();

            return new ApiResponse {
                Data = roomTypes,
                StatusCode = 200,
                IsSuccess = true,
                Message = "Lấy danh sách thành công"
            };

        }

        public async Task<ApiResponse> GetById(int id)
        {
            var room = await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == id);
            if (room == null)
            {
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy phòng",
                };
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
                RoomImages = _mapper.Map<List<RoomImageModel>>(image),
                RoomFacilitys = _mapper.Map<List<RoomFacilityModel>>(facility)
            };
            return new ApiResponse {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Lấy thông tin phòng thành công",
                Data = roomtype
            };

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
                    foreach (var img in deleteImage)
                    {
                        _fileServices.Delete(img.Url);
                    }
                    _context.RemoveRange(roomImg);

                    foreach (var img in roomTypeDTO.RoomImages)
                    {
                        var imgUpload = _fileServices.Upload(img);
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
                            RoomImages = _mapper.Map<List<RoomImageModel>>(rt.RoomImages),
                            RoomFacilitys = _mapper.Map<List<RoomFacilityModel>>(rt.RoomFacilitys)
                        };
            return await _pagingService.GetPagedAsync<RoomTypeVM>(query, pageIndex, pageSize);
        }

        public async Task<ApiResponse> SaveRoom(int roomId)
        {
            try
            {
                var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return new ApiResponse
                    {
                        IsSuccess = false,
                        StatusCode = 401, // 401 là chuẩn cho "Unauthorized"
                        Message = "Người dùng chưa đăng nhập",
                    };
                }

                var existingSaved = await _context.SavedRooms
                    .FirstOrDefaultAsync(x => x.AppUserId == userId && x.RoomTypeId == roomId);

                if (existingSaved != null)
                {
                    _context.SavedRooms.Remove(existingSaved);
                    await _context.SaveChangesAsync(); // ❗ Bạn thiếu đoạn này!

                    return new ApiResponse
                    {
                        IsSuccess = true,
                        StatusCode = 200,
                        Message = "Đã xóa phòng khỏi danh sách yêu thích",
                    };
                }

                var save = new SavedRoom
                {
                    AppUserId = userId,
                    RoomTypeId = roomId,
                };

                await _context.SavedRooms.AddAsync(save);
                await _context.SaveChangesAsync(); // ❗ Bạn cũng thiếu đoạn này ở đây!

                return new ApiResponse
                {
                    IsSuccess = true,
                    StatusCode = 200,
                    Message = "Lưu phòng thành công",
                };
            }
            catch (Exception ex)
            {
                // Gợi ý: có thể log lỗi tại đây nếu cần
                return new ApiResponse
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi máy chủ",
                };
            }
        }
        public async Task<bool> CheckSaved(int roomId)
        {
            var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return false;
            }

            var check = await _context.SavedRooms.Where(x => x.AppUserId == userId).Where(x => x.RoomTypeId == roomId).CountAsync();
            if (check > 0)
            {
                return true;
            }
            return false;

        }
        public async Task<ApiResponse> GetListSavedRoom()
        {
            var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? throw new Exception("User not found");
            var roomTypes = await(from rt in _context.RoomTypes
                                  join sv in _context.SavedRooms on rt.Id equals sv.RoomTypeId
                                  where(sv.AppUserId == userId)
                                  select new CheckRoomVM()
                                  {
                                      Id = rt.Id,
                                      Name = rt.Name,
                                      Content = rt.Content,
                                      Slug = rt.Slug,
                                      View = rt.View,
                                      Size = rt.Size,
                                      BedType = rt.BedType,
                                      Capacity = rt.Capacity,
                                      Price = rt.Price,
                                      RoomImages = _mapper.Map<List<RoomImageModel>>(rt.RoomImages!.ToList())
                                  }).ToListAsync();
            foreach (var roomType in roomTypes)
            {
                roomType.IsSaved = await CheckSaved(roomType.Id);
            }
            return new ApiResponse
            {
                IsSuccess = true,
                StatusCode = 200,
                Message = "Lấy dữ liệu thành công",
                Data = roomTypes
            };
        }
    }

  }

