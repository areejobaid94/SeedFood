using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models
{
    public class Notifications 
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://infoseednotificationhubprodns.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=esWXqsL6HUocg/VQlCXg70g7e/olWYD9OPfwrMtyofs=",

                 "infoseednotificationhubProd");
        }
    }
}
