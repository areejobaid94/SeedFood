using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static CampaignSendNew.WhatsAppApi.PostWhatsAppMessageModel;

namespace CampaignSendNew.WhatsAppApi
{

    public class WhatsAppAppService
    {
    
        public async Task<bool> SendToWhatsApp(string postBody, string phonnumberId, string fbToken)
        {

            using (var httpClient = new HttpClient())
            {


                var postUrl = Constant.WhatsAppApiUrl + phonnumberId + "/messages";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbToken);

                using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                {
                    using (var content = response.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        return true;

                    }
                }
            }
            return false;
        }

        public async Task<SendToWhatsAppResultModel> SendMsgToWhatsApp(string postBody, string phonnumberId, string fbToken)
        {
            using (var httpClient = new HttpClient())
            {

                var postUrl = Constant.WhatsAppApiUrl + phonnumberId + "/messages";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbToken);

                using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                {

                    using (var content = response.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        SendToWhatsAppResultModel sendToWhatsAppResultModel = new SendToWhatsAppResultModel();

                        sendToWhatsAppResultModel.response = response;
                        sendToWhatsAppResultModel.content = result;
                        return sendToWhatsAppResultModel;
                    }
                }
            }
        }

        public async Task<PostWhatsAppMessageModel> BotChatWithCustomer(Activity msgBot, string from, string botId)
        {

            PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();


            bool isBotSendAttachments = false;
            bool Islist = true;

            if (msgBot.SuggestedActions == null)
            {


                if (msgBot.InputHint == "image" || msgBot.InputHint == "file" || msgBot.InputHint == "video")
                {

                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {

                        mediaUrl = msgBot.Text.Replace("\r\n", ""),
                        to = from,
                        type = msgBot.InputHint,
                        fileName = msgBot.Speak,
                        document = new PostWhatsAppMessageModel.Document
                        {
                            link = msgBot.Text.Replace("\r\n", ""),
                            filename = msgBot.Speak,
                        }

                    };


                    isBotSendAttachments = true;
                }
                else
                {
                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {
                        to = from,
                        type = "text",
                        text = new PostWhatsAppMessageModel.Text
                        {
                            body = msgBot.Text

                        }

                    };
                }



            }
            else
            {
                if (msgBot.Attachments == null)
                {
                    List<InteractiveButtonModel> buttons = new List<InteractiveButtonModel>();
                    foreach (var button in msgBot.SuggestedActions.Actions)
                    {
                        buttons.Add(new InteractiveButtonModel
                        {
                            reply = new InteractiveButtonsReplyModel { id = button.Title, title = button.Title },
                            type = "reply"
                        });

                    }




                    if (msgBot.Summary == null)
                    {

                        if (msgBot.Text.Length <= 1000)
                        {
                            postWhatsAppMessageModel = new PostWhatsAppMessageModel
                            {
                                to = from,
                                type = "interactive",
                                postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel()
                                {
                                    type = "button",
                                    // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                                    interactive = new InteractiveButtonsModel()
                                    {
                                        type = "button",



                                        body = new InteractiveBodyModel { text = msgBot.Text },


                                        // footer = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
                                        action = new InteractiveButtonActionModel
                                        {
                                            buttons = buttons

                                        }
                                    }



                                }

                            };
                        }
                        else
                        {
                            postWhatsAppMessageModel = new PostWhatsAppMessageModel
                            {
                                to = from,
                                type = "text",
                                text = new PostWhatsAppMessageModel.Text
                                {
                                    body = msgBot.Text

                                }

                            };

                            postWhatsAppMessageModel = new PostWhatsAppMessageModel
                            {
                                to = from,
                                type = "interactive",
                                postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
                                {
                                    type = "button",
                                    interactive = new InteractiveButtonsModel
                                    {
                                        type = "button",
                                        // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                                        body = new InteractiveBodyModel { text = msgBot.Summary },
                                        // footer = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
                                        action = new InteractiveButtonActionModel
                                        {
                                            buttons = buttons

                                        }
                                    }


                                }

                            };



                        }




                    }
                    else
                    {

                        if (msgBot.Summary.Contains("هل تريد") || msgBot.Summary.Contains("Do you want"))
                        {

                            if (msgBot.Text.Length <= 1000)
                            {
                                postWhatsAppMessageModel = new PostWhatsAppMessageModel
                                {
                                    to = from,
                                    type = "interactive",
                                    postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
                                    {
                                        type = "button",
                                        interactive = new InteractiveButtonsModel
                                        {
                                            type = "button",
                                            // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                                            body = new InteractiveBodyModel { text = msgBot.Text },
                                            footer = new InteractiveFooterModel { text = msgBot.Summary },
                                            action = new InteractiveButtonActionModel
                                            {
                                                buttons = buttons

                                            }
                                        }


                                    }


                                };


                                //postWhatsAppMessageModel = new PostWhatsAppMessageModel
                                //{
                                //    to = from,
                                //    type = "interactive",
                                //    postWhatsAppInteractiveModel = new PostWhatsAppMessageModel.PostWhatsAppInteractiveModel
                                //    {

                                //        interactiveButtonsModel = new InteractiveButtonsModel
                                //        {
                                //            type = "button",
                                //            // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                                //            body = new InteractiveButtonsBodyModel { text = msgBot.Summary },
                                //         //   fo = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
                                //            action = new InteractiveButtonActionModel
                                //            {
                                //                buttons = buttons

                                //            }
                                //        }


                                //    }

                                //};






                            }
                            else
                            {
                                postWhatsAppMessageModel = new PostWhatsAppMessageModel
                                {
                                    to = from,
                                    type = "text",
                                    text = new PostWhatsAppMessageModel.Text
                                    {
                                        body = msgBot.Text

                                    }

                                };

                                //var result22 = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, postWhatsAppMessageModel, telemetry).Result;

                                ////update Bot massage in cosmoDB 

                                //if (result22 == HttpStatusCode.Created)
                                //{

                                //    Content message = contentM(msgBot, Tenant.botId);
                                //    var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                                //    Customer.CreateDate = CustomerChat.CreateDate;
                                //    Customer.customerChat = CustomerChat;
                                //    var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                                //    SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                                //}
                                //else
                                //{
                                //    this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog , the user name is  : {Customer.displayName}", SeverityLevel.Critical);
                                //    MessagesSent[Customer.userId] = false;
                                //    return Ok();

                                //}


                                postWhatsAppMessageModel = new PostWhatsAppMessageModel
                                {
                                    to = from,
                                    type = "interactive",
                                    postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
                                    {
                                        type = "button",
                                        interactive = new InteractiveButtonsModel
                                        {
                                            type = "button",
                                            // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                                            body = new InteractiveBodyModel { text = msgBot.Summary },
                                            //   fo = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
                                            action = new InteractiveButtonActionModel
                                            {
                                                buttons = buttons

                                            }
                                        }


                                    }

                                };





                            }



                        }
                        else
                        {

                            postWhatsAppMessageModel = new PostWhatsAppMessageModel
                            {
                                to = from,
                                type = "interactive",
                                postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
                                {
                                    type = "button",
                                    interactive = new InteractiveButtonsModel
                                    {
                                        type = "button",
                                        // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                                        body = new InteractiveBodyModel { text = msgBot.Text },
                                        footer = new InteractiveFooterModel { text = msgBot.Summary },
                                        action = new InteractiveButtonActionModel
                                        {
                                            buttons = buttons

                                        }
                                    }


                                }


                            };

                        }


                    }

                }
                else
                {


                    if (msgBot.InputHint == "منطقتك" || msgBot.SuggestedActions.Actions.Count() > 10)//msgBot.SuggestedActions.Actions.Count()>10
                    {
                        WhatsAppContent message2 = PrepareMessageContent(msgBot, botId);

                        postWhatsAppMessageModel = new PostWhatsAppMessageModel
                        {
                            to = from,
                            type = "text",
                            text = new PostWhatsAppMessageModel.Text
                            {
                                body = message2.text
                            }


                        };

                    }
                    else
                    {
                        List<InteractiveSectionListModel> sections = new List<InteractiveSectionListModel>();
                        List<InteractiveRowListModel> rows = new List<InteractiveRowListModel>();
                        foreach (var button in msgBot.SuggestedActions.Actions)
                        {

                            rows.Add(new InteractiveRowListModel
                            {
                                id = button.Title.Replace(" ", ""),
                                title = button.Title,
                                description = ""

                            });



                        }

                        sections.Add(new InteractiveSectionListModel
                        {
                            title = msgBot.InputHint,
                            rows = rows
                        });

                        postWhatsAppMessageModel = new PostWhatsAppMessageModel
                        {
                            to = from,
                            type = "interactive",
                            postWhatsAppInteractiveListModel = new PostInteractiveListMessageModel
                            {
                                type = "list",
                                interactive = new InteractiveListModel
                                {
                                    type = "list",
                                    // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                                    body = new InteractiveBodyModel { text = msgBot.Text },
                                    footer = new InteractiveFooterListModel { text = msgBot.Summary },
                                    action = new InteractiveActionListModel
                                    {

                                        button = msgBot.InputHint,
                                        sections = sections

                                    }

                                }

                            }

                        };



                    }

                }

            }






            return postWhatsAppMessageModel;
        }

        public async Task<bool> postToFB(PostWhatsAppMessageModel postWhatsAppMessageModel, string phonnumberId, string fbToken)
        {

            string postBody = String.Empty;
            WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
            switch (postWhatsAppMessageModel.type)
            {
                case nameof(WhatsContentTypeEnum.interactive):
                    postBody = whatsAppAppService.PrepareInteractiveMessage(postWhatsAppMessageModel);
                    break;

                case nameof(WhatsContentTypeEnum.text):
                    postBody = whatsAppAppService.PrepareTextMessage(postWhatsAppMessageModel);
                    break;

                case nameof(WhatsContentTypeEnum.video):
                    postBody = whatsAppAppService.PrepareVideoMessage(postWhatsAppMessageModel);
                    break;

                case nameof(WhatsContentTypeEnum.image):
                    postBody = whatsAppAppService.PrepareImageMessage(postWhatsAppMessageModel);
                    break;

                case nameof(WhatsContentTypeEnum.file):
                    postBody = whatsAppAppService.PrepareDocumentMessage(postWhatsAppMessageModel);

                    break;

                case nameof(WhatsContentTypeEnum.location):
                    postBody = whatsAppAppService.PrepareLocationMessage(postWhatsAppMessageModel);
                    break;

                case nameof(WhatsContentTypeEnum.document):
                    postBody = whatsAppAppService.PrepareDocumentMessage(postWhatsAppMessageModel);
                    break;

                case nameof(WhatsContentTypeEnum.audio):
                    postBody = whatsAppAppService.PrepareVoiceMessage(postWhatsAppMessageModel);
                    break;

                default:
                    break;
            }


            return await SendToWhatsApp(postBody, phonnumberId, fbToken);
        }
        private WhatsAppContent PrepareMessageContent(Activity msgBot, string botId)
        {
            string tMessageToSend = string.Empty;
            List<CardAction> tOutActions = new List<CardAction>();
            int tOrder = 1;
            var optionlst = new Dictionary<int, string>();
            if (msgBot.SuggestedActions != null && msgBot.SuggestedActions.Actions.Count > 0)
            {
                tOutActions.AddRange(msgBot.SuggestedActions.Actions);
            }

            foreach (var hc in tOutActions)
            {
                tMessageToSend += tOrder.ToString() + "- " + hc.Title + "\r\n";
                optionlst.Add(tOrder, hc.Title);
                tOrder++;
            }

            WhatsAppContent message = new WhatsAppContent
            {
                text = msgBot.Text + "\r\n" + tMessageToSend,
                type = "text",
                agentName = botId,
                agentId = "1000000"

            };

            return message;

        }



        public string MassageTypeText(List<WhatsAppMessageModel> messages, string fbToken, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, ref string medaiUrl)
        {
            var url = string.Empty;
            var type = string.Empty;
            medaiUrl = string.Empty;
            string msg = string.Empty;
            if (messages[0].Type == WhatsContentTypeEnum.video.ToString())
            {

                var RetrievingMedia = RetrievingMediaAsync(messages[0].video.id, fbToken).Result;

                byte[] videoBytes = RetrievingMedia.contentByte;

                var extention = "." + messages[0].video.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = videoBytes,
                //    Extension = extention,
                //    MimeType = messages[0].video.mime_type
                //};
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //messages.FirstOrDefault().video.link = url;

                //type = "video";
                //// messages.FirstOrDefault().Type = messages[0].video.mime_type;
                ////messages.FirstOrDefault().mediaUrl = url;

                //medaiUrl = url;
               // tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].video.mime_type, url, videoBytes));


            }

            else if (messages[0].Type == WhatsContentTypeEnum.image.ToString())
            {
                var RetrievingMedia = RetrievingMediaAsync(messages[0].image.id, fbToken).Result;

                byte[] imageBytes = RetrievingMedia.contentByte;

                var extention = "." + messages[0].image.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = imageBytes,
                //    Extension = extention,
                //    MimeType = messages[0].image.mime_type
                //};
               // url = azureBlobProvider.Save(attachmentContent).Result;
                // messages.FirstOrDefault().image.link = url;

                type = "image";
                //messages.FirstOrDefault().Type = messages[0].image.mime_type;
                //messages.FirstOrDefault().mediaUrl = url;
              //  medaiUrl = url;

             //   tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].image.mime_type, url, imageBytes, "", url));


            }
            else if (messages[0].Type == WhatsContentTypeEnum.document.ToString())
            {
                var RetrievingMedia = RetrievingMediaAsync(messages[0].document.id, fbToken).Result;
                byte[] imageBytes = RetrievingMedia.contentByte;
                var extention = "." + messages[0].document.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = imageBytes,
                //    Extension = extention,
                //    MimeType = messages[0].document.mime_type
                //};
                type = "file";
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //  messages.FirstOrDefault().document.link = url;
                //  messages.FirstOrDefault().document.mime_type = "file";

               // messages.FirstOrDefault().Type = "file";
                // messages.FirstOrDefault().mediaUrl = url;
                //medaiUrl = url;
              //  tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].document.mime_type, url, imageBytes, "", url));

            }

            else if (messages[0].Type == WhatsContentTypeEnum.audio.ToString())
            {
                var RetrievingMedia = RetrievingMediaAsync(messages[0].voice.id, fbToken).Result;
                byte[] imageBytes = RetrievingMedia.contentByte;
                var extention = "." + messages[0].voice.mime_type.Split("/")[1];
             //   AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
             //   AttachmentContent attachmentContent = new AttachmentContent()
              //  {
               //     Content = imageBytes,
               //     Extension = extention,
               //    MimeType = messages[0].voice.mime_type
               // };
                //type = "audio";
                //url = azureBlobProvider.Save(attachmentContent).Result;
                ///medaiUrl = url;

                // messages.FirstOrDefault().type = "audio";
              //  messages.FirstOrDefault().mediaUrl = url;

               // tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].voice.mime_type, url, imageBytes, "", url));

            }

            else if (messages[0].Type == WhatsContentTypeEnum.location.ToString())
            {
                //var query = messages[0].location.latitude.ToString() + "," + messages[0].location.longitude.ToString();
                //messages.FirstOrDefault().text = new WebHookD360Model.Text { body = query };
                //messages.FirstOrDefault().type = "location";
                //type = "location";
                //.messages.FirstOrDefault().textD360 = query;
                //msg = query;
            }
            else if (messages[0].Type == WhatsContentTypeEnum.interactive.ToString())
            {
                if (messages[0].interactive.type == "list_reply")
                {

                    msg = messages[0].interactive.list_reply.title;


                    type = "text";

                }
                else
                {

                    msg = messages[0].interactive.button_reply.title;



                    type = "text";
                }

            }

            else if (messages[0].Type == WhatsContentTypeEnum.text.ToString())
            {
                msg = messages[0].Text.Body;
            }




            return msg;
        }
        public string PrepareTextMessage(PostWhatsAppMessageModel postWhatsAppMessageModel)
        {
            string result = string.Empty;
            PostWhatsAppTextMessageModel postWhatsAppTextMessageModel = new PostWhatsAppTextMessageModel();
            PostWhatsAppMessageModel.Text text = new PostWhatsAppMessageModel.Text();
            postWhatsAppTextMessageModel.messaging_product = "whatsapp";
            postWhatsAppTextMessageModel.recipient_type = "individual";
            postWhatsAppTextMessageModel.to = postWhatsAppMessageModel.to;
            postWhatsAppTextMessageModel.type = "text";
            text.preview_url = false;
            text.body = postWhatsAppMessageModel.text.body;
            postWhatsAppTextMessageModel.text = text;
            result = JsonConvert.SerializeObject(postWhatsAppTextMessageModel);


            return result;
        }
        public string PrepareVideoMessage(PostWhatsAppMessageModel postWhatsAppMessageModel, bool isTeamInbox = false)
        {
            string result = string.Empty;

            if (isTeamInbox)
            {
                PostWhatsAppMediaModel postWhatsAppImageModel = new PostWhatsAppMediaModel();
                PostWhatsAppMessageModel.Video video = new PostWhatsAppMessageModel.Video();
                // image.id = postWhatsAppMessageModel.image.id;
                postWhatsAppImageModel.messaging_product = "whatsapp";
                postWhatsAppImageModel.recipient_type = "individual";
                postWhatsAppImageModel.to = postWhatsAppMessageModel.to;
                postWhatsAppImageModel.type = "video";
                video.link = postWhatsAppMessageModel.video.link;
                postWhatsAppImageModel.video = video;
                result = JsonConvert.SerializeObject(postWhatsAppImageModel);
            }
            else
            {
                PostWhatsAppMessageModel postVideoMessageModel = new PostWhatsAppMessageModel();
                PostWhatsAppMessageModel.Video video = new PostWhatsAppMessageModel.Video();
                postVideoMessageModel.messaging_product = "whatsapp";
                postVideoMessageModel.recipient_type = "individual";
                postVideoMessageModel.to = postWhatsAppMessageModel.to;
                postVideoMessageModel.type = "video";
                video.id = postWhatsAppMessageModel.video.id;
                video.caption = postWhatsAppMessageModel.video.caption;
                video.link = postWhatsAppMessageModel.video.link;
                postVideoMessageModel.video = video;
                result = JsonConvert.SerializeObject(postVideoMessageModel);

            }
            return result;
        }
        public string PrepareImageMessage(PostWhatsAppMessageModel postWhatsAppMessageModel, bool isTeamInbox = false)
        {
            string result = string.Empty;

            if (isTeamInbox)
            {
                PostWhatsAppMediaModel postWhatsAppImageModel = new PostWhatsAppMediaModel();
                PostWhatsAppMessageModel.Image image = new PostWhatsAppMessageModel.Image();
                // image.id = postWhatsAppMessageModel.image.id;
                postWhatsAppImageModel.messaging_product = "whatsapp";
                postWhatsAppImageModel.recipient_type = "individual";
                postWhatsAppImageModel.to = postWhatsAppMessageModel.to;
                postWhatsAppImageModel.type = "image";
                image.link = postWhatsAppMessageModel.image.link;
                postWhatsAppImageModel.image = image;
                result = JsonConvert.SerializeObject(postWhatsAppImageModel);
            }
            else
            {

                PostWhatsAppMessageModel objPostWhatsAppMessageModel = new PostWhatsAppMessageModel();
                PostWhatsAppMessageModel.Image image = new PostWhatsAppMessageModel.Image();
                objPostWhatsAppMessageModel.messaging_product = "whatsapp";
                objPostWhatsAppMessageModel.recipient_type = "individual";
                objPostWhatsAppMessageModel.to = postWhatsAppMessageModel.to;
                objPostWhatsAppMessageModel.type = "image";
                //image.id = postWhatsAppMessageModel.image.id;
                image.link = postWhatsAppMessageModel.image.link;
                objPostWhatsAppMessageModel.image = image;
                result = JsonConvert.SerializeObject(objPostWhatsAppMessageModel);
            }


            return result;
        }
        public string PrepareVoiceMessage(PostWhatsAppMessageModel postWhatsAppMessageModel, bool isTeamInbox = false)
        {
            string result = string.Empty;




            if (isTeamInbox)
            {
                PostWhatsAppMediaModel postWhatsAppImageModel = new PostWhatsAppMediaModel();
                PostWhatsAppMessageModel.Audio audio = new PostWhatsAppMessageModel.Audio();
                // image.id = postWhatsAppMessageModel.image.id;
                postWhatsAppImageModel.messaging_product = "whatsapp";
                postWhatsAppImageModel.recipient_type = "individual";
                postWhatsAppImageModel.to = postWhatsAppMessageModel.to;
                postWhatsAppImageModel.type = "voice";
                audio.link = postWhatsAppMessageModel.audio.link;
                postWhatsAppImageModel.audio = audio;
                result = JsonConvert.SerializeObject(postWhatsAppImageModel);
            }
            else
            {

                PostWhatsAppMessageModel postAudioMessageModel = new PostWhatsAppMessageModel();
                PostWhatsAppMessageModel.Audio audio = new PostWhatsAppMessageModel.Audio();
                postAudioMessageModel.messaging_product = "whatsapp";
                postAudioMessageModel.recipient_type = "individual";
                postAudioMessageModel.to = postWhatsAppMessageModel.to;
                postAudioMessageModel.type = "voice";
                audio.id = postWhatsAppMessageModel.audio.id;
                audio.link = postWhatsAppMessageModel.audio.link;
                postAudioMessageModel.audio = audio;
                result = JsonConvert.SerializeObject(postAudioMessageModel);
            }

            return result;
        }
        public string PrepareLocationMessage(PostWhatsAppMessageModel postWhatsAppMessageModel)
        {
            string result = string.Empty;
            PostWhatsAppMessageModel postLocationMessageModel = new PostWhatsAppMessageModel();
            PostWhatsAppMessageModel.Location location = new PostWhatsAppMessageModel.Location();
            postLocationMessageModel.messaging_product = "whatsapp";
            postLocationMessageModel.recipient_type = "individual";
            postLocationMessageModel.to = postWhatsAppMessageModel.to;
            postLocationMessageModel.type = "location";
            location.latitude = postWhatsAppMessageModel.location.latitude.ToString();
            location.longitude = postWhatsAppMessageModel.location.longitude.ToString();
            location.name = postWhatsAppMessageModel.location.name;
            location.address = postWhatsAppMessageModel.location.address;
            postLocationMessageModel.location = location;
            result = JsonConvert.SerializeObject(postLocationMessageModel);


            return result;
        }
        public string PrepareDocumentMessage(PostWhatsAppMessageModel postWhatsAppMessageModel, bool isTeamInbox = false)
        {
            string result = string.Empty;

            if (isTeamInbox)
            {
                PostWhatsAppMediaModel postWhatsAppMediaModel = new PostWhatsAppMediaModel();
                PostWhatsAppMessageModel.Document document = new PostWhatsAppMessageModel.Document();
                // image.id = postWhatsAppMessageModel.image.id;
                postWhatsAppMediaModel.messaging_product = "whatsapp";
                postWhatsAppMediaModel.recipient_type = "individual";
                postWhatsAppMediaModel.to = postWhatsAppMessageModel.to;
                postWhatsAppMediaModel.type = "document";
                document.link = postWhatsAppMessageModel.document.link;
                document.filename = postWhatsAppMessageModel.document.filename;
                postWhatsAppMediaModel.document = document;
                result = JsonConvert.SerializeObject(postWhatsAppMediaModel);

            }
            else
            {
                PostWhatsAppMediaModel postWhatsAppMediaModel = new PostWhatsAppMediaModel();
                PostWhatsAppMessageModel.Document document = new PostWhatsAppMessageModel.Document();
                // image.id = postWhatsAppMessageModel.image.id;
                postWhatsAppMediaModel.messaging_product = "whatsapp";
                postWhatsAppMediaModel.recipient_type = "individual";
                postWhatsAppMediaModel.to = postWhatsAppMessageModel.to;
                postWhatsAppMediaModel.type = "document";
                document.link = postWhatsAppMessageModel.document.link;
                document.filename = postWhatsAppMessageModel.document.filename;
                postWhatsAppMediaModel.document = document;
                result = JsonConvert.SerializeObject(postWhatsAppMediaModel);
            }

            return result;
        }
        public string PrepareInteractiveMessage(PostWhatsAppMessageModel postWhatsAppMessageModel)
        {

            if (postWhatsAppMessageModel.postWhatsAppInteractiveListModel != null)
            {
                return PrepareInteractiveListMessage(postWhatsAppMessageModel);
            }
            else
            {
                return PrepareInteractiveButtonMessage(postWhatsAppMessageModel);
            }

        }
        public string PrepareInteractiveButtonMessage(PostWhatsAppMessageModel postWhatsAppMessageModel)
        {
            string result = string.Empty;

            PostWhatsAppInteractiveButtonModel postWhatsAppInteractiveModel = new PostWhatsAppInteractiveButtonModel();
            postWhatsAppInteractiveModel.messaging_product = "whatsapp";
            postWhatsAppInteractiveModel.recipient_type = "individual";
            postWhatsAppInteractiveModel.to = postWhatsAppMessageModel.to;
            postWhatsAppInteractiveModel.type = "interactive";

            InteractiveButtonsModel interactiveButtonsModel = new InteractiveButtonsModel();

            interactiveButtonsModel.type = "button";

            // fill body
            interactiveButtonsModel.body = new InteractiveBodyModel();
            InteractiveBodyModel interactiveButtonsBodyModel = new InteractiveBodyModel();
            interactiveButtonsBodyModel.text = postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.body.text;
            interactiveButtonsModel.body = interactiveButtonsBodyModel;


            //// fill footer
            //if (postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.footer != null)
            //{
            //    interactiveButtonsModel.footer = new InteractiveFooterModel();
            //    InteractiveFooterModel interactiveFooterModel = new InteractiveFooterModel();
            //    interactiveFooterModel.text = postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.footer.text;
            //    interactiveButtonsModel.footer = interactiveFooterModel;
            //}


            // fill actions 
            interactiveButtonsModel.action = new InteractiveButtonActionModel();
            InteractiveButtonActionModel interactiveButtonActionModel = new InteractiveButtonActionModel();
            interactiveButtonActionModel.buttons = new List<InteractiveButtonModel>();

            List<InteractiveButtonModel> lstInteractiveButtonModel = new List<InteractiveButtonModel>();

            foreach (var item in postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons)
            {
                InteractiveButtonModel objInteractiveButtonModel = new InteractiveButtonModel();
                objInteractiveButtonModel.type = item.type;
                InteractiveButtonsReplyModel interactiveButtonsReplyModel = new InteractiveButtonsReplyModel();
                interactiveButtonsReplyModel.id = item.reply.id;
                interactiveButtonsReplyModel.title = item.reply.title;
                objInteractiveButtonModel.reply = new InteractiveButtonsReplyModel();
                objInteractiveButtonModel.reply = interactiveButtonsReplyModel;
                lstInteractiveButtonModel.Add(objInteractiveButtonModel);
            }


            interactiveButtonActionModel.buttons = lstInteractiveButtonModel;


            interactiveButtonsModel.action = interactiveButtonActionModel;



            postWhatsAppInteractiveModel.interactive = interactiveButtonsModel;
            result = JsonConvert.SerializeObject(postWhatsAppInteractiveModel);


            return result;
        }
        public string PrepareInteractiveListMessage(PostWhatsAppMessageModel postWhatsAppMessageModel)
        {
            string result = string.Empty;

            PostInteractiveListMessageModel postWhatsAppInteractiveModel = new PostInteractiveListMessageModel();
            postWhatsAppInteractiveModel.messaging_product = "whatsapp";
            postWhatsAppInteractiveModel.recipient_type = "individual";
            postWhatsAppInteractiveModel.to = postWhatsAppMessageModel.to;
            postWhatsAppInteractiveModel.type = "interactive";

            postWhatsAppInteractiveModel.interactive = new InteractiveListModel();
            InteractiveListModel interactiveListModel = new InteractiveListModel();

            interactiveListModel.type = "list";


            // fill body
            interactiveListModel.body = new InteractiveBodyModel();
            InteractiveBodyModel bodyListModel = new InteractiveBodyModel();
            bodyListModel.text = postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.body.text;
            interactiveListModel.body = bodyListModel;


            // fill footer
            if (postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.footer != null)
            {
                interactiveListModel.footer = new InteractiveFooterListModel();
                InteractiveFooterListModel interactiveFooterListModel = new InteractiveFooterListModel();
                interactiveFooterListModel.text = postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.footer.text;
                interactiveListModel.footer = interactiveFooterListModel;
            }

            // fill actions 

            InteractiveActionListModel actionListModel = new InteractiveActionListModel();
            actionListModel.button = postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.action.button;

            List<InteractiveSectionListModel> lstInteractiveSectionModel = new List<InteractiveSectionListModel>();

            foreach (var item in postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.action.sections)
            {
                InteractiveSectionListModel objInteractiveSectionListModel = new InteractiveSectionListModel();

                objInteractiveSectionListModel.title = item.title;

                List<InteractiveRowListModel> rows = new List<InteractiveRowListModel>();
                foreach (var objrow in item.rows)
                {
                    InteractiveRowListModel interactiveRowListModel = new InteractiveRowListModel();
                    interactiveRowListModel.id = objrow.id;
                    interactiveRowListModel.title = objrow.title;
                    interactiveRowListModel.description = objrow.description;
                    rows.Add(interactiveRowListModel);
                }

                objInteractiveSectionListModel.rows = new List<InteractiveRowListModel>();
                objInteractiveSectionListModel.rows = rows;


                lstInteractiveSectionModel.Add(objInteractiveSectionListModel);
            }

            actionListModel.sections = new List<InteractiveSectionListModel>();
            actionListModel.sections = lstInteractiveSectionModel;

            interactiveListModel.action = new InteractiveActionListModel();
            interactiveListModel.action = actionListModel;



            postWhatsAppInteractiveModel.interactive = interactiveListModel;
            result = JsonConvert.SerializeObject(postWhatsAppInteractiveModel);


            return result;
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


            return attachmentModel2;

        }

        private static async Task<WhatsAppAttachmentModel> DownloadMediaAsync(string mediaurl, string fbToken)
        {

            WhatsAppAttachmentModel attachmentModel = new WhatsAppAttachmentModel();



            //var client = new RestClient(mediaurl);
            //client.Timeout = -1;
            //var request = new RestRequest(Method.GET);
            //request.AddHeader("Authorization", "Bearer " + fbToken);
            //IRestResponse response = client.Execute(request);
            //var ssss = response.Content;
            //attachmentModel.contentByte = response.RawBytes;
            //using (var contentdd = response.Content)
            //{
            //    // WhatsAppMediaResponse whatsAppMediaResponse = new WhatsAppMediaResponse();

            //    //var result = JsonConvert.DeserializeObject<WhatsAppAttachmentModel>(await content.ReadAsStringAsync());
            //    attachmentModel.contentByte = response.Content.ReadAsByteArrayAsync().Result;
            //    attachmentModel.contentType = response.Content.Headers.ContentType.MediaType;

            //}
            //var = response.Content.
            //Console.WriteLine(response.Content);




            //using (var httpClient = new HttpClient())
            //{


            //    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbToken);

            //    using (var response = await httpClient.GetAsync(mediaurl))
            //    {
            //        using (var content = response.Content)
            //        {
            //            // WhatsAppMediaResponse whatsAppMediaResponse = new WhatsAppMediaResponse();

            //            //var result = JsonConvert.DeserializeObject<WhatsAppAttachmentModel>(await content.ReadAsStringAsync());
            //            attachmentModel.contentByte = response.Content.ReadAsByteArrayAsync().Result;
            //            attachmentModel.contentType = response.Content.Headers.ContentType.MediaType;

            //        }
            //    }
            //}


            return attachmentModel;

        }


    }
}
