using CorePush.Google;
using Infoseed.MessagingPortal.Web.Models.Firebase;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Web.Models.Firebase.GoogleNotification;

namespace Infoseed.MessagingPortal.Web.Services
{
    public interface INotificationService
    {
        Task<ResponseModel> SendNotification(NotificationModel notificationModel);
    }

    public class NotificationService : INotificationService
    {
        private readonly FcmNotificationSetting _fcmNotificationSetting;
        public NotificationService(IOptions<FcmNotificationSetting> settings)
        {
            _fcmNotificationSetting = settings.Value;
        }

        public async Task<ResponseModel> SendNotification(NotificationModel notificationModel)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                if (notificationModel.IsAndroiodDevice)
                {
                    /* FCM Sender (Android Device) */
                    FcmSettings settings = new FcmSettings()
                    {
                       // SenderId = _fcmNotificationSetting.SenderId,
                       // ServerKey = _fcmNotificationSetting.ServerKey
                        SenderId = "504731491640",
                        ServerKey = "AAAAdYRXXTg:APA91bG6mt8s843EBVwSjydUJ8Q4lHG9RQDyk7-KIGUt_znU1TSUzngBtj1hOpGRIAvrGohLbTEdHXdU4j6FATXZ-p5K6iXLWtfrblFSHFLpKBpZu8gIOkK2J8bNowTANvoeXRoEEFJ4"
                    };
                    HttpClient httpClient = new HttpClient();

                    string authorizationKey = string.Format("key={0}", settings.ServerKey);
                    string deviceToken = notificationModel.DeviceId;

                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationKey);
                    httpClient.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    DataPayload dataPayload = new DataPayload();
                    dataPayload.Title = notificationModel.Title;
                    dataPayload.Body = notificationModel.Body;

                    GoogleNotification notification = new GoogleNotification();
                    notification.Data = dataPayload;
                    notification.Notification = dataPayload;

                    var fcm = new FcmSender(settings, httpClient);
                    var fcmSendResponse = await fcm.SendAsync("d__Z7JX4RjyUt3Wus4F5-K:APA91bGZT2EBQrYx14jAEoebkyPDg8a0PihpUtDuQqlefSgofZgYsOkHP8w6jhDx6hy7c2pDADtzKnVzyUuA1CZNN6hOUmlvfqW1n8nNw1V-Nt7gFV8YW6IASsUnNIcJN5pIYyRafX5E", notification);

                    if (fcmSendResponse.IsSuccess())
                    {
                        response.IsSuccess = true;
                        response.Message = "Notification sent successfully";
                        return response;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = fcmSendResponse.Results[0].Error;
                        return response;
                    }
                }
                else
                {
                    /* Code here for APN Sender (iOS Device) */
                    //var apn = new ApnSender(apnSettings, httpClient);
                    //await apn.SendAsync(notification, deviceToken);
                }
                return response;
            }
            catch (Exception )
            {
                response.IsSuccess = false;
                response.Message = "Something went wrong";
                return response;
            }
        }
    }
}