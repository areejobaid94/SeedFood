using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models
{
    public class SendNotfificationModel
    {
     
            public Notification notification { get; set; }
            public Data data { get; set; }
       

        public class Notification
        {
            public string title { get; set; }
            public string body { get; set; }
        }

        public class Data
        {
            public string property1 { get; set; }
            public int property2 { get; set; }
        }

    }
}
