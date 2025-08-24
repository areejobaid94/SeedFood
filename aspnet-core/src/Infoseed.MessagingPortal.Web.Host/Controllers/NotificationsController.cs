using Infoseed.MessagingPortal.Web.Models;
using Infoseed.MessagingPortal.Web.Models.Firebase;
using Microsoft.AspNetCore.Mvc;
using Nancy.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : MessagingPortalControllerBase
    {
        private string serverKey = "AAAAKInP3fI:APA91bHUaWCGK8ea8261aoGceJ4uGd7mJLHAl5vUMDiUFskG_t5iImtYqYMaLHZpiRHyFNAA2Cl4zEQ18Gnq6pRbP57PEhw1Z8DYIL9G07wqvZcaW1Gg7gbqqOvY403IzoJfQXx-CqSi";
        private string senderId = "174110793202";
        private string webAddr = "https://fcm.googleapis.com/fcm/send";
       // private readonly INotificationService _notificationService;
        //public NotificationsController(INotificationService notificationService)
        //{
        //    _notificationService = notificationService;
        //}


        [Route("SendNotifications")]
        [HttpGet]
        public async Task SendNotificationsAsync(string pns, string to_tag)
        {
            var user = HttpContext.User.Identity.Name;
            string[] userTag = new string[1];
            userTag[0] = to_tag;
            // userTag[1] = "from:" + user;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;
            try
            {
                switch (pns.ToLower())
                {
                    case "wns":
                        // Windows 8.1 / Windows Phone 8.1
                        var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                                    "From " + user + ": " + "message" + "</text></binding></visual></toast>";
                        outcome = await Models.Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast, userTag);
                        break;
                    case "apns":
                        // iOS
                        var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + "message" + "\"}}";
                        outcome = await Models.Notifications.Instance.Hub.SendAppleNativeNotificationAsync(alert, userTag);
                        break;
                    case "fcm":
                        // Android
                        SendNotfificationModel sendNotfificationModel = new SendNotfificationModel
                        {
                            notification = new SendNotfificationModel.Notification
                            {
                                title = "infoseed",
                                body = ""
                            },
                            data = new SendNotfificationModel.Data
                            {
                                property1 = "value1",
                                property2 = 42
                            }

                        };
                        var notif = JsonConvert.SerializeObject(sendNotfificationModel).ToString();
                        // var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + "message" + "\"}}";
                        outcome = await Models.Notifications.Instance.Hub.SendFcmNativeNotificationAsync(notif);
                        break;
                }
            }
            catch (Exception )
            {


            }


            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }

        }

        [Route("send")]
        [HttpPost]
        public async Task<IActionResult> SendNotification(NotificationModel notificationModel)
        {
           // var result = await _notificationService.SendNotification(notificationModel);
            return Ok();
        }

        [Route("sendw")]
        [HttpPost]
        public string SendNotification(string DeviceToken, string title, string msg)
        {

            var result = "-1";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
            httpWebRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
            httpWebRequest.Method = "POST";



            //
            // var response = FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync(
            // registrationTokens, topic);




            var registrationTokens = new List<string>()
{
"fNwZgcYFTBmU54WwFzsmdj:APA91bEtdu76r5XvhHWVr3LJZWoGzfeNz8ck3zGHyxXk0XUegq85-oaEKx9P9ZRVwtHIzjCcd9gT6VIpz2j2DHJvYj-eUVcGNRXy5NoDz0lqOVGSIcoNyflBBkS30g3ZyUoEpuYn9N4Z",
//"dTI4_qrLREuSSv32JHWGBy:APA91bH3XVzTfAlxav8G-2ZjEx3idCdha8hSUDm-QAmlfvZLnKDZ0_RnOoep4TrAH7zHUwIP0dAkEaUWrexmAh-FIoww0zcpmssRfCyz6CM3AUnh0AVx9K8mJMqYHbPRyrf-JINGfPfS",




"fvNS2T9zRT-krmVR-WEBzt:APA91bEkE8-5TuunHoD3q4kT1bfld-4RkHhYrhXR6R9s_o7QNZX-OZODmNRcjl-rgxxzIiw4IBlgSwATQ7JkSM7q5XwABb-TT-aHCJEXB6eFkAFG899wDsqCN0e9O1nbsvzmG1xAm94e",
// ...
// "d__Z7JX4RjyUt3Wus4F5-K:APA91bGZT2EBQrYx14jAEoebkyPDg8a0PihpUtDuQqlefSgofZgYsOkHP8w6jhDx6hy7c2pDADtzKnVzyUuA1CZNN6hOUmlvfqW1n8nNw1V-Nt7gFV8YW6IASsUnNIcJN5pIYyRafX5E",
};


            var payload = new
            {
                registration_ids = registrationTokens,
                notification = new
                {

                    body = "test",
                    title = "test",
                    sound = "cancel"
                },
                priority = "high",
                payload = new
                {
                    aps = new
                    {
                        sound = "cancel"
                    }
                },
                data = new
                {
                    // google.sent_time= 1588863942303,
                    click_action = "FLUTTER_NOTIFICATION_CLICK",
                    collapse_key = "com.infoseed.seedfoodmobile",
                    sound = "cancel",
                    //" google.ttl": 60
                }
                ,
                android= new {
                notification=new {
                    channel_id="high_importance_channel_cancel"
                }
            }

        };


            //var payload = new
            //{
            //    registration_ids = registrationTokens,





            //    // to = FirebaseMessaging.DefaultInstance.SubscribeToTopicAsync("iyuiyu").Result,



            //    priority = "high",
            //    content_available = true,
            //    notification = new
            //    {
            //        body = msg,
            //        title = title,
            //        sound = "sound"
            //    },
            //};
            var serializer = new JavaScriptSerializer();
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = serializer.Serialize(payload);
                streamWriter.Write(json);
                streamWriter.Flush();
            }



            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }


    }
}
