using AutoMapper;
using Hotel.Data;
using Hotel.Data.Enum;
using Hotel.Data.Models;
using Hotel.Data.Ultils;
using Hotel.Data.Ultils.Email;
using Hotel.Data.ViewModels.Contacts;
using Hotel.Data.ViewModels.Rooms;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace Hotel.Services
{
    public class ReplyModel
    {
        public int Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }
    public interface IContactService {
        Task<ApiResponse> CreateContact(ContactVM contact);
        Task<ApiResponse> DeleteContact(int id);
        Task<Paging<ContactVM>> GetListPaging(PagingModel model);
        Task<ApiResponse> ReplyEmail(ReplyModel model);

    }

    public class ContactService : IContactService
    {
        private readonly HotelContext _context;
        private readonly IMapper _mapper;
        private readonly IPagingService _pagingService;
        private readonly IMailService _mailService;

        public ContactService(HotelContext context, IMapper mapper, IPagingService pagingService, IMailService mailService)
        {
            _context = context;
            _mapper = mapper;
            _pagingService = pagingService;
            _mailService = mailService;
        }

        public async Task<ApiResponse> CreateContact(ContactVM contact)
        {
            var newContact = new Contact { 
             UserName = contact.UserName,
             Email = contact.Email,
             Phone = contact.Phone,
             Message = contact.Message,
             CreatedDate = DateTime.Now,
             Status = "Pending"
            };      
            _context.Contacts.Add(newContact);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return new ApiResponse { 
                  StatusCode = 200,
                  IsSuccess = true,
                  Message = "Thêm thành công"
                };
            }
            return new ApiResponse
            {
                StatusCode = 400,
                IsSuccess = false,
                Message = "Thêm không thành công"
            };
        }

        public async Task<ApiResponse> DeleteContact(int id)
        {
            var contact = _context.Contacts.FirstOrDefault(x => x.Id == id);
            if(contact == null)
            {
                return new ApiResponse
                {
                    StatusCode = 400,
                    IsSuccess = false,
                    Message = "Không tìm thấy dữ liệu"
                };
            }
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();
            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Xóa thành công"
            };
        }

        public async Task<Paging<ContactVM>> GetListPaging(PagingModel model)
        {
            var query = from c in _context.Contacts
                        select new ContactVM
                        {
                            Id = c.Id,
                            UserName = c.UserName,
                            Email = c.Email,
                            Phone = c.Phone,
                            Message = c.Message,
                            CreatedDate = c.CreatedDate,
                            Status = c.Status,
                        };
            if(!string.IsNullOrEmpty(model.KeyWord))
            {                            
                    switch (model.FilterType)
                    {
                        case "select":
                            query = query.Where(c => c.Status.Contains(model.KeyWord));
                            break;
                        case "input":
                            query = query.Where(c => c.UserName.Contains(model.KeyWord));
                            break;                     
                        default:                          
                            break;
                    }
            }
                return await _pagingService.GetPagedAsync<ContactVM>(query, model.PageIndex,model.PageSize);
        }

        public async Task<ApiResponse> ReplyEmail(ReplyModel model)
        {     
             var bodyHtml = EmailMessage.EmailBody(model.Message!);
             var Title = "Phản hồi từ khách sạn";
             await _mailService.SendEmailAsync(model.Email!, Title, bodyHtml);
            var contactDetail = _context.Contacts.FirstOrDefault(i => i.Id == model.Id);
            if (contactDetail != null) {
                contactDetail.Status = ContactStatus.Replied;
              await _context.SaveChangesAsync();
            }
            return new ApiResponse
            {
                StatusCode = 200,
                IsSuccess = true,
                Message = "Gửi thành công"
            };
             
        }
    }
}
