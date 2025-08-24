using BotService.Interfaces;
using BotService.Models;
using BotService.Models.API;
using BotService.Services;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Xml.Linq;
using Attachment = Microsoft.Bot.Connector.DirectLine.Attachment;

namespace BotService.Controllers
{
    public class BotServiceController : Controller
    {

        public IBotApis _botApis;
        public BotServiceController(IBotApis botApis)
        {

            _botApis=botApis;
        }

        // https://localhost:44323/BotService/BotMessageHandler 
        // { "ConversationsCount": 14, "DeliveryOrder": 0, "TakeAwayOrder": 0, "TotalOrder": 0, "loyalityPoint": 0, "ContactID": "40793", "IsSelectedPage": false, "IsComplaint": true, "IsBotCloseChat": false, "IsBotChat": true, "IsSupport": false, "IsNewContact": true, "IsBlock": false, "customerChat": { "lastNotificationsData": "0001-01-01T00:00:00", "notificationsText": null, "notificationID": null, "UnreadMessagesCount": 0, "TenantId": 27, "messageId": "5e72b3fe-17b7-428b-95fe-783bf83a3444", "userId": "27_962779746365", "SunshineConversationId": "6c87ba14aa01707d598b28bec4838ea3", "CreateDate": "2023-01-16T10:08:08.5850328+00:00", "type": "text", "text": "31.955986264757136, 35.86070085", "status": 1, "fileName": null, "sender": 2, "mediaUrl": null, "agentName": "", "agentId": "1000000", "ItemType": 1, "id": null, "_rid": null, "_self": null, "_etag": null, "_attachments": null, "_ts": 0 }, "blockByAgentName": null, "isBlockCustomer": false, "lastNotificationsData": "2023-01-02T13:49:47.8086649+00:00", "notificationsText": "Closed By :admin1", "notificationID": "86df4268-84fa-4ea5-9e67-cac0473f56ad", "UnreadMessagesCount": 1, "agentId": 88, "userId": "27_962779746365", "avatarUrl": "", "displayName": "Hasan", "phoneNumber": "962779746365", "type": "text", "MicrosoftBotId": null, "ConversationId": "6c87ba14aa01707d598b28bec4838ea3", "D360Key": "101366392639014", "SunshineAppID": null, "CreateDate": "2022-12-08T07:02:20.1935479+00:00", "ModifyDate": null, "LastMessageData": "2023-01-16T10:08:07.5860978+00:00", "LastConversationStartDateTime": "2023-01-16T09:36:47.5606266+00:00", "LastMessageText": null, "IsLockedByAgent": false, "IsConversationExpired": false, "LockedByAgentName": "admin1", "ItemType": 0, "CustomerStatus": 1, "CustomerStatusID": 1, "CustomerChatStatus": 1, "CustomerChatStatusID": 1, "IsOpen": false, "Website": null, "EmailAddress": null, "Description": "Unnamed Road - Al Sahel - Wadi As-Seir - Amman - Jordan", "IsNew": true, "TenantId": 27, "LiveChatStatus": 1, "LiveChatStatusName": "Done", "IsliveChat": true, "requestedLiveChatTime": "2022-12-20T09:22:23.7398689+00:00", "OpenTime": "2022-12-13T10:10:07.1292153+00:00", "CloseTime": "2022-12-13T10:10:09.1446447+00:00", "Department": null, "id": "efb0e77d-aaed-4239-bc37-74753c8118f3", "_rid": "Pic5AKzdR4oBIAIAAAAAAA==", "_self": "dbs/Pic5AA==/colls/Pic5AKzdR4o=/docs/Pic5AKzdR4oBIAIAAAAAAA==/", "_etag": "\"7f00a4cd-0000-0d00-0000-63c5220a0000\"", "_attachments": "attachments/", "CustomerStepModel": { "LangId": 1, "LangString": "ar", "ChatStepId": 1, "ChatStepPervoiusId": 0, "ChatStepNextId": 1, "ChatStepLevelId": 1, "ChatStepLevelPreviousId": 0, "SelectedAreaId": 0, "OrderId": 0, "OrderNumber": 0, "OrderTypeId": "Delivery___0b_000_000_111_000", "OrderTotal": 0, "OrderDeliveryCost": 0, "PageNumber": 0, "Address": "Ibn Mudaa 113 - Al Sahel - Wadi As-Seir - Amman - Jordan", "DeliveryCostAfter": 0, "DeliveryCostBefor": 0, "isOrderOfferCost": false, "LocationId": 0, "LocationAreaName": "", "AddressLatLong": "31.957677841187,35.855850219727", "IsLinkMneuStep": false, "IsNotSupportLocation": false, "Discount": 0, "BotCancelOrderId": 0, "OrderCreationTime": "0001-01-01T00:00:00" }, "_ts": 1673863690 }

        [HttpPost]
        public List<Activity> BotMessageHandler([FromBody] CustomerModel model)
        {
            var TempName = "Temp1";
            List<Activity> Bot = new List<Activity>();
            switch (TempName)
            {
                case "Temp1":
                    Bot= Temp1(model);
                    break;
                case "Temp2":
                    break;
                case "Temp3":
                    break;
                case "Temp4":
                    break;
            }
            _botApis.UpdateCustomer(model);//update  Customer
            return Bot;
        }


        [HttpGet]
        public List<Activity> BotStart(int TenantId,string CustomerPhoneNumber,string text)
        {
            var Customer = _botApis.GetCustomer(TenantId+"_"+CustomerPhoneNumber);//Get  Customer
            Customer.customerChat.text=text;

            return BotMessageHandler(Customer);
          
        }

        #region Temp
        private List<Activity> Temp1(CustomerModel model)
        {
            List<Activity> Bot = new List<Activity>();



            if (model.customerChat.text=="#")
            {
                Bot= step1(model);
            }else if (model.customerChat.text=="Cancel"|| model.customerChat.text=="الغاء")
            {
                Bot= ChatStepTriggersCancel(model);
                model.CustomerStepModel.ChatStepId=6;
                model.CustomerStepModel.ChatStepPervoiusId=0;
            }
            else
            {
                switch (model.CustomerStepModel.ChatStepId)
                {
                    case 0:
                        Bot= step(model);
                        break;
                    case 1:
                        Bot= step1(model);
                        break;
                    case 2:
                        Bot=step2(model);
                        break;
                    case 3:
                        Bot= step3(model);
                        break;
                    case 4:
                        Bot= step4(model);
                        break;
                    case 5:
                        Bot= step5(model);
                        break;
                    case 6:
                        Bot= step6(model);
                        break;
                }

            }

           
            
            return Bot;
        }
        #endregion

        #region Step

        private List<Activity> step(CustomerModel model)
        {
            List<Activity> Bot = new List<Activity>();

            Bot= ChatLang(model);


            model.CustomerStepModel.ChatStepId=1;
            model.CustomerStepModel.ChatStepPervoiusId=0;

            model.CustomerStepModel.OrderId=0;
            model.CustomerStepModel.OrderTotal=0;
            model.CustomerStepModel.IsLinkMneuStep=false;
            model.CustomerStepModel.OrderTypeId="";
            model.CustomerStepModel.SelectedAreaId=0;
            model.CustomerStepModel.DeliveryCostAfter=0;
            model.CustomerStepModel.DeliveryCostBefor=0;
            model.CustomerStepModel.AddressLatLong="";
            model.CustomerStepModel.Address="";
            model.CustomerStepModel.Discount=0;
            model.CustomerStepModel.isOrderOfferCost=false;
            model.CustomerStepModel.OrderDeliveryCost=0;
            model.CustomerStepModel.PageNumber=0;
            //model.CustomerStepModel.pageSize=8;

            return Bot;
        }
        private List<Activity> step1(CustomerModel model)
        {
            List<Activity> Bot = new List<Activity>();

            
            if (model.customerChat.text=="العربية")
            {
                model.CustomerStepModel.LangId=1;
                model.CustomerStepModel.LangString="ar";
                Bot= ChatStart(model);
                model.CustomerStepModel.ChatStepId=2;
                model.CustomerStepModel.ChatStepPervoiusId=1;

            }
            else if(model.customerChat.text=="English")
            {
                model.CustomerStepModel.LangId=2;
                model.CustomerStepModel.LangString="en";
                Bot= ChatStart(model);
                model.CustomerStepModel.ChatStepId=2;
                model.CustomerStepModel.ChatStepPervoiusId=1;
            }
            else
            {
                Bot= step(model);

            }
         

            model.CustomerStepModel.OrderId=0;
            model.CustomerStepModel.OrderTotal=0;
            model.CustomerStepModel.IsLinkMneuStep=false;
            model.CustomerStepModel.OrderTypeId="";
            model.CustomerStepModel.SelectedAreaId=0;
            model.CustomerStepModel.DeliveryCostAfter=0;
            model.CustomerStepModel.DeliveryCostBefor=0;
            model.CustomerStepModel.AddressLatLong="";
            model.CustomerStepModel.Address="";
            model.CustomerStepModel.Discount=0;
            model.CustomerStepModel.isOrderOfferCost=false;
            model.CustomerStepModel.OrderDeliveryCost=0;

            return Bot;
        }
        private List<Activity> step2(CustomerModel model)
        {
            List<Activity> Bot = new List<Activity>();

            if (model.CustomerStepModel.OrderTypeId!="")
            {
                switch (model.CustomerStepModel.OrderTypeId)
                {
                    case "Pickup":

                        Bot= ChatStepPickup(model);
                        model.CustomerStepModel.ChatStepId=3;
                        model.CustomerStepModel.ChatStepPervoiusId=2;
                        model.CustomerStepModel.OrderTypeId="Pickup";

                        break;

                    case "Delivery":
                        Bot= ChatStepDelivery(model);
                        model.CustomerStepModel.ChatStepId=3;
                        model.CustomerStepModel.ChatStepPervoiusId=2;
                        model.CustomerStepModel.OrderTypeId="Delivery";
                        break;
                }

            }
            else
            {
                if (model.customerChat.text=="استلام"|| model.customerChat.text=="Pickup")
                {
                    Bot= ChatStepPickup(model);
                    model.CustomerStepModel.ChatStepId=3;
                    model.CustomerStepModel.ChatStepPervoiusId=2;
                    model.CustomerStepModel.OrderTypeId="Pickup";

                }
                else if (model.customerChat.text=="توصيل" || model.customerChat.text=="Delivery")
                {
                    Bot= ChatStepDelivery(model);
                    model.CustomerStepModel.ChatStepId=3;
                    model.CustomerStepModel.ChatStepPervoiusId=2;
                    model.CustomerStepModel.OrderTypeId="Delivery";

                }
                else
                {
                    model.CustomerStepModel.ChatStepId=1;
                    model.CustomerStepModel.ChatStepPervoiusId=0;
                    model.CustomerStepModel.OrderTypeId="";
                    Bot= step1(model);
                }

            }

          

            return Bot;
        }
        private List<Activity> step3(CustomerModel model)
        {
            List<Activity> Bot = new List<Activity>();

            if (model.CustomerStepModel.IsLinkMneuStep)
            {
                Bot= ChatStepLink(model);
            }
            else
            {
                switch (model.CustomerStepModel.OrderTypeId)
                {
                    case "Pickup":
                        if (model.customerChat.text=="اخرى"||model.customerChat.text=="Others"||model.customerChat.text=="العودة"||model.customerChat.text=="Back")
                        {
                            Bot= step2(model);
                        }
                        else
                        {
                            var Branches = _botApis.GetBranch(model.TenantId, model.customerChat.text, model.CustomerStepModel.LangString);

                            if (Branches.AreaName== model.customerChat.text || Branches.AreaNameEnglish== model.customerChat.text)
                            {
                                model.CustomerStepModel.SelectedAreaId=Branches.Id;
                                model.CustomerStepModel.IsLinkMneuStep=true;

                                Bot= ChatStepLink(model);
                                model.CustomerStepModel.ChatStepId=4;
                                model.CustomerStepModel.ChatStepPervoiusId=3;
                              
                            }
                            else
                            {
                                Bot= step2(model);
                            }
                        }
                                     
                        break;

                    case "Delivery":

                        SendLocationUserModel input = new SendLocationUserModel
                        {
                            query=model.customerChat.text,
                             tenantID=model.TenantId.Value

                        };

                        var location = _botApis.GetlocationUserModel(input);

                        if (location.DeliveryCostBefor!=-1)
                        {
                            model.CustomerStepModel.LocationId=location.LocationId;
                            model.CustomerStepModel.IsLinkMneuStep=true;

                            model.CustomerStepModel.DeliveryCostAfter=location.DeliveryCostAfter;
                            model.CustomerStepModel.DeliveryCostBefor=location.DeliveryCostBefor;

                            model.CustomerStepModel.AddressLatLong=model.customerChat.text;
                            model.CustomerStepModel.Address=location.Address;

                            Bot= ChatStepLink(model);
                            model.CustomerStepModel.ChatStepId=4;
                            model.CustomerStepModel.ChatStepPervoiusId=3;
                          
                        }
                        else
                        {
                            Bot= step2(model);
                        }
                        break;

                    default:

                        Bot= step2(model);
                        break;

                }

            }



            return Bot;
        }
        private List<Activity> step4(CustomerModel model)
        {

            List<Activity> Bot = new List<Activity>();
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);


            if (model.customerChat.text=="اختبار" && model.CustomerStepModel.IsLinkMneuStep)
            {
                SendOrderAndDetailModel sendOrderAndDetailModel = new SendOrderAndDetailModel();
                sendOrderAndDetailModel.DeliveryCostTextTow=caption.Where(x => x.TextResourceId==10).FirstOrDefault().Text;//10
                sendOrderAndDetailModel.captionQuantityText=caption.Where(x => x.TextResourceId==18).FirstOrDefault().Text;//18
                sendOrderAndDetailModel.captionAddtionText=caption.Where(x => x.TextResourceId==19).FirstOrDefault().Text;//19
                sendOrderAndDetailModel.captionTotalText=caption.Where(x => x.TextResourceId==20).FirstOrDefault().Text;//20
                sendOrderAndDetailModel.captionTotalOfAllText=caption.Where(x => x.TextResourceId==21).FirstOrDefault().Text;//21

                if (model.CustomerStepModel.OrderTypeId=="Pickup")
                {
                    sendOrderAndDetailModel.ContactId=int.Parse(model.ContactID);
                    sendOrderAndDetailModel.TenantID=model.TenantId.Value;
                    sendOrderAndDetailModel.lang=model.CustomerStepModel.LangString;
                    sendOrderAndDetailModel.MenuType=model.CustomerStepModel.SelectedAreaId.ToString();
                    sendOrderAndDetailModel.LocationId=(int)model.CustomerStepModel.SelectedAreaId;

                    sendOrderAndDetailModel.isOrderOffer=false;

                }
                else
                {

                    sendOrderAndDetailModel.ContactId=int.Parse(model.ContactID);
                    sendOrderAndDetailModel.TenantID=model.TenantId.Value;
                    sendOrderAndDetailModel.lang=model.CustomerStepModel.LangString;
                    sendOrderAndDetailModel.MenuType=model.CustomerStepModel.SelectedAreaId.ToString();
                    sendOrderAndDetailModel.LocationId=(int)model.CustomerStepModel.SelectedAreaId;

                    sendOrderAndDetailModel.isOrderOffer=false;

                    sendOrderAndDetailModel.TypeChoes=model.CustomerStepModel.OrderTypeId;
                    sendOrderAndDetailModel.deliveryCostBefor=model.CustomerStepModel.DeliveryCostBefor;
                    sendOrderAndDetailModel.deliveryCostAfter=model.CustomerStepModel.DeliveryCostAfter;
                    sendOrderAndDetailModel.Cost=model.CustomerStepModel.DeliveryCostAfter;

                    sendOrderAndDetailModel.LocationInfo=null;

                }
                var Details = _botApis.GetOrderAndDetails(sendOrderAndDetailModel);

                Bot= ChatStepOrderDetails(model, Details.DetailText);
                model.CustomerStepModel.ChatStepId=5;
                model.CustomerStepModel.ChatStepPervoiusId=4;
                model.CustomerStepModel.IsLinkMneuStep=true;

                model.CustomerStepModel.OrderId=Details.orderId;
                model.CustomerStepModel.OrderTotal=Details.total.Value;
                model.CustomerStepModel.Discount=Details.Discount;
            }
            else
            {
                Bot= step3(model);
            }

            return Bot;
        }
        private List<Activity> step5(CustomerModel model)
        {


            List<Activity> Bot = new List<Activity>();

            if (model.customerChat.text=="تاكيد" ||model.customerChat.text=="Confirm")
            {
               
                Bot= ChatStepCreateOrder(model);

                model.CustomerStepModel.ChatStepId=1;
                model.CustomerStepModel.ChatStepPervoiusId=0;
                model.CustomerStepModel.OrderId=0;
                model.CustomerStepModel.OrderTotal=0;
                model.CustomerStepModel.IsLinkMneuStep=false;
                model.CustomerStepModel.OrderTypeId="";
                model.CustomerStepModel.SelectedAreaId=0;
                model.CustomerStepModel.DeliveryCostAfter=0;
                model.CustomerStepModel.DeliveryCostBefor=0;
                model.CustomerStepModel.AddressLatLong="";
                model.CustomerStepModel.Address="";
                model.CustomerStepModel.Discount=0;
                model.CustomerStepModel.isOrderOfferCost=false;
                model.CustomerStepModel.OrderDeliveryCost=0;
                model.CustomerStepModel.PageNumber=0;
            }
            else if (model.customerChat.text=="الغاء الطلب" || model.customerChat.text=="Cancelling Order")
            {
                Bot= ChatStepCancelOrder(model,true);

            }
            else
            {
                model.customerChat.text="اختبار";
                Bot = step4(model);
            }


            return Bot;
        }
        private List<Activity> step6(CustomerModel model)
        {


            List<Activity> Bot = new List<Activity>();
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);
            var text = caption.Where(x => x.TextResourceId==17).FirstOrDefault().Text;
         
            var Details = _botApis.UpdateCancelOrder(model.TenantId, model.customerChat.text,int.Parse(model.ContactID), text);

            if (Details.IsTrueOrder)
            {
                Bot= ChatStepCancelOrder(model, !Details.IsTrueOrder);
            }
            else
            {
                Bot= ChatStepTriggersCancel(model);
            }


            return Bot;
        }
        #endregion

        #region ChatStep
        private List<Activity> ChatLang(CustomerModel model)
        {
            List<CardAction> cardActions = new List<CardAction>();
            cardActions.Add(new CardAction()
            {
                Title="العربية",
                Value="العربية"
            });
            cardActions.Add(new CardAction()
            {
                Title="English",
                Value="English"
            });
            List<Activity> activities = new List<Activity>();

            var text = "الرجاء اختيار اللغة - Please choose the language";
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary="لتغيير اللغة ارسل كلمة لغة",
                Locale =   "ar",
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStart(CustomerModel model)
        {
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);
            var text = caption.Where(x=>x.TextResourceId==14).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var Summary = "";
            
            if (model.CustomerStepModel.LangString=="ar")
            {
                 op1 = "توصيل";
                 op2 = "استلام";
                 Summary = "لتغيير اللغة ارسل كلمة لغة";
            }
            else
            {
                op1 = "Pickup";
                op2 = "Delivery";
                Summary = "To change the language, send a language word";
            }
            List<CardAction> cardActions = new List<CardAction>();
            cardActions.Add(new CardAction()
            {
                Title=op1,
                Value=op1
            });
            cardActions.Add(new CardAction()
            {
                Title=op2,
                Value=op2
            });
            List<Activity> activities = new List<Activity>();

            
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary=Summary,
                Locale =   model.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepDelivery(CustomerModel model)
        {
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);
            var text = caption.Where(x => x.TextResourceId==9).FirstOrDefault().Text;

          //  var text = "الرجاء ارسال المــوقـــع (ـLocation) 📌";//9
            var text2 = "";
            var Summary = "";
            if (model.CustomerStepModel.LangString=="ar")
            {
                text2 = "نحن لا ندعم التوصيل الى هذه المنطقه";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                text2 = "We do not support delivery to this region";
                Summary = "Please send # to return to the main menu";
            }

            List<Activity> activities = new List<Activity>();

            if (model.CustomerStepModel.DeliveryCostBefor==-1)
            {
                Activity Bot2 = new Activity
                {
                    From = new ChannelAccount(model.userId, model.displayName.Trim()),
                    Text = text2,
                    Speak= text2,
                    Summary= Summary,
                    Type = ActivityTypes.Message,
                    Locale =   model.CustomerStepModel.LangString,
                    Attachments = null
                };

                activities.Add(Bot2);
            }

           

            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak= text,
                Summary= Summary,
                Type = ActivityTypes.Message,
                Locale =   model.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepPickup(CustomerModel model)
        {
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);
            var text = caption.Where(x => x.TextResourceId==12).FirstOrDefault().Text;
           // var text = "يرجى اختيار الفرع:";//12
            var Summary = "";

            if (model.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
            }


            if (model.customerChat.text=="اخرى"||model.customerChat.text=="Others")
            {
                model.CustomerStepModel.PageNumber=model.CustomerStepModel.PageNumber+1;
            }
            if (model.customerChat.text=="العودة"||model.customerChat.text=="Back")
            {
                model.CustomerStepModel.PageNumber=model.CustomerStepModel.PageNumber-1;
            }

            List<CardAction> cardActions = new List<CardAction>();

            var Branches= _botApis.GetBranchesWithPage(model.TenantId, model.CustomerStepModel.LangString, model.CustomerStepModel.PageNumber, 8);

            foreach (var branch in Branches)
            {
                cardActions.Add(new CardAction()
                {
                    Title=branch,
                    Value=branch
                });
            }

            List<Activity> activities = new List<Activity>();
            List<Attachment> ss = new List<Attachment>();

           
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak=text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary=Summary,
                Locale =   model.CustomerStepModel.LangString,
                Attachments = ss
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepLink(CustomerModel model)
        {
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);
            var text = caption.Where(x => x.TextResourceId==2).FirstOrDefault().Text;
          //  var text = "الرجاء الضغط على الرابط\r\n\r\n👇👇👇👇👇👇\r\n *https://infoseedordersystem-stg.azurewebsites.net/Index1?TenantID={0}&ContactId={1}&PhoneNumber={2}&Menu={3}&LanguageBot={4}&lang={5}&OrderType={6}*\r\n👆👆👆👆👆👆";//2

            if (model.CustomerStepModel.OrderTypeId=="Pickup")
            {
                text=text.Replace("{3}", model.CustomerStepModel.SelectedAreaId.ToString()).Replace("{6}", "0");
            }
            else
            {
                text=text.Replace("{3}", model.CustomerStepModel.LocationId.ToString()).Replace("{6}", "1");
            }

            text=text.Replace("{0}", model.TenantId.Value.ToString()).Replace("{1}", model.ContactID).Replace("{2}", "").Replace("{4}", model.CustomerStepModel.LangId.ToString()).Replace("{5}", model.CustomerStepModel.LangString);
            MenuContcatKeyModel menuContcatKeyModel = new MenuContcatKeyModel
            {
                 ContactID=int.Parse(model.ContactID),
                  
                 Value=text
            };
            var link = _botApis.AddMenuContcatKey(menuContcatKeyModel);

            text=link;
            var Summary = "";
            if (model.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
            }
            List<Activity> activities = new List<Activity>();
           
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary=Summary,
                Locale =  model.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepOrderDetails(CustomerModel model,string DetailText)
        {
           


            var text = DetailText;
            var op1 = "";
            var op2 = "";
            var Summary = "";

            if (model.CustomerStepModel.LangString=="ar")
            {
                op1 = "تاكيد";
                op2 = "الغاء الطلب";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                op1 = "Confirm";
                op2 = "Cancelling Order";
                Summary = "Please send # to return to the main menu";
            }

            List<Activity> activities = new List<Activity>();
            List<CardAction> cardActions = new List<CardAction>();
            cardActions.Add(new CardAction()
            {
                Title=op1,
                Value=op1
            });
            cardActions.Add(new CardAction()
            {
                Title=op2,
                Value=op2
            });


           
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary=Summary,
                Locale =   model.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepCreateOrder(CustomerModel model)
        {
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);

            UpdateOrderModel updateOrderModel = new UpdateOrderModel();
            updateOrderModel.captionOrderInfoText=caption.Where(x => x.TextResourceId==22).FirstOrDefault().Text;//22
            updateOrderModel.captionOrderNumberText=caption.Where(x => x.TextResourceId==24).FirstOrDefault().Text;//24
            updateOrderModel.captionTotalOfAllOrderText=caption.Where(x => x.TextResourceId==21).FirstOrDefault().Text;//21
            updateOrderModel.captionEndOrderText=caption.Where(x => x.TextResourceId==25).FirstOrDefault().Text;//25
            updateOrderModel.aptionAreaNameText=caption.Where(x => x.TextResourceId==28).FirstOrDefault().Text;//28
            updateOrderModel.captionBranchCostText=caption.Where(x => x.TextResourceId==26).FirstOrDefault().Text;//26
            updateOrderModel.captionFromLocationText=caption.Where(x => x.TextResourceId==27).FirstOrDefault().Text;//27

            if (model.CustomerStepModel.OrderTypeId=="Pickup")
            {
                updateOrderModel.ContactId=int.Parse(model.ContactID);
                updateOrderModel.TenantID=model.TenantId.Value;
                updateOrderModel.OrderTotal=model.CustomerStepModel.OrderTotal;
                updateOrderModel.loyalityPoint=0;
                updateOrderModel.OrderId=model.CustomerStepModel.OrderId;

                updateOrderModel.MenuId=model.CustomerStepModel.SelectedAreaId;
                updateOrderModel.BranchId=(int)model.CustomerStepModel.SelectedAreaId;
                updateOrderModel.BranchName="";
                updateOrderModel.TypeChoes=model.CustomerStepModel.OrderTypeId;
                updateOrderModel.IsSpecialRequest=false;
                updateOrderModel.SpecialRequest="";
                updateOrderModel.BotLocal=model.CustomerStepModel.LangString;
            
                updateOrderModel.IsItemOffer=false;
                updateOrderModel.ItemOffer=0;
                updateOrderModel.IsDeliveryOffer=false;
                updateOrderModel.DeliveryOffer=0;

            }
            else
            {
                updateOrderModel.ContactId=int.Parse(model.ContactID);
                updateOrderModel.TenantID=model.TenantId.Value;
                updateOrderModel.OrderTotal=model.CustomerStepModel.OrderTotal;
                updateOrderModel.loyalityPoint=0;
                updateOrderModel.OrderId=model.CustomerStepModel.OrderId;

                updateOrderModel.MenuId=model.CustomerStepModel.LocationId;
                updateOrderModel.BranchId=(int)model.CustomerStepModel.SelectedAreaId;
                updateOrderModel.BranchName="";
                updateOrderModel.TypeChoes=model.CustomerStepModel.OrderTypeId;
                updateOrderModel.IsSpecialRequest=false;
                updateOrderModel.SpecialRequest="";
                updateOrderModel.BotLocal=model.CustomerStepModel.LangString;

                updateOrderModel.IsItemOffer=false;
                updateOrderModel.ItemOffer=0;
                updateOrderModel.IsDeliveryOffer=false;
                updateOrderModel.DeliveryOffer=0;


                updateOrderModel.isOrderOfferCost=false;
                updateOrderModel.Address=model.CustomerStepModel.Address;
                updateOrderModel.DeliveryCostAfter=model.CustomerStepModel.DeliveryCostAfter.Value;
                updateOrderModel.DeliveryCostBefor=model.CustomerStepModel.DeliveryCostBefor.Value;
                updateOrderModel.IsPreOrder=false;
                updateOrderModel.SelectDay="";
                updateOrderModel.SelectTime="";
                updateOrderModel.LocationFrom=model.CustomerStepModel.AddressLatLong;
                updateOrderModel.BuyType="";
                updateOrderModel.IsItemOffer=false;
                updateOrderModel.ItemOffer=0;
                updateOrderModel.IsDeliveryOffer=false;
                updateOrderModel.DeliveryOffer=0;


            }

            var text = _botApis.UpdateOrder(updateOrderModel);
            var Summary = "";
            if (model.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
            }
            List<Activity> activities = new List<Activity>();
            
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary=Summary,
                Locale =   model.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepCancelOrder(CustomerModel model,bool isDelete)
        {
            if (isDelete)
            {
                _botApis.DeleteOrderDraft(model.TenantId.Value, model.CustomerStepModel.OrderId);
            }
          
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);

            var text = caption.Where(x => x.TextResourceId==11).FirstOrDefault().Text;
          //  var text = "تم الغاء طلبك يمكنك الطلب من جديد";//11
            var Summary = "";
            if (model.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
            }

            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary = Summary,
                Locale =   model.CustomerStepModel.LangString,
                Attachments = null
            };

            var listbot = step1(model);
            listbot.Add(Bot);
            return listbot;
        }

        #endregion


        #region ChatStepTriggers
        private List<Activity> ChatStepTriggersCancel(CustomerModel model)
        {
            var caption = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);

            var text = caption.Where(x => x.TextResourceId==3).FirstOrDefault().Text;
          //  var text = "الرجاء ادخال رقم الطلب ";//5
            var Summary = "";
            if (model.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
            }

            List<Activity> activities = new List<Activity>();
            
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.userId, model.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary= Summary,
                Locale =   model.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        #endregion
    }
}
