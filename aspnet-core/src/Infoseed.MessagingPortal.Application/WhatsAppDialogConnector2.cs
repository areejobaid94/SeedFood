using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal
{
    public  class WhatsAppDialogConnector2
    {
        public static async Task<HttpStatusCode> PostMsgToSmooch(string key, Send2WhatsAppD360Model msg)
        {
            if (msg == null)
            {
                return HttpStatusCode.BadRequest;
            }

            if (msg.type == "voice")
            {
                Send2WhatsAppD360Model sendWhatsAppD360Model = new Send2WhatsAppD360Model
                {
                    type = "text",
                    to = msg.to,
                    text = new Send2WhatsAppD360Model.Text
                    {
                        body = msg.mediaUrl
                    }
                };

                var rez = SendTextClientAsync(key, sendWhatsAppD360Model);
                return rez.Result;
            }
            if (msg.type == "text")
            {

                var rez = SendTextClientAsync(key, msg);
                return rez.Result;
            }
            else
            {

                if (msg.type == "voice")
                {
                    var rez = getD360HttpClientAsync(key, msg, null);

                    return rez.Result;

                }
                var rezID = UploadMedia(key, msg).Result;

                if (rezID != null)
                {
                    var rez = getD360HttpClientAsync(key, msg, rezID);

                    return rez.Result;

                }
                return HttpStatusCode.BadRequest;

            }

        }


        public static async Task<HttpStatusCode> SendTextClientAsync(string appKey, Send2WhatsAppD360Model msg)
        {

            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://waba.360dialog.io/v1/messages"))
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

                    return response.StatusCode;
                }
            }

        }

        public static async Task<HttpStatusCode> getD360HttpClientAsync(string appKey, Send2WhatsAppD360Model msg, MediaIDModel2 mediaIDModel)
        {
            Send2WhatsAppD360Model masseges = new Send2WhatsAppD360Model();
            if (msg.type.ToLower() == "image")
            {


                masseges = new Send2WhatsAppD360Model
                {
                    to = msg.to,

                    type = "image",
                    image = new Send2WhatsAppD360Model.Image
                    {
                        id = mediaIDModel.media.FirstOrDefault().id,

                    }

                };


            }
            else if (msg.type.ToLower() == "video")
            {
                masseges = new Send2WhatsAppD360Model
                {

                    to = msg.to,
                    type = "video",
                    video = new Send2WhatsAppD360Model.Video
                    {
                        id = mediaIDModel.media.FirstOrDefault().id,

                    }

                };

            }
            else if (msg.type.ToLower() == "voice")
            {
                masseges = new Send2WhatsAppD360Model
                {

                    to = msg.to,
                    type = "audio",
                    audio = new Send2WhatsAppD360Model.Audio
                    {
                        //id = mediaIDModel.media.FirstOrDefault().id,
                        link = msg.mediaUrl
                    }

                };

            }
            else if (msg.type.ToLower() == "document")
            {
                masseges = new Send2WhatsAppD360Model
                {

                    to = msg.to,
                    type = "document",
                    document = new Send2WhatsAppD360Model.Document
                    {
                        id = mediaIDModel.media.FirstOrDefault().id,

                    }

                };

            }


            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://waba.360dialog.io/v1/messages"))
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

                    return response.StatusCode;
                }
            }

        }

        public static async Task<MediaIDModel2> UploadMedia(string appKey, Send2WhatsAppD360Model msg)
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
                        //    Console.WriteLine("File deleted.");
                        //}
                        //else Console.WriteLine("File not found");
                    }
                    catch (IOException )
                    {
                      //  Console.WriteLine(ioExp.Message);

                    }


                    return JsonConvert.DeserializeObject<MediaIDModel2>(Jso);

                }
            }

        }

    }
}
