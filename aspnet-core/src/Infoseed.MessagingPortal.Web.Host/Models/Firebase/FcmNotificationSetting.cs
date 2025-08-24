using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.Firebase
{
    public class FcmNotificationSetting
    {
        public static string SenderId { get; set; }
        public static  string ServerKey { get; set; }
        public static string webAddr { get; set; }
    }
}