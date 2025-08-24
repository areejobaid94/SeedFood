using Abp.Extensions;
using DocumentFormat.OpenXml.Math;
using Infoseed.MessagingPortal.BotFlow.Dtos;
using Infoseed.MessagingPortal.FacebookDTO.DTO;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.BotFlow.Dtos.GetBotFlowForViewDto;
using static Infoseed.MessagingPortal.WhatsApp.Dto.InstagramModel;
using static Infoseed.MessagingPortal.WhatsApp.Dto.PostWhatsAppMessageModel;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppEnum;

namespace Infoseed.MessagingPortal.WhatsApp
{
    public class WhatsAppAppService
    {
        public static string D360DialogUrl = "https://waba-sandbox.360dialog.io/v1/messages";
        public async Task<WhatsAppAnalyticsModel> GetWhatsAppAnalyticAsync()
        {
            WhatsAppAnalyticsModel objWhatsAppAnalytic = new WhatsAppAnalyticsModel();

            using (var httpClient = new HttpClient())
            {
                string[] obj = { "CONVERSATION_DIRECTION", "CONVERSATION_TYPE", "COUNTRY", "PHONE" };

                //int wapa,int start, int end,string granularity,string FbToken
                //var postUrl = "https://graph.facebook.com/v14.0/" + wapa + "?fields=conversation_analytics.start(" + start + ").end(" + end + ").granularity(" + granularity + ").dimensions(["+ "CONVERSATION_DIRECTION" + ", " + "CONVERSATION_TYPE" + ", " + "COUNTRY" + ", " + "PHONE" + "])&access_token = "+FbToken+"";
                var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + 102833862462013 + "?fields=conversation_analytics.start(" + 1654074000 + ").end(" + 1655802000 + ").granularity(DAILY).dimensions(" + obj[0] +"," +obj[1] +","+ obj[2] +","+ obj[3] +")&access_token = EAAPZAQZBRzVzYBACa0sXe5WLgOOmSnVDoUV3qKYMB8aVZCV5h9P4mY9gBnS65J5kADUaVpJ3xd80wFokjnl9GwQ0S5yNpN8lUqR6x9xncgmRY5ZCbgwrqXiFZClLrUEydvZAbHMfq2WWCkrmWrcH5z5vO6tHuvzv4fvOwgmdefvZC889hAXLuRbN6xVAHZBZAg8wCbrntry64zrgDgLfioZCHF";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "EAAPZAQZBRzVzYBACa0sXe5WLgOOmSnVDoUV3qKYMB8aVZCV5h9P4mY9gBnS65J5kADUaVpJ3xd80wFokjnl9GwQ0S5yNpN8lUqR6x9xncgmRY5ZCbgwrqXiFZClLrUEydvZAbHMfq2WWCkrmWrcH5z5vO6tHuvzv4fvOwgmdefvZC889hAXLuRbN6xVAHZBZAg8wCbrntry64zrgDgLfioZCHF");

                using (var response = await httpClient.GetAsync(postUrl))
                {
                    using (var content = response.Content)
                    {
                        var WhatsAppAnalytic = await content.ReadAsStringAsync();
                        objWhatsAppAnalytic = JsonConvert.DeserializeObject<WhatsAppAnalyticsModel>(WhatsAppAnalytic);

                    }
                }

                return objWhatsAppAnalytic;

            }
        }

        public async Task<bool> SendToWhatsApp(string postBody, string phonnumberId, string fbToken, bool IsD360Dialog)
        {

            using (var httpClient = new HttpClient())
            {

                if (IsD360Dialog)
                {
                    var postUrl = D360DialogUrl;
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("D360-API-KEY", phonnumberId);
                    // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbToken);

                    using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                    {
                        using (var content = response.Content)
                        {
                            var result = await content.ReadAsStringAsync();
                            return true;

                        }
                    }
                }
                else
                {
                    var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + phonnumberId + "/messages";
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

               
            }
            //return false;
        }
        public async Task<string> SendToFacebookMessenger(string postBody, string fbPageAccessToken)
        {
            using (var httpClient = new HttpClient())
            {
                var postUrl = $"https://graph.facebook.com/v18.0/me/messages?access_token={fbPageAccessToken}";
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                {
                    using (var content = response.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        return result;
                    }
                }
            }
        }

        public async Task<string> SendToWhatsAppNew(string postBody, string phonnumberId, string fbToken, string channel)
        {

            using (var httpClient = new HttpClient())
            {
                string postUrl;

                if (channel.ToLower() == "facebook")
                {
                    postUrl = "https://graph.facebook.com/v22.0/me/messages";

                }
                else if (channel == "instagram")
                {
                    postUrl = Constants.WhatsAppTemplates.InstagramUrl + phonnumberId + "/messages";
                }
                else
                {
                    postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + phonnumberId + "/messages";
                }           
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbToken);

                using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                {
                    using (var content = response.Content)
                    {
                        var result = await content.ReadAsStringAsync();
                        return result;

                    }
                }
            }

            return null;
            //return false;
        }

        public async Task<List<PostFacebookMessageModel>> BotChatWithCustomerFacebook(Activity msgBot, string recipientId, string botId)
        {
            List<PostFacebookMessageModel> lstPostFacebookMessageModel = new List<PostFacebookMessageModel>();
            PostFacebookMessageModel postFacebookMessageModel = new PostFacebookMessageModel();

            if (msgBot.SuggestedActions == null)
            {
                if (msgBot.InputHint == "file")
                {
                    postFacebookMessageModel = new PostFacebookMessageModel
                    {
                        RecipientId = recipientId,
                        MessagingType = "RESPONSE",
                        File = new PostFacebookMessageModel.FileAttachment
                        {
                            Url = msgBot.Text.Replace("\r\n", ""),
                            Reusable = true
                        }
                    };
                }
                else if (msgBot.InputHint == "image")
                {
                    postFacebookMessageModel = new PostFacebookMessageModel
                    {
                        RecipientId = recipientId,
                        MessagingType = "RESPONSE",
                        Image = new PostFacebookMessageModel.ImageAttachment
                        {
                            Url = msgBot.Text.Replace("\r\n", ""),
                            Reusable = true
                        }
                    };
                }
                else if (msgBot.InputHint == "video")
                {
                    postFacebookMessageModel = new PostFacebookMessageModel
                    {
                        RecipientId = recipientId,
                        MessagingType = "RESPONSE",
                        Video = new PostFacebookMessageModel.VideoAttachment
                        {
                            Url = msgBot.Text.Replace("\r\n", ""),
                            Reusable = true
                        }
                    };
                }
                else
                {
                    postFacebookMessageModel = new PostFacebookMessageModel
                    {
                        RecipientId = recipientId,
                        MessagingType = "RESPONSE",
                        Text = new PostFacebookMessageModel.TextMessage
                        {
                            Text = msgBot.Text
                        }
                    };
                }
            }
            else
            {
                if (msgBot.Attachments == null)
                {
                    List<PostFacebookMessageModel.QuickReplyItem> quickReplies = new List<PostFacebookMessageModel.QuickReplyItem>();
                    foreach (var button in msgBot.SuggestedActions.Actions)
                    {
                        quickReplies.Add(new PostFacebookMessageModel.QuickReplyItem
                        {
                            ContentType = "text",
                            Title = button.Title,
                            Payload = button.Title,
                            ImageUrl = button.Image
                        });
                    }

                    if (msgBot.Summary == null)
                    {
                        msgBot.Summary = "";

                        if (msgBot.Text.Length <= 1000)
                        {
                            postFacebookMessageModel = new PostFacebookMessageModel
                            {
                                RecipientId = recipientId,
                                MessagingType = "RESPONSE",
                                Text = new PostFacebookMessageModel.TextMessage
                                {
                                    Text = msgBot.Text
                                },
                                QuickReplyData = new PostFacebookMessageModel.QuickReply
                                {
                                    QuickReplies = quickReplies
                                }
                            };
                        }
                        else
                        {
                            postFacebookMessageModel = new PostFacebookMessageModel
                            {
                                RecipientId = recipientId,
                                MessagingType = "RESPONSE",
                                Text = new PostFacebookMessageModel.TextMessage
                                {
                                    Text = msgBot.Text
                                }
                            };
                            lstPostFacebookMessageModel.Add(postFacebookMessageModel);

                            postFacebookMessageModel = new PostFacebookMessageModel
                            {
                                RecipientId = recipientId,
                                MessagingType = "RESPONSE",
                                Text = new PostFacebookMessageModel.TextMessage
                                {
                                    Text = msgBot.Summary
                                },
                                QuickReplyData = new PostFacebookMessageModel.QuickReply
                                {
                                    QuickReplies = quickReplies
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
                                postFacebookMessageModel = new PostFacebookMessageModel
                                {
                                    RecipientId = recipientId,
                                    MessagingType = "RESPONSE",
                                    Text = new PostFacebookMessageModel.TextMessage
                                    {
                                        Text = msgBot.Text
                                    },
                                    QuickReplyData = new PostFacebookMessageModel.QuickReply
                                    {
                                        QuickReplies = quickReplies
                                    }
                                };
                            }
                            else
                            {
                                postFacebookMessageModel = new PostFacebookMessageModel
                                {
                                    RecipientId = recipientId,
                                    MessagingType = "RESPONSE",
                                    Text = new PostFacebookMessageModel.TextMessage
                                    {
                                        Text = msgBot.Text
                                    }
                                };
                                lstPostFacebookMessageModel.Add(postFacebookMessageModel);

                                postFacebookMessageModel = new PostFacebookMessageModel
                                {
                                    RecipientId = recipientId,
                                    MessagingType = "RESPONSE",
                                    Text = new PostFacebookMessageModel.TextMessage
                                    {
                                        Text = msgBot.Summary
                                    },
                                    QuickReplyData = new PostFacebookMessageModel.QuickReply
                                    {
                                        QuickReplies = quickReplies
                                    }
                                };
                            }
                        }
                        else
                        {
                            postFacebookMessageModel = new PostFacebookMessageModel
                            {
                                RecipientId = recipientId,
                                MessagingType = "RESPONSE",
                                Text = new PostFacebookMessageModel.TextMessage
                                {
                                    Text = msgBot.Text
                                },
                                QuickReplyData = new PostFacebookMessageModel.QuickReply
                                {
                                    QuickReplies = quickReplies
                                }
                            };
                        }
                    }
                }
                else
                {
                    if (msgBot.InputHint == "منطقتك" || msgBot.SuggestedActions.Actions.Count() > 10)
                    {
                        FacebookContent message2 = PrepareMessageContentFacebook(msgBot, botId, "", 0, "");

                        try
                        {
                            if (!string.IsNullOrEmpty(msgBot.Speak))
                            {
                                postFacebookMessageModel = new PostFacebookMessageModel
                                {
                                    RecipientId = recipientId,
                                    MessagingType = "RESPONSE",
                                    Image = new PostFacebookMessageModel.ImageAttachment
                                    {
                                        Url = msgBot.Speak,
                                        Reusable = true
                                    }
                                };
                            }
                            else
                            {
                                postFacebookMessageModel = new PostFacebookMessageModel
                                {
                                    RecipientId = recipientId,
                                    MessagingType = "RESPONSE",
                                    Text = new PostFacebookMessageModel.TextMessage
                                    {
                                        Text = message2.Text
                                    }
                                };
                            }
                        }
                        catch
                        {
                            postFacebookMessageModel = new PostFacebookMessageModel
                            {
                                RecipientId = recipientId,
                                MessagingType = "RESPONSE",
                                Text = new PostFacebookMessageModel.TextMessage
                                {
                                    Text = message2.Text
                                }
                            };
                        }
                    }
                    else
                    {
                        List<PostFacebookMessageModel.QuickReplyItem> quickReplies = new List<PostFacebookMessageModel.QuickReplyItem>();
                        foreach (var button in msgBot.SuggestedActions.Actions)
                        {
                            quickReplies.Add(new PostFacebookMessageModel.QuickReplyItem
                            {
                                ContentType = "text",
                                Title = button.Title,
                                Payload = button.Title,
                                ImageUrl = button.Image
                            });
                        }

                        postFacebookMessageModel = new PostFacebookMessageModel
                        {
                            RecipientId = recipientId,
                            MessagingType = "RESPONSE",
                            Text = new PostFacebookMessageModel.TextMessage
                            {
                                Text = msgBot.Text
                            },
                            QuickReplyData = new PostFacebookMessageModel.QuickReply
                            {
                                QuickReplies = quickReplies
                            }
                        };
                    }
                }
            }

            lstPostFacebookMessageModel.Add(postFacebookMessageModel);
            return lstPostFacebookMessageModel;
        }
        public async Task<List<PostWhatsAppMessageModel>> BotChatWithCustomer(Activity msgBot, string from, string botId)
        {

            List<PostWhatsAppMessageModel> lstPostWhatsAppMessageModel = new List<PostWhatsAppMessageModel>();
            PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();


            //bool isBotSendAttachments = false;
            //bool Islist = true;

            if (msgBot.SuggestedActions == null)
            {


                if (msgBot.InputHint == "file")
                {

                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {

                        mediaUrl = msgBot.Text.Replace("\r\n", ""),
                        to = from,
                        type = "file",
                        fileName = msgBot.Speak,
                        document = new PostWhatsAppMessageModel.Document
                        {
                            link = msgBot.Text.Replace("\r\n", ""),
                            filename = msgBot.Speak,
                        }

                    };


                    //isBotSendAttachments = true;
                }
                else if (msgBot.InputHint == "image")
                {

                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {

                        mediaUrl = msgBot.Text.Replace("\r\n", ""),
                        to = from,
                        type = "image",
                        fileName = msgBot.Speak,
                        image = new PostWhatsAppMessageModel.Image
                        {
                            link = msgBot.Text.Replace("\r\n", ""),
                            id = msgBot.Speak,
                        }

                    };


                    //isBotSendAttachments = true;
                }
                else if (msgBot.InputHint == "video")
                {

                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {

                        mediaUrl = msgBot.Text.Replace("\r\n", ""),
                        to = from,
                        type = "video",
                        fileName = msgBot.Speak,
                        video = new PostWhatsAppMessageModel.Video
                        {
                            link = msgBot.Text.Replace("\r\n", ""),
                            id = msgBot.Speak,
                        }

                    };


                    //isBotSendAttachments = true;
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
                            reply = new InteractiveButtonsReplyModel { id = button.Title, title = button.Title, image = button.Image },
                            type = "reply"
                        });

                    }




                    if (msgBot.Summary == null)
                    {

                        msgBot.Summary = "";

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

                                        footer = new InteractiveFooterModel { text = msgBot.Summary },
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
                            lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);

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
                                        footer = new InteractiveFooterModel { text = msgBot.Summary },
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

                                lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);




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
                                            footer = new InteractiveFooterModel { text = msgBot.Summary },
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
                        WhatsAppContent message2 = PrepareMessageContent(msgBot, botId, "", 0, "");


                        try
                        {
                            if (!string.IsNullOrEmpty(msgBot.Speak))
                            {
                                postWhatsAppMessageModel = new PostWhatsAppMessageModel
                                {
                                    to = from,
                                    type = "image",
                                    image = new PostWhatsAppMessageModel.Image
                                    {
                                        link = msgBot.Speak,
                                        caption = message2.text

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
                                        body = message2.text
                                    }


                                };


                            }
                        }
                        catch
                        {
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
                                description = button.Image

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




            lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);

            return lstPostWhatsAppMessageModel;
        }

        //public async Task<List<PostWhatsAppMessageModel>> BotChatWithCustomerFaceboo(Activity msgBot, string from, string botId)
        //{

        //    List<PostWhatsAppMessageModel> lstPostWhatsAppMessageModel = new List<PostWhatsAppMessageModel>();
        //    PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();


        //    //bool isBotSendAttachments = false;
        //    //bool Islist = true;

        //    if (msgBot.SuggestedActions == null)
        //    {


        //        if (msgBot.InputHint == "file")
        //        {

        //            postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //            {

        //                mediaUrl = msgBot.Text.Replace("\r\n", ""),
        //                to = from,
        //                type = "file",
        //                fileName = msgBot.Speak,
        //                document = new PostWhatsAppMessageModel.Document
        //                {
        //                    link = msgBot.Text.Replace("\r\n", ""),
        //                    filename = msgBot.Speak,
        //                }

        //            };


        //            //isBotSendAttachments = true;
        //        }
        //        else if (msgBot.InputHint == "image")
        //        {

        //            postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //            {

        //                mediaUrl = msgBot.Text.Replace("\r\n", ""),
        //                to = from,
        //                type = "image",
        //                fileName = msgBot.Speak,
        //                image = new PostWhatsAppMessageModel.Image
        //                {
        //                    link = msgBot.Text.Replace("\r\n", ""),
        //                    id = msgBot.Speak,
        //                }

        //            };


        //            //isBotSendAttachments = true;
        //        }
        //        else if (msgBot.InputHint == "video")
        //        {

        //            postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //            {

        //                mediaUrl = msgBot.Text.Replace("\r\n", ""),
        //                to = from,
        //                type = "video",
        //                fileName = msgBot.Speak,
        //                video = new PostWhatsAppMessageModel.Video
        //                {
        //                    link = msgBot.Text.Replace("\r\n", ""),
        //                    id = msgBot.Speak,
        //                }

        //            };


        //            //isBotSendAttachments = true;
        //        }
        //        else
        //        {
        //            postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //            {
        //                to = from,
        //                type = "text",
        //                text = new PostWhatsAppMessageModel.Text
        //                {
        //                    body = msgBot.Text

        //                }

        //            };
        //        }

        //    }
        //    else
        //    {
        //        if (msgBot.Attachments == null)
        //        {



        //            List<InteractiveButtonModel> buttons = new List<InteractiveButtonModel>();
        //            foreach (var button in msgBot.SuggestedActions.Actions)
        //            {
        //                buttons.Add(new InteractiveButtonModel
        //                {
        //                    reply = new InteractiveButtonsReplyModel { id = button.Title, title = button.Title , image= button.Image},
        //                    type = "reply"
        //                });

        //            }




        //            if (msgBot.Summary == null)
        //            {

        //                msgBot.Summary="";

        //                if (msgBot.Text.Length <= 1000)
        //                {
        //                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                    {
        //                        to = from,
        //                        type = "interactive",
        //                        postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel()
        //                        {
        //                            type = "button",
        //                            // header=new postWhatsAppMessageModel.Header {type="text", text="" },
        //                            interactive = new InteractiveButtonsModel()
        //                            {
        //                                type = "button",



        //                                body = new InteractiveBodyModel { text = msgBot.Text },

        //                                footer = new InteractiveFooterModel { text = msgBot.Summary },
        //                                // footer = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
        //                                action = new InteractiveButtonActionModel
        //                                {
        //                                    buttons = buttons

        //                                }
        //                            }



        //                        }

        //                    };
        //                }
        //                else
        //                {
        //                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                    {
        //                        to = from,
        //                        type = "text",
        //                        text = new PostWhatsAppMessageModel.Text
        //                        {
        //                            body = msgBot.Text

        //                        }

        //                    };
        //                    lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);

        //                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                    {
        //                        to = from,
        //                        type = "interactive",
        //                        postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
        //                        {
        //                            type = "button",
        //                            interactive = new InteractiveButtonsModel
        //                            {
        //                                type = "button",
        //                                // header=new postWhatsAppMessageModel.Header {type="text", text="" },
        //                                body = new InteractiveBodyModel { text = msgBot.Summary },
        //                                footer = new InteractiveFooterModel { text = msgBot.Summary },
        //                                // footer = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
        //                                action = new InteractiveButtonActionModel
        //                                {
        //                                    buttons = buttons

        //                                }
        //                            }


        //                        }

        //                    };



        //                }




        //            }
        //            else
        //            {

        //                if (msgBot.Summary.Contains("هل تريد") || msgBot.Summary.Contains("Do you want"))
        //                {

        //                    if (msgBot.Text.Length <= 1000)
        //                    {
        //                        postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                        {
        //                            to = from,
        //                            type = "interactive",
        //                            postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
        //                            {
        //                                type = "button",
        //                                interactive = new InteractiveButtonsModel
        //                                {
        //                                    type = "button",
        //                                    // header=new postWhatsAppMessageModel.Header {type="text", text="" },
        //                                    body = new InteractiveBodyModel { text = msgBot.Text },
        //                                    footer = new InteractiveFooterModel { text = msgBot.Summary },
        //                                    action = new InteractiveButtonActionModel
        //                                    {
        //                                        buttons = buttons

        //                                    }
        //                                }


        //                            }


        //                        };


        //                        //postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                        //{
        //                        //    to = from,
        //                        //    type = "interactive",
        //                        //    postWhatsAppInteractiveModel = new PostWhatsAppMessageModel.PostWhatsAppInteractiveModel
        //                        //    {

        //                        //        interactiveButtonsModel = new InteractiveButtonsModel
        //                        //        {
        //                        //            type = "button",
        //                        //            // header=new postWhatsAppMessageModel.Header {type="text", text="" },
        //                        //            body = new InteractiveButtonsBodyModel { text = msgBot.Summary },
        //                        //         //   fo = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
        //                        //            action = new InteractiveButtonActionModel
        //                        //            {
        //                        //                buttons = buttons

        //                        //            }
        //                        //        }


        //                        //    }

        //                        //};






        //                    }
        //                    else
        //                    {
        //                        postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                        {
        //                            to = from,
        //                            type = "text",
        //                            text = new PostWhatsAppMessageModel.Text
        //                            {
        //                                body = msgBot.Text

        //                            }

        //                        };

        //                        lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);




        //                        //var result22 = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, postWhatsAppMessageModel, telemetry).Result;

        //                        ////update Bot massage in cosmoDB 

        //                        //if (result22 == HttpStatusCode.Created)
        //                        //{

        //                        //    Content message = contentM(msgBot, Tenant.botId);
        //                        //    var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
        //                        //    Customer.CreateDate = CustomerChat.CreateDate;
        //                        //    Customer.customerChat = CustomerChat;
        //                        //    var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
        //                        //    SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
        //                        //}
        //                        //else
        //                        //{
        //                        //    this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog , the user name is  : {Customer.displayName}", SeverityLevel.Critical);
        //                        //    MessagesSent[Customer.userId] = false;
        //                        //    return Ok();

        //                        //}


        //                        postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                        {
        //                            to = from,
        //                            type = "interactive",
        //                            postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
        //                            {
        //                                type = "button",
        //                                interactive = new InteractiveButtonsModel
        //                                {
        //                                    type = "button",
        //                                    // header=new postWhatsAppMessageModel.Header {type="text", text="" },
        //                                    body = new InteractiveBodyModel { text = msgBot.Summary },
        //                                    footer = new InteractiveFooterModel { text = msgBot.Summary },
        //                                    //   fo = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
        //                                    action = new InteractiveButtonActionModel
        //                                    {
        //                                        buttons = buttons

        //                                    }
        //                                }


        //                            }

        //                        };





        //                    }



        //                }
        //                else
        //                {

        //                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                    {
        //                        to = from,
        //                        type = "interactive",
        //                        postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
        //                        {
        //                            type = "button",
        //                            interactive = new InteractiveButtonsModel
        //                            {
        //                                type = "button",
        //                                // header=new postWhatsAppMessageModel.Header {type="text", text="" },
        //                                body = new InteractiveBodyModel { text = msgBot.Text },
        //                                footer = new InteractiveFooterModel { text = msgBot.Summary },
        //                                action = new InteractiveButtonActionModel
        //                                {
        //                                    buttons = buttons

        //                                }
        //                            }


        //                        }


        //                    };

        //                }


        //            }

        //        }
        //        else
        //        {


        //            if (msgBot.InputHint == "منطقتك" || msgBot.SuggestedActions.Actions.Count() > 10)//msgBot.SuggestedActions.Actions.Count()>10
        //            {
        //                WhatsAppContent message2 = PrepareMessageContent(msgBot, botId, "", 0, "");


        //                try
        //                {
        //                    if (!string.IsNullOrEmpty(msgBot.Speak))
        //                    {
        //                        postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                        {
        //                            to = from,
        //                            type = "image",
        //                            image = new PostWhatsAppMessageModel.Image
        //                            {
        //                                link=msgBot.Speak,
        //                                 caption=message2.text

        //                            }


        //                        };

        //                    }
        //                    else
        //                    {
        //                        postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                        {
        //                            to = from,
        //                            type = "text",
        //                            text = new PostWhatsAppMessageModel.Text
        //                            {
        //                                body = message2.text
        //                            }


        //                        };


        //                    }
        //                }
        //                catch
        //                {
        //                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                    {
        //                        to = from,
        //                        type = "text",
        //                        text = new PostWhatsAppMessageModel.Text
        //                        {
        //                            body = message2.text
        //                        }


        //                    };
        //                }

                       
                       

        //            }
        //            else
        //            {
        //                List<InteractiveSectionListModel> sections = new List<InteractiveSectionListModel>();
        //                List<InteractiveRowListModel> rows = new List<InteractiveRowListModel>();
        //                foreach (var button in msgBot.SuggestedActions.Actions)
        //                {

        //                    rows.Add(new InteractiveRowListModel
        //                    {
        //                        id = button.Title.Replace(" ", ""),
        //                        title = button.Title,
        //                        description = button.Image

        //                    });



        //                }

        //                sections.Add(new InteractiveSectionListModel
        //                {
        //                    title = msgBot.InputHint,
        //                    rows = rows
        //                });

        //                postWhatsAppMessageModel = new PostWhatsAppMessageModel
        //                {
        //                    to = from,
        //                    type = "interactive",
        //                    postWhatsAppInteractiveListModel = new PostInteractiveListMessageModel
        //                    {
        //                        type = "list",
        //                        interactive = new InteractiveListModel
        //                        {
        //                            type = "list",
        //                            // header=new postWhatsAppMessageModel.Header {type="text", text="" },
        //                            body = new InteractiveBodyModel { text = msgBot.Text },
        //                            footer = new InteractiveFooterListModel { text = msgBot.Summary },
        //                            action = new InteractiveActionListModel
        //                            {

        //                                button = msgBot.InputHint,
        //                                sections = sections

        //                            }

        //                        }

        //                    }

        //                };



        //            }

        //        }

        //    }




        //    lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);

        //    return lstPostWhatsAppMessageModel;
        //}

        public async Task<List<PostWhatsAppMessageModel>> BotInfoChatWithCustomer(Activity msgBot, string from, string botId)
        {

            List<PostWhatsAppMessageModel> lstPostWhatsAppMessageModel = new List<PostWhatsAppMessageModel>();
            PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();


            //bool isBotSendAttachments = false;
            //bool Islist = true;

            if (msgBot.SuggestedActions == null)
            {


                if (msgBot.InputHint == "file")
                {

                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {

                        mediaUrl = msgBot.Text.Replace("\r\n", ""),
                        to = from,
                        type = "file",
                        fileName = msgBot.Speak,
                        document = new PostWhatsAppMessageModel.Document
                        {
                            link = msgBot.Text.Replace("\r\n", ""),
                            filename = msgBot.Speak,
                        }

                    };


                    //isBotSendAttachments = true;
                }
                else if (msgBot.InputHint == "image")
                {

                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {

                        mediaUrl = msgBot.Text.Replace("\r\n", ""),
                        to = from,
                        type = "image",
                        fileName = msgBot.Speak,
                        image = new PostWhatsAppMessageModel.Image
                        {
                            link = msgBot.Text.Replace("\r\n", ""),
                            id = msgBot.Speak,
                        }

                    };


                    //isBotSendAttachments = true;
                }
                else if (msgBot.InputHint == "video")
                {

                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {

                        mediaUrl = msgBot.Text.Replace("\r\n", ""),
                        to = from,
                        type = "video",
                        fileName = msgBot.Speak,
                        video = new PostWhatsAppMessageModel.Video
                        {
                            link = msgBot.Text.Replace("\r\n", ""),
                            id = msgBot.Speak,
                        }

                    };


                    //isBotSendAttachments = true;
                }
                else
                {
                    postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    {
                        to = from,
                        type = "text",
                        text = new PostWhatsAppMessageModel.Text
                        {
                            body = msgBot.Text,
                        },


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
                            reply = new InteractiveButtonsReplyModel { id = button.Value.ToString(), title = button.Title, image= button.Image },
                            type = "reply"
                        });

                    }




                    //if (msgBot.Summary == null)
                    //{

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


                                    footer = new InteractiveFooterModel { text = msgBot.Summary },
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
                        lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);

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
                                    body = new InteractiveBodyModel { text = msgBot.Speak },
                                    footer = new InteractiveFooterModel { text = msgBot.Summary },
                                    action = new InteractiveButtonActionModel
                                    {
                                        buttons = buttons

                                    }
                                }


                            }

                        };



                    }




                    //}
                    //else
                    //{

                    //    if (msgBot.Summary.Contains("هل تريد") || msgBot.Summary.Contains("Do you want"))
                    //    {

                    //        if (msgBot.Text.Length <= 1000)
                    //        {
                    //            postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    //            {
                    //                to = from,
                    //                type = "interactive",
                    //                postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
                    //                {
                    //                    type = "button",
                    //                    interactive = new InteractiveButtonsModel
                    //                    {
                    //                        type = "button",
                    //                        // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                    //                        body = new InteractiveBodyModel { text = msgBot.Text },
                    //                        footer = new InteractiveFooterModel { text = msgBot.Summary },
                    //                        action = new InteractiveButtonActionModel
                    //                        {
                    //                            buttons = buttons

                    //                        }
                    //                    }


                    //                }


                    //            };


                    //            //postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    //            //{
                    //            //    to = from,
                    //            //    type = "interactive",
                    //            //    postWhatsAppInteractiveModel = new PostWhatsAppMessageModel.PostWhatsAppInteractiveModel
                    //            //    {

                    //            //        interactiveButtonsModel = new InteractiveButtonsModel
                    //            //        {
                    //            //            type = "button",
                    //            //            // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                    //            //            body = new InteractiveButtonsBodyModel { text = msgBot.Summary },
                    //            //         //   fo = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
                    //            //            action = new InteractiveButtonActionModel
                    //            //            {
                    //            //                buttons = buttons

                    //            //            }
                    //            //        }


                    //            //    }

                    //            //};






                    //        }
                    //        else
                    //        {
                    //            postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    //            {
                    //                to = from,
                    //                type = "text",
                    //                text = new PostWhatsAppMessageModel.Text
                    //                {
                    //                    body = msgBot.Text

                    //                }

                    //            };

                    //            lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);




                    //            //var result22 = WhatsAppDialogConnector.PostMsgToSmooch(D360Key, postWhatsAppMessageModel, telemetry).Result;

                    //            ////update Bot massage in cosmoDB 

                    //            //if (result22 == HttpStatusCode.Created)
                    //            //{

                    //            //    Content message = contentM(msgBot, Tenant.botId);
                    //            //    var CustomerChat = _dbService.UpdateCustomerChatD360(Customer.TenantId, message, Customer.userId, Customer.ConversationId);
                    //            //    Customer.CreateDate = CustomerChat.CreateDate;
                    //            //    Customer.customerChat = CustomerChat;
                    //            //    var x = _hub.Clients.All.SendAsync("brodCastAgentMessage", Customer);
                    //            //    SocketIOManager.SendChat(Customer, Customer.TenantId.Value);
                    //            //}
                    //            //else
                    //            //{
                    //            //    this.telemetry.TrackTrace($"error while send from bot to user throw the 360 dialog , the user name is  : {Customer.displayName}", SeverityLevel.Critical);
                    //            //    MessagesSent[Customer.userId] = false;
                    //            //    return Ok();

                    //            //}


                    //            postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    //            {
                    //                to = from,
                    //                type = "interactive",
                    //                postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
                    //                {
                    //                    type = "button",
                    //                    interactive = new InteractiveButtonsModel
                    //                    {
                    //                        type = "button",
                    //                        // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                    //                        body = new InteractiveBodyModel { text = msgBot.Summary },
                    //                        //   fo = new postWhatsAppMessageModel.Footer { text = msgBot.Summary },
                    //                        action = new InteractiveButtonActionModel
                    //                        {
                    //                            buttons = buttons

                    //                        }
                    //                    }


                    //                }

                    //            };





                    //        }



                    //    }
                    //    else
                    //    {

                    //        postWhatsAppMessageModel = new PostWhatsAppMessageModel
                    //        {
                    //            to = from,
                    //            type = "interactive",
                    //            postWhatsAppInteractiveButtonModel = new PostWhatsAppInteractiveButtonModel
                    //            {
                    //                type = "button",
                    //                interactive = new InteractiveButtonsModel
                    //                {
                    //                    type = "button",
                    //                    // header=new postWhatsAppMessageModel.Header {type="text", text="" },
                    //                    body = new InteractiveBodyModel { text = msgBot.Text },
                    //                    footer = new InteractiveFooterModel { text = msgBot.Summary },
                    //                    action = new InteractiveButtonActionModel
                    //                    {
                    //                        buttons = buttons

                    //                    }
                    //                }


                    //            }


                    //        };

                    //    }


                    //}

                }
                else
                {


                    if (msgBot.InputHint == "منطقتك" || msgBot.SuggestedActions.Actions.Count() > 10)//msgBot.SuggestedActions.Actions.Count()>10
                    {
                        //WhatsAppContent message2 = PrepareMessageContent(msgBot, botId, "", 0, "");

                        //postWhatsAppMessageModel = new PostWhatsAppMessageModel
                        //{
                        //    to = from,
                        //    type = "text",
                        //    text = new PostWhatsAppMessageModel.Text
                        //    {
                        //        body = message2.text
                        //    }


                        //};

                    }
                    else
                    {
                        List<InteractiveSectionListModel> sections = new List<InteractiveSectionListModel>();
                        List<InteractiveRowListModel> rows = new List<InteractiveRowListModel>();
                        foreach (var button in msgBot.SuggestedActions.Actions)
                        {

                            rows.Add(new InteractiveRowListModel
                            {
                                id = button.Value.ToString(),
                                title = button.Title,
                                description = button.Image

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




            lstPostWhatsAppMessageModel.Add(postWhatsAppMessageModel);

            return lstPostWhatsAppMessageModel;
        }


        public async Task<bool> postToFB(PostWhatsAppMessageModel postWhatsAppMessageModel, string phonnumberId, string fbToken, bool IsD360Dialog)
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


            return await SendToWhatsApp(postBody, phonnumberId, fbToken, IsD360Dialog);
        }

        public string PrepareCatalogMessage(PostWhatsAppMessageModel postWhatsAppMessageModel,
                                          CatalogTemplateDto messageModel,
                                          string catalogType = null)
        {
            if (messageModel?.Catalog == null)
                throw new InvalidOperationException("Catalog message requires a Catalog object");

            if (string.IsNullOrWhiteSpace(postWhatsAppMessageModel?.to))
                throw new ArgumentException("Recipient phone number is required");

            bool isMultiProduct;
            switch (catalogType)
            {
                case "Cataloge_Single_Product":
                    isMultiProduct = false;
                    break;
                case "Cataloge_multity_Product":
                case null:
                    isMultiProduct = true;
                    break;
                default:
                    throw new ArgumentException($"Invalid catalog type: {catalogType}");
            }

            var bodyText = messageModel.Body?.Text ?? string.Empty;

            object header = null;
            if (!string.IsNullOrEmpty(messageModel.Header?.Text))
            {
                messageModel.Header.Type = string.IsNullOrEmpty(messageModel.Header.Type)
                    ? "text"
                    : messageModel.Header.Type.ToLower();

                if (messageModel.Header.Type != "text")
                    throw new ArgumentException("Catalog messages only support text headers");

                header = new
                {
                    type = messageModel.Header.Type,
                    text = messageModel.Header.Text
                };
            }

            var footer = !string.IsNullOrEmpty(messageModel.Footer?.Text)
                ? new { text = messageModel.Footer.Text }
                : null;


            object action;
            if (isMultiProduct)
            {
                if (messageModel.Catalog.Products == null || !messageModel.Catalog.Products.Any())
                    throw new InvalidOperationException("Multi-product message requires at least one product");

                action = new
                {
                    catalog_id = messageModel.Catalog.CatalogId,
                    sections = new[]
                    {
                new
                {
                    title = messageModel.Catalog.SectionTitle ?? "Our Products",
                    product_items = messageModel.Catalog.Products
                        .Where(p => !string.IsNullOrEmpty(p.retailer_Id))
                        .Select(p => new { product_retailer_id = p.retailer_Id })
                        .ToArray()
                }
            }
                };
            }
            else 
            {
                var firstProduct = messageModel.Catalog.Products?.FirstOrDefault();
                if (firstProduct == null || string.IsNullOrEmpty(firstProduct.ProductId))
                    throw new InvalidOperationException("Single product message requires a valid product");

                action = new
                {
                    catalog_id = messageModel.Catalog.CatalogId,
                    product_retailer_id = firstProduct.retailer_Id
                };
            }

            var requestBody = new
            {
                messaging_product = "whatsapp",
                recipient_type = "individual",
                to = postWhatsAppMessageModel.to,
                type = "interactive",
                interactive = new
                {
                    type = isMultiProduct ? "product_list" : "product",
                    body = new { text = bodyText },
                    header,
                    footer,
                    action
                }
            };
            var json = JsonConvert.SerializeObject(requestBody);
            return JsonConvert.SerializeObject(requestBody);
        }

        public async Task<string> postToFBNew(PostWhatsAppMessageModel postWhatsAppMessageModel, string phonnumberId, string fbToken,string channel , CatalogTemplateDto catalogTemplateDto)
        {

            //postWhatsAppMessageModel.type = catalog;
            string postBody = String.Empty;
            WhatsAppAppService whatsAppAppService = new WhatsAppAppService();

            if (postWhatsAppMessageModel.type== "Cataloge_multity_Product" || postWhatsAppMessageModel.type == "Cataloge_Single_Product")
            {

                if (channel.ToLower() == "whatsapp" || channel.ToLower() != "facebook" || channel != "instagram")
                {
                    postBody = whatsAppAppService.PrepareCatalogMessage(postWhatsAppMessageModel, catalogTemplateDto, postWhatsAppMessageModel.type);
                }

            }
            else
            {
                switch (postWhatsAppMessageModel.type)
                {
                    case nameof(WhatsContentTypeEnum.interactive):
                        if (channel.ToLower() == "facebook" || channel.ToLower() == "instagram")
                        {
                            postBody = whatsAppAppService.PrepareFaceBookInteractive(postWhatsAppMessageModel);

                        }
                        else
                        {
                            postBody = whatsAppAppService.PrepareInteractiveMessage(postWhatsAppMessageModel);

                        }
                        break;

                    case nameof(WhatsContentTypeEnum.text):
                        if (channel.ToLower() == "facebook")
                        {
                            postBody = whatsAppAppService.PrepareFacebookTextMessage(postWhatsAppMessageModel);

                        }
                        else if (channel == "instagram")
                        {
                            postBody = whatsAppAppService.PrepareInstagramTextMessage(postWhatsAppMessageModel);
                        }
                        else
                        {
                            postBody = whatsAppAppService.PrepareTextMessage(postWhatsAppMessageModel);

                        }

                        break;

                    case nameof(WhatsContentTypeEnum.video):
                        if (channel.ToLower() == "facebook")
                        {
                            postBody = whatsAppAppService.PrepareFacebookVideoMessage(postWhatsAppMessageModel);
                        }
                        else if (channel.ToLower() == "instagram")
                        {
                            postBody = whatsAppAppService.PrepareFacebookVideoMessage(postWhatsAppMessageModel);
                        }
                        else
                        {
                            postBody = whatsAppAppService.PrepareVideoMessage(postWhatsAppMessageModel);
                        }
                        break;
                    case nameof(WhatsContentTypeEnum.image):
                        if (channel.ToLower() == "facebook" || channel == "instagram")
                        {
                            postBody = whatsAppAppService.PrepareFacebookImageMessage(postWhatsAppMessageModel);
                        }
                        else
                        {
                            postBody = whatsAppAppService.PrepareImageMessage(postWhatsAppMessageModel);
                        }
                        break;
                    case nameof(WhatsContentTypeEnum.file):
                        if (channel.ToLower() == "facebook" || channel == "instagram")
                        {
                            postBody = whatsAppAppService.PrepareFacebookFileMessage(postWhatsAppMessageModel);
                        }
                        else
                        {
                            postBody = whatsAppAppService.PrepareDocumentMessage(postWhatsAppMessageModel);
                        }
                        break;
                    case nameof(WhatsContentTypeEnum.location):
                        if (channel.ToLower() == "facebook")
                        {
                            postBody = whatsAppAppService.PrepareFacebookLocationMessage(postWhatsAppMessageModel);
                        }
                        else
                        {
                            postBody = whatsAppAppService.PrepareLocationMessage(postWhatsAppMessageModel);
                        }
                        break;

                    case nameof(WhatsContentTypeEnum.document):
                        if (channel.ToLower() == "facebook" || channel == "instagram")
                        {
                            postBody = whatsAppAppService.PrepareFacebookDocumentMessage(postWhatsAppMessageModel);
                        }
                        else
                        {
                            postBody = whatsAppAppService.PrepareDocumentMessage(postWhatsAppMessageModel);
                        }
                        break;
                    case nameof(WhatsContentTypeEnum.audio):
                        if (channel.ToLower() == "facebook" || channel == "instagram")
                        {
                            postBody = whatsAppAppService.PrepareFacebookVoiceMessage(postWhatsAppMessageModel);
                        }
                        else
                        {
                            postBody = whatsAppAppService.PrepareVoiceMessage(postWhatsAppMessageModel);
                        }
                        break;

                    default:
                        break;
                }

                }


                return await SendToWhatsAppNew(postBody, phonnumberId, fbToken, channel);
        }

        //public async Task<string> PostToFBNewFacebook(PostFacebookMessageModel postFacebookMessageModel, string fbToken)
        //{
        //    string postBody = string.Empty;
        //    WhatsAppAppService facebookAppService = new WhatsAppAppService();

        //    switch (postFacebookMessageModel.Type)
        //    {
        //        case "text":
        //           // postBody = facebookAppService.PrepareFacebookTextMessage(postFacebookMessageModel);
        //            break;

        //        case "image":
        //            postBody = facebookAppService.PrepareFacebookImageMessage(postFacebookMessageModel);
        //            break;

        //        case "video":
        //            postBody = facebookAppService.PrepareFacebookVideoMessage(postFacebookMessageModel);
        //            break;

        //        case "file":
        //            postBody = facebookAppService.PrepareFacebookFileMessage(postFacebookMessageModel);
        //            break;

        //        case "audio":
        //            postBody = facebookAppService.PrepareFacebookAudioMessage(postFacebookMessageModel);
        //            break;

        //        case "quick_reply":
        //            postBody = facebookAppService.PrepareFacebookQuickReplyMessage(postFacebookMessageModel);
        //            break;

        //        default:
        //            throw new ArgumentException("Unsupported message type");
        //    }

        //    return await SendToFacebook(postBody, fbToken);
        //}
        private async Task<string> SendToFacebook(string postBody, string pageAccessToken)
        {
            using (var httpClient = new HttpClient())
            {

                var requestUrl = $"https://graph.facebook.com/v15.0/me/messages?access_token=EAAMlb5ZBkNS0BOZCXUDDkDOWlmKISWTTrFvQQSoJZBNeSu7Pu0Lqt0GZC1Wo30mv08ClLU7rfVMwMnQtbTSg1ctYNBo43zzAFqXv2SjpZBzV8A65bQCZB8MQjAcRJbOgMVx4Xz3QRX1D51yFWu3iZBZCMGnKMyN22KEdgKLygRqE2Wx3hN6NGfx4ZBr40812DP7zSYdp3vEhzBB1BjKdk6OHzxK3PIAKM";
                //var requestUrl = $"https://graph.facebook.com/v15.0/me/messages?access_token={pageAccessToken}";
                var content = new StringContent(postBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(requestUrl, content);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public FacebookContent PrepareMessageContentFacebook(Activity msgBot, string botId, string userId, int tenantId, string conversationId)
        {
            string messageToSend = string.Empty;
            List<CardAction> actions = new List<CardAction>();
            int order = 1;
            var options = new Dictionary<int, string>();

            if (msgBot.SuggestedActions != null && msgBot.SuggestedActions.Actions.Count > 0)
            {
                actions.AddRange(msgBot.SuggestedActions.Actions);
            }

            foreach (var action in actions)
            {
                messageToSend += order.ToString() + "- " + action.Title + "\r\n";
                options.Add(order, action.Title);
                order++;
            }

            FacebookContent message = new FacebookContent
            {
                Type = "text", 
                Text = msgBot.Text + "\r\n" + messageToSend, 
                AgentName = botId, 
                AgentId = "1000000", 
                UserId = userId, 
                TenantId = tenantId, 
                ConversationId = conversationId, 
                IsQuickReply = actions.Count > 0, 
                QuickReplies = new List<FacebookContent.QuickReply>() 
            };

            if (actions.Count > 0)
            {
                foreach (var action in actions)
                {
                    message.QuickReplies.Add(new FacebookContent.QuickReply
                    {
                        ContentType = "text", // Default content type is text
                        Title = action.Title, // Title of the quick reply
                        Payload = action.Value?.ToString() // Payload (e.g., action value)
                    });
                }
            }

            return message;
        }
        public WhatsAppContent PrepareMessageContent(Activity msgBot, string botId, string userId, int tenantId, string conversationID)
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
                agentId = "1000000",
                userId=userId,
                tenantId=tenantId,
                conversationID= conversationID,
            };

            return message;

        }
        public string MassageTypeText360Dailog(string massag, string fbToken, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, ref string medaiUrl, AttachmentMessageModel attachmentMessageModel, out string interactiveId)
        {

            //var aaaa = JsonConvert.SerializeObject(jsonData);
             var messages = JsonConvert.DeserializeObject<List<WhatsAppMessageModel>>(massag);
            var url = string.Empty;
            var type = string.Empty;
            medaiUrl = string.Empty;
            string msg = string.Empty;
            interactiveId = string.Empty;
            if (messages[0].Type == WhatsContentTypeEnum.video.ToString())
            {

                attachmentMessageModel.IsHasAttachment = true;
                attachmentMessageModel.FcToken = fbToken;
                attachmentMessageModel.AttachmentId = messages[0].video.id;
                attachmentMessageModel.AttachmentMimeType = messages[0].video.id;

                //var RetrievingMedia = RetrievingMediaAsync(messages[0].video.id, fbToken).Result;

                byte[] videoBytes = new byte[0];// RetrievingMedia.contentByte;

                //var extention = "." + messages[0].video.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = videoBytes,
                //    Extension = extention,
                //    MimeType = messages[0].video.mime_type
                //};
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //messages.FirstOrDefault().video.link = url;

                // type = "video";
                // messages.FirstOrDefault().Type = messages[0].video.mime_type;
                //messages.FirstOrDefault().mediaUrl = url;

                medaiUrl =url;
                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].video.mime_type, "", new byte[0], attachmentMessageModel.AttachmentId));


            }

            else if (messages[0].Type == WhatsContentTypeEnum.image.ToString())
            {

                attachmentMessageModel.IsHasAttachment = true;
                attachmentMessageModel.FcToken = fbToken;
                attachmentMessageModel.AttachmentId = messages[0].image.id;
                attachmentMessageModel.AttachmentMimeType = messages[0].image.mime_type;


                //var RetrievingMedia = RetrievingMediaAsync(messages[0].image.id, fbToken).Result;

                byte[] imageBytes = new byte[0];//= RetrievingMedia.contentByte;

                //var extention = "." + messages[0].image.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = imageBytes,
                //    Extension = extention,
                //    MimeType = messages[0].image.mime_type
                //};
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //// messages.FirstOrDefault().image.link = url;

                //type = "image";
                ////messages.FirstOrDefault().Type = messages[0].image.mime_type;
                ////messages.FirstOrDefault().mediaUrl = url;
                //medaiUrl = url;

                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].image.mime_type, "", new byte[0], attachmentMessageModel.AttachmentId));


            }
            else if (messages[0].Type == WhatsContentTypeEnum.document.ToString())
            {

                attachmentMessageModel.IsHasAttachment = true;
                attachmentMessageModel.FcToken = fbToken;
                attachmentMessageModel.AttachmentId = messages[0].document.id;
                attachmentMessageModel.AttachmentMimeType = messages[0].document.mime_type;

                //var RetrievingMedia = RetrievingMediaAsync(messages[0].document.id, fbToken).Result;
                //byte[] imageBytes = null;//= RetrievingMedia.contentByte;
                //var extention = "." + messages[0].document.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = imageBytes,
                //    Extension = extention,
                //    MimeType = messages[0].document.mime_type
                //};
                //type = "file";
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //  messages.FirstOrDefault().document.link = url;
                //  messages.FirstOrDefault().document.mime_type = "file";

                messages.FirstOrDefault().Type = "file";
                // messages.FirstOrDefault().mediaUrl = url;
                medaiUrl = url;
                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].document.mime_type, "", new byte[0], attachmentMessageModel.AttachmentId));

            }

            else if (messages[0].Type == WhatsContentTypeEnum.audio.ToString())
            {

                attachmentMessageModel.IsHasAttachment = true;
                attachmentMessageModel.FcToken = fbToken;
                attachmentMessageModel.AttachmentId = messages[0].voice.id;
                attachmentMessageModel.AttachmentMimeType = messages[0].voice.mime_type;

                //var RetrievingMedia = RetrievingMediaAsync(messages[0].voice.id, fbToken).Result;
                // byte[] imageBytes = null; //RetrievingMedia.contentByte;
                //var extention = "." + messages[0].voice.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = imageBytes,
                //    Extension = extention,
                //    MimeType = messages[0].voice.mime_type
                //};
                //type = "audio";
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //medaiUrl = url;

                //// messages.FirstOrDefault().type = "audio";
                //messages.FirstOrDefault().mediaUrl = url;

                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].voice.mime_type, "", new byte[0], attachmentMessageModel.AttachmentId));

            }

            else if (messages[0].Type == WhatsContentTypeEnum.location.ToString())
            {
                var query = messages[0].location.latitude.ToString() + "," + messages[0].location.longitude.ToString();
                // messages.FirstOrDefault().Text = new  WhatsAppTextModel {  Body = query };
                // messages.FirstOrDefault().Type = "location";

                type = "location";
                //  messages.FirstOrDefault().Text = query;
                msg = query;
            }
            else if (messages[0].Type == WhatsContentTypeEnum.interactive.ToString())
            {
                if (messages[0].interactive.type == "list_reply")
                {

                    msg = messages[0].interactive.list_reply.title;
                    interactiveId = messages[0].interactive.list_reply.id;


                    type = "text";

                }
                else
                {

                    msg = messages[0].interactive.button_reply.title;
                    interactiveId = messages[0].interactive.button_reply.id;



                    type = "text";
                }

            }

            else if (messages[0].Type == WhatsContentTypeEnum.text.ToString())
            {
                msg = messages[0].Text.Body;
            }




            return msg;
        }

        public string MassageTypeText(List<WhatsAppMessageModel> messages, string fbToken, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, ref string medaiUrl, AttachmentMessageModel attachmentMessageModel, out string  interactiveId)
        {
            var url = string.Empty;
            var type = string.Empty;
            medaiUrl = string.Empty;
            string msg = string.Empty;
             interactiveId = string.Empty;
            if (messages[0].Type == WhatsContentTypeEnum.video.ToString())
            {

                attachmentMessageModel.IsHasAttachment = true;
                attachmentMessageModel.FcToken = fbToken;
                attachmentMessageModel.AttachmentId = messages[0].video.id;
                attachmentMessageModel.AttachmentMimeType = messages[0].video.id;

                //var RetrievingMedia = RetrievingMediaAsync(messages[0].video.id, fbToken).Result;

                byte[] videoBytes = new byte[0];// RetrievingMedia.contentByte;

                //var extention = "." + messages[0].video.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = videoBytes,
                //    Extension = extention,
                //    MimeType = messages[0].video.mime_type
                //};
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //messages.FirstOrDefault().video.link = url;

                // type = "video";
                // messages.FirstOrDefault().Type = messages[0].video.mime_type;
                //messages.FirstOrDefault().mediaUrl = url;

                medaiUrl =url;
                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].video.mime_type, "", new byte[0], attachmentMessageModel.AttachmentId));

                if (messages[0].video.caption!=null)
                {
                    msg=messages[0].video.caption;
                }
            }

            else if (messages[0].Type == WhatsContentTypeEnum.image.ToString())
            {

                attachmentMessageModel.IsHasAttachment = true;
                attachmentMessageModel.FcToken = fbToken;
                attachmentMessageModel.AttachmentId = messages[0].image.id;
                attachmentMessageModel.AttachmentMimeType = messages[0].image.mime_type;


                //var RetrievingMedia = RetrievingMediaAsync(messages[0].image.id, fbToken).Result;

                byte[] imageBytes = new byte[0];//= RetrievingMedia.contentByte;

                //var extention = "." + messages[0].image.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = imageBytes,
                //    Extension = extention,
                //    MimeType = messages[0].image.mime_type
                //};
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //// messages.FirstOrDefault().image.link = url;

                //type = "image";
                ////messages.FirstOrDefault().Type = messages[0].image.mime_type;
                ////messages.FirstOrDefault().mediaUrl = url;
                medaiUrl = url;

                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].image.mime_type, "", new byte[0], attachmentMessageModel.AttachmentId));
                if (messages.FirstOrDefault().image.caption!=null)
                {
                    msg=messages.FirstOrDefault().image.caption;
                }

            }
            else if (messages[0].Type == WhatsContentTypeEnum.document.ToString())
            {

                attachmentMessageModel.IsHasAttachment = true;
                attachmentMessageModel.FcToken = fbToken;
                attachmentMessageModel.AttachmentId = messages[0].document.id;
                attachmentMessageModel.AttachmentMimeType = messages[0].document.mime_type;

                //var RetrievingMedia = RetrievingMediaAsync(messages[0].document.id, fbToken).Result;
                //byte[] imageBytes = null;//= RetrievingMedia.contentByte;
                //var extention = "." + messages[0].document.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = imageBytes,
                //    Extension = extention,
                //    MimeType = messages[0].document.mime_type
                //};
                //type = "file";
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //  messages.FirstOrDefault().document.link = url;
                //  messages.FirstOrDefault().document.mime_type = "file";

                messages.FirstOrDefault().Type = "file";
                // messages.FirstOrDefault().mediaUrl = url;
                medaiUrl = url;
                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].document.mime_type, "", new byte[0], attachmentMessageModel.AttachmentId));
                if (messages[0].document.caption!=null)
                {
                    msg=messages[0].document.caption;
                }
            }

            else if (messages[0].Type == WhatsContentTypeEnum.audio.ToString())
            {

                attachmentMessageModel.IsHasAttachment = true;
                attachmentMessageModel.FcToken = fbToken;
                attachmentMessageModel.AttachmentId = messages[0].voice.id;
                attachmentMessageModel.AttachmentMimeType = messages[0].voice.mime_type;

                //var RetrievingMedia = RetrievingMediaAsync(messages[0].voice.id, fbToken).Result;
                // byte[] imageBytes = null; //RetrievingMedia.contentByte;
                //var extention = "." + messages[0].voice.mime_type.Split("/")[1];
                //AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                //AttachmentContent attachmentContent = new AttachmentContent()
                //{
                //    Content = imageBytes,
                //    Extension = extention,
                //    MimeType = messages[0].voice.mime_type
                //};
                //type = "audio";
                //url = azureBlobProvider.Save(attachmentContent).Result;
                //medaiUrl = url;

                //// messages.FirstOrDefault().type = "audio";
                //messages.FirstOrDefault().mediaUrl = url;

                tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].voice.mime_type, "", new byte[0], attachmentMessageModel.AttachmentId));
                if (messages[0].voice.caption!=null)
                {
                    msg=messages[0].voice.caption;
                }
            }

            else if (messages[0].Type == WhatsContentTypeEnum.location.ToString())
            {
                var query = messages[0].location.latitude.ToString() + "," + messages[0].location.longitude.ToString();
                // messages.FirstOrDefault().Text = new  WhatsAppTextModel {  Body = query };
                // messages.FirstOrDefault().Type = "location";

                type = "location";
                //  messages.FirstOrDefault().Text = query;
                msg = query;
            }
            else if (messages[0].Type == WhatsContentTypeEnum.interactive.ToString())
            {
                if (messages[0].interactive.type == "list_reply")
                {

                    msg = messages[0].interactive.list_reply.title;
                    interactiveId = messages[0].interactive.list_reply.id;


                    type = "text";

                }
                else
                {

                    msg = messages[0].interactive.button_reply.title;
                    interactiveId = messages[0].interactive.button_reply.id;



                    type = "text";
                }

            }

            else if (messages[0].Type == WhatsContentTypeEnum.text.ToString())
            {
                msg = messages[0].Text.Body;
            }

            else if (messages[0].Type == WhatsContentTypeEnum.order.ToString())
            {

                var order = messages[0].order;

                if (order != null)
                {
                    msg = "📦 New Order:\n\n";
                    msg += "Item\tQuantity\tPrice\n";  

                    foreach (var item in order.product_items)
                    {
                        msg += $"{item.product_retailer_id}\t{item.quantity}\t{item.item_price} {item.currency}\n";
                    }

                    if (!string.IsNullOrEmpty(order.text))
                    {
                        msg += $"\n📝 Notes: {order.text}\n";
                    }
                }
            }

            return msg;
        }

        public string MassageTypeFaceBook(List<Messaging> messages, string fbToken, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, ref string medaiUrl, AttachmentMessageModel attachmentMessageModel, out string interactiveId)
        {
            var url = string.Empty;
            var type = string.Empty;
            medaiUrl = string.Empty;
            string msg = string.Empty;
            interactiveId = string.Empty;
            if (messages[0].Message.attachments!=null&& messages[0].Message.attachments.Length>0)
            {
                switch (messages[0].Message.attachments[0].type)
                {

                    case "image":
                        interactiveId="image";
                        attachmentMessageModel.IsHasAttachment = true;

                        medaiUrl = messages[0].Message.attachments[0].payload.url;
                        tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].Message.attachments[0].type, "", new byte[0], ""));                       
                        break;


                    case "audio":
                        interactiveId="audio";
                        attachmentMessageModel.IsHasAttachment = true;

                        medaiUrl = messages[0].Message.attachments[0].payload.url; ;
                        tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].Message.attachments[0].type, "", new byte[0], ""));
                        break;
                    case "video":
                        interactiveId="video";
                        attachmentMessageModel.IsHasAttachment = true;

                        medaiUrl = messages[0].Message.attachments[0].payload.url; ;
                        tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].Message.attachments[0].type, "", new byte[0], ""));
                        break;

                    case "file":
                        interactiveId="file";
                        attachmentMessageModel.IsHasAttachment = true;

                        medaiUrl = messages[0].Message.attachments[0].payload.url; ;
                        tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages[0].Message.attachments[0].type, "", new byte[0], ""));
                        break;
                }

            }
            else
            {
                interactiveId="text";
                msg=messages[0].Message.Text;

            }

            return msg;
        }
        public string MassageTypeInstgram(Message3453 messages, string fbToken, List<Microsoft.Bot.Connector.DirectLine.Attachment> tAttachments, ref string medaiUrl, AttachmentMessageModel attachmentMessageModel, out string interactiveId)
        {
            var url = string.Empty;
            var type = string.Empty;
            medaiUrl = string.Empty;
            string msg = string.Empty;
            interactiveId = string.Empty;
            if (messages.attachments!=null&& messages.attachments.Length>0)
            {
                switch (messages.attachments[0].type)
                {

                    case "image":
                        interactiveId="image";
                        attachmentMessageModel.IsHasAttachment = true;

                        medaiUrl = messages.attachments[0].payload.url;
                        tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages.attachments[0].type, "", new byte[0], ""));
                        break;


                    case "audio":
                        interactiveId="audio";
                        attachmentMessageModel.IsHasAttachment = true;

                        medaiUrl = messages.attachments[0].payload.url; ;
                        tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages.attachments[0].type, "", new byte[0], ""));
                        break;
                    case "video":
                        interactiveId="video";
                        attachmentMessageModel.IsHasAttachment = true;

                        medaiUrl = messages.attachments[0].payload.url; ;
                        tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages.attachments[0].type, "", new byte[0], ""));
                        break;

                    case "file":
                        interactiveId="file";
                        attachmentMessageModel.IsHasAttachment = true;

                        medaiUrl = messages.attachments[0].payload.url; ;
                        tAttachments.Add(new Microsoft.Bot.Connector.DirectLine.Attachment(messages.attachments[0].type, "", new byte[0], ""));
                        break;
                }

            }
            else
            {
                interactiveId="text";
                msg=messages.text;

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
            text.preview_url = true;
            text.body = postWhatsAppMessageModel.text.body;
            postWhatsAppTextMessageModel.text = text;
            result = JsonConvert.SerializeObject(postWhatsAppTextMessageModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return result;
        }

        public string PrepareFacebookTextMessage(PostWhatsAppMessageModel postFacebookMessageModel)
        {
            var textMessage = new
            {
                recipient = new { id = postFacebookMessageModel.to },
                message = new { text = postFacebookMessageModel.text.body},
                messaging_type = "RESPONSE",
            };

            return JsonConvert.SerializeObject(textMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        public string PrepareInstagramTextMessage(PostWhatsAppMessageModel postInstagramMessageModel)
        {
            var textMessage = new
            {
                recipient = new { id = postInstagramMessageModel.to },
                message = new { text = postInstagramMessageModel.text.body },
                messaging_type = "RESPONSE",
            };

            return JsonConvert.SerializeObject(textMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public string PrepareFacebookImageMessage(PostFacebookMessageModel postFacebookMessageModel)
        {
            var imageMessage = new
            {
                recipient = new { id = postFacebookMessageModel.RecipientId },
                message = new
                {
                    attachment = new
                    {
                        type = "image",
                        payload = new
                        {
                            url = postFacebookMessageModel.Image.Url,
                            is_reusable = postFacebookMessageModel.Image.Reusable
                        }
                    }
                },
                messaging_type = postFacebookMessageModel.MessagingType,
                notification_type = postFacebookMessageModel.NotificationType
            };

            return JsonConvert.SerializeObject(imageMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        //public string PrepareFacebookVideoMessage(PostFacebookMessageModel postFacebookMessageModel)
        //{
        //    var videoMessage = new
        //    {
        //        recipient = new { id = postFacebookMessageModel.RecipientId },
        //        message = new
        //        {
        //            attachment = new
        //            {
        //                type = "video",
        //                payload = new
        //                {
        //                    url = postFacebookMessageModel.Video.Url,
        //                    is_reusable = postFacebookMessageModel.Video.Reusable
        //                }
        //            }
        //        },
        //        messaging_type = postFacebookMessageModel.MessagingType,
        //        notification_type = postFacebookMessageModel.NotificationType
        //    };

        //    return JsonConvert.SerializeObject(videoMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        //}
        public string PrepareFacebookAudioMessage(PostWhatsAppMessageModel postFacebookMessageModel)
        {
            var audioMessage = new
            {
                recipient = new { id = postFacebookMessageModel.to },
                message = new
                {
                    attachment = new
                    {
                        type = "audio",
                        payload = new
                        {
                            url = postFacebookMessageModel.audio.link,
                            is_reusable = true
                        }
                    }
                }
               // messaging_type = postFacebookMessageModel.MessagingType,
               // notification_type = postFacebookMessageModel.NotificationType
            };

            return JsonConvert.SerializeObject(audioMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public string PrepareFacebookFileMessage(PostWhatsAppMessageModel postFacebookMessageModel)
        {
            var fileMessage = new
            {
                recipient = new { id = postFacebookMessageModel.to },
                message = new
                {
                    attachment = new
                    {
                        type = "file",
                        payload = new
                        {
                            url = postFacebookMessageModel.document.link,
                            is_reusable = true
                        }
                    }
                },
               messaging_type = "RESPONSE"
                // notification_type = postFacebookMessageModel.NotificationType
            };

            return JsonConvert.SerializeObject(fileMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        public string PrepareFacebookQuickReplyMessage(PostFacebookMessageModel postFacebookMessageModel)
        {
            var quickReplies = postFacebookMessageModel.QuickReplyData.QuickReplies.Select(qr => new
            {
                content_type = qr.ContentType,
                title = qr.Title,
                payload = qr.Payload,
                image_url = qr.ImageUrl
            }).ToList();

            var quickReplyMessage = new
            {
                recipient = new { id = postFacebookMessageModel.RecipientId },
                message = new
                {
                    text = postFacebookMessageModel.Text.Text,
                    quick_replies = quickReplies
                },
                messaging_type = postFacebookMessageModel.MessagingType,
                notification_type = postFacebookMessageModel.NotificationType
            };

            return JsonConvert.SerializeObject(quickReplyMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
                video.caption = postWhatsAppMessageModel.video.caption;
                video.link = postWhatsAppMessageModel.video.link;
                postWhatsAppImageModel.video = video;
                result = JsonConvert.SerializeObject(postWhatsAppImageModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            else
            {
                PostWhatsAppMessageModel postVideoMessageModel = new PostWhatsAppMessageModel();
                PostWhatsAppMessageModel.Video video = new PostWhatsAppMessageModel.Video();
                postVideoMessageModel.messaging_product = "whatsapp";
                postVideoMessageModel.recipient_type = "individual";
                postVideoMessageModel.to = postWhatsAppMessageModel.to;
                postVideoMessageModel.type = "video";
                //video.id = postWhatsAppMessageModel.video.id;
                video.caption = postWhatsAppMessageModel.video.caption;
                video.link = postWhatsAppMessageModel.video.link;
                postVideoMessageModel.video = video;
                // result = JsonConvert.SerializeObject(postVideoMessageModel);
                result = JsonConvert.SerializeObject(postVideoMessageModel, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            }
            return result;
        }
        public string PrepareFacebookVideoMessage(PostWhatsAppMessageModel postFacebookMessageModel)
        {
            var videoMessage = new
            {
                recipient = new { id = postFacebookMessageModel.to },
                message = new
                {
                    attachment = new
                    {
                        type = "video",
                        payload = new
                        {
                            url = postFacebookMessageModel.video.link,
                            is_reusable = true
                        }
                    }
                },
                messaging_type = "RESPONSE"
            };

            return JsonConvert.SerializeObject(videoMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
                image.caption = postWhatsAppMessageModel.image.caption;
                postWhatsAppImageModel.image = image;

                result = JsonConvert.SerializeObject(postWhatsAppImageModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
                image.caption = postWhatsAppMessageModel.image.caption;
                objPostWhatsAppMessageModel.image = image;
                result = JsonConvert.SerializeObject(objPostWhatsAppMessageModel, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }


            return result;
        }
        public string PrepareFacebookImageMessage(PostWhatsAppMessageModel postFacebookMessageModel)
        {
            var imageMessage = new
            {
                recipient = new { id = postFacebookMessageModel.to },
                message = new
                {
                    attachment = new
                    {
                        type = "image",
                        payload = new
                        {
                            url = postFacebookMessageModel.image.link,
                            is_reusable = true
                        }
                    }
                },
                messaging_type = "RESPONSE"
            };

            return JsonConvert.SerializeObject(imageMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
                postWhatsAppImageModel.type = "audio";
                audio.link = postWhatsAppMessageModel.audio.link;
                postWhatsAppImageModel.audio = audio;
                result = JsonConvert.SerializeObject(postWhatsAppImageModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            else
            {

                PostWhatsAppMessageModel postAudioMessageModel = new PostWhatsAppMessageModel();
                PostWhatsAppMessageModel.Audio audio = new PostWhatsAppMessageModel.Audio();
                postAudioMessageModel.messaging_product = "whatsapp";
                postAudioMessageModel.recipient_type = "individual";
                postAudioMessageModel.to = postWhatsAppMessageModel.to;
                postAudioMessageModel.type = "audio";
                audio.id = postWhatsAppMessageModel.audio.id;
                audio.link = postWhatsAppMessageModel.audio.link;
                postAudioMessageModel.audio = audio;
                //result = JsonConvert.SerializeObject(postAudioMessageModel);
                result = JsonConvert.SerializeObject(postAudioMessageModel, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            }

            return result;
        }
        public string PrepareFacebookVoiceMessage(PostWhatsAppMessageModel postFacebookMessageModel)
        {
            var voiceMessage = new
            {
                recipient = new { id = postFacebookMessageModel.to },
                message = new
                {
                    attachment = new
                    {
                        type = "audio",
                        payload = new
                        {
                            url = postFacebookMessageModel.audio.link,
                            is_reusable = true
                        }
                    }
                },
                messaging_type = "RESPONSE"
            };

            return JsonConvert.SerializeObject(voiceMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
            result = JsonConvert.SerializeObject(postLocationMessageModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            return result;
        }
        public string PrepareFacebookLocationMessage(PostWhatsAppMessageModel postFacebookMessageModel)
        {
            var locationMessage = new
            {
                recipient = new { id = postFacebookMessageModel.to },
                message = new
                {
                    text = $"Location: {postFacebookMessageModel.location.name}\n" +
                           $"Address: {postFacebookMessageModel.location.address}\n" +
                           $"Latitude: {postFacebookMessageModel.location.latitude}\n" +
                           $"Longitude: {postFacebookMessageModel.location.longitude}"
                },
                messaging_type = "RESPONSE"
            };

            return JsonConvert.SerializeObject(locationMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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

                try
                {
                    if (!string.IsNullOrEmpty(document.filename) && document.filename.Contains("."))
                    {

                        document.filename = postWhatsAppMessageModel.document.filename.Split(".")[0];

                    }

                }
                catch
                {


                }
                result = JsonConvert.SerializeObject(postWhatsAppMediaModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

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



                try
                {
                    if (!string.IsNullOrEmpty(document.filename) && document.filename.Contains("."))
                    {

                        document.filename = postWhatsAppMessageModel.document.filename.Split(".")[0];

                    }

                }
                catch
                {


                }

                result = JsonConvert.SerializeObject(postWhatsAppMediaModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }

            return result;
        }
        public string PrepareFacebookDocumentMessage(PostWhatsAppMessageModel postFacebookMessageModel)
        {
            var documentMessage = new
            {
                recipient = new { id = postFacebookMessageModel.to },
                message = new
                {
                    attachment = new
                    {
                        type = "file",
                        payload = new
                        {
                            url = postFacebookMessageModel.document.link,
                            is_reusable = true
                        }
                    }
                },
                messaging_type = "RESPONSE"
            };

            return JsonConvert.SerializeObject(documentMessage, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
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
        public string PrepareFaceBookInteractive(PostWhatsAppMessageModel postWhatsAppMessageModel)
        {

            string result = string.Empty;
            var text = "";
            List<QuickrepliesModel> quick_replies = new List<QuickrepliesModel>();
            if (postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel!=null)
            {
                foreach (var item in postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons)
                {



                    QuickrepliesModel replies = new QuickrepliesModel();
                    replies.content_type = "text";
                    replies.title = item.reply.title;
                    replies.payload = item.reply.title;
                    if (item.reply.image!=null)
                    {

                        replies.image_url=item.reply.image;

                    }

                    quick_replies.Add(replies);
                }
                text=postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.body.text;
            }
            else
            {
                foreach (var item in postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.action.sections[0].rows)
                {



                    QuickrepliesModel replies = new QuickrepliesModel();
                    replies.content_type = "text";
                    replies.title = item.title;
                    replies.payload = item.title;

                    quick_replies.Add(replies);
                }
                text=postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.body.text;

            }


            var payload = new
            {
                recipient = new { id = postWhatsAppMessageModel.to },
                messaging_type = "RESPONSE",
                message = new
                {
                    text = text,
                    quick_replies = quick_replies
                }
            };


            result = JsonConvert.SerializeObject(payload, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return result;

        }
        static string GetFileTypeFromUrl(string url)
        {
            try
            {
                // Create a web request with a HEAD method to get the headers
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "HEAD";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    string contentType = response.ContentType;

                    if (!string.IsNullOrEmpty(contentType))
                    {

                        if (contentType=="application/pdf")
                        {
                            return "file";
                        }
                        // The Content-Type header can contain information about the file type
                        // For example, "image/jpeg" for JPEG images
                        // You can extract the file type from this header
                        return contentType.Split("/")[0];
                    }
                }
            }
            catch (WebException)
            {
                // Handle any errors, such as invalid URLs or network issues
            }

            return "Unknown";
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
            if (postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel != null)
            {
                if (postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive != null)
                {
                    if (postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.footer != null)
                    {
                        if (!string.IsNullOrEmpty(postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.footer.text))
                        {
                            if (postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons!=null)
                            {
                                if (postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons[0].reply.image!=null)
                                {

                                    string fileType = GetFileTypeFromUrl(postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons[0].reply.image);

                                    if (fileType != null)
                                    {
                                        if (fileType.Contains("image"))
                                        {
                                            interactiveButtonsModel.header=new InteractiveHeaderModel() { type="image", image=new Interactiveimage() { link=postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons[0].reply.image } };

                                        }
                                        else if(fileType.Contains("video"))
                                        {
                                            interactiveButtonsModel.header=new InteractiveHeaderModel() { type="video", video=new  WhatsAppVideoModel() {  link=postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons[0].reply.image } };

                                        }
                                        else if (fileType.Contains("document"))
                                        {
                                            interactiveButtonsModel.header=new InteractiveHeaderModel() { type="document", document=new  WhatsAppDocumentModel() { link=postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons[0].reply.image } };

                                        }


                                    }
                                }

                                
                            }
                            
                            interactiveButtonsModel.footer = new InteractiveFooterModel();
                            InteractiveFooterModel interactiveFooterModel = new InteractiveFooterModel();
                            interactiveFooterModel.text = postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.footer.text;
                            interactiveButtonsModel.footer = interactiveFooterModel;
                        }
                    }
                }
            }


            // fill actions 
            interactiveButtonsModel.action = new InteractiveButtonActionModel();
            InteractiveButtonActionModel interactiveButtonActionModel = new InteractiveButtonActionModel();
            interactiveButtonActionModel.buttons = new List<InteractiveButtonModel>();

            List<InteractiveButtonModel> lstInteractiveButtonModel = new List<InteractiveButtonModel>();

            foreach (var item in postWhatsAppMessageModel.postWhatsAppInteractiveButtonModel.interactive.action.buttons)
            {
                
                InteractiveButtonModel objInteractiveButtonModel = new InteractiveButtonModel();

                if (item.reply.image!=null)
                {

                    string fileType = GetFileTypeFromUrl(item.reply.image);

                    if (fileType != null)
                    {
                        if (fileType.Contains("image"))
                        {
                            interactiveButtonsModel.header=new InteractiveHeaderModel() { type="image", image=new Interactiveimage() { link=item.reply.image } };

                        }
                        else if (fileType.Contains("video"))
                        {
                            interactiveButtonsModel.header=new InteractiveHeaderModel() { type="video", video=new WhatsAppVideoModel() { link=item.reply.image } };

                        }
                        else if (fileType.Contains("document"))
                        {
                            interactiveButtonsModel.header=new InteractiveHeaderModel() { type="document", document=new WhatsAppDocumentModel() { link=item.reply.image } };

                        }


                    }

                }
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
            result = JsonConvert.SerializeObject(postWhatsAppInteractiveModel,Formatting.None,new JsonSerializerSettings{ NullValueHandling = NullValueHandling.Ignore});


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
            if (postWhatsAppMessageModel.postWhatsAppInteractiveListModel != null)
            {
                if (postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive != null)
                {
                    if (postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.footer != null)
                    {
                        //if (postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.action.sections!=null)
                        //{
                        //    if (postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.action.sections[0].rows[0].description!=null)
                        //    {
                        //        interactiveListModel.header=new InteractiveHeaderListModel() { type="image", image =new Interactiveimage() { link=postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.action.sections[0].rows[0].description } };
                        //    }


                        //}
                        interactiveListModel.footer = new InteractiveFooterListModel();
                        InteractiveFooterListModel interactiveFooterListModel = new InteractiveFooterListModel();
                        interactiveFooterListModel.text = postWhatsAppMessageModel.postWhatsAppInteractiveListModel.interactive.footer.text;
                        interactiveListModel.footer = interactiveFooterListModel;
                    }
                }
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
                    interactiveRowListModel.description ="";// objrow.description;
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
            result = JsonConvert.SerializeObject(postWhatsAppInteractiveModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            return result;
        }
        public string PrepareMessageTemplate(PostWhatsAppMessageModel postWhatsAppMessageModel)
        {
            string result = string.Empty;
            PostMessageTemplateModel postMessageTemplateModel = new PostMessageTemplateModel();
            WhatsAppTemplateModel whatsAppTemplateModel = new WhatsAppTemplateModel();
            WhatsAppLanguageModel whatsAppLanguageModel = new WhatsAppLanguageModel();
            postMessageTemplateModel.messaging_product = "whatsapp";
            postMessageTemplateModel.type = "template";
            postMessageTemplateModel.to = postWhatsAppMessageModel.to;
            whatsAppTemplateModel.name = postWhatsAppMessageModel.postMessageTemplateModel.template.name;
            whatsAppLanguageModel.code = postWhatsAppMessageModel.postMessageTemplateModel.template.language.code;
            whatsAppTemplateModel.language = whatsAppLanguageModel;
            postMessageTemplateModel.template = whatsAppTemplateModel;

            result = JsonConvert.SerializeObject(postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            return result;
        }
        private static async Task<WhatsAppAttachmentModel> RetrievingMediaAsync(string mediaId, string fbToken)
        {

            WhatsAppAttachmentModel attachmentModel = new WhatsAppAttachmentModel();


            using (var httpClient = new HttpClient())
            {


                var FBUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + mediaId;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fbToken);

                using (var response = await httpClient.GetAsync(FBUrl))
                {
                    using (var content = response.Content)
                    {
                        // WhatsAppMediaResponse whatsAppMediaResponse = new WhatsAppMediaResponse();

                        attachmentModel  = JsonConvert.DeserializeObject<WhatsAppAttachmentModel>(await content.ReadAsStringAsync());
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



            var client = new RestClient(mediaurl);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Authorization", "Bearer "+ fbToken);
            IRestResponse response = client.Execute(request);
            var ssss = response.Content;
            attachmentModel.contentByte= response.RawBytes;
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
