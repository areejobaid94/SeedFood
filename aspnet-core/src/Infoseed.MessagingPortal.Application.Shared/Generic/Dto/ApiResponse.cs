using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Generic.Dto
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorEn { get; set; }
    }
}
