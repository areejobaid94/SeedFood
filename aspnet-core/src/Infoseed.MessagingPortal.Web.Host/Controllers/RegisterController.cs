

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.NotificationHubs;
//using Microsoft.Azure.NotificationHubs.Messaging;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Threading.Tasks;

//// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

//namespace Infoseed.MessagingPortal.Web.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RegisterController : MessagingPortalControllerBase
//    {
//        private NotificationHubClient hub;

//        public RegisterController()
//        {
//            hub = Models.Notifications.Instance.Hub;
//        }

//        public class DeviceRegistration
//        {
//            public string Platform { get; set; }
//            public string Handle { get; set; }
//            public string[] Tags { get; set; }
//        }


//        // POST api/register
//        // This creates a registration id
//        public async Task<string> Post(string handle = null)
//        {
//            string newRegistrationId = null;

//            // make sure there are no existing registrations for this push handle (used for iOS and Android)
//            if (handle != null)
//            {
//                var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

//                foreach (RegistrationDescription registration in registrations)
//                {
//                    if (newRegistrationId == null)
//                    {
//                        newRegistrationId = registration.RegistrationId;
//                    }
//                    else
//                    {
//                        await hub.DeleteRegistrationAsync(registration);
//                    }
//                }
//            }

//            if (newRegistrationId == null)
//                newRegistrationId = await hub.CreateRegistrationIdAsync();

//            return newRegistrationId;
//        }

//        // PUT api/register/5
//        // This creates or updates a registration (with provided channelURI) at the specified id
//        public async Task<HttpResponseMessage> Put(string id, DeviceRegistration deviceUpdate)
//        {
//            RegistrationDescription registration = null;
//            switch (deviceUpdate.Platform)
//            {
//                case "mpns":
//                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
//                    break;
//                case "wns":
//                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
//                    break;
//                case "apns":
//                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
//                    break;
//                case "fcm":
//                    registration = new FcmRegistrationDescription(deviceUpdate.Handle);
//                    break;
//                default:
//                    throw new System.Web.Http.HttpResponseException(HttpStatusCode.BadRequest);
//            }

//            registration.RegistrationId = id;
//            var username = HttpContext.User.Identity.Name;

//            // add check if user is allowed to add these tags
//            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
//            registration.Tags.Add("username:" + username);

//            try
//            {
//                await hub.CreateOrUpdateRegistrationAsync(registration);
//            }
//            catch (MessagingException e)
//            {
//                ReturnGoneIfHubResponseIsGone(e);
//            }

//            return null;
//        }

//        // DELETE api/register/5
//        public async Task<HttpResponseMessage> Delete(string id)
//        {
//            await hub.DeleteRegistrationAsync(id);
//            return null;
//        }

//        private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
//        {
//            var webex = e.InnerException as WebException;
//            if (webex.Status == WebExceptionStatus.ProtocolError)
//            {
//                var response = (HttpWebResponse)webex.Response;
//                if (response.StatusCode == HttpStatusCode.Gone)
//                    throw new HttpRequestException(HttpStatusCode.Gone.ToString());
//            }
//        }
//    }
//}
