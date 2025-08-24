using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace InfoSeedAzureFunction
{

    public static class WhatsAppDialogConnector
    {
        private static string URL= "https://waba.360dialog.io/v1/messages";
       //  private static string URL = "https://waba-sandbox.360dialog.io/v1/messages";



        public static async Task<HttpStatusCode> PostMsgToSmooch(string key, SendWhatsAppD360Model msg, TelemetryClient telemetry)
        {
            if (msg == null)
            {
                //telemetry.TrackTrace("msg is null", SeverityLevel.Critical);
                return HttpStatusCode.BadRequest;
            }

            if ( msg.type == "voice")
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

                var rez = SendTextClientAsync(key, sendWhatsAppD360Model , telemetry);

                return rez.Result;
            }
            if (msg.type == "text")
            {

                var rez = SendTextClientAsync(key, msg , telemetry);
                return rez.Result;
            }
            else
            {
                if (msg.type == "interactive")
                {
                    var rez = SendTextClientAsync(key, msg, telemetry);
                    return rez.Result;

                }

                if (msg.type == "voice")
                {
                    var rez = getD360HttpClientAsync(key, msg, null, telemetry);

                    return rez.Result;

                }
                var rezID = UploadMedia(key, msg , telemetry).Result;

                if (rezID != null)
                {
                    var rez = getD360HttpClientAsync(key, msg, rezID, telemetry);

                    return rez.Result;

                }
                return HttpStatusCode.BadRequest;

            }
           
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

                          //  telemetry.TrackTrace("error to send 360 deialog : "+ Jso, SeverityLevel.Critical);
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

        public static async Task<HttpStatusCode> getD360HttpClientAsync(string appKey, SendWhatsAppD360Model msg, MediaIDModel mediaIDModel, TelemetryClient telemetry)
        {
            SendWhatsAppD360Model masseges = new SendWhatsAppD360Model();
            if (msg.type.ToLower() == "image")
            {


                masseges = new SendWhatsAppD360Model
                {
                     to= msg.to,

                    type = "image",
                    image = new SendWhatsAppD360Model.Image
                    {
                         id= mediaIDModel.media.FirstOrDefault().id,
                      
                    }

                };


            }
            else if (msg.type.ToLower() == "video")
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
            else if (msg.type.ToLower() == "voice")
            {
                masseges = new SendWhatsAppD360Model
                {

                    to = msg.to,
                    type = "audio",
                     audio = new SendWhatsAppD360Model.Audio
                    {
                        //id = mediaIDModel.media.FirstOrDefault().id,
                       link= msg.mediaUrl
                     }

                };

            }
            else if(msg.type.ToLower() == "document")
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

                            return response.StatusCode;
                        }
                        else
                        {

                           // telemetry.TrackTrace("error to send 360 deialog : " + Jso, SeverityLevel.Critical);
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
                            // Check if file exists with its full path    
                            //if (System.IO.File.Exists("wwwroot\\UploadFile\\" + msg.fileName))
                            //{
                            //    // If file found, delete it    
                            //    System.IO.File.Delete("wwwroot\\UploadFile\\" + msg.fileName);

                            //    //telemetry.TrackTrace("File deleted. :" + Jso, SeverityLevel.Critical);
                            //    Console.WriteLine("File deleted.");
                            //}
                            //else {

                            //   // telemetry.TrackTrace("File deleted. :"+ Jso, SeverityLevel.Critical);
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
