using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Hotel.Services
{

    public interface IFileServices {
        string Upload(string file);
        void Delete(string file);
    }

    public class FileServices : IFileServices
    {
        public  string Upload(string base64Image)
        {
            try
            {
                if (ImageExist(base64Image))
                {
                    return base64Image;
                }
                   byte[] imageBytes = Convert.FromBase64String(base64Image);
                    var fileName = $"{Guid.NewGuid()}.jpeg";
                    var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    var filePath = Path.Combine(imagesFolder, fileName);
                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    return fileName;
            }
            catch 
            {
                throw new Exception("lỗi khi thêm ảnh");
            }

        }

        public bool ImageExist(string fileName)
        {
            var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            var filePath = Path.Combine(imagesFolder, fileName);          
            return File.Exists(filePath);
        }

        public void Delete(string fileName)
        {
            try
            {              

                var imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                var filePath = Path.Combine(imagesFolder, fileName);

                
                if (File.Exists(filePath))
                {               
                    File.Delete(filePath);
                    
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while deleting image: {ex.Message}");
            }
        }

    }
}
