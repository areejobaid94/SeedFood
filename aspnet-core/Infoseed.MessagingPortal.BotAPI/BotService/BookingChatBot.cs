using Abp.Runtime.Caching;
using Framework.Data;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.BotAPI.Interfaces;
using Infoseed.MessagingPortal.BotAPI.Models;
using Infoseed.MessagingPortal.BotAPI.Models.BotModel;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Nest;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Constants;
using static Infoseed.MessagingPortal.Customers.Dtos.CustomerBehaviourEnums;
using Attachment = Microsoft.Bot.Connector.DirectLine.Attachment;

namespace Infoseed.MessagingPortal.BotAPI.BotService
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingChatBot : MessagingPortalControllerBase
    {
        private readonly IDocumentClient _IDocumentClient;
        private readonly IDBService _dbService;
        private readonly ICacheManager _cacheManager;
        public IBotApis _botApis;
        public static Dictionary<string, bool> MessagesSent { get; set; }
        public IWhatsAppMessageTemplateAppService _whatsAppMessageTemplateAppService;
        public BookingChatBot(ICacheManager cacheManager, IDBService dbService, IDocumentClient IDocumentClient, IBotApis botApis, IWhatsAppMessageTemplateAppService whatsAppMessageTemplateAppService)
        {

            _cacheManager=cacheManager;
            _dbService=dbService;
            _IDocumentClient=IDocumentClient;
            _botApis=botApis;
            _whatsAppMessageTemplateAppService=whatsAppMessageTemplateAppService;
        }
        [Route("DeleteCacheBooking")]
        [HttpGet]
        public string DeleteCacheBooking(int TenantId)
        {
            var model = _botApis.GetTenantAsync(TenantId).Result;
            _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + TenantId.ToString());
            _cacheManager.GetCache("CacheTenant").Remove(model.D360Key);

            return "Done";

        }
        [Route("BotStartBooking")]
        [HttpGet]
        public List<Activity> BotStartBooking(int TenantId, string CustomerPhoneNumber, string text)
        {
            var Customer = GetCustomer(TenantId+"_"+CustomerPhoneNumber);//Get  Customer
            Customer.customerChat.text=text;
            return BookingMessageHandler(Customer);

            //SocketIOManager.SendChat(Customer, TenantId);
            //List<Activity> Bot = new List<Activity>();
            //return Bot;

        }

        [Route("BookingMessageHandler")]
        [HttpPost]
        public List<Activity> BookingMessageHandler(CustomerModel model)
        {

            BotStepModel botStepModel = new BotStepModel();
            List<Activity> Bot = new List<Activity>();

            TenantModel Tenant = new TenantModel();
            List<CaptionDto> captionDtos = new List<CaptionDto>();

            CacheFun(model, out Tenant, out captionDtos);

            botStepModel.customerModel=model;
            botStepModel.captionDtos=captionDtos;
            botStepModel.tenantModel=Tenant;







            if (MessagesSent == null)
                MessagesSent = new Dictionary<string, bool>();

            MessagesSent.TryAdd(model.userId, false);


            if (!MessagesSent[model.userId])
            {
                try
                {
                    MessagesSent[model.userId] = true;

                    if (botStepModel.customerModel.BotBookingParmeterModel==null)
                    {
                        botStepModel.customerModel.BotBookingParmeterModel=new BotBookingParmeterModel() { };

                    }



                    Bot=TriggersFun(botStepModel, Bot);
                    botStepModel.Bot=Bot;
                    if (Bot.Count>0)
                    {

                    }
                    else
                    {
                        Bot = Temp1(botStepModel);

                    }



                    botStepModel.Bot=Bot;
                    UpdateCustomer(botStepModel);//update  Customer
                    var x = BotSendToWhatsApp(botStepModel);

                    MessagesSent[model.userId] = false;

                }
                catch
                {
                    MessagesSent[model.userId] = false;
                }
              

            }

            return Bot;
        }


        #region Temp
        private List<Activity> Temp1(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();

            if (model.customerModel.CustomerStepModel.IsLiveChat)
            {
                return Bot;

            }
            if (model.customerBehaviourModel!=null)
            {
                if (model.customerBehaviourModel.Start)
                {
                    model.customerBehaviourModel.CustomerOPt = (int)CustomerBehaviourOptEnum.OptIn;
                }
                else if (model.customerBehaviourModel.Stop)
                {
                    model.customerBehaviourModel.CustomerOPt = (int)CustomerBehaviourOptEnum.OptOut;
                }
                model.customerModel.CustomerOPT=model.customerBehaviourModel.CustomerOPt;
                return model.Bot;
            }
            switch (model.customerModel.CustomerStepModel.ChatStepId)
            {
                case 0:
                    if (model.tenantModel.IsBotLanguageAr&&model.tenantModel.IsBotLanguageEn)
                    {

                        Bot= step(model);
                    }
                    else
                    {
                        model.customerModel.CustomerStepModel.ChatStepId=model.customerModel.CustomerStepModel.ChatStepId+1;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=model.customerModel.CustomerStepModel.ChatStepPervoiusId+1;
                        if (model.tenantModel.IsBotLanguageAr)
                        {
                            model.customerModel.CustomerStepModel.LangId=1;
                            model.customerModel.CustomerStepModel.LangString="ar";
                            model.customerModel.customerChat.text="العربية";
                        }
                        if (model.tenantModel.IsBotLanguageEn)
                        {
                            model.customerModel.CustomerStepModel.LangId=2;
                            model.customerModel.CustomerStepModel.LangString="en";
                            model.customerModel.customerChat.text="English";
                        }
                        Bot=step1(model);
                    }
                    break;
                case 1:
                    Bot= step1(model);
                    break;
                case 2:
                    Bot= step2(model);
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
                case 7:
                    Bot= step7(model);
                    break;
                case 8:
                    Bot= step8(model);
                    break;
                case 9:
                    Bot= step9(model);
                    break;
                case 10:
                    Bot= step10(model);
                    break;
                case 11:
                    Bot= step11(model);
                    break;
                case 12:
                    Bot= step12(model);
                    break;
                case 13:
                    Bot= step13(model);
                    break;
                case 14:
                    Bot= step14(model);
                    break;
                case 15:
                    Bot= step15(model);
                    break;
                case 16:
                    Bot= step16(model);
                    break;
                case 17:
                    Bot= step17(model);
                    break;
                case 18:
                    Bot= step18(model);
                    break;
                case 19:
                    Bot= step19(model);
                    break;
                case 20:
                    Bot= step20(model);
                    break;


            }



            return Bot;
        }
        #endregion


        #region Step

        private List<Activity> step(BotStepModel model)
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
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary="الرجاء ارسال # للعودة للقائمة الرئسية",
                Locale =   "ar",
                Attachments = null
            };
            activities.Add(Bot);


            model.customerModel.CustomerStepModel.ChatStepId=1;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

            model.customerModel.CustomerStepModel.OrderId=0;
            model.customerModel.CustomerStepModel.OrderTotal=0;
            model.customerModel.CustomerStepModel.IsLinkMneuStep=false;
            model.customerModel.CustomerStepModel.OrderTypeId="";
            model.customerModel.CustomerStepModel.SelectedAreaId=0;
            model.customerModel.CustomerStepModel.DeliveryCostAfter=0;
            model.customerModel.CustomerStepModel.DeliveryCostBefor=0;
            model.customerModel.CustomerStepModel.AddressLatLong="";
            model.customerModel.CustomerStepModel.Address="";
            model.customerModel.CustomerStepModel.Discount=0;
            model.customerModel.CustomerStepModel.isOrderOfferCost=false;
            model.customerModel.CustomerStepModel.OrderDeliveryCost=0;
            model.customerModel.CustomerStepModel.PageNumber=0;

            return activities;
        }
        private List<Activity> step1(BotStepModel model)
        {

            if (model.customerModel.customerChat.text=="العربية")
            {
                model.customerModel.CustomerStepModel.LangId=1;
                model.customerModel.CustomerStepModel.LangString="ar";

            }
            else if (model.customerModel.customerChat.text=="English")
            {
                model.customerModel.CustomerStepModel.LangId=2;
                model.customerModel.CustomerStepModel.LangString="en";
            }
            else
            {
                model.customerModel.CustomerStepModel.ChatStepId=0;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                return step(model);
            }


            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1257 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var op3 = "";
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                op1 = "حجز موعد🗓️";
                op2 = "الغاء موعد❌";
                op3 = "للاستفسار❓";
                if (model.tenantModel.IsBotLanguageAr&&model.tenantModel.IsBotLanguageEn)
                {
                    Summary = "لتغيير اللغة ، أرسل (اللغة)";
                }
                else
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }

                InputHint="الرجاء الاختيار";
            }
            else
            {
                op1 = "Book Appointment🗓️";
                op2 = "Cancel Appointment❌";
                op3 = "For Inquiries❓";
                if (model.tenantModel.IsBotLanguageAr&&model.tenantModel.IsBotLanguageEn)
                {
                    Summary = "To change the language, send a (language)";

                }
                else
                {
                    Summary = "Please send # re-main";

                }
                InputHint="Please Select";
            }


            List<CardAction> cardActions = new List<CardAction>();
            cardActions.Add(new CardAction()
            {
                Title=op1,
                Value=op1,
                Image=model.tenantModel.Image
            });

            cardActions.Add(new CardAction()
            {
                Title=op2,
                Value=op2,
                Image=model.tenantModel.Image
            });

            if (model.tenantModel.BotTemplateId.Value==2)
            {

                cardActions.Add(new CardAction()
                {
                    Title=op3,
                    Value=op3,
                    Image=model.tenantModel.Image
                });
            }


            List<Activity> activities = new List<Activity>();



            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions, },
                Summary=Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);




            model.customerModel.CustomerStepModel.ChatStepId=2;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=1;

            model.customerModel.CustomerStepModel.OrderId=0;
            model.customerModel.CustomerStepModel.OrderTotal=0;
            model.customerModel.CustomerStepModel.IsLinkMneuStep=false;
            model.customerModel.CustomerStepModel.OrderTypeId="";
            model.customerModel.CustomerStepModel.SelectedAreaId=0;
            model.customerModel.CustomerStepModel.DeliveryCostAfter=0;
            model.customerModel.CustomerStepModel.DeliveryCostBefor=0;
            model.customerModel.CustomerStepModel.AddressLatLong="";
            model.customerModel.CustomerStepModel.Address="";
            model.customerModel.CustomerStepModel.Discount=0;
            model.customerModel.CustomerStepModel.isOrderOfferCost=false;
            model.customerModel.CustomerStepModel.OrderDeliveryCost=0;

            return activities;
        }
        private List<Activity> step2(BotStepModel model)
        {

            List<Activity> activities = new List<Activity>();

            if (model.customerModel.customerChat.text.Trim()=="حجز موعد🗓️" || model.customerModel.customerChat.text.Trim()=="Book Appointment🗓️")
            {
              
                activities= step3(model);

            }
            else if (model.customerModel.customerChat.text.Trim()=="الغاء موعد❌" || model.customerModel.customerChat.text.Trim()=="Cancel Appointment❌")
            {
                activities= step18(model);

            }
            else if (model.customerModel.customerChat.text.Trim()=="للاستفسار❓" || model.customerModel.customerChat.text.Trim()=="For Inquiries❓")
            {
                activities= step3(model);

                model.customerModel.CustomerStepModel.ChatStepId=13;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

            }
            else
            {

                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {

                    model.customerModel.customerChat.text="العربية";
                }
                else
                {
                    model.customerModel.customerChat.text="English";
                }
                model.customerModel.CustomerStepModel.ChatStepId=2;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                activities=step1(model);
            
            }

;

            return activities;
        }
        private List<Activity> step3(BotStepModel model)
        {

            List<Activity> activities = new List<Activity>();

            var caption = model.captionDtos;
           // var text = caption.Where(x => x.TextResourceId==12 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
             var text = "يرجى اختيار:";//12
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "يرجى اختيار:";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text = "Please Select";
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }


            if (model.customerModel.customerChat.text=="اخرى"||model.customerModel.customerChat.text=="Others")
            {
                model.customerModel.CustomerStepModel.PageNumber=model.customerModel.CustomerStepModel.PageNumber+1;
            }
            if (model.customerModel.customerChat.text=="العودة"||model.customerModel.customerChat.text=="Back")
            {
                model.customerModel.CustomerStepModel.PageNumber=model.customerModel.CustomerStepModel.PageNumber-1;
            }

            List<CardAction> cardActions = new List<CardAction>();

            var Branches = _botApis.GetAreasWithPage(model.tenantModel.TenantId.Value.ToString(), model.customerModel.CustomerStepModel.LangString, 0, model.customerModel.CustomerStepModel.PageNumber, 8, false);

            foreach (var branch in Branches)
            {
                cardActions.Add(new CardAction()
                {
                    Title=branch,
                    Value=branch
                });
            }
            List<Attachment> ss = new List<Attachment>();

            Activity Bot = new Activity();

            if (Branches.Count()==1)
            {
                model.customerModel.CustomerStepModel.ChatStepId=4;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;
                model.customerModel.customerChat.text=Branches.FirstOrDefault();
                return step4(model);
            }
            else
            {


                if (Branches.Count()<=3)
                {

                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text,
                        Speak=text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = null,


                    };

                }
                else
                {
                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text,
                        Speak=text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = ss,
                        InputHint=InputHint


                    };

                }

                activities.Add(Bot);



            }
            model.customerModel.CustomerStepModel.ChatStepId=4;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;
            return activities;
        }

        private List<Activity> step4(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();

            if (model.customerModel.customerChat.text=="اخرى"||model.customerModel.customerChat.text=="Others"||model.customerModel.customerChat.text=="العودة"||model.customerModel.customerChat.text=="Back")
            {
                model.customerModel.CustomerStepModel.ChatStepId=3;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                return step3(model);
                
            }
            else
            {
                var Branches = _botApis.GetAreasID2(model.tenantModel.TenantId.Value.ToString(), model.customerModel.customerChat.text, 0, model.customerModel.CustomerStepModel.LangString);

                if (Branches!=null)
                {
                    if (Branches.AreaName!=null)
                    {
                        if (Branches.AreaName.Trim()== model.customerModel.customerChat.text.Trim() || Branches.AreaNameEnglish.Trim()== model.customerModel.customerChat.text.Trim())
                        {
                            model.customerModel.CustomerStepModel.ChatStepId=5;
                            model.customerModel.CustomerStepModel.ChatStepPervoiusId=4;

                            model.customerModel.BotBookingParmeterModel.AreaId=Branches.Id;
                            model.customerModel.BotBookingParmeterModel.UserIds=Branches.UserIds;


                            return step5(model);

                        }
                        else
                        {
                            return step3(model);
                           // model.customerModel.CustomerStepModel.ChatStepId=3;
                            //model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                        }

                    }
                    else
                    {
                        return step3(model);
                        //model.customerModel.CustomerStepModel.ChatStepId=3;
                       // model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                    }

                }
                else
                {
                    return step3(model);
                    //model.customerModel.CustomerStepModel.ChatStepId=3;
                   // model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                }

            }

            return activities;
        }
        private List<Activity> step5(BotStepModel model)
        {


            List<Activity> activities = new List<Activity>();

            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1265 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            // var text = "يرجى اختيار الطبيبب:";//12
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }



            List<CardAction> cardActions = new List<CardAction>();

            var Doctor = _botApis.GetUserByBranches(model.tenantModel.TenantId.Value, model.customerModel.BotBookingParmeterModel.UserIds).Result;

            foreach (var doct in Doctor)
            {
                cardActions.Add(new CardAction()
                {
                    Title=doct,
                    Value=doct
                });
            }
            List<Attachment> ss = new List<Attachment>();

            Activity Bot = new Activity();

            if (Doctor.Count()==1)
            {
                model.customerModel.CustomerStepModel.ChatStepId=6;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=5;
                model.customerModel.customerChat.text=Doctor.FirstOrDefault();
                return step6(model);
            }
            else
            {


                if (Doctor.Count()<=3)
                {

                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text,
                        Speak=text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = null,


                    };

                }
                else
                {
                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text,
                        Speak=text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = ss,
                        InputHint=InputHint


                    };

                }

                activities.Add(Bot);



            }



            model.customerModel.CustomerStepModel.ChatStepId=6;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=5;

            return activities;
        }

        private List<Activity> step6(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();


            var Doctor = _botApis.GetUserByBranches(model.tenantModel.TenantId.Value, model.customerModel.BotBookingParmeterModel.UserIds).Result;



            if (Doctor.Contains(model.customerModel.customerChat.text.Trim()))
            {
                model.customerModel.BotBookingParmeterModel.selectDoctor=model.customerModel.customerChat.text.Trim();

                return  step7(model);
                //model.customerModel.CustomerStepModel.ChatStepId=7;
              //  model.customerModel.CustomerStepModel.ChatStepPervoiusId=6;
            }
            else
            {
                return step5(model);
              //  model.customerModel.CustomerStepModel.ChatStepId=5;
               // model.customerModel.CustomerStepModel.ChatStepPervoiusId=4;
            }



            return activities;
        }

        private List<Activity> step7(BotStepModel model)
        {



            List<Activity> activities = new List<Activity>();
            var caption = model.captionDtos;
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }
            if (model.isFall)
            {
                var text = caption.Where(x => x.TextResourceId==3001 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;

                Activity Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak= text,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };

                activities.Add(Bot);
                model.isFall=false;

            }
            var Doctor = _botApis.GetUserModelByBranches(model.tenantModel.TenantId.Value, model.customerModel.BotBookingParmeterModel.selectDoctor).Result;
            model.customerModel.BotBookingParmeterModel.selectDoctorId=Doctor.Id;
            var dyas = _botApis.GetBookingDay(Doctor.Id, model.customerModel.CustomerStepModel.LangId);

            if (dyas.Count==0)
            {
               
                var text = caption.Where(x => x.TextResourceId==3001 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
               

                Activity Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak= text,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot);
                activities = step5(model);
            }
            else
            {
                List<CardAction> cardActions = new List<CardAction>();
                foreach (var doy in dyas)
                {
                    cardActions.Add(new CardAction()
                    {
                        Title=doy.Key,
                        Value=doy.Value
                    });
                }
                List<Attachment> ss = new List<Attachment>();

                Activity Bot = new Activity();
                var text2 = caption.Where(x => x.TextResourceId==1262 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;

                if (dyas.Count()<=3)
                {

                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text2,
                        Speak=text2,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = null,


                    };

                }
                else
                {
                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text2,
                        Speak=text2,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = ss,
                        InputHint=InputHint


                    };

                }
                activities.Add(Bot);

                model.customerModel.CustomerStepModel.ChatStepId=8;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=7;

            }


            return activities;
        }


        private List<Activity> step8(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();


            var dyas = _botApis.GetBookingDay(model.customerModel.BotBookingParmeterModel.selectDoctorId, model.customerModel.CustomerStepModel.LangId);
          

            int n;
            bool isNumeric = int.TryParse(model.customerModel.customerChat.text.Trim(), out n);
            bool found = false;


            if (isNumeric)
            {
                try
                {
                    var arraylist = dyas.ToArray();

                    model.customerModel.BotBookingParmeterModel.BookingDate=arraylist[n-1].Value;
                    found =true;
                }
                catch
                {
                    found =false;

                }


            }
            else
            {
                foreach (var day in dyas)
                {
                    if (day.Key==model.customerModel.customerChat.text.Trim())
                    {

                        model.customerModel.BotBookingParmeterModel.BookingDate=day.Value;
                        found=true;

                        break;
                    }


                }

            }




           
            if (found)
            {
                model.isFall=false;
                return step9(model);
                //model.customerModel.CustomerStepModel.ChatStepId=9;
                //model.customerModel.CustomerStepModel.ChatStepPervoiusId=8;
            }
            else
            {



                model.isFall=true;

                return step7(model);
            }



            return activities;
        }

        private List<Activity> step9(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            var caption = model.captionDtos;
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }
            if (model.isFall)
            {
                var text = caption.Where(x => x.TextResourceId==3001 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;

                Activity Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak= text,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };

                activities.Add(Bot);
                model.isFall=false;
            }
            var time = _botApis.GetBookingTime(model.customerModel.BotBookingParmeterModel.BookingDate, model.tenantModel.TenantId.Value, model.customerModel.BotBookingParmeterModel.selectDoctorId);

            if (time.Count==0)
            {

                var text = caption.Where(x => x.TextResourceId==3001 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;


                Activity Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak= text,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot);
                model.isFall=true;
                activities = step7(model);
            }
            else
            {
                List<CardAction> cardActions = new List<CardAction>();
                foreach (var tim in time)
                {
                    cardActions.Add(new CardAction()
                    {
                        Title=tim,
                        Value=tim
                    });
                }
                List<Attachment> ss = new List<Attachment>();

                Activity Bot = new Activity();
                var text2 = caption.Where(x => x.TextResourceId==1263 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;

                if (time.Count()<=3)
                {

                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text2,
                        Speak=text2,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = null,


                    };

                }
                else
                {
                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text2,
                        Speak=text2,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = ss,
                        InputHint=InputHint


                    };

                }
                activities.Add(Bot);

                model.customerModel.CustomerStepModel.ChatStepId=10;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=9;

            }


            return activities;
        }

        private List<Activity> step10(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();


            var time = _botApis.GetBookingTime(model.customerModel.BotBookingParmeterModel.BookingDate, model.tenantModel.TenantId.Value, model.customerModel.BotBookingParmeterModel.selectDoctorId);



            int n;
            bool isNumeric = int.TryParse(model.customerModel.customerChat.text.Trim(), out n);
            bool found = false;


            if (isNumeric)
            {
                try
                {
                    var arraylist = time.ToArray();

                    model.customerModel.BotBookingParmeterModel.BookingTime=arraylist[n-1];
                    found =true;
                }
                catch
                {
                    found =false;

                }


            }
            else
            {
                if (time.Contains(model.customerModel.customerChat.text.Trim()))
                {
                    model.customerModel.BotBookingParmeterModel.BookingTime=model.customerModel.customerChat.text.Trim();
                    found =true;

                }
                else
                {
                    found =false;

                }

            }



            if (found)
            {
                model.isFall=true;
                return step11(model);
            }
            else
            {
                model.isFall=false;
                return step9(model);
            }



            return activities;
        }
        private List<Activity> step11(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            var caption = model.captionDtos;
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }

            var text = caption.Where(x => x.TextResourceId==1266 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;


            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary = Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);


            model.customerModel.CustomerStepModel.ChatStepId=12;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=11;

            return activities;
        }


        private List<Activity> step12(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            var caption = model.captionDtos;
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }


            model.customerModel.BotBookingParmeterModel.Note=model.customerModel.customerChat.text.Trim();

            BookingModel booking = new BookingModel()
            {
                ContactId=int.Parse(model.customerModel.ContactID),
                PhoneNumber=model.customerModel.phoneNumber,
                TenantId=model.customerModel.TenantId.Value,
                CustomerName=model.customerModel.displayName,
                BookingDate=model.customerModel.BotBookingParmeterModel.BookingDate,
                BookingTime=model.customerModel.BotBookingParmeterModel.BookingTime,
                StatusId=1,
                LanguageId=model.customerModel.CustomerStepModel.LangId,
                CreatedBy=int.Parse(model.customerModel.ContactID),
                AreaId=model.customerModel.BotBookingParmeterModel.AreaId,
                BookingTypeId=2,
                UserName=model.customerModel.BotBookingParmeterModel.selectDoctor,
                Note=model.customerModel.BotBookingParmeterModel.Note,
                UserId=int.Parse(model.customerModel.BotBookingParmeterModel.selectDoctorId.ToString())


            };

            var rez = _botApis.CreateBooking(booking).Result;


            if (rez=="booking_success")
            {

                var text = caption.Where(x => x.TextResourceId==1258 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;


                Activity Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak= text,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot);
            }
            else
            {
                var text = caption.Where(x => x.TextResourceId==3001 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;


                Activity Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak= text,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot);
            }


            model.customerModel.CustomerStepModel.ChatStepId=0;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

            return activities;
        }

        private List<Activity> step13(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();

            if (model.customerModel.customerChat.text=="اخرى"||model.customerModel.customerChat.text=="Others"||model.customerModel.customerChat.text=="العودة"||model.customerModel.customerChat.text=="Back")
            {
                activities= step3(model);
                model.customerModel.CustomerStepModel.ChatStepId=13;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            }
            else
            {
                var Branches = _botApis.GetAreasID2(model.tenantModel.TenantId.Value.ToString(), model.customerModel.customerChat.text, 0, model.customerModel.CustomerStepModel.LangString);

                if (Branches!=null)
                {
                    if (Branches.AreaName!=null)
                    {
                        if (Branches.AreaName.Trim()== model.customerModel.customerChat.text.Trim() || Branches.AreaNameEnglish.Trim()== model.customerModel.customerChat.text.Trim())
                        {
                            model.customerModel.CustomerStepModel.ChatStepId=14;
                            model.customerModel.CustomerStepModel.ChatStepPervoiusId=13;

                            model.customerModel.BotBookingParmeterModel.AreaId=Branches.Id;
                            model.customerModel.BotBookingParmeterModel.UserIds=Branches.UserIds;


                            return step14(model);

                        }
                        else
                        {
                            activities= step3(model);
                             model.customerModel.CustomerStepModel.ChatStepId=13;
                            model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                        }

                    }
                    else
                    {
                        activities= step3(model);
                        model.customerModel.CustomerStepModel.ChatStepId=13;
                         model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                    }

                }
                else
                {
                    activities= step3(model);
                    model.customerModel.CustomerStepModel.ChatStepId=13;
                     model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                }

            }

            return activities;
        }

        private List<Activity> step14(BotStepModel model)
        {


            List<Activity> activities = new List<Activity>();

            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1265 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            // var text = "يرجى اختيار الطبيبب:";//12
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }



            List<CardAction> cardActions = new List<CardAction>();

            var Doctor = _botApis.GetUserByBranches(model.tenantModel.TenantId.Value, model.customerModel.BotBookingParmeterModel.UserIds).Result;

            foreach (var doct in Doctor)
            {
                cardActions.Add(new CardAction()
                {
                    Title=doct,
                    Value=doct
                });
            }
            List<Attachment> ss = new List<Attachment>();

            Activity Bot = new Activity();

            if (Doctor.Count()==1)
            {
                model.customerModel.CustomerStepModel.ChatStepId=15;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=14;
                model.customerModel.customerChat.text=Doctor.FirstOrDefault();
                return step15(model);
            }
            else
            {


                if (Doctor.Count()<=3)
                {

                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text,
                        Speak=text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = null,


                    };

                }
                else
                {
                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text,
                        Speak=text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = ss,
                        InputHint=InputHint


                    };

                }

                activities.Add(Bot);



            }



            model.customerModel.CustomerStepModel.ChatStepId=15;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=14;

            return activities;
        }


        private List<Activity> step15(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();


            var Doctor = _botApis.GetUserByBranches(model.tenantModel.TenantId.Value, model.customerModel.BotBookingParmeterModel.UserIds).Result;



            if (Doctor.Contains(model.customerModel.customerChat.text.Trim()))
            {
                model.customerModel.BotBookingParmeterModel.selectDoctor=model.customerModel.customerChat.text.Trim();

                return step16(model);
                //model.customerModel.CustomerStepModel.ChatStepId=7;
                //  model.customerModel.CustomerStepModel.ChatStepPervoiusId=6;
            }
            else
            {
                return step14(model);
            }



            return activities;
        }

        private List<Activity> step16(BotStepModel model)
        {
            var caption = model.captionDtos;

           // var text = caption.Where(x => x.TextResourceId==6 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
              var text = "يرجى ادخال استفسارك ";//6
            var Summary = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "يرجى ادخال استفسارك ";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                text = "Please enter your inquiry ";
                Summary = "Please send # to return to the main menu";
            }
            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary = Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };

            activities.Add(Bot);


              model.customerModel.CustomerStepModel.ChatStepId=17;
              model.customerModel.CustomerStepModel.ChatStepPervoiusId=16;
            return activities;
        }
        private List<Activity> step17(BotStepModel model)
        {
            var text = _botApis.UpdateLiveChatAsync(model.tenantModel.TenantId, model.customerModel.phoneNumber, model.customerModel.BotBookingParmeterModel.selectDoctor).Result;

            if (text=="")
            {
                var caption = model.captionDtos;
                text = caption.Where(x => x.TextResourceId==1028 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            }
            var Summary = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
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
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary = Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };

            activities.Add(Bot);
            model.customerModel.CustomerStepModel.ChatStepId=0;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            model.customerModel.CustomerStepModel.IsLiveChat=true;
            return activities;
        }


        private List<Activity> step18(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            var caption = model.captionDtos;
            var contactBooking = _botApis.GetContactBooking(int.Parse(model.customerModel.ContactID),model.tenantModel.TenantId.Value,  model.customerModel.CustomerStepModel.LangId);

            Activity Bot = new Activity();



            var text = caption.Where(x => x.TextResourceId==1262 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            // var text = "يرجى اختيار الطبيبب:";//12
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }


            if (contactBooking.Count()<=0)
            {

                var textF = caption.Where(x => x.TextResourceId==1260 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;               
              
                
                Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = textF,
                    Speak= textF,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };

                activities.Add(Bot);


                model.customerModel.CustomerStepModel.ChatStepId=0;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                return activities;

            }




            List<CardAction> cardActions = new List<CardAction>();
            foreach (var doct in contactBooking)
            {
                cardActions.Add(new CardAction()
                {
                    Title=doct.ContactBookingTime,
                    Value=doct.Id
                });
            }
            List<Attachment> ss = new List<Attachment>();


            if (contactBooking.Count()==1)
            {
                model.customerModel.CustomerStepModel.ChatStepId=19;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=18;
                model.customerModel.customerChat.text=contactBooking.FirstOrDefault().ContactBookingTime;
                activities =step19(model);
            }
            else
            {


                if (contactBooking.Count()<=3)
                {

                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text,
                        Speak=text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = null,


                    };

                }
                else
                {
                    Bot = new Activity
                    {
                        From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                        Text = text,
                        Speak=text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =   model.customerModel.CustomerStepModel.LangString,
                        Attachments = ss,
                        InputHint=InputHint


                    };

                }

                activities.Add(Bot);



            }

            model.customerModel.CustomerStepModel.ChatStepId=19;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=18;
            return activities;
        }


        private List<Activity> step19(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();


            var contactBooking = _botApis.GetContactBooking(int.Parse(model.customerModel.ContactID), model.tenantModel.TenantId.Value, model.customerModel.CustomerStepModel.LangId);


            int n;
            bool isNumeric = int.TryParse(model.customerModel.customerChat.text.Trim(), out n);
            bool isfound = false;


            if (isNumeric)
            {
                try
                {
                    var arraylist = contactBooking.ToArray();

                    model.customerModel.BotBookingParmeterModel.ContactBookingId=arraylist[n-1].Id;
                    model.customerModel.BotBookingParmeterModel.ContactBookingTime=arraylist[n-1].BookingTime;
                    isfound =true;
                }
                catch
                {
                    isfound =false;

                }
          

            }
            else
            {

                
                foreach (var con in contactBooking)
                {
                    if (con.ContactBookingTime==model.customerModel.customerChat.text.Trim())
                    {
                        model.customerModel.BotBookingParmeterModel.ContactBookingTime=model.customerModel.customerChat.text.Trim();

                        model.customerModel.BotBookingParmeterModel.ContactBookingId=con.Id;
                        isfound =true;
                    }


                }

            }


            if (isfound)
            {

                activities= step20(model);
            }
            else
            {
                activities= step18(model);
            }



            return activities;
        }


        private List<Activity> step20(BotStepModel model)
        {
            var text = _botApis.CancelBooking(model.customerModel.BotBookingParmeterModel.ContactBookingId.ToString()).Result;

            if (text=="The appointment has been Cancelled" || text=="تم الغاء الموعد")
            {
                var caption = model.captionDtos;
                text = caption.Where(x => x.TextResourceId==1261 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            }
            else
            {
                var activitiesw = step18(model);
                return activitiesw;
            }
            var Summary = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
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
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary = Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };

            activities.Add(Bot);
            model.customerModel.CustomerStepModel.ChatStepId=0;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            //model.customerModel.CustomerStepModel.IsLiveChat=true;
            return activities;
        }
        #endregion

        #region Dialog



        #endregion

        private List<Activity> TriggersFun(BotStepModel botStepModel, List<Activity> Bot)
        {

            if (botStepModel.customerModel.CustomerStepModel.ChatStepId==-1)
            {
                botStepModel.isFall=false;

                botStepModel.customerModel.CustomerStepModel.ChatStepId=0;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

                botStepModel.customerModel.CustomerStepModel.OrderId=0;
                botStepModel.customerModel.CustomerStepModel.OrderTotal=0;
                botStepModel.customerModel.CustomerStepModel.IsLinkMneuStep=false;
                botStepModel.customerModel.CustomerStepModel.OrderTypeId="";
                botStepModel.customerModel.CustomerStepModel.SelectedAreaId=0;
                botStepModel.customerModel.CustomerStepModel.DeliveryCostAfter=0;
                botStepModel.customerModel.CustomerStepModel.DeliveryCostBefor=0;
                botStepModel.customerModel.CustomerStepModel.AddressLatLong="";
                botStepModel.customerModel.CustomerStepModel.Address="";
                botStepModel.customerModel.CustomerStepModel.Discount=0;
                botStepModel.customerModel.CustomerStepModel.isOrderOfferCost=false;
                botStepModel.customerModel.CustomerStepModel.OrderDeliveryCost=0;
                botStepModel.customerModel.CustomerStepModel.PageNumber=0;
                botStepModel.customerModel.CustomerStepModel.TotalPoints=0;
                botStepModel.customerModel.CustomerStepModel.SelectDay="";
                botStepModel.customerModel.CustomerStepModel.SelectTime="";
                botStepModel.customerModel.CustomerStepModel.IsPreOrder=false;
                botStepModel.customerModel.CustomerStepModel.IsLiveChat=false;


            }
            if (botStepModel.customerModel.customerChat.text.Trim()=="اللغة" || botStepModel.customerModel.customerChat.text.ToLower().Trim()=="language")
            {
                botStepModel.customerModel.CustomerStepModel.ChatStepId=0;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            }

            if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="تأكيد"||botStepModel.customerModel.customerChat.text.ToLower().Trim()=="Confirm")
            {

                var bookingId = botStepModel.customerModel.interactiveId.Replace("1&&&", "");
                if (!string.IsNullOrEmpty(bookingId))
                {

                    var booking = _botApis.ConfirmBooking(bookingId).Result;
                    botStepModel.customerModel.customerChat.text=booking;
                    botStepModel.customerModel.CustomerStepModel.ChatStepId=0;
                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                    Bot =ChatStepTriggersConfirmBooking(botStepModel);
                }
              
            }
            if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="رفض" || botStepModel.customerModel.customerChat.text.ToLower().Trim()=="Reject")
            {

                var bookingId = botStepModel.customerModel.interactiveId.Replace("2&&&", "");
                if (!string.IsNullOrEmpty(bookingId))
                {
                    var booking = _botApis.CancelBooking(bookingId).Result;
                    botStepModel.customerModel.customerChat.text=booking;
                    botStepModel.customerModel.CustomerStepModel.ChatStepId=0;
                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                    Bot =ChatStepTriggersCancelBooking(botStepModel);

                }
               
            }


            if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="start")
            {
                CustomerBehaviourModel customerBehaviourModel = new CustomerBehaviourModel()
                {
                    ContactId=int.Parse(botStepModel.customerModel.ContactID),
                    TenantID=botStepModel.tenantModel.TenantId,
                    Start=true,
                    Stop=false,

                };
                _botApis.UpdateCustomerBehavior(customerBehaviourModel);
                botStepModel.customerBehaviourModel=customerBehaviourModel;

                Bot =ChatStepTriggersUpdateCustomerBehavior(botStepModel);
            }

            if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="stop")
            {
                CustomerBehaviourModel customerBehaviourModel = new CustomerBehaviourModel()
                {
                    ContactId=int.Parse(botStepModel.customerModel.ContactID),
                    TenantID=botStepModel.tenantModel.TenantId,
                    Start=false,
                    Stop=true,

                };
                _botApis.UpdateCustomerBehavior(customerBehaviourModel);
                botStepModel.customerBehaviourModel=customerBehaviourModel;
                Bot=ChatStepTriggersUpdateCustomerBehavior(botStepModel);
            }

            if (botStepModel.customerModel.customerChat.text=="#")
            {
                botStepModel.isFall=false;

                botStepModel.customerModel.CustomerStepModel.ChatStepId=0;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

                botStepModel.customerModel.CustomerStepModel.OrderId=0;
                botStepModel.customerModel.CustomerStepModel.OrderTotal=0;
                botStepModel.customerModel.CustomerStepModel.IsLinkMneuStep=false;
                botStepModel.customerModel.CustomerStepModel.OrderTypeId="";
                botStepModel.customerModel.CustomerStepModel.SelectedAreaId=0;
                botStepModel.customerModel.CustomerStepModel.DeliveryCostAfter=0;
                botStepModel.customerModel.CustomerStepModel.DeliveryCostBefor=0;
                botStepModel.customerModel.CustomerStepModel.AddressLatLong="";
                botStepModel.customerModel.CustomerStepModel.Address="";
                botStepModel.customerModel.CustomerStepModel.Discount=0;
                botStepModel.customerModel.CustomerStepModel.isOrderOfferCost=false;
                botStepModel.customerModel.CustomerStepModel.OrderDeliveryCost=0;
                botStepModel.customerModel.CustomerStepModel.PageNumber=0;
                botStepModel.customerModel.CustomerStepModel.TotalPoints=0;
                botStepModel.customerModel.CustomerStepModel.SelectDay="";
                botStepModel.customerModel.CustomerStepModel.SelectTime="";
                botStepModel.customerModel.CustomerStepModel.IsPreOrder=false;
                botStepModel.customerModel.CustomerStepModel.IsLiveChat=false;
            }
            else
            {

                var word = _whatsAppMessageTemplateAppService.GetBotReservedWords(0, 20, botStepModel.tenantModel.TenantId);
                foreach (var wor in word.lstBotReservedWordsModel)
                {
                    var listbut = wor.ButtonText.Split(",");

                    if (listbut.Contains(botStepModel.customerModel.customerChat.text.Trim()))
                    {

                        if (wor.ActionEn=="live chat")
                        {
                            Bot= step3(botStepModel);

                            botStepModel.customerModel.CustomerStepModel.ChatStepId=13;
                            botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

                            break;
                        }

                        if (wor.ActionEn=="Request")
                        {


                        }

                    }

                }
            }
            return Bot;
        }

        #region UpdateCustomerBehavior
        private List<Activity> ChatStepTriggersUpdateCustomerBehavior(BotStepModel model)
        {
            var caption = model.captionDtos;

            //var text = caption.Where(x => x.TextResourceId==3 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            var text = "";
            var Summary = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar" && model.customerBehaviourModel.Start)
            {
                text="تم تفعيل الخدمة بنجاح في اي وقت يمكنك ارسال STOP لايقاف الخدمة";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else if (model.customerModel.CustomerStepModel.LangString=="en" && model.customerBehaviourModel.Start)
            {
                text="The service has been activated successfully. At any time, you can send STOP to stop the service";
                Summary = "Please send # to return to the main menu";
            }


            if (model.customerModel.CustomerStepModel.LangString=="ar" && model.customerBehaviourModel.Stop)
            {
                text="تم ايقاف الخدمة في اي وقت يمكنك الضغط Start لتفعيل الخدمة";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else if (model.customerModel.CustomerStepModel.LangString=="en" && model.customerBehaviourModel.Stop)
            {
                text="The service has been stopped at any time. You can press Start to activate the service";
                Summary = "Please send # to return to the main menu";
            }

            List<Activity> activities = new List<Activity>();

            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary= Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        #endregion
        private List<Activity> ChatStepTriggersConfirmBooking(BotStepModel model)
        {
            var caption = model.captionDtos;

            //var text = caption.Where(x => x.TextResourceId==3 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            var text = model.customerModel.customerChat.text;
            var Summary = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar" && model.customerBehaviourModel.Start)
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else if (model.customerModel.CustomerStepModel.LangString=="en" && model.customerBehaviourModel.Start)
            {
                Summary = "Please send # to return to the main menu";
            }


            List<Activity> activities = new List<Activity>();

            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary= Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepTriggersCancelBooking(BotStepModel model)
        {
            var caption = model.captionDtos;

            //var text = caption.Where(x => x.TextResourceId==3 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            var text = model.customerModel.customerChat.text;
            var Summary = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar" && model.customerBehaviourModel.Start)
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else if (model.customerModel.CustomerStepModel.LangString=="en" && model.customerBehaviourModel.Start)
            {
                Summary = "Please send # to return to the main menu";
            }


            List<Activity> activities = new List<Activity>();

            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary= Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private void CacheFun(CustomerModel model, out TenantModel Tenant, out List<CaptionDto> captionDtos)
        {

            if (model.CustomerStepModel==null)
            {
                model.CustomerStepModel=new CustomerStepModel() { ChatStepId=0 };
            }

            // Cache Tenant
            var objTenant = _cacheManager.GetCache("CacheTenant").Get(model.TennantPhoneNumberId.ToString(), cache => cache);
            if (objTenant.Equals(model.TennantPhoneNumberId.ToString()))
            {
                Tenant = _dbService.GetTenantByKey("", model.TennantPhoneNumberId.ToString()).Result;
                _cacheManager.GetCache("CacheTenant").Set(model.TennantPhoneNumberId.ToString(), Tenant);
            }
            else
            {
                Tenant = (TenantModel)objTenant;
            }

            //Cache Caption
            var objTenant1 = _cacheManager.GetCache("CacheTenant_CaptionStps").Get("Step_" + Tenant.TenantId.ToString(), cache => cache);
            if (objTenant1.Equals("Step_" + Tenant.TenantId.ToString()))
            {
                captionDtos = _botApis.GetAllCaption(model.TenantId.Value, model.CustomerStepModel.LangString);
                _cacheManager.GetCache("CacheTenant_CaptionStps").Set("Step_" + Tenant.TenantId.ToString(), captionDtos);
            }
            else
            {
                captionDtos=(List<CaptionDto>)objTenant1;
            }

        }

        private async Task BotSendToWhatsApp(BotStepModel model)
        {
            List<Activity> botListMessages = new List<Activity>();


            foreach (var msgBot in model.Bot)
            {

                if (msgBot.Text.Contains("The process cannot access the file") || msgBot.Text.Contains("Object reference not set to an instance of an object") || msgBot.Text.Contains("An item with the same key has already been added") || msgBot.Text.Contains("Operations that change non-concurrent collections must have exclusive access") || msgBot.Text.Contains("Maximum nesting depth of") || msgBot.Text.Contains("Response status code does not indicate success"))
                {


                }
                else
                {
                    botListMessages.Add(msgBot);
                }


            }

            WhatsAppAppService whatsAppAppService = new WhatsAppAppService();


            foreach (var msgBot in botListMessages)
            {


                List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = await whatsAppAppService.BotChatWithCustomer(msgBot, model.customerModel.phoneNumber, model.tenantModel.botId);

                foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
                {
                    string result = null;
                    if (model.customerModel.channel == "facebook")
                    {
                        result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.FacebookPageId, model.tenantModel.FacebookAccessToken, model.customerModel.channel ,null);

                    }
                    else if(model.customerModel.channel == "instagram")
                    {
                        result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.InstagramId, model.tenantModel.InstagramAccessToken, model.customerModel.channel, null);
                    }
                    else
                    {
                        result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.D360Key, model.tenantModel.AccessToken, model.customerModel.channel, null);
                    }


                    if (result!=null)
                    {
                        var modelR = JsonConvert.DeserializeObject<WhatsAppResult>(result);

                        WhatsAppContent model2 = new WhatsAppAppService().PrepareMessageContent(msgBot, model.tenantModel.botId, model.customerModel.userId, model.customerModel.TenantId.Value, model.customerModel.ConversationId);
                        try
                        {
                            model2.Buttons=model.customerModel.customerChat.Buttons;
                            model2.IsButton=model.customerModel.customerChat.IsButton;

                        }
                        catch
                        {
                            model2.Buttons=new List<string>();
                            model2.IsButton=false;
                        }

                        if (modelR.messages!=null)
                        {
                            model2.conversationID=modelR.messages.FirstOrDefault().id;


                        }

                        var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model2);

                        model.customerModel.customerChat = CustomerChat;
                        SocketIOManager.SendChat(model.customerModel, model.customerModel.TenantId.Value);
                    }
                    //var result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.D360Key, model.tenantModel.AccessToken, model.tenantModel.IsD360Dialog);
                    //if (result!=null)
                    //{
                    //    var modelR = JsonConvert.DeserializeObject<WhatsAppResult>(result);

                    //    WhatsAppContent model2 = new WhatsAppAppService().PrepareMessageContent(msgBot, model.tenantModel.botId, model.customerModel.userId, model.customerModel.TenantId.Value, model.customerModel.ConversationId);
                    //    model2.conversationID=modelR.messages.FirstOrDefault().id;
                    //    var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model2);

                    //    model.customerModel.customerChat = CustomerChat;
                    //    SocketIOManager.SendChat(model.customerModel, model.customerModel.TenantId.Value);
                    //}
                }

            }
        }

        private void UpdateCustomer(BotStepModel model)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0  && a.userId == model.customerModel.userId);//&& a.TenantId== TenantId
            var Result = itemsCollection.UpdateItemAsync(model.customerModel._self, model.customerModel).Result;

            // BotSendToWhatsApp(Tenant, customer, Bot);

        }

        private CustomerModel GetCustomer(string id)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == id);//&& a.TenantId== TenantId
            return customerResult.Result;
        }

    }
}
