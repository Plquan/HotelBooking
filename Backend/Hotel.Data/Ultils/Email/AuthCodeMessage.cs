using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Ultils.Email
{
    public class AuthCodeMessage
    {
        public static string EmailBody(string title,string code,int time)
        {
            var html = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n  <title>Document</title>\r\n</head>\r\n<h1>{title}</h1>\r\n</br>\r\n<h1>Mã code của bạn là: {code}</h1>\r\n</br>\r\n<h1>Thời gian hết hạn: {time} phút</h1>\r\n\r\n</body>\r\n\r\n</html>";
             return html;
        }
    }
}
