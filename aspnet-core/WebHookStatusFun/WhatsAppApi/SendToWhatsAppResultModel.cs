using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace WebHookStatusFun
{
    public class SendToWhatsAppResultModel
    {
       public HttpResponseMessage response { get; set; }
       public string content { get; set; }
    }
}
