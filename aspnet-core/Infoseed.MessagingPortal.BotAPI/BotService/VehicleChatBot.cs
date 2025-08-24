using Abp.Runtime.Caching;
using DocumentFormat.OpenXml.Office2019.Excel.RichData2;
using Framework.Data;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.BotAPI.Interfaces;
using Infoseed.MessagingPortal.BotAPI.Models;
using Infoseed.MessagingPortal.BotAPI.Models.BotModel;
using Infoseed.MessagingPortal.Branches;
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
    public class VehicleChatBot : MessagingPortalControllerBase
    {
        private readonly IDocumentClient _IDocumentClient;
        private readonly IDBService _dbService;
        private readonly ICacheManager _cacheManager;
        public IBotApis _botApis;
        public static Dictionary<string, bool> MessagesSent { get; set; }
        public IWhatsAppMessageTemplateAppService _whatsAppMessageTemplateAppService;
        public VehicleChatBot(ICacheManager cacheManager, IDBService dbService, IDocumentClient IDocumentClient, IBotApis botApis, IWhatsAppMessageTemplateAppService whatsAppMessageTemplateAppService)
        {

            _cacheManager=cacheManager;
            _dbService=dbService;
            _IDocumentClient=IDocumentClient;
            _botApis=botApis;
            _whatsAppMessageTemplateAppService=whatsAppMessageTemplateAppService;
        }
        [Route("DeleteCacheVehicle")]
        [HttpGet]
        public string DeleteCacheVehicle(int TenantId)
        {
            var model = _botApis.GetTenantAsync(TenantId).Result;
            _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + TenantId.ToString());
            _cacheManager.GetCache("CacheTenant").Remove(model.D360Key);

            return "Done";

        }
        [Route("BotStartVehicle")]
        [HttpGet]
        public List<Activity> BotStartVehicle(int TenantId, string CustomerPhoneNumber, string text)
        {
            var Customer = GetCustomer(TenantId+"_"+CustomerPhoneNumber);//Get  Customer
            Customer.customerChat.text=text;
            Customer.TennantPhoneNumberId="110677438393126";
            return VehicleMessageHandler(Customer);

            //SocketIOManager.SendChat(Customer, TenantId);
            //List<Activity> Bot = new List<Activity>();
            //return Bot;

        }
        [Route("VehicleMessageHandler")]
        [HttpPost]
        public List<Activity> VehicleMessageHandler(CustomerModel model)
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
                                                 //   var x = BotSendToWhatsApp(botStepModel);

                    MessagesSent[model.userId] = false;
                }
                catch
                {
                    MessagesSent[model.userId] = false;

                }


            }


            return Bot;
        }


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


            //if oreade select lang 
            //if (model.customerModel.CustomerStepModel.ChatStepId==0 && (model.customerModel.CustomerStepModel.LangId!=0))
            //{
            //    model.customerModel.CustomerStepModel.ChatStepId=model.customerModel.CustomerStepModel.ChatStepId+1;
            //    model.customerModel.CustomerStepModel.ChatStepPervoiusId=model.customerModel.CustomerStepModel.ChatStepPervoiusId+1;
            //}



            switch (model.customerModel.CustomerStepModel.ChatStepId)
            {
                case 0:
                    if (model.tenantModel.IsBotLanguageAr&&model.tenantModel.IsBotLanguageEn)
                    {

                        Bot= LanguageDialog(model);
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
                        Bot=CheckLanguageAndSelectNameDialog(model);
                    }
                    break;
                case 1:
                    Bot= CheckLanguageAndSelectNameDialog(model);
                    break;
                case 2:
                    Bot= CheckNameAndSelectMainDialog(model);
                    break;
                case 3:
                    Bot= CheckMainDialog(model);
                    break;
                case 4:
                    Bot= CheckAssetDetailDialog(model);

                    break;
                case 5:
                    Bot= CheckLiveChatDialog(model);
                    break;
                case 6:// مبيعات
                    Bot= CarSalesDialog(model);
                    break;
                case 7:
                    Bot= CheckCarSalesDialog(model);
                    break;
                case 8://page reques
                    Bot= BookTestDriveDialog(model);
                    break;
                case 9:
                    Bot= CheckBookTestDriveAndSelectTimeDialog(model);
                    break;
                case 10:
                    Bot= CheckTimeAndSelectCommentDialog(model);
                    break;
                case 11:// end page reques
                    Bot= CheckCommentDialog(model);
                    break;
                case 12://loop asset
                    Bot = LoopAssetDialog(model);
                    break;
                case 13:
                    Bot= HyundaiMaintenanceDialog(model);
                    break;
                case 14:
                    Bot= CheckHyundaiMaintenanceDialog(model);
                    break;
                case 15:
                    Bot= HyundaiComplaintsDialog(model);
                    break;
                case 16:
                    Bot= CheckHyundaiComplaintsDialog(model);
                    break;
                case 17:
                    Bot= HyundaiSparePartsDialog(model);
                    break;
                case 18:
                    Bot= HyundaiSparePartsDisDialog(model);
                    break;
                case 19:
                    model.customerModel.customerChat.text="دردشة مباشرة";
                    Bot= CheckLiveChatDialog(model);
                    break;
                case 20:
                    Bot= HyundaiQueriesDialog(model);
                    break;
                case 21:
                    Bot= CheckHyundaiQueriesDialog(model);
                    break;
                case 22:
                    Bot= CheckHyundaiQueriesAndLivChatDialog(model);
                    break;
                case 23:
                    Bot= HyundaiAppointmentSelectNameDialog(model);
                    break;
                case 24:
                    Bot= CheckNameHyundaiAppointmentSelectPicDialog(model);
                    break;
                case 25:
                    Bot= CheckSelectPicHyundaiAppointmentDialog(model);
                    break;
                case 26:
                    Bot= CheckPicAndNumberHyundaiAppointmentDialog(model);
                    break;
            }




            return Bot;
        }


        #region Dialog
        private List<Activity> LanguageDialog(BotStepModel model)
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

        private List<Activity> CheckLanguageAndSelectNameDialog(BotStepModel model)
        {
            // check Lang
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

                    return LanguageDialog(model);
                

            }

            // select name




            var name = _botApis.checkIsFillDisplayName(int.Parse(model.customerModel.ContactID));

            if (name!=null)
            {
                model.customerModel.CustomerStepModel.ChatStepId=2;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=1;


                return CheckNameAndSelectMainDialog(model);
            }





            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1173 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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

            model.customerModel.CustomerStepModel.ChatStepId=2;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=1;


            return activities;
        }

        private List<Activity> CheckNameAndSelectMainDialog(BotStepModel model)
        {
            // check Name

            var name = _botApis.checkIsFillDisplayName(int.Parse(model.customerModel.ContactID));

            if (name==null)
            {

                _botApis.updateContcatDisplayName(int.Parse(model.customerModel.ContactID), model.customerModel.customerChat.text);

                model.customerModel.displayName=model.customerModel.customerChat.text;
            }

            // select Main dailog
            List<Activity> Bot = new List<Activity>();

            switch (model.customerModel.TenantId.Value)
            {
                case 45:
                    break;
                case 52:
                    break;
                case 59:
                    break;
                case 60://Hyundai
                    Bot=  HyundaiMainDialog(model);
                    break;
                case 67:
                    break;
            }



            return Bot;
        }

        private List<Activity> AssetDetailsDialog(BotStepModel model)
        {
            // check Asset
            List<Activity> activities = new List<Activity>();
            List<GetListPDFModel> Asset = new List<GetListPDFModel>();

            // check Main
            if (model.customerModel.CustomerStepModel.IsAssetOffer)
            {
                Asset = _botApis.GetAssetOffers(model.customerModel.TenantId.Value, model.customerModel.phoneNumber).Result;

            }
            else
            {
                Asset = _botApis.GetAsset(model.customerModel.TenantId.Value, model.customerModel.phoneNumber, 0, model.customerModel.CustomerStepModel.AssetLevelOneId, model.customerModel.CustomerStepModel.AssetLevelTowId, model.customerModel.CustomerStepModel.AssetLevelThreeId, 0, model.customerModel.CustomerStepModel.IsAssetOffer).Result;


            }

            if (Asset.Count()==0)// no asset
            {


                var caption = model.captionDtos;
                var text2 = caption.Where(x => x.TextResourceId==1229 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

                var Summary2 = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary2 = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary2 = "Please send # to return to the main menu";
                }
                Activity Bot2 = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text2,
                    Speak= text2,
                    Type = ActivityTypes.Message,
                    Summary = Summary2,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };



                activities.Add(Bot2);
                model.customerModel.CustomerStepModel.ChatStepId=0;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;


                return activities;
            }


            var selectAsset = "";

            if (Asset.Count()==1)
            {
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    selectAsset=Asset.FirstOrDefault().AssetNameAr;
                }
                else
                {
                    selectAsset=Asset.FirstOrDefault().AssetNameEn;
                }

            }

            if (selectAsset!="")
            {
                model.customerModel.CustomerStepModel.ChatStepId=4;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;

                return CheckAssetDetailDialog(model);

            }
            var text = "";
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text="الرجاء الاختيار";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text="Please choose";
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }

            List<CardAction> cardActions = new List<CardAction>();

            List<string> Buttons = new List<string>();
            foreach (var ast in Asset)
            {
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    cardActions.Add(new CardAction()
                    {
                        Title=ast.AssetNameAr,
                        Value=ast.AssetNameAr,
                    });
                    Buttons.Add(ast.AssetNameAr);
                }
                else
                {
                    cardActions.Add(new CardAction()
                    {
                        Title=ast.AssetNameEn,
                        Value=ast.AssetNameEn,
                    });
                    Buttons.Add(ast.AssetNameEn);

                }

            }

            List<Attachment> ss = new List<Attachment>();

            Activity Bot = new Activity();


            if (Asset.Count()<=3)
            {

                model.customerModel.customerChat.IsButton=true;
                model.customerModel.customerChat.Buttons=Buttons;



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
                    //InputHint=InputHint


                };
                activities.Add(Bot);
            }
            else
            {

                model.customerModel.customerChat.IsButton=false;
                model.customerModel.customerChat.Buttons=Buttons;


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
                activities.Add(Bot);
            }

            model.customerModel.CustomerStepModel.ChatStepId=4;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;
            return activities;
        }
        private List<Activity> CheckAssetDetailDialog(BotStepModel model)
        {
            List<Activity> Activity = new List<Activity>();

            List<GetListPDFModel> Asset = new List<GetListPDFModel>();

            // check Main
            if (model.customerModel.CustomerStepModel.IsAssetOffer)
            {
                Asset = _botApis.GetAssetOffers(model.customerModel.TenantId.Value, model.customerModel.phoneNumber).Result;

            }
            else
            {
                Asset = _botApis.GetAsset(model.customerModel.TenantId.Value, model.customerModel.phoneNumber,0, model.customerModel.CustomerStepModel.AssetLevelOneId, model.customerModel.CustomerStepModel.AssetLevelTowId, model.customerModel.CustomerStepModel.AssetLevelThreeId,0, model.customerModel.CustomerStepModel.IsAssetOffer).Result;

                CustomerInterestedOf customerInterestedOf = new CustomerInterestedOf()
                {
                    ContactId=int.Parse(model.customerModel.ContactID),
                    TenantID=model.customerModel.TenantId.Value,
                    levleOneId=model.customerModel.CustomerStepModel.AssetLevelOneId,
                    levelTwoId=model.customerModel.CustomerStepModel.AssetLevelTowId,
                    levelThreeId=model.customerModel.CustomerStepModel.AssetLevelThreeId


                };
                _botApis.CreateInterestedOf(customerInterestedOf);
            }

            bool isSelectAsset = false;
            Activity Bot = new Activity();
            foreach (var ast in Asset)
            {

                if (model.customerModel.CustomerStepModel.LangString=="ar") {

                    if (ast.AssetNameAr.Trim()==model.customerModel.customerChat.text.Trim())
                    {
                        isSelectAsset=true;
                        model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();
                        Bot = new Activity
                        {
                            From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                            Text = ast.AssetDescriptionAr,
                            Speak= ast.AssetDescriptionAr,
                            Type = ActivityTypes.Message,
                            Summary= "",
                            Locale =   model.customerModel.CustomerStepModel.LangString,
                            Attachments = null
                        };
                        Activity.Add(Bot);
                        break;
                    }
                    //else
                    //{
                    //    model.customerModel.CustomerStepModel.ChatStepId=4;
                    //    model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;

                    //    Bot= AssetDetailsDialog(model);
                    //}

                }
                else
                {
                    if (ast.AssetNameEn.Trim()==model.customerModel.customerChat.text.Trim())
                    {
                        isSelectAsset=true;
                        model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();
                        Bot = new Activity
                        {
                            From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                            Text = ast.AssetDescriptionEn,
                            Speak= ast.AssetDescriptionEn,
                            Type = ActivityTypes.Message,
                            Summary= "",
                            Locale =   model.customerModel.CustomerStepModel.LangString,
                            Attachments = null
                        };
                        Activity.Add(Bot);
                        break;
                    }
                    //else
                    //{
                    //    model.customerModel.CustomerStepModel.ChatStepId=4;
                    //    model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;

                    //    Bot= AssetDetailsDialog(model);
                    //}
                }


            }


            if (!isSelectAsset&& string.IsNullOrEmpty(model.customerModel.CustomerStepModel.Department2))
            {
                if (model.customerModel.CustomerStepModel.IsAssetOffer)
                {
                    model.customerModel.CustomerStepModel.ChatStepId=3;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;

                    return AssetDetailsDialog(model);
                }
                else
                {
                    model.customerModel.CustomerStepModel.ChatStepId=3;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;

                    return AssetDetailsDialog(model);

                }
       
            }

            foreach (var ast in Asset)
            {
                Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = ast.AttachmentUrl,
                    Speak= ast.AttachmentName,
                    InputHint= ast.AttachmentType,
                    Type = ActivityTypes.Message,
                    Summary= "",
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                Activity.Add(Bot);
            }


            var caption = model.captionDtos;
            var text = "";//  caption.Where(x => x.TextResourceId==3000 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "الرجاء الاختيار";
                op1 = "تواصل مباشرة";
                op2 = "دردشة مباشرة";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text = "Please choose";
                op1 = "Contact Directly";
                op2 = "Live Chat";
                Summary = "To change the language, send a #";
                InputHint="Please Select";
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

            List<string> Buttons = new List<string>();

            foreach (var bt in cardActions)
            {
                Buttons.Add(bt.Title);

            }
            model.customerModel.customerChat.IsButton=true;
            model.customerModel.customerChat.Buttons=Buttons;

            //List<Activity> activities = new List<Activity>();
            Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary=Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            Activity.Add(Bot);

            model.customerModel.CustomerStepModel.ChatStepId=5;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=4;
            return Activity;
        }

        private List<Activity> CheckLiveChatDialog(BotStepModel model)
        {
            // check Name

            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity();
            var caption = model.captionDtos;

            if (model.customerModel.customerChat.text.Trim()=="تواصل مباشرة" || model.customerModel.customerChat.text.Trim()=="Contact Directly")
            {
                var text = caption.Where(x => x.TextResourceId==3000 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
                var Summary = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                Bot = new Activity
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
                model.customerModel.CustomerStepModel.Department2=null;
                model.customerModel.CustomerStepModel.Department1=null;

            }
            else if (model.customerModel.customerChat.text.Trim()=="دردشة مباشرة" || model.customerModel.customerChat.text.Trim()=="Live Chat")
            {
                var livechat = _botApis.UpdateLiveChatAsync(model.customerModel.TenantId.Value, model.customerModel.phoneNumber, model.customerModel.CustomerStepModel.Department1, model.customerModel.CustomerStepModel.Department2).Result;

                model.customerModel.CustomerStepModel.IsLiveChat=true;
                var text = "";// caption.Where(x => x.TextResourceId==1210 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

                if (livechat!= "")
                {
                    text = livechat;//14

                }
                else
                {
                    text = caption.Where(x => x.TextResourceId==1210 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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
                Bot = new Activity
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
                model.customerModel.CustomerStepModel.Department2=null;
                model.customerModel.CustomerStepModel.Department1=null;
            }
            else
            {
                model.customerModel.CustomerStepModel.ChatStepId=4;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;
                activities=CheckAssetDetailDialog(model);
            }

            return activities;
        }
        private List<Activity> LoopAssetDialog(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();


            switch (model.customerModel.CustomerStepModel.AssetStepId)
            {
                case 1:
                    Bot= AssetListDialog(model);
                    break;
                case 2:
                    Bot= AssetListDialog(model);
                    break;
                case 3:
                    Bot= AssetListDialog(model);
                    break;
                case 4:
                    Bot= AssetListDialog(model);
                    break;
               
            }
            return Bot;
        }
        private List<Activity> AssetListDialog(BotStepModel model)
        {
            List<GetAssetModel> assetlist = new List<GetAssetModel>();
            if (model.customerModel.CustomerStepModel.AssetStepId-1>0)
            {

                assetlist = _botApis.GetAssetLevel(model.tenantModel.TenantId.Value, model.customerModel.CustomerStepModel.LangString, model.customerModel.CustomerStepModel.AssetStepId-1, model.customerModel.CustomerStepModel.AssetlevelId);
                if (model.customerModel.CustomerStepModel.AssetStepId>1)
                {
                    try {



                        var assetID = assetlist.Find(x => x.Value==model.customerModel.customerChat.text.Trim()).Key;
                        model.customerModel.CustomerStepModel.AssetlevelId=int.Parse(assetID.ToString());
                        assetlist = _botApis.GetAssetLevel(model.tenantModel.TenantId.Value, model.customerModel.CustomerStepModel.LangString, model.customerModel.CustomerStepModel.AssetStepId, model.customerModel.CustomerStepModel.AssetlevelId);

                      

                        if (model.customerModel.CustomerStepModel.AssetStepId-1==1)
                        {
                            model.customerModel.CustomerStepModel.AssetLevelOneId=model.customerModel.CustomerStepModel.AssetlevelId;
                        }
                        if (model.customerModel.CustomerStepModel.AssetStepId-1==2)
                        {
                            model.customerModel.CustomerStepModel.AssetLevelTowId=model.customerModel.CustomerStepModel.AssetlevelId;
                        }
                        if (model.customerModel.CustomerStepModel.AssetStepId-1==3)
                        {
                            model.customerModel.CustomerStepModel.AssetLevelThreeId=model.customerModel.CustomerStepModel.AssetlevelId;
                        }
                        if (model.customerModel.CustomerStepModel.AssetStepId-1==4)
                        {
                            model.customerModel.CustomerStepModel.AssetLevelOneId=model.customerModel.CustomerStepModel.AssetlevelId;
                        }
                        model.customerModel.CustomerStepModel.AssetStepId=model.customerModel.CustomerStepModel.AssetStepId+1;
                    }
                    catch
                    {
                        model.customerModel.CustomerStepModel.AssetStepId=model.customerModel.CustomerStepModel.AssetStepId-1;
                    }

                   
                   

                }
            }
            else
            {

                assetlist = _botApis.GetAssetLevel(model.tenantModel.TenantId.Value, model.customerModel.CustomerStepModel.LangString, model.customerModel.CustomerStepModel.AssetStepId, model.customerModel.CustomerStepModel.AssetlevelId);

                model.customerModel.CustomerStepModel.AssetStepId=model.customerModel.CustomerStepModel.AssetStepId+1;

            }

            if (assetlist.Count()==0)
            {
                model.customerModel.CustomerStepModel.ChatStepId=4;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;
                return AssetDetailsDialog(model);

            }



            var caption = model.captionDtos;
            var text = "";// caption.Where(x => x.TextResourceId==12 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
      
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "الرجاء الاختيار";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text = "Please choose";
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }


           

            List<CardAction> cardActions = new List<CardAction>();

            List<string> Buttons = new List<string>();
            foreach (var ast in assetlist)
            {
                cardActions.Add(new CardAction()
                {
                    Title=ast.Value,
                    Value=ast.Value,
                });
                Buttons.Add(ast.Value);
            }
            List<Activity> activities = new List<Activity>();
            List<Attachment> ss = new List<Attachment>();

            Activity Bot = new Activity();

            if (assetlist.Count()==1)
            {
               
            }
            if (assetlist.Count()<=3)
            {

                model.customerModel.customerChat.IsButton=true;
                model.customerModel.customerChat.Buttons=Buttons;


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
                    //InputHint=InputHint


                };

            }
            else
            {


                model.customerModel.customerChat.IsButton=false;
                model.customerModel.customerChat.Buttons=Buttons;


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



            return activities;
        }

        #endregion

        #region privet
        private List<Activity> TriggersFun(BotStepModel botStepModel, List<Activity> Bot)
        {
            if (botStepModel.customerModel.customerChat.text.Trim()=="اللغة" || botStepModel.customerModel.customerChat.text.ToLower().Trim()=="language")
            {
                botStepModel.customerModel.CustomerStepModel.ChatStepId=0;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
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
                botStepModel.customerModel.CustomerStepModel.Department2=null;
                botStepModel.customerModel.CustomerStepModel.Department1=null;

                botStepModel.customerModel.CustomerStepModel.AssetStepId=1;
                botStepModel.customerModel.CustomerStepModel.AssetlevelId=0;

                botStepModel.customerModel.CustomerStepModel.SelectDay=null;
                botStepModel.customerModel.CustomerStepModel.SelectTime=null;
                botStepModel.customerModel.CustomerStepModel.selectTypeNumber=null;
                botStepModel.customerModel.CustomerStepModel.SelectName=null;
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
                            botStepModel.customerModel.customerChat.text="دردشة مباشرة";
                            Bot= CheckLiveChatDialog(botStepModel);

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
                    if (model.customerModel.channel=="Facebook")
                    {
                        result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.FacebookPageId, model.tenantModel.FacebookAccessToken, model.customerModel.channel, null);
                    }
                    else if (model.customerModel.channel == "Instagram")
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
        #endregion


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

        #region Hyundai

        private List<Activity> HyundaiMainDialog(BotStepModel model)
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


            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            text =text.Replace("{0}", model.customerModel.displayName);
            string op1 = "";
            string op2 = "";
            string op3 = "";
            string Summary = "";
            string InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                op1 = "مبيعات السيارات🚗";
                op2 = "الصيانة⚙️";
                op3 = "الحملات والعروض🎁";
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
                op1 = "Car Sales🚗";
                op2 = "Maintenance⚙️";
                op3 = "Campaigns & Offers🎁";
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

            cardActions.Add(new CardAction()
            {
                Title=op3,
                Value=op3,
                Image=model.tenantModel.Image
            });


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

            model.customerModel.CustomerStepModel.ChatStepId=3;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;

            return activities;
        }

        private List<Activity> CheckMainDialog(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();
            // check Main
            if (model.customerModel.customerChat.text.Trim()=="مبيعات السيارات🚗" || model.customerModel.customerChat.text.Trim()=="Car Sales🚗")
            {
                model.customerModel.CustomerStepModel.ChatStepId=6;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=5;
                Bot= CarSalesDialog(model);

            }else if (model.customerModel.customerChat.text.Trim()=="الصيانة⚙️" || model.customerModel.customerChat.text.Trim()=="Maintenance⚙️")
            {
                model.customerModel.CustomerStepModel.ChatStepId=13;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                Bot= HyundaiMaintenanceDialog(model);


            }
            else if (model.customerModel.customerChat.text.Trim()=="الحملات والعروض🎁" || model.customerModel.customerChat.text.Trim()=="Campaigns & Offers🎁")
            {
                model.customerModel.CustomerStepModel.IsAssetOffer=true;
                Bot = AssetDetailsDialog(model);
            }
            else
            {

                model.customerModel.CustomerStepModel.ChatStepId=2;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=1;
                Bot= CheckNameAndSelectMainDialog(model);
            }


            return Bot;
        }
        private List<Activity> CarSalesDialog(BotStepModel model)
        {
            List<Activity> Activity = new List<Activity>();
            // check Main     
            Activity Bot = new Activity();   
            var caption = model.captionDtos;
            var text = "";//  caption.Where(x => x.TextResourceId==3000 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var op3 = "";
            var op4 = "";
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "الرجاء الاختيار";

                op1 = "استبدل سياراتك";
                op2 = "موقع المعرض";
                op3 = "احجز لتجربة القيادة";
                op4 = "اسعار ومواصفات";
               
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text = "Please choose";

                op1 = "Exchange Car";
                op2 = "Sales Site";
                op3 = "Book a Test Drive";
                op4 = "Car Prices";
                Summary = "To change the language, send a #";
                InputHint="Please Select";
            }

            List<Attachment> ss = new List<Attachment>();
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
            cardActions.Add(new CardAction()
            {
                Title=op3,
                Value=op3
            });
            cardActions.Add(new CardAction()
            {
                Title=op4,
                Value=op4
            });

            List<string> Buttons = new List<string>();

            foreach (var bt in cardActions)
            {
                Buttons.Add(bt.Title);

            }
            model.customerModel.customerChat.IsButton=false;
            model.customerModel.customerChat.Buttons=Buttons;

            //List<Activity> activities = new List<Activity>();
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
            Activity.Add(Bot);

            model.customerModel.CustomerStepModel.ChatStepId=7;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=6;
            return Activity;
        }

        private List<Activity> CheckCarSalesDialog(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            // check Main
            if (model.customerModel.customerChat.text.Trim()=="استبدل سياراتك" || model.customerModel.customerChat.text.Trim()=="Exchange Car")
            {
                model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();
                model.customerModel.customerChat.text="Live Chat";
                activities= CheckLiveChatDialog(model);
            }
            else if (model.customerModel.customerChat.text.Trim()=="موقع المعرض" || model.customerModel.customerChat.text.Trim()=="Sales Site") { 
                //model.customerModel.CustomerStepModel.IsLiveChat=true;

                var text = caption.Where(x => x.TextResourceId==1217 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

                var Summary = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                Bot = new Activity
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


            }
            else if (model.customerModel.customerChat.text.Trim()=="احجز لتجربة القيادة" || model.customerModel.customerChat.text.Trim()=="Book a Test Drive")
            {
                model.customerModel.CustomerStepModel.ChatStepId=8;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=7;
                activities =BookTestDriveDialog(model);
            }
            else if (model.customerModel.customerChat.text.Trim()=="اسعار ومواصفات" || model.customerModel.customerChat.text.Trim()=="Car Prices")
            {
                model.customerModel.CustomerStepModel.IsAssetOffer=false;
                model.customerModel.CustomerStepModel.AssetStepId=1;
                model.customerModel.CustomerStepModel.ChatStepId=12;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=11;
                activities= LoopAssetDialog(model);
            }
            else
            {
                model.customerModel.CustomerStepModel.ChatStepId=6;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=5;
                activities= CarSalesDialog(model);
            }
            return activities;
        }

        private List<Activity> BookTestDriveDialog(BotStepModel model)
        {
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1207 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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

            var day = _botApis.GetDay(model.customerModel.CustomerStepModel.LangString);

            foreach (var t in day)
            {
                cardActions.Add(new CardAction()
                {
                    Title=t,
                    Value=t
                });
            }
            List<Activity> activities = new List<Activity>();
            List<Attachment> ss = new List<Attachment>();

            List<string> Buttons = new List<string>();

            foreach (var bt in cardActions)
            {
                Buttons.Add(bt.Title);

            }
            model.customerModel.customerChat.IsButton=false;
            model.customerModel.customerChat.Buttons=Buttons;

            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary = Summary,
                InputHint=InputHint,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = ss
            };

            activities.Add(Bot);
            model.customerModel.CustomerStepModel.ChatStepId=9;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=8;
            return activities;
        }
        private List<Activity> CheckBookTestDriveAndSelectTimeDialog(BotStepModel model)
        {

            var day = _botApis.GetDay(model.customerModel.CustomerStepModel.LangString);
            if (string.IsNullOrEmpty(model.customerModel.CustomerStepModel.SelectDay))
            {
                if (day.Contains(model.customerModel.customerChat.text.Trim()) || day.Contains(model.customerModel.customerChat.text.Trim()))
                {
                    model.customerModel.CustomerStepModel.SelectDay=model.customerModel.customerChat.text.Trim();
                }
                else
                {
                    model.customerModel.CustomerStepModel.ChatStepId=8;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=7;
                    return BookTestDriveDialog(model);
                }

            }
      



            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1208 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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

            var time = _botApis.GetTime(model.customerModel.TenantId.Value, model.customerModel.CustomerStepModel.SelectDay, model.customerModel.CustomerStepModel.LangString);

            foreach (var t in time)
            {
                cardActions.Add(new CardAction()
                {
                    Title=t,
                    Value=t
                });
            }
            List<Activity> activities = new List<Activity>();
            List<Attachment> ss = new List<Attachment>();

            List<string> Buttons = new List<string>();

            foreach (var bt in cardActions)
            {
                Buttons.Add(bt.Title);

            }
            model.customerModel.customerChat.IsButton=false;
            model.customerModel.customerChat.Buttons=Buttons;

            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary = Summary,
                InputHint=InputHint,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = ss
            };

            activities.Add(Bot);
            model.customerModel.CustomerStepModel.ChatStepId=10;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=9;
            return activities;
        }
        private List<Activity> CheckTimeAndSelectCommentDialog(BotStepModel model)
        {

            var time = _botApis.GetTime(model.customerModel.TenantId.Value, model.customerModel.CustomerStepModel.SelectDay, model.customerModel.CustomerStepModel.LangString);

            if (time.Contains(model.customerModel.customerChat.text.Trim()) || time.Contains(model.customerModel.customerChat.text.Trim()))
            {
                model.customerModel.CustomerStepModel.SelectTime=model.customerModel.customerChat.text.Trim();
            }
            else
            {
                model.customerModel.CustomerStepModel.ChatStepId=9;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=8;
                return CheckBookTestDriveAndSelectTimeDialog(model);
            }




            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1218 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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

           
            List<Activity> activities = new List<Activity>();
  
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary = Summary,
                InputHint=InputHint,
                Locale =   model.customerModel.CustomerStepModel.LangString,
            };

            activities.Add(Bot);
            model.customerModel.CustomerStepModel.ChatStepId=11;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=9;
            return activities;
        }
        private List<Activity> CheckCommentDialog(BotStepModel model)
        {
            UpdateSaleOfferModel updateSaleOfferModel = new UpdateSaleOfferModel();
            List<AttachmentModel> attachmentModel= new List<AttachmentModel>();
            var data = model.customerModel.CustomerStepModel.SelectDay+","+model.customerModel.CustomerStepModel.SelectTime+","+model.customerModel.customerChat.text;

            if (!string.IsNullOrEmpty(model.customerModel.CustomerStepModel.selectTypeNumber))
            {

                if (model.customerModel.CustomerStepModel.IsPic)
                {

                    foreach (var at in model.customerModel.attachments)
                    {
                        attachmentModel.Add( new AttachmentModel
                        {
                             contentName=at.Name,
                              contentType=at.ContentType,
                               contentUrl=at.ContentUrl

                        });

                    }
                    updateSaleOfferModel = new UpdateSaleOfferModel()
                    {

                        ContactName=model.customerModel.displayName,
                        ContactId=int.Parse(model.customerModel.ContactID),
                        TenantID=model.customerModel.TenantId.Value,
                        PhoneNumber=model.customerModel.phoneNumber,
                        information=data,
                         AttachmetArray= attachmentModel.ToArray()


                    };

                }
                else
                {
                    data=data+","+model.customerModel.CustomerStepModel.selectTypeNumber;
                    updateSaleOfferModel = new UpdateSaleOfferModel()
                    {

                        ContactName=model.customerModel.displayName,
                        ContactId=int.Parse(model.customerModel.ContactID),
                        TenantID=model.customerModel.TenantId.Value,
                        PhoneNumber=model.customerModel.phoneNumber,
                        information=data


                    };

                }

            }
            else
            {

                updateSaleOfferModel = new UpdateSaleOfferModel()
                {

                    ContactName=model.customerModel.displayName,
                    ContactId=int.Parse(model.customerModel.ContactID),
                    TenantID=model.customerModel.TenantId.Value,
                    PhoneNumber=model.customerModel.phoneNumber,
                    information=data


                };
            }


           
             _botApis.SendPrescription(updateSaleOfferModel);


            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1210 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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


            List<Activity> activities = new List<Activity>();

            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                Summary = Summary,
                InputHint=InputHint,
                Locale =   model.customerModel.CustomerStepModel.LangString,
            };

            activities.Add(Bot);
            model.customerModel.CustomerStepModel.ChatStepId=0;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            model.customerModel.CustomerStepModel.SelectDay=null;
            model.customerModel.CustomerStepModel.SelectTime=null;
            model.customerModel.CustomerStepModel.selectTypeNumber=null;
            model.customerModel.CustomerStepModel.SelectName=null;
            return activities;
        }

        private List<Activity> HyundaiMaintenanceDialog(BotStepModel model)
        {
            List<Activity> Activity = new List<Activity>();
            // check Main     
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            var text = "";//  caption.Where(x => x.TextResourceId==3000 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var op3 = "";
            var op4 = "";
            var op5 = "";
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "الرجاء الاختيار";

                op1 = "أستفسارات";
                op2 = "حجز موعد";
                op3 = "قطع الغيار";
                op4 = "اقتراحات و شكاوي";
                op5 = "موقع الصيانة";

                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text = "Please choose";

                op1 = "Queries";
                op2 = "Appointment Booking";
                op3 = "Spare Parts";
                op4 = "Complaints";
                op5 = "Maintenance Site";

                Summary = "To change the language, send a #";
                InputHint="Please Select";
            }

            List<Attachment> ss = new List<Attachment>();
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
            cardActions.Add(new CardAction()
            {
                Title=op3,
                Value=op3
            });
            cardActions.Add(new CardAction()
            {
                Title=op4,
                Value=op4
            });
            cardActions.Add(new CardAction()
            {
                Title=op5,
                Value=op5
            });

            List<string> Buttons = new List<string>();

            foreach (var bt in cardActions)
            {
                Buttons.Add(bt.Title);

            }
            model.customerModel.customerChat.IsButton=false;
            model.customerModel.customerChat.Buttons=Buttons;

            //List<Activity> activities = new List<Activity>();
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
            Activity.Add(Bot);

            model.customerModel.CustomerStepModel.ChatStepId=14;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=13;
            return Activity;
        }
        private List<Activity> CheckHyundaiMaintenanceDialog(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            // check Main
            if (model.customerModel.customerChat.text.Trim()=="أستفسارات" || model.customerModel.customerChat.text.Trim()=="Queries")
            {
                model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();
                model.customerModel.CustomerStepModel.ChatStepId=20;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=13;
                activities= HyundaiQueriesDialog(model);

            }
            else if (model.customerModel.customerChat.text.Trim()=="حجز موعد" || model.customerModel.customerChat.text.Trim()=="Appointment Booking")
            {
                model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();
                model.customerModel.CustomerStepModel.ChatStepId=23;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=13;
                activities = HyundaiAppointmentSelectNameDialog(model);

            }
            else if (model.customerModel.customerChat.text.Trim()=="قطع الغيار" || model.customerModel.customerChat.text.Trim()=="Spare Parts")
            {
                model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();
                model.customerModel.CustomerStepModel.ChatStepId=17;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=13;
                activities = HyundaiSparePartsDialog(model);

            }
            else if (model.customerModel.customerChat.text.Trim()=="اقتراحات و شكاوي" || model.customerModel.customerChat.text.Trim()=="Car Prices")
            {
                model.customerModel.CustomerStepModel.Department1=model.customerModel.customerChat.text.Trim();
                model.customerModel.CustomerStepModel.ChatStepId=15;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=13;
                activities= HyundaiComplaintsDialog(model);
            }
            else if (model.customerModel.customerChat.text.Trim()=="موقع الصيانة" || model.customerModel.customerChat.text.Trim()=="Maintenance Site")
            {
                model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();
                var text = caption.Where(x => x.TextResourceId==1202 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

                var Summary = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                Bot = new Activity
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
            }
            else
            {
                model.customerModel.CustomerStepModel.ChatStepId=13;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                activities= HyundaiMaintenanceDialog(model);
            }
            return activities;
        }

        private List<Activity> HyundaiComplaintsDialog(BotStepModel model)
        {
            List<Activity> Activity = new List<Activity>();
            // check Main     
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            var text =   caption.Where(x => x.TextResourceId==1201 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var op3 = "";
          
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                //text = "الرجاء الاختيار";

                op1 = "قطع غيار";
                op2 = "صيانة";
                op3 = "مبيعات";
              

                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                //text = "Please choose";

                op1 = "Spare Parts";
                op2 = "Maintenance";
                op3 = "Sales";
                

                Summary = "To change the language, send a #";
                InputHint="Please Select";
            }

            List<Attachment> ss = new List<Attachment>();
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
            cardActions.Add(new CardAction()
            {
                Title=op3,
                Value=op3
            });
           

            List<string> Buttons = new List<string>();

            foreach (var bt in cardActions)
            {
                Buttons.Add(bt.Title);

            }
            model.customerModel.customerChat.IsButton=true;
            model.customerModel.customerChat.Buttons=Buttons;

            //List<Activity> activities = new List<Activity>();
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
                //InputHint=InputHint


            };
            Activity.Add(Bot);

            model.customerModel.CustomerStepModel.ChatStepId=16;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=14;
            return Activity;
        }
        private List<Activity> CheckHyundaiComplaintsDialog(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            // check Main
            if (model.customerModel.customerChat.text.Trim()=="قطع غيار" || model.customerModel.customerChat.text.Trim()=="Spare Parts"||model.customerModel.customerChat.text.Trim()=="صيانة" || model.customerModel.customerChat.text.Trim()=="Maintenance"||model.customerModel.customerChat.text.Trim()=="مبيعات" || model.customerModel.customerChat.text.Trim()=="Sales")
            {
                model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();

                model.customerModel.customerChat.text="دردشة مباشرة";
                activities= CheckLiveChatDialog(model);


            }else
            {
                model.customerModel.CustomerStepModel.ChatStepId=15;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                activities= HyundaiComplaintsDialog(model);
            }
            return activities;
        }


        private List<Activity> HyundaiSparePartsDialog(BotStepModel model)
        {
           
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1203 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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

            model.customerModel.CustomerStepModel.ChatStepId=18;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=17;


            return activities;
        }
        private List<Activity> HyundaiSparePartsDisDialog(BotStepModel model)
        {

            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1204 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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

            model.customerModel.CustomerStepModel.ChatStepId=19;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=18;


            return activities;
        }


        private List<Activity> HyundaiQueriesDialog(BotStepModel model)
        {
            List<Activity> Activity = new List<Activity>();
            // check Main     
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            var text =   caption.Where(x => x.TextResourceId==1211 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var op3 = "";
            var op4 = "";
            var op5 = "";
            var op6 = "";
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {

                op1 = "زيت الجير المناسب";
                op2 = "زيت المحرك المناسب";
                op3 = "صيانة دورية لسيارات";
                op4 = "موعد الصيانة القادم";
                op5 = "غير وارد الوكالة";
                op6 = "استبدال الاطارات";

                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {

                op1 = "gear oil";
                op2 = "engine oil";
                op3 = "periodic maintenance";
                op4 = "Next maintenance";
                op5 = "agency not including";
                op6 = "replace rires";

                Summary = "To change the language, send a #";
                InputHint="Please Select";
            }

            List<Attachment> ss = new List<Attachment>();
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
            cardActions.Add(new CardAction()
            {
                Title=op3,
                Value=op3
            });
            cardActions.Add(new CardAction()
            {
                Title=op4,
                Value=op4
            });
            cardActions.Add(new CardAction()
            {
                Title=op5,
                Value=op5
            });
            cardActions.Add(new CardAction()
            {
                Title=op6,
                Value=op6
            });

            List<string> Buttons = new List<string>();

            foreach (var bt in cardActions)
            {
                Buttons.Add(bt.Title);

            }
            model.customerModel.customerChat.IsButton=false;
            model.customerModel.customerChat.Buttons=Buttons;

            //List<Activity> activities = new List<Activity>();
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
            Activity.Add(Bot);

            model.customerModel.CustomerStepModel.ChatStepId=21;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=20;
            return Activity;
        }

        private List<Activity> CheckHyundaiQueriesDialog(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            // check Main
            if (model.customerModel.customerChat.text.Trim()=="زيت الجير المناسب" || model.customerModel.customerChat.text.Trim()=="gear oil")
            {
                var text = caption.Where(x => x.TextResourceId==1212 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

                var Summary = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                Bot = new Activity
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
                model.customerModel.CustomerStepModel.ChatStepId=22;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=21;
            }
            else if(model.customerModel.customerChat.text.Trim()=="زيت المحرك المناسب" || model.customerModel.customerChat.text.Trim()=="engine oil")
            {
                var text = caption.Where(x => x.TextResourceId==1213 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

                var Summary = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                Bot = new Activity
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
                model.customerModel.CustomerStepModel.ChatStepId=22;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=21;
            }
            else if (model.customerModel.customerChat.text.Trim()=="صيانة دورية لسيارات" || model.customerModel.customerChat.text.Trim()=="periodic maintenance")
            {
                model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();

                var text =  caption.Where(x => x.TextResourceId==1210 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

               
                var Summary = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak= text,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities= CheckLiveChatDialog(model);
                activities.Add(Bot);
                activities.Reverse();
            }
            else if (model.customerModel.customerChat.text.Trim()=="موعد الصيانة القادم" || model.customerModel.customerChat.text.Trim()=="Next maintenance")
            {
                var text = caption.Where(x => x.TextResourceId==1215 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

                var Summary = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                Bot = new Activity
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
                model.customerModel.CustomerStepModel.ChatStepId=22;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=21;
            }
            else if (model.customerModel.customerChat.text.Trim()=="غير وارد الوكالة" || model.customerModel.customerChat.text.Trim()=="agency not including")
            {
                model.customerModel.CustomerStepModel.Department2=model.customerModel.customerChat.text.Trim();

                model.customerModel.customerChat.text="دردشة مباشرة";
                activities= CheckLiveChatDialog(model);
            }
            else if (model.customerModel.customerChat.text.Trim()=="استبدال الاطارات" || model.customerModel.customerChat.text.Trim()=="replace rires")
            {
                var text = caption.Where(x => x.TextResourceId==1216 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

                var Summary = "";
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                Bot = new Activity
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
                model.customerModel.CustomerStepModel.ChatStepId=22;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=21;
            }
            else
            {
                model.customerModel.CustomerStepModel.ChatStepId=20;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=13;
                activities= HyundaiQueriesDialog(model);
            }
            return activities;
        }
        private List<Activity> CheckHyundaiQueriesAndLivChatDialog(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();

            model.customerModel.customerChat.text="دردشة مباشرة";
            activities= CheckLiveChatDialog(model);

            return activities;
        }


        private List<Activity> HyundaiAppointmentSelectNameDialog(BotStepModel model)
        {
        

          
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==1205 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14

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

            model.customerModel.CustomerStepModel.ChatStepId=24;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=23;


            return activities;
        }

        private List<Activity> CheckNameHyundaiAppointmentSelectPicDialog(BotStepModel model)
        {
            if(string.IsNullOrEmpty(model.customerModel.CustomerStepModel.SelectName))
            model.customerModel.CustomerStepModel.SelectName=model.customerModel.customerChat.text.Trim();



            List<CardAction> cardActions = new List<CardAction>();
            var Summary = "";
            var text = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "الرجاء الاختيار";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                cardActions.Add(new CardAction()
                {
                    Title="ارسال رقم اللوحة",
                    Value="ارسال رقم اللوحة"
                });
                cardActions.Add(new CardAction()
                {
                    Title="صورة رقم اللوحة",
                    Value="صورة رقم اللوحة"
                });
            }
            else
            {
                text = "Please choose";
                Summary = "Please send # to return to the main menu";
                cardActions.Add(new CardAction()
                {
                    Title="Send Plate Number",
                    Value="Send Plate Number"
                });
                cardActions.Add(new CardAction()
                {
                    Title="Picture Number",
                    Value="Picture Number"
                });
            }

            
            List<Activity> activities = new List<Activity>();

         
            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Type = ActivityTypes.Message,
                SuggestedActions=new SuggestedActions() { Actions=cardActions },
                Summary=Summary,
                Locale =   "ar",
                Attachments = null
            };
            activities.Add(Bot);


            model.customerModel.CustomerStepModel.ChatStepId=25;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=24;


            return activities;
        }

        private List<Activity> CheckSelectPicHyundaiAppointmentDialog(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            var Summary = "";
            var text = "";

            if(!string.IsNullOrEmpty(model.customerModel.CustomerStepModel.selectTypeNumber))
            {
                model.customerModel.customerChat.text=model.customerModel.CustomerStepModel.selectTypeNumber;
            }

            if (model.customerModel.customerChat.text.Trim()=="ارسال رقم اللوحة" || model.customerModel.customerChat.text.Trim()=="Send Plate Number")
            {
             
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    text = "الرجاء ارسال رقم اللوحة";
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                  
                }
                else
                {
                    text = "Please send the plate number";
                    Summary = "Please send # to return to the main menu";
                  
                }
             
                Bot = new Activity
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
                model.customerModel.CustomerStepModel.ChatStepId=26;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=25;
                model.customerModel.CustomerStepModel.selectTypeNumber=model.customerModel.customerChat.text.Trim();
                model.customerModel.CustomerStepModel.IsPic=false;
                activities = CheckPicAndNumberHyundaiAppointmentDialog(model);

            }
            if (model.customerModel.customerChat.text.Trim()=="صورة رقم اللوحة" || model.customerModel.customerChat.text.Trim()=="Picture Number")
            {
                if (model.customerModel.CustomerStepModel.LangString=="ar")
                {
                    text = "الرجاء ارسال صورة رقم اللوحة";
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";

                }
                else
                {
                    text = "Please send a picture of the plate number";
                    Summary = "Please send # to return to the main menu";

                }
                Bot = new Activity
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
                model.customerModel.CustomerStepModel.ChatStepId=26;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=25;
                model.customerModel.CustomerStepModel.selectTypeNumber=model.customerModel.customerChat.text.Trim();
                model.customerModel.CustomerStepModel.IsPic=true;
                activities= CheckPicAndNumberHyundaiAppointmentDialog(model);
            }
            else
            {
                model.customerModel.CustomerStepModel.ChatStepId=24;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=23;
                activities= CheckNameHyundaiAppointmentSelectPicDialog(model);
            }
            return activities;
        }

        private List<Activity> CheckPicAndNumberHyundaiAppointmentDialog(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity();
            var caption = model.captionDtos;
            // check Main
            if (model.customerModel.customerChat.text.Trim()=="ارسال رقم اللوحة" || model.customerModel.customerChat.text.Trim()=="Send Plate Number")
            {
                model.customerModel.CustomerStepModel.selectTypeNumber=model.customerModel.customerChat.text.Trim();

                model.customerModel.CustomerStepModel.ChatStepId=8;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=26;
                activities= BookTestDriveDialog(model);


            }
            if (model.customerModel.customerChat.text.Trim()=="صورة رقم اللوحة" || model.customerModel.customerChat.text.Trim()=="Picture Number")
            {

                model.customerModel.CustomerStepModel.ChatStepId=8;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=26;
                activities= BookTestDriveDialog(model);
            }
            else
            {
                model.customerModel.CustomerStepModel.ChatStepId=25;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                activities= CheckSelectPicHyundaiAppointmentDialog(model);
            }
            return activities;
        }
        #endregion

    }
}
