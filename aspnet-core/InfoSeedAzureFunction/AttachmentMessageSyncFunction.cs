using Infoseed.InfoSeedAzureFunction;
using InfoSeedAzureFunction.DAL;
using InfoSeedAzureFunction.Model;
using InfoSeedAzureFunction.WhatsAppApi;
using Microsoft.Azure.WebJobs;
using NAudio.Lame;
using NAudio.Vorbis;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InfoSeedAzureFunction
{
    public class AttachmentMessageSyncFunction
    {

        [FunctionName("AttachmentMessageFunctionTest")]
        public static void ProcessQueueMessage([QueueTrigger("attachment-messages", Connection = "AzureWebJobsStorage")] string message, TextWriter log)
        {
            try
            {

                // SocketIOManager.Init();
                AttachmentMessageModel obj = JsonConvert.DeserializeObject<AttachmentMessageModel>(message);
                Sync(obj);
            }
            catch
            {

            }


        }


        private static async Task Sync(AttachmentMessageModel attachmentMessageModel)
        {

            var RetrievingMedia = RetrievingMediaAsync(attachmentMessageModel.AttachmentId, attachmentMessageModel.FcToken).Result;

            var extention = "";
            if (RetrievingMedia.contentType=="application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                extention=".docx";

            }
            else if (RetrievingMedia.contentType=="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {

                extention=".xlsx";

            }
            else
            {
                extention = "." + RetrievingMedia.mime_type.Split("/").LastOrDefault();

            }

             
            var type =  RetrievingMedia.mime_type.Split("/")[0];
            AzureBlobProvider azureBlobProvider = new AzureBlobProvider();

            AttachmentContent attachmentContent = new AttachmentContent()
            {
                Content = RetrievingMedia.contentByte,
                Extension = extention,
                MimeType = RetrievingMedia.mime_type,
                AttacmentName = RetrievingMedia.id.ToString()
            };

            var url = azureBlobProvider.Save(attachmentContent).Result;
            UpdateCustomerChatAsync(attachmentMessageModel.CustomerModel, url, type);

        }

        private static async Task<WhatsAppAttachmentModel> RetrievingMediaAsync(string mediaId, string fbToken)
        {

            WhatsAppAttachmentModel attachmentModel = new WhatsAppAttachmentModel();


            using (var httpClient = new HttpClient())
            {


                var FBUrl = Constant.WhatsAppApiUrl + mediaId;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbToken);

                using (var response = await httpClient.GetAsync(FBUrl))
                {
                    using (var content = response.Content)
                    {
                        // WhatsAppMediaResponse whatsAppMediaResponse = new WhatsAppMediaResponse();

                        attachmentModel = JsonConvert.DeserializeObject<WhatsAppAttachmentModel>(await content.ReadAsStringAsync());
                        // attachmentModel.contentByte = response.Content.ReadAsByteArrayAsync().Result;
                        //  attachmentModel.contentType = response.Content.Headers.ContentType.MediaType;


                    }
                }
            }
          
           

            var attachmentModel2 = await DownloadMediaAsync(attachmentModel.url, fbToken);


            attachmentModel2.sha256=attachmentModel.sha256;
            attachmentModel2.messaging_product=attachmentModel.messaging_product;
            attachmentModel2.mime_type=attachmentModel.mime_type;
            attachmentModel2.url=attachmentModel.url;
            attachmentModel2.file_size=attachmentModel.file_size;
            attachmentModel2.id=attachmentModel.id;




            return attachmentModel2;

        }


        private static async Task<WhatsAppAttachmentModel> DownloadMediaAsync(string mediaurl, string fbToken)
        {

            WhatsAppAttachmentModel attachmentModel = new WhatsAppAttachmentModel();


            var client = new RestClient(mediaurl);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer " + fbToken);
            IRestResponse response = client.Execute(request);

            attachmentModel.contentByte = response.RawBytes;
            attachmentModel.contentType = response.ContentType;


            return attachmentModel;

        }



        public static async void UpdateCustomerChatAsync(string CustomerModel, string url,string type)
        {
            try{
                CustomerModel customer = JsonConvert.DeserializeObject<CustomerModel>(CustomerModel);


                var text = "";
                if (customer.customerChat!=null)
                {
                    text=customer.customerChat.text;

                }

                var itemsCollection = new DocumentDBHelper<CustomerChat>(CollectionTypes.ItemsCollection);
                CustomerChat customerChat = new CustomerChat()
                {
                    messageId = "",
                    TenantId = customer.TenantId,
                    userId = customer.userId,
                    text = text,
                    type = type,//hjhj
                    CreateDate = DateTime.Now,
                    status = (int)Messagestatus.New,
                    sender = MessageSenderType.Customer,
                    ItemType= ContainerItemTypes.ConversationItem,
                    mediaUrl = url,
                    UnreadMessagesCount = 0,
                    agentName = "admin",
                    agentId = "",
                };




                var Result = itemsCollection.CreateItemAsync(customerChat).Result;

                //customerModel.customerChat = customerChat;
                // var objCustomer = itemsCollectionCustomer.UpdateItemAsync(customerModel._self, customerModel).Result;

                //SocketIOManager.SendChat(customerChat, customerChat.TenantId.Value);

            }
            catch
            {


            }


        }



    }
}
