using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Data.Ultils.Email
{
    public static class EmailMessage
    {
        public static string EmailBody(string code)
        {
            var html = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Document</title>\r\n</head>\r\n<body>\r\n    <p>Nội dung: {code}</p>\r\n</body>\r\n</html>";
            return html;
        }
    }
}
