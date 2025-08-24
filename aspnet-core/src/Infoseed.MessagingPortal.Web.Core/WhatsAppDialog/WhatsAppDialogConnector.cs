
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Models.WhatsAppDialog;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.WhatsAppDialog
{

    public static class WhatsAppDialogConnector
    {
        private static string URL= "https://waba.360dialog.io/v1/messages";
       //  private static string URL = "https://waba-sandbox.360dialog.io/v1/messages";



        public static async Task<HttpStatusCode> PostMsgToSmooch(string key, SendWhatsAppD360Model msg, TelemetryClient telemetry, bool IsModel2 = false)
        {
            if (msg == null)
            {
                telemetry.TrackTrace("msg is null", SeverityLevel.Critical);
                return HttpStatusCode.BadRequest;
            }

                var rezID =   UploadMedia(key, msg , telemetry).Result;

                var rez =  getD360HttpClientAsync(key, msg, rezID, telemetry, IsModel2);

                return rez.Result;

        }

        private static object GetContentMessageModel(WhatsAppDialogModel msg)
        {
            throw new NotImplementedException();
        }
        public static async Task<HttpStatusCode> SendTextClientAsync(string appKey, SendWhatsAppD360Model msg,TelemetryClient telemetry)
        {

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), URL))
                    {

                       
                        var Jso = JsonConvert.SerializeObject(msg,
                           Newtonsoft.Json.Formatting.None,
                           new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           });

                        request.Headers.TryAddWithoutValidation("D360-Api-Key", appKey);
                        request.Content = new StringContent(Jso);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        var response = await httpClient.SendAsync(request);

                        if(response.StatusCode == HttpStatusCode.Created)
                        {

                            return response.StatusCode;
                        }
                        else
                        {

                            telemetry.TrackTrace("error to send 360 deialog : "+ Jso, SeverityLevel.Critical);
                            return HttpStatusCode.BadRequest;
                        }


                       
                       
                    }
                }

            }
            catch(Exception ex)
            {
                telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                return HttpStatusCode.BadRequest;
            }
           

        }

        public static async Task<HttpStatusCode> getD360HttpClientAsync(string appKey, SendWhatsAppD360Model msg, MediaIDModel mediaIDModel, TelemetryClient telemetry ,bool IsModel2=false)
        {
            SendWhatsAppD360Model masseges = new SendWhatsAppD360Model();
            if (msg.type.ToLower() == "image")
            {

                if (!IsModel2)
                {
                    masseges = new SendWhatsAppD360Model
                    {
                        to = msg.to,

                        type = "image",
                        image = new SendWhatsAppD360Model.Image
                        {
                            id = mediaIDModel.media.FirstOrDefault().id,

                        }

                    };
                }
                else
                {
                    
                    masseges = new SendWhatsAppD360Model
                    {
                        to = msg.to,

                        type = "image",
                        image = new SendWhatsAppD360Model.Image
                        {
                            link = msg.mediaUrl

                        }

                    };
                }



            }
            else if (msg.type.ToLower() == "video")
            {
                if (!IsModel2)
                {
                    masseges = new SendWhatsAppD360Model
                    {

                        to = msg.to,
                        type = "video",
                        video = new SendWhatsAppD360Model.Video
                        {
                            id = mediaIDModel.media.FirstOrDefault().id,

                        }

                    };
                }
                else
                {
                    masseges = new SendWhatsAppD360Model
                    {

                        to = msg.to,
                        type = "video",
                        video = new SendWhatsAppD360Model.Video
                        {
                            link = msg.mediaUrl

                        }

                    };
                }



            }
            else if (msg.type.ToLower() == "voice")
            {

                SendWhatsAppD360Model sendWhatsAppD360Model = new SendWhatsAppD360Model
                {
                    type = "text",
                    to = msg.to,
                    text = new SendWhatsAppD360Model.Text
                    {
                        body = msg.mediaUrl
                    }
                };
                var rez = SendTextClientAsync(appKey, sendWhatsAppD360Model, telemetry);
                return rez.Result;

            }
            else if(msg.type.ToLower() == "document"||msg.type.ToLower() == "file")
            {
                if (!IsModel2)
                {
                    masseges = new SendWhatsAppD360Model
                    {

                        to = msg.to,
                        type = "document",
                        document = new SendWhatsAppD360Model.Document
                        {
                            id = mediaIDModel.media.FirstOrDefault().id,

                        }

                    };
                }
                else
                {
                    masseges = new SendWhatsAppD360Model
                    {

                        to = msg.to,
                        type = "document",
                        document = new SendWhatsAppD360Model.Document
                        {
                            filename= msg.fileName,
                            link = msg.mediaUrl

                        }

                    };

                }


            }
            else 
            {
                masseges=msg;
                //var rez = SendTextClientAsync(appKey, msg, telemetry);
                //return rez.Result;

            }
            //else if (msg.type.ToLower() == "interactive")
            //{
            //    var rez = SendTextClientAsync(appKey, msg, telemetry);
            //    return rez.Result;

            //}


            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), URL))
                    {
                       
                        var Jso = JsonConvert.SerializeObject(masseges,
                           Newtonsoft.Json.Formatting.None,
                           new JsonSerializerSettings
                           {
                               NullValueHandling = NullValueHandling.Ignore
                           });

                        request.Headers.TryAddWithoutValidation("D360-Api-Key", appKey);
                        request.Content = new StringContent(Jso);
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                        var response = await httpClient.SendAsync(request);

                        if (response.StatusCode == HttpStatusCode.Created)
                        {
                            if(msg.type.ToLower() != "interactive" && msg.type.ToLower() != "text")
                            {
                                Thread.Sleep(3000);
                            }

                            return response.StatusCode;
                        }
                        else
                        {

                            telemetry.TrackTrace("error to send 360 deialog : " + Jso, SeverityLevel.Critical);
                            return HttpStatusCode.BadRequest;
                        }
                    }
                }

            }
            catch (Exception ex )
            {


                telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                return HttpStatusCode.BadRequest;

            }
            
            
        }

        public static async Task<MediaIDModel> UploadMedia(string appKey, SendWhatsAppD360Model msg, TelemetryClient telemetry)
        {

            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://waba.360dialog.io/v1/media"))
                    {

                        var stream = new FileStream(msg.filePath, FileMode.Open);
                        request.Content = new StreamContent(stream);
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(msg.typeContent);
                        request.Headers.TryAddWithoutValidation("D360-Api-Key", appKey);

                        var response = await httpClient.SendAsync(request);
                        string Jso = response.Content.ReadAsStringAsync().Result;

                        try
                        {
                            //// Check if file exists with its full path    
                            //if (System.IO.File.Exists("wwwroot\\UploadFile\\" + msg.fileName))
                            //{
                            //    // If file found, delete it    
                            //    System.IO.File.Delete("wwwroot\\UploadFile\\" + msg.fileName);

                            //    telemetry.TrackTrace("File deleted. :" + Jso, SeverityLevel.Critical);
                            //    Console.WriteLine("File deleted.");
                            //}
                            //else {

                            //    telemetry.TrackTrace("File deleted. :"+ Jso, SeverityLevel.Critical);
                            //    Console.WriteLine("File not found");

                                    
                            //}
                        }
                        catch (IOException ioExp)
                        {
                            Console.WriteLine(ioExp.Message);

                        }


                        return JsonConvert.DeserializeObject<MediaIDModel>(Jso);

                    }
                }

            }
            catch(Exception ex)
            {

                telemetry.TrackTrace(ex.Message, SeverityLevel.Critical);
                return null;
            }

          

        }

    }
}
