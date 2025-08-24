using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Payment.Interfaces
{
    public interface IAPIBase
    {
         string BaseUrl { get; set; }
         //string access_token { get; set; }
         //string refresh_token { get; set; }
         IConfiguration _configuration { get; set; }
    }
}
