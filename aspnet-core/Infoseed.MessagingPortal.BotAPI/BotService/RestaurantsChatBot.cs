using Abp.Runtime.Caching;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using Framework.Data;
using Infoseed.MessagingPortal.BotAPI.Interfaces;
using Infoseed.MessagingPortal.BotAPI.Models;
using Infoseed.MessagingPortal.BotAPI.Models.BotModel;
using Infoseed.MessagingPortal.BotFlow;
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
using Microsoft.Azure.Documents.Spatial;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Constants;
using static Infoseed.MessagingPortal.Customers.Dtos.CustomerBehaviourEnums;
using Attachment = Microsoft.Bot.Connector.DirectLine.Attachment;
namespace Infoseed.MessagingPortal.Bot.API.BotService
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsChatBot : MessagingPortalControllerBase
    {

        private readonly IDocumentClient _IDocumentClient;
        private readonly IDBService _dbService;
        private readonly ICacheManager _cacheManager;
        public IBotApis _botApis;
        public IWhatsAppMessageTemplateAppService _whatsAppMessageTemplateAppService;
        public IBotFlowAppService _botFlowAppService;
        public static Dictionary<string, bool> MessagesSent { get; set; }
        public RestaurantsChatBot(ICacheManager cacheManager, IDBService dbService, IDocumentClient IDocumentClient, IBotApis botApis, IWhatsAppMessageTemplateAppService whatsAppMessageTemplateAppService, IBotFlowAppService botFlowAppService)
        {

            _cacheManager=cacheManager;
            _dbService=dbService;
            _IDocumentClient=IDocumentClient;
            _botApis=botApis;
            _whatsAppMessageTemplateAppService=whatsAppMessageTemplateAppService;
            _botFlowAppService=botFlowAppService;
        }

        [Route("GetTenant")]
        [HttpGet]
        public TenantModel GetTenant(int TenantId)
        {
            var model = _botApis.GetTenantAsync(TenantId).Result;


            return model;

        }

        [Route("DeleteCache")]
        [HttpGet]
        public string DeleteCache(int TenantId)
        {
            var model = _botApis.GetTenantAsync(TenantId).Result;
            _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + TenantId.ToString());
            _cacheManager.GetCache("CacheTenant").Remove(model.D360Key);
            try
            {
                _cacheManager.GetCache("CacheTenant").Remove(model.FacebookPageId);
            }
            catch
            {

            }

            try
            {
                _cacheManager.GetCache("CacheTenant").Remove(model.InstagramId);

            }
            catch
            {

            }
           
          
            return "Done";

        }
        [Route("BotStart")]
        [HttpGet]
        public List<Activity> BotStart(int TenantId, string CustomerPhoneNumber, string text)
        {
            var Customer = GetCustomer(TenantId+"_"+CustomerPhoneNumber);//Get  Customer
            Customer.customerChat.text=text;
            return RestaurantsMessageHandler(Customer);

        }

        [Route("RestaurantsMessageHandler")]
        [HttpPost]
        public List<Activity> RestaurantsMessageHandler(CustomerModel model)
        {

            BotStepModel botStepModel = new BotStepModel();

            TenantModel Tenant = new TenantModel();
            List<CaptionDto> captionDtos = new List<CaptionDto>();

            CacheFun(model, out Tenant, out captionDtos);

            botStepModel.customerModel=model;
            botStepModel.captionDtos=captionDtos;
            botStepModel.tenantModel=Tenant;





            List<Activity> Bot = new List<Activity>();

    


            if (botStepModel.customerModel.IsHumanhandover&& !botStepModel.tenantModel.isReplyAfterHumanHandOver)
            {

                return new List<Activity>();
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
            ///end old

            try
            {
                botStepModel.Bot=Bot;
                UpdateCustomer(botStepModel);//update  Customer
                var x = BotSendToWhatsApp(botStepModel);
            }
            catch
            {

            }







            //if (MessagesSent == null)
            //    MessagesSent = new Dictionary<string, bool>();

            //MessagesSent.TryAdd(model.userId, false);


            //if (!MessagesSent[model.userId])
            //{
            //    try
            //    {
            //        MessagesSent[model.userId] = true;



            //        ///new

            //        //BotFlow(model, botStepModel, Bot);

            //        ///end new



            //        ///old
            //        Bot=TriggersFun(botStepModel, Bot);
            //        botStepModel.Bot=Bot;

            //        if (Bot.Count>0)
            //        {

            //        }
            //        else
            //        {
            //            Bot = Temp1(botStepModel);

            //        }
            //        ///end old

            //        botStepModel.Bot=Bot;
            //        UpdateCustomer(botStepModel);//update  Customer
            //        var x = BotSendToWhatsApp(botStepModel);
            //        MessagesSent[model.userId] = false;

            //    }
            //    catch
            //    {
            //        MessagesSent[model.userId] = false;

            //    }



            //}




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

        private static void SettingBot(BotStepModel botStepModel)
        {
            if (botStepModel.tenantModel.IsMenuLinkFirst)
            {
                botStepModel.customerModel.CustomerStepModel.LangId=1;
                botStepModel.customerModel.CustomerStepModel.LangString="ar";

                botStepModel.customerModel.CustomerStepModel.OrderTypeId="Pickup";
                botStepModel.customerModel.CustomerStepModel.ChatStepId=3;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                //if (botStepModel.customerModel.customerChat.text.ToLower()=="*")
                //{
                //    PervoiusStep(botStepModel);
                //}
            }
            else
            {
                if (botStepModel.tenantModel.IsPickup && !botStepModel.tenantModel.IsDelivery&& !botStepModel.tenantModel.IsPreOrder && !botStepModel.tenantModel.IsInquiry)
                {
                    botStepModel.customerModel.CustomerStepModel.OrderTypeId="Pickup";
                    botStepModel.customerModel.CustomerStepModel.ChatStepId=3;
                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                    //if (botStepModel.customerModel.customerChat.text.ToLower()=="*")
                    //{
                    //    PervoiusStep(botStepModel);
                    //}
                }
                if (!botStepModel.tenantModel.IsPickup && botStepModel.tenantModel.IsDelivery && !botStepModel.tenantModel.IsPreOrder && !botStepModel.tenantModel.IsInquiry)
                {
                    botStepModel.customerModel.CustomerStepModel.OrderTypeId="Delivery";
                    botStepModel.customerModel.CustomerStepModel.ChatStepId=3;
                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                    //if (botStepModel.customerModel.customerChat.text.ToLower()=="*")
                    //{
                    //    PervoiusStep(botStepModel);
                    //}
                }
                if (!botStepModel.tenantModel.IsPickup && !botStepModel.tenantModel.IsDelivery && botStepModel.tenantModel.IsPreOrder && !botStepModel.tenantModel.IsInquiry)
                {
                    botStepModel.customerModel.CustomerStepModel.OrderTypeId="Delivery";
                    botStepModel.customerModel.CustomerStepModel.ChatStepId=9;
                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                    //if (botStepModel.customerModel.customerChat.text.ToLower()=="*")
                    //{
                    //    PervoiusStep(botStepModel);
                    //}
                }
                if (!botStepModel.tenantModel.IsPickup && !botStepModel.tenantModel.IsDelivery && !botStepModel.tenantModel.IsPreOrder && botStepModel.tenantModel.IsInquiry)
                {
                    botStepModel.customerModel.CustomerStepModel.OrderTypeId="Delivery";
                    botStepModel.customerModel.CustomerStepModel.ChatStepId=7;
                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                    //if (botStepModel.customerModel.customerChat.text.ToLower()=="*")
                    //{
                    //    PervoiusStep(botStepModel);
                    //}
                }

            }




        }

        private List<Activity> TriggersFun(BotStepModel botStepModel, List<Activity> Bot)
        {

            if (botStepModel.customerModel.CustomerStepModel.ChatStepId==0&& !botStepModel.customerModel.IsHumanhandover)
            {


                botStepModel.customerModel.customerChat.text="#";

            }

            if (botStepModel.customerModel.CustomerStepModel.ChatStepId==-1)
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


            }
            if (botStepModel.customerModel.customerChat.text.Trim()=="اللغة" || botStepModel.customerModel.customerChat.text.ToLower().Trim()=="language")
            {
                //Bot= ChatStepTriggersEvaluationQuestionl(botStepModel);
                botStepModel.customerModel.CustomerStepModel.ChatStepId=0;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            }

            if (botStepModel.customerModel.customerChat.text.Trim()=="EvaluationQuestion")
            {
                //Bot= ChatStepTriggersEvaluationQuestionl(botStepModel);
                botStepModel.customerModel.CustomerStepModel.ChatStepId=14;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            }

            if (botStepModel.customerModel.customerChat.text=="⏰تواصي"|| botStepModel.customerModel.customerChat.text=="⏰Pre-order")
            {
                // Bot= ChatStepInquiries(botStepModel);
                botStepModel.customerModel.CustomerStepModel.ChatStepId=9;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            }

            if (botStepModel.customerModel.customerChat.text=="❓للاستفسار"|| botStepModel.customerModel.customerChat.text=="❓For Inquiries")
            {
                // Bot= ChatStepInquiries(botStepModel);
                botStepModel.customerModel.CustomerStepModel.ChatStepId=7;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
            }

            if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="cancel"|| botStepModel.customerModel.customerChat.text.Trim()=="الغاء")
            {
                Bot= ChatStepTriggersCancel(botStepModel);
                botStepModel.customerModel.CustomerStepModel.ChatStepId=6;
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

                // Bot=TenantLanguage(botStepModel, Bot);
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
                    var text = botStepModel.customerModel.customerChat.text.Replace("1&&&", "").Replace("2&&&", "").Trim();
                    if (listbut.Contains(text))
                    {

                        if (wor.ActionEn=="live chat")
                        {
                            Bot= ChatStepInquiries(botStepModel);
                            botStepModel.customerModel.CustomerStepModel.ChatStepId=8;
                            botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=7;

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
                    model.customerModel.CustomerStepModel.OrderId=0;
                    if (model.tenantModel.IsMenuLinkFirst)
                    {
                        model.customerModel.CustomerStepModel.ChatStepId=model.customerModel.CustomerStepModel.ChatStepId+1;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=model.customerModel.CustomerStepModel.ChatStepPervoiusId+1;
                        model.customerModel.CustomerStepModel.LangId=1;
                        model.customerModel.CustomerStepModel.LangString="ar";
                        model.customerModel.customerChat.text="العربية";
                        Bot=Temp1(model);

                    }
                    else if (model.tenantModel.IsBotLanguageAr&&model.tenantModel.IsBotLanguageEn)
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
                        Bot=Temp1(model);
                    }
                    break;
                case 1:
                    model.customerModel.CustomerStepModel.OrderId=0;
                    SettingBot(model);
                    if (model.customerModel.CustomerStepModel.ChatStepId==1)
                    {
                        Bot = step1(model);
                    }
                    else
                    {
                        if (model.tenantModel.IsBotLanguageAr && model.customerModel.customerChat.text=="العربية")
                        {
                            model.customerModel.CustomerStepModel.LangId=1;
                            model.customerModel.CustomerStepModel.LangString="ar";
                        }
                        if (model.tenantModel.IsBotLanguageEn && model.customerModel.customerChat.text=="English")
                        {
                            model.customerModel.CustomerStepModel.LangId=2;
                            model.customerModel.CustomerStepModel.LangString="en";
                        }
                        Bot= Temp1(model);
                    }

                    break;
                case 2:
                    model.customerModel.CustomerStepModel.OrderId=0;
                    SettingBot(model);
                    if (model.customerModel.CustomerStepModel.ChatStepId==2)
                    {
                        Bot=step2(model);
                    }
                    else
                    {
                        Bot= Temp1(model);
                    }

                    break;
                case 3:
                    //if (model.customerModel.CustomerStepModel.OrderId!=0)
                    //{
                    //    model.customerModel.CustomerStepModel.ChatStepId=13;
                    //    model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                    //    Bot= Temp1(model);
                    //}
                    //else
                    //{
                        SettingBot(model);
                        if (model.customerModel.CustomerStepModel.IsLinkMneuStep)
                        {
                            model.customerModel.CustomerStepModel.ChatStepId=4;
                            model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;
                            Bot= Temp1(model);
                        }
                        else
                        {

                            Bot= step3(model);
                        }

                    //}

                    break;
                case 4:
                    Bot = step4(model);
                    break;
                case 5:
                    Bot= step5(model);
                    break;
                case 6:
                    Bot= step6(model);
                    break;
                case 7://For Inquiries                   
                    Bot= ChatStepInquiries(model);
                    model.customerModel.CustomerStepModel.ChatStepId=8;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=7;

                    break;
                case 8://For Inquiries
                    Bot= step8(model);
                    model.customerModel.IsHumanhandover=true;
                    model.customerModel.CustomerStepModel.ChatStepId=0;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                    model.customerModel.CustomerStepModel.IsLiveChat=true;
                    break;
                case 9://Pre order
                    Bot= ChatStepPreorder(model);
                    model.customerModel.CustomerStepModel.ChatStepId=10;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=9;
                    break;
                case 10://Pre order
                    if (model.customerModel.customerChat.text=="اليوم"||model.customerModel.customerChat.text=="بكرا"||model.customerModel.customerChat.text== "Today"||model.customerModel.customerChat.text=="Tomorrow")
                    {
                        Bot= step10(model);
                        model.customerModel.CustomerStepModel.ChatStepId=11;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=10;
                    }
                    else
                    {
                        model.customerModel.CustomerStepModel.ChatStepId=9;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                        Bot= Temp1(model);
                    }
                    break;
                case 11://Pre order

                    var time = _botApis.GetTime(model.customerModel.TenantId.Value, "", model.customerModel.CustomerStepModel.LangString);

                    if (time.Contains(model.customerModel.customerChat.text))
                    {

                        model.customerModel.CustomerStepModel.SelectTime=model.customerModel.customerChat.text;
                        model.customerModel.CustomerStepModel.IsPreOrder=true;
                        Bot= ChatStepDelivery(model);
                        model.customerModel.CustomerStepModel.ChatStepId=3;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                        model.customerModel.CustomerStepModel.OrderTypeId="Delivery";

                    }
                    else
                    {
                        model.customerModel.customerChat.text="اليوم";
                        model.customerModel.CustomerStepModel.ChatStepId=10;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=9;
                        Bot= Temp1(model);
                    }


                    break;
                case 12://Pre order
                    Bot= step12(model);
                    break;
                case 13://Pre order
                    Bot= step13(model);
                    break;

                case 14://Evaluations
                    Bot= step14(model);
                    break;
                case 15://Evaluations
                    Bot= step15(model);
                    break;
            }



            return Bot;
        }

        private List<Activity> TenantLanguage(BotStepModel model, List<Activity> Bot)
        {


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
                }
                if (model.tenantModel.IsBotLanguageEn)
                {
                    model.customerModel.CustomerStepModel.LangId=2;
                    model.customerModel.CustomerStepModel.LangString="en";
                }

            }



            return Bot;
        }

        private static void PervoiusStep(BotStepModel model)
        {
            model.customerModel.CustomerStepModel.ChatStepId=model.customerModel.CustomerStepModel.ChatStepId-1;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=model.customerModel.CustomerStepModel.ChatStepPervoiusId-1;

            if (model.customerModel.CustomerStepModel.ChatStepId<0)
            {
                model.customerModel.CustomerStepModel.ChatStepId=0;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=1;

            }
            if (model.customerModel.CustomerStepModel.OrderTypeId!=""&& model.customerModel.CustomerStepModel.ChatStepId==2)
            {
                model.customerModel.CustomerStepModel.OrderTypeId="";

            }
            if (model.customerModel.CustomerStepModel.IsLinkMneuStep && model.customerModel.CustomerStepModel.ChatStepId==3)
            {
                model.customerModel.CustomerStepModel.IsLinkMneuStep=false;

            }
        }
        #endregion

        #region Step

        private List<Activity> step(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();

            Bot= ChatLang(model);


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
            //model.customerModel.CustomerStepModel.pageSize=8;

            return Bot;
        }
        private List<Activity> step1(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();


            if (model.customerModel.customerChat.text=="العربية")
            {
                model.customerModel.CustomerStepModel.LangId=1;
                model.customerModel.CustomerStepModel.LangString="ar";
                Bot= ChatStart(model);
                model.customerModel.CustomerStepModel.ChatStepId=2;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=1;
                //_cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + model.tenantmodel.tenantModel.TenantId.ToString());

            }
            else if (model.customerModel.customerChat.text=="English")
            {
                model.customerModel.CustomerStepModel.LangId=2;
                model.customerModel.CustomerStepModel.LangString="en";
                Bot= ChatStart(model);
                model.customerModel.CustomerStepModel.ChatStepId=2;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=1;
                // _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + model.tenantmodel.tenantModel.TenantId.ToString());
            }
            else
            {
                Bot= step(model);

            }


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

            return Bot;
        }
        private List<Activity> step2(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();

            if (model.customerModel.CustomerStepModel.OrderTypeId!="")
            {
                switch (model.customerModel.CustomerStepModel.OrderTypeId)
                {
                    case "Pickup":

                        Bot= ChatStepPickup(model);
                        model.customerModel.CustomerStepModel.ChatStepId=3;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                        model.customerModel.CustomerStepModel.OrderTypeId="Pickup";

                        break;

                    case "Delivery":
                        Bot= ChatStepDelivery(model);
                        model.customerModel.CustomerStepModel.ChatStepId=3;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                        model.customerModel.CustomerStepModel.OrderTypeId="Delivery";
                        break;
                }

            }
            else
            {
                if (model.customerModel.customerChat.text=="🚶استلام"|| model.customerModel.customerChat.text=="🚶Pickup")
                {
                    Bot= ChatStepPickup(model);
                    model.customerModel.CustomerStepModel.ChatStepId=3;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                    model.customerModel.CustomerStepModel.OrderTypeId="Pickup";

                }
                else if (model.customerModel.customerChat.text=="🚚توصيل" || model.customerModel.customerChat.text=="🚚Delivery")
                {
                    Bot= ChatStepDelivery(model);
                    model.customerModel.CustomerStepModel.ChatStepId=3;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                    model.customerModel.CustomerStepModel.OrderTypeId="Delivery";

                }
                else
                {
                    model.customerModel.CustomerStepModel.ChatStepId=2;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=1;
                    model.customerModel.CustomerStepModel.OrderTypeId="";
                    Bot= ChatStart(model);
                }

            }



            return Bot;
        }
        private List<Activity> step3(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();


            if (model.customerModel.customerChat.text=="العربية")
            {
                model.customerModel.CustomerStepModel.LangId=1;
                model.customerModel.CustomerStepModel.LangString="ar";
                //_cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + model.tenantmodel.tenantModel.TenantId.ToString());

            }
            else if (model.customerModel.customerChat.text=="English")
            {
                model.customerModel.CustomerStepModel.LangId=2;
                model.customerModel.CustomerStepModel.LangString="en";
                // _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + model.tenantmodel.tenantModel.TenantId.ToString());
            }


            if (model.customerModel.CustomerStepModel.IsLinkMneuStep)
            {
                Bot= ChatStepLink(model);
                model.customerModel.CustomerStepModel.IsLinkMneuStep=true;
                model.customerModel.CustomerStepModel.ChatStepId=4;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;
            }
            else
            {
                switch (model.customerModel.CustomerStepModel.OrderTypeId)
                {
                    case "Pickup":
                        if (model.customerModel.customerChat.text=="اخرى"||model.customerModel.customerChat.text=="Others"||model.customerModel.customerChat.text=="العودة"||model.customerModel.customerChat.text=="Back")
                        {
                            Bot= step2(model);
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
                                        model.customerModel.CustomerStepModel.SelectedAreaId=Branches.Id;
                                        model.customerModel.CustomerStepModel.IsLinkMneuStep=true;

                                        Bot= ChatStepLink(model);
                                        model.customerModel.CustomerStepModel.ChatStepId=4;
                                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;

                                    }
                                    else
                                    {
                                        Bot= step2(model);
                                    }

                                }
                                else
                                {
                                    Bot= step2(model);
                                }

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
                            query=model.customerModel.customerChat.text.Replace("https://maps.google.com/?q=", ""),
                            tenantID=model.tenantModel.TenantId.Value

                        };

                        var location = _botApis.GetlocationUserModel(input);

                        if (location.DeliveryCostBefor!=-1)
                        {
                            model.customerModel.CustomerStepModel.LocationId=location.LocationId;
                            model.customerModel.CustomerStepModel.IsLinkMneuStep=true;

                            model.customerModel.CustomerStepModel.DeliveryCostAfter=location.DeliveryCostAfter;
                            model.customerModel.CustomerStepModel.DeliveryCostBefor=location.DeliveryCostAfter;

                            model.customerModel.CustomerStepModel.AddressLatLong=model.customerModel.customerChat.text;
                            model.customerModel.CustomerStepModel.Address=location.Address;

                            Bot= ChatStepLink(model);
                            model.customerModel.CustomerStepModel.ChatStepId=4;
                            model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;

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
        private List<Activity> step4(BotStepModel model)
        {

            List<Activity> Bot = new List<Activity>();
            var caption = model.captionDtos;


            if (model.customerModel.customerChat.text=="اختبار" && model.customerModel.CustomerStepModel.IsLinkMneuStep)
            {
                GetOrderAndDetailModel sendOrderAndDetailModel = new GetOrderAndDetailModel();
                sendOrderAndDetailModel.DeliveryCostTextTow=caption.Where(x => x.TextResourceId==10 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//10
                sendOrderAndDetailModel.captionQuantityText=caption.Where(x => x.TextResourceId==18 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//18
                sendOrderAndDetailModel.captionAddtionText=caption.Where(x => x.TextResourceId==19 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//19
                sendOrderAndDetailModel.captionTotalText=caption.Where(x => x.TextResourceId==20 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//20
                sendOrderAndDetailModel.captionTotalOfAllText=caption.Where(x => x.TextResourceId==21 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//21

                if (model.customerModel.CustomerStepModel.OrderTypeId=="Pickup")
                {
                    sendOrderAndDetailModel.ContactId=int.Parse(model.customerModel.ContactID);
                    sendOrderAndDetailModel.TenantID=model.tenantModel.TenantId.Value;
                    sendOrderAndDetailModel.lang=model.customerModel.CustomerStepModel.LangString;
                    sendOrderAndDetailModel.MenuType=model.customerModel.CustomerStepModel.SelectedAreaId.ToString();
                    sendOrderAndDetailModel.LocationId=(int)model.customerModel.CustomerStepModel.SelectedAreaId;

                    sendOrderAndDetailModel.isOrderOffer=false;

                }
                else
                {

                    sendOrderAndDetailModel.ContactId=int.Parse(model.customerModel.ContactID);
                    sendOrderAndDetailModel.TenantID=model.tenantModel.TenantId.Value;
                    sendOrderAndDetailModel.lang=model.customerModel.CustomerStepModel.LangString;
                    sendOrderAndDetailModel.MenuType=model.customerModel.CustomerStepModel.SelectedAreaId.ToString();
                    sendOrderAndDetailModel.LocationId=(int)model.customerModel.CustomerStepModel.LocationId;

                    sendOrderAndDetailModel.isOrderOffer=false;

                    sendOrderAndDetailModel.TypeChoes=model.customerModel.CustomerStepModel.OrderTypeId;
                    sendOrderAndDetailModel.deliveryCostBefor=model.customerModel.CustomerStepModel.DeliveryCostBefor;
                    sendOrderAndDetailModel.deliveryCostAfter=model.customerModel.CustomerStepModel.DeliveryCostAfter;
                    sendOrderAndDetailModel.Cost=model.customerModel.CustomerStepModel.DeliveryCostAfter;




                    sendOrderAndDetailModel.LocationInfo=null;

                }
                var Details = _botApis.GetOrderAndDetails(sendOrderAndDetailModel);

                if (Details!=null)
                {
                    model.DetailText= Details.DetailText;


                    Bot = ChatStepOrderDetails(model);
                    model.customerModel.CustomerStepModel.ChatStepId=5;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=4;
                    model.customerModel.CustomerStepModel.IsLinkMneuStep=true;

                    model.customerModel.CustomerStepModel.OrderId=Details.orderId;
                    model.customerModel.CustomerStepModel.OrderTotal=Details.total.Value;
                    model.customerModel.CustomerStepModel.Discount=Details.Discount;
                    if (Details.IsDeliveryOffer)
                    {
                        model.customerModel.CustomerStepModel.DeliveryCostAfter=Details.GetLocationInfo.DeliveryCostAfter;
                        //  model.customerModel.CustomerStepModel.DeliveryCostBefor=Details.GetLocationInfo.DeliveryCostBefor;

                        model.customerModel.CustomerStepModel.IsDeliveryOffer=Details.IsDeliveryOffer;
                        model.customerModel.CustomerStepModel.DeliveryOffer= model.customerModel.CustomerStepModel.DeliveryCostBefor;

                    }

                    if (Details.IsItemOffer)
                    {
                        model.customerModel.CustomerStepModel.IsItemOffer=Details.IsItemOffer;
                        model.customerModel.CustomerStepModel.ItemOffer=Details.Discount;
                        model.customerModel.CustomerStepModel.Discount=Details.Discount;

                    }


                }
                else
                {
                    Bot= step3(model);
                }



            }
            else
            {
                Bot= step3(model);
            }

            return Bot;
        }
        private List<Activity> step5(BotStepModel model)
        {


            List<Activity> Bot = new List<Activity>();

            if (model.customerModel.customerChat.text=="تاكيد" ||model.customerModel.customerChat.text=="Confirm")
            {

                if (model.tenantModel.IsMenuLinkFirst)
                {
                    model.customerModel.CustomerStepModel.OrderTypeId="Delivery";
                    Bot = step13(model);
                    model.customerModel.CustomerStepModel.ChatStepId=13;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                    model.customerModel.CustomerStepModel.OrderTypeId="Delivery";


                }
                else
                {
                    if (model.tenantModel.IsSelectPaymentMethod)//model.tenantModel.IsSelectPayment
                    {
                        Bot= ChatStepSelectPayment(model);
                        model.customerModel.CustomerStepModel.ChatStepId=12;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

                    }
                    else
                    {
                        Bot= ChatStepCreateOrder(model);
                        model.customerModel.CustomerStepModel.BayType="";
                        model.customerModel.CustomerStepModel.ChatStepId=0;
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
                        model.customerModel.CustomerStepModel.SelectDay="";
                        model.customerModel.CustomerStepModel.SelectTime="";
                        model.customerModel.CustomerStepModel.IsPreOrder=false;
                        model.customerModel.CustomerStepModel.TotalPoints=0;

                    

                    }

                }


            }
            else if (model.customerModel.customerChat.text=="الغاء الطلب" || model.customerModel.customerChat.text=="Cancelling Order")
            {
                model.isDelete=true;
                Bot = ChatStepCancelOrder(model);

            }
            else
            {
                model.customerModel.customerChat.text="اختبار";
                Bot = step4(model);
            }


            return Bot;
        }
        private List<Activity> step6(BotStepModel model)
        {


            List<Activity> Bot = new List<Activity>();
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==17 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;

            var Details = _botApis.UpdateCancelOrder(model.tenantModel.TenantId, model.customerModel.customerChat.text, int.Parse(model.customerModel.ContactID), text);

            if (Details.CancelOrder)
            {
                model.isDelete=!Details.IsTrueOrder;
                Bot = ChatStepCancelOrder(model);
            }
            else
            {
                model.isDelete=Details.CancelOrder;
                model.CancelText=Details.TextCancelOrder;
                Bot= ChatStepTriggersCancel(model);
            }


            return Bot;
        }
        private List<Activity> step8(BotStepModel model)
        {
            var text = _botApis.UpdateNewLiveChatAsync(model.tenantModel.TenantId, model.customerModel.phoneNumber).Result;
            model.customerModel.IsHumanhandover=true;
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
        
            return activities;
        }
        private List<Activity> step10(BotStepModel model)
        {
            var day = _botApis.GetDay(model.customerModel.CustomerStepModel.LangString);

            var selectDay = "";
            if (model.customerModel.customerChat.text=="بكرا" || model.customerModel.customerChat.text=="Tomorrow")
            {
                selectDay=day[1];
            }
            else
            {
                selectDay=day[0];
            }

            model.customerModel.CustomerStepModel.SelectDay=selectDay;

            var text = "";
            var Summary = "";
            var InputHint = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text="اختر وقت تسليم الطلب";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text="Choose when to deliver the order";
                Summary = "Please send # to return to the main menu";
                InputHint="Please Select";
            }

            List<CardAction> cardActions = new List<CardAction>();

            var time = _botApis.GetTime(model.customerModel.TenantId.Value, "", model.customerModel.CustomerStepModel.LangString);

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

            return activities;
        }
        private List<Activity> step12(BotStepModel model)
        {


            List<Activity> Bot = new List<Activity>();

            if (model.customerModel.customerChat.text=="فيزا" ||model.customerModel.customerChat.text=="كاش"||model.customerModel.customerChat.text=="visa"||model.customerModel.customerChat.text=="cash")
            {
                model.customerModel.CustomerStepModel.BayType=model.customerModel.customerChat.text;
                Bot = ChatStepCreateOrder(model);
                model.customerModel.CustomerStepModel.BayType="";
                model.customerModel.CustomerStepModel.ChatStepId=0;
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
                model.customerModel.CustomerStepModel.SelectDay="";
                model.customerModel.CustomerStepModel.SelectTime="";
                model.customerModel.CustomerStepModel.IsPreOrder=false;
                model.customerModel.CustomerStepModel.TotalPoints=0;



            }
            else
            {
                model.customerModel.customerChat.text="تاكيد";
                model.customerModel.CustomerStepModel.ChatStepId=5;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=4;
                Bot = step5(model);

            }



            return Bot;
        }
        private List<Activity> step13(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();


            switch (model.customerModel.CustomerStepModel.OrderTypeId)
            {
                case "Pickup":
                    if (model.customerModel.customerChat.text=="اخرى"||model.customerModel.customerChat.text=="Others"||model.customerModel.customerChat.text=="العودة"||model.customerModel.customerChat.text=="Back")
                    {
                        Bot= step2(model);
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
                                    model.customerModel.CustomerStepModel.SelectedAreaId=Branches.Id;
                                    model.customerModel.CustomerStepModel.IsLinkMneuStep=true;

                                    Bot= ChatStepLink(model);
                                    model.customerModel.CustomerStepModel.ChatStepId=4;
                                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=3;

                                }
                                else
                                {
                                    Bot= step2(model);
                                }

                            }
                            else
                            {
                                Bot= step2(model);
                            }

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
                        query=model.customerModel.customerChat.text.Replace("https://maps.google.com/?q=", ""),
                        tenantID=model.tenantModel.TenantId.Value

                    };

                    var location = _botApis.GetlocationUserModel(input);

                    if (location.DeliveryCostBefor!=-1)
                    {
                        model.customerModel.CustomerStepModel.LocationId=location.LocationId;
                        model.customerModel.CustomerStepModel.IsLinkMneuStep=true;

                        model.customerModel.CustomerStepModel.DeliveryCostAfter=location.DeliveryCostAfter;
                        model.customerModel.CustomerStepModel.DeliveryCostBefor=location.DeliveryCostAfter;

                        model.customerModel.CustomerStepModel.AddressLatLong=model.customerModel.customerChat.text;
                        model.customerModel.CustomerStepModel.Address=location.Address;
                        model.customerModel.customerChat.text="تاكيد";
                        model.tenantModel.IsMenuLinkFirst=false;
                        Bot = step5(model);
                        model.customerModel.CustomerStepModel.ChatStepId=12;
                        model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

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





            return Bot;
        }
        private List<Activity> step14(BotStepModel model)
        {
            List<Activity> Bot = new List<Activity>();

            var op1 = "";
            var op2 = "";
            var op3 = "";
            var op4 = "";
            var op5 = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                op1 = "⭐ ( ضعيف)";
                op2 = "⭐⭐ ( مقبول)";
                op3 = "⭐⭐⭐ ( جيد)";
                op4 = "⭐⭐⭐⭐ ( جيد جدا)";
                op5 = "⭐⭐⭐⭐⭐ ( ممتاز)";
            }
            else
            {
                op1 = "⭐ ( weak)";
                op2 = "⭐⭐ ( Acceptable)";
                op3 = "⭐⭐⭐ ( Good)";
                op4 = "⭐⭐⭐⭐ ( very good)";
                op5 = "⭐⭐⭐⭐⭐ ( excellent)";
            }
            if (model.customerModel.customerChat.text==op1||model.customerModel.customerChat.text==op2||model.customerModel.customerChat.text==op3||model.customerModel.customerChat.text==op4||model.customerModel.customerChat.text==op5)
            {
                
                Bot = ChatStepEvaluationQuestionl(model);
                model.customerModel.CustomerStepModel.ChatStepId=15;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=14;
                model.customerModel.CustomerStepModel.EvaluationsReat=model.customerModel.customerChat.text;
            }
            else
            {

                Bot = ChatStepTriggersEvaluationQuestionl(model);
            }







            return Bot;
        }
        private List<Activity> step15(BotStepModel model)
        {


            _botApis.CreateEvaluations(model.customerModel.TenantId.Value, model.customerModel.phoneNumber, model.customerModel.displayName, model.customerModel.customerChat.text, model.customerModel.CustomerStepModel.OrderNumber.ToString(),model.customerModel.CustomerStepModel.EvaluationsReat);
            model.customerModel.CustomerStepModel.ChatStepId=0;
            model.customerModel.CustomerStepModel.ChatStepPervoiusId=0;

            model.customerModel.CustomerStepModel.OrderNumber=0;
            model.customerModel.CustomerStepModel.OrderId=0;

            var caption = model.captionDtos;

            var text = caption.Where(x => x.TextResourceId==8 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            //  var text = "يرجى ادخال استفسارك ";//6
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
            return activities;

        }
        #endregion

        #region ChatStep
        private List<Activity> ChatLang(BotStepModel model)
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


            List<string> Buttons = new List<string>();

            Buttons.Add("العربية");
            Buttons.Add("English");
            model.customerModel.customerChat.IsButton=true;
           model.customerModel.customerChat.Buttons=Buttons;
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStart(BotStepModel model)
        {
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==14 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var op3 = "";
            var op4 = "";
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                op1 = "🚚توصيل";
                op2 = "🚶استلام";
                op3 = "❓للاستفسار";
                op4 = "⏰تواصي";
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
                op1 = "🚚Delivery";
                op2 = "🚶Pickup";
                op3 = "❓For Inquiries";
                op4 = "⏰Pre-order";
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

            if (model.tenantModel.IsDelivery)
            {


                cardActions.Add(new CardAction()
                {
                    Title=op1,
                    Value=op1,
                    Image=model.tenantModel.Image
                });
            }
            if (model.tenantModel.IsPickup)
            {


                cardActions.Add(new CardAction()
                {
                    Title=op2,
                    Value=op2,
                    Image=model.tenantModel.Image
                });
            }
            if (model.tenantModel.IsInquiry)
            {


                cardActions.Add(new CardAction()
                {
                    Title=op3,
                    Value=op3,
                    Image=model.tenantModel.Image
                });
            }
            if (model.tenantModel.IsPreOrder)
            {
                cardActions.Add(new CardAction()
                {
                    Title=op4,
                    Value=op4,
                    Image=model.tenantModel.Image
                });
            }
            List<Activity> activities = new List<Activity>();

            if (cardActions.Count()<=3)
            {

                List<string> Buttons = new List<string>();

                foreach(var bt in cardActions)
                {
                    Buttons.Add(bt.Title);

                }
                model.customerModel.customerChat.IsButton=true;
               model.customerModel.customerChat.Buttons=Buttons;


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



            }
            else
            {
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
                    SuggestedActions=new SuggestedActions() { Actions=cardActions, },
                    Summary=Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    InputHint=InputHint,
                    Attachments = new List<Attachment>()
                };
                activities.Add(Bot);

            }






            return activities;
        }
        private List<Activity> ChatStepDelivery(BotStepModel model)
        {
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==9 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            var text2 = caption.Where(x => x.TextResourceId==13 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;

            //  var text = "الرجاء ارسال المــوقـــع (ـLocation) 📌";//9

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

            if (model.customerModel.CustomerStepModel.DeliveryCostBefor==-1)
            {
                Activity Bot2 = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text2,
                    Speak= text2,
                    Summary= Summary,
                    Type = ActivityTypes.Message,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };

                activities.Add(Bot2);
            }
            if (model.customerModel.CustomerStepModel.ChatStepId==3 && model.tenantModel.IsPickup)
            {
                Activity Bot2 = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text2,
                    Speak= text2,
                    Summary= Summary,
                    Type = ActivityTypes.Message,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot2);

            }


            Activity Bot = new Activity
            {
                From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                Text = text,
                Speak= text,
                Summary= Summary,
                Type = ActivityTypes.Message,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepPickup(BotStepModel model)
        {
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==12 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            // var text = "يرجى اختيار الفرع:";//12
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
            List<Activity> activities = new List<Activity>();
            List<Attachment> ss = new List<Attachment>();

            Activity Bot = new Activity();

            if (Branches.Count()==1)
            {
                model.customerModel.CustomerStepModel.ChatStepId=3;
                model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                model.customerModel.CustomerStepModel.OrderTypeId="Pickup";
                model.customerModel.customerChat.text=Branches.FirstOrDefault();
                activities =step3(model);
            }
            else
            {


                if (model.tenantModel.IsMenuLinkFirst)
                {
                    model.customerModel.CustomerStepModel.ChatStepId=3;
                    model.customerModel.CustomerStepModel.ChatStepPervoiusId=2;
                    model.customerModel.CustomerStepModel.OrderTypeId="Pickup";
                    model.customerModel.customerChat.text=Branches.FirstOrDefault();
                    activities =step3(model);


                }
                else
                {

                    if (Branches.Count()<=3)
                    {

                        List<string> Buttons = new List<string>();

                        foreach (var bt in cardActions)
                        {
                            Buttons.Add(bt.Title);

                        }
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
                        List<string> Buttons = new List<string>();

                        foreach (var bt in cardActions)
                        {
                            Buttons.Add(bt.Title);

                        }
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
                }



            }





            return activities;
        }
        private List<Activity> ChatStepInquiries(BotStepModel model)
        {
            var caption = model.captionDtos;

            var text = caption.Where(x => x.TextResourceId==6 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            //  var text = "يرجى ادخال استفسارك ";//6
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
            return activities;
        }
        private List<Activity> ChatStepPreorder(BotStepModel model)
        {
            var caption = model.captionDtos;
            var text = "";// caption.Where(x => x.TextResourceId==14 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "اختر وقت تسليم الطلب";
                op1 = "اليوم";
                op2 = "بكرا";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text = "Choose when to deliver the order";
                op1 = "Today";
                op2 = "Tomorrow";
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

            List<Activity> activities = new List<Activity>();
            Activity Bot = new Activity
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
            activities.Add(Bot);


            return activities;
        }
        private List<Activity> ChatStepLink(BotStepModel model)
        {
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==2 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            var text1 = "الرجاء الضغط على الرابط\r\n\r\n👇👇👇👇👇👇\r\n *https://infoseedordersystem-stg.azurewebsites.net/Index1?TenantID={0}&ContactId={1}&PhoneNumber={2}&Menu={3}&LanguageBot={4}&lang={5}&OrderType={6}*\r\n👆👆👆👆👆👆";//2

            if (model.customerModel.CustomerStepModel.OrderTypeId=="Pickup")
            {
                text=text.Replace("{3}", model.customerModel.CustomerStepModel.SelectedAreaId.ToString()).Replace("{6}", "0");
            }
            else
            {
                text=text.Replace("{3}", model.customerModel.CustomerStepModel.LocationId.ToString()).Replace("{6}", "1");
            }

            text=text.Replace("{0}", model.tenantModel.TenantId.Value.ToString()).Replace("{1}", model.customerModel.ContactID).Replace("{2}", "").Replace("{4}", model.customerModel.CustomerStepModel.LangId.ToString()).Replace("{5}", model.customerModel.CustomerStepModel.LangString);
            MenuContcatKeyModel menuContcatKeyModel = new MenuContcatKeyModel
            {
                ContactID=int.Parse(model.customerModel.ContactID),

                Value=text
            };
            var link = _botApis.AddMenuContcatKey(menuContcatKeyModel);

            text=link;
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
                Summary=Summary,
                Locale =  model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepOrderDetails(BotStepModel model)
        {



            var text = model.DetailText;
            var text2 = model.DetailText;
            var op1 = "";
            var op2 = "";
            var Summary = "";

            if (model.customerModel.CustomerStepModel.LangString == "ar")
            {
                op1 = "تاكيد";
                op2 = "الغاء الطلب";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                text2 = "هل تريد";
            }
            else
            {
                op1 = "Confirm";
                op2 = "Cancelling Order";
                Summary = "Please send # to return to the main menu";
                text2 = "Do you want";
            }

            List<Activity> activities = new List<Activity>();
            List<CardAction> cardActions = new List<CardAction>();
            cardActions.Add(new CardAction()
            {
                Title = op1,
                Value = op1
            });
            cardActions.Add(new CardAction()
            {
                Title = op2,
                Value = op2
            });

            List<string> Buttons = new List<string>();

            foreach (var bt in cardActions)
            {
                Buttons.Add(bt.Title);

            }
            model.customerModel.customerChat.IsButton = true;
            model.customerModel.customerChat.Buttons = Buttons;

            if (model.DetailText.Length >= 1000)
            {
                Activity Bot2 = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak = text,
                    Type = ActivityTypes.Message,
                    // SuggestedActions=new SuggestedActions() { Actions=cardActions },
                    Summary = Summary,
                    Locale = model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot2);

                Activity Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text2,
                    Speak = text2,
                    Type = ActivityTypes.Message,
                    SuggestedActions = new SuggestedActions() { Actions = cardActions },
                    Summary = Summary,
                    Locale = model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot);

            }
            else
            {
                Activity Bot = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = text,
                    Speak = text,
                    Type = ActivityTypes.Message,
                    SuggestedActions = new SuggestedActions() { Actions = cardActions },
                    Summary = Summary,
                    Locale = model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot);

            }
            
            return activities;
        }
        private List<Activity> ChatStepCreateOrder(BotStepModel model)
        {
            var caption = model.captionDtos;

            UpdateOrderModel updateOrderModel = new UpdateOrderModel();
            updateOrderModel.captionOrderInfoText=caption.Where(x => x.TextResourceId==22 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//22
            updateOrderModel.captionOrderNumberText=caption.Where(x => x.TextResourceId==24 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//24
            //updateOrderModel.captionTotalOfAllOrderText=caption.Where(x => x.TextResourceId==21 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//21
            updateOrderModel.captionEndOrderText=caption.Where(x => x.TextResourceId==25 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//25
                                                                                                                                                                                 // updateOrderModel.aptionAreaNameText=caption.Where(x => x.TextResourceId==28 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//28
            updateOrderModel.captionBranchCostText=caption.Where(x => x.TextResourceId==26 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//26
                                                                                                                                                                                   // updateOrderModel.captionFromLocationText=caption.Where(x => x.TextResourceId==27 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//27

            if (model.customerModel.CustomerStepModel.OrderTypeId=="Pickup")
            {
                updateOrderModel.ContactId=int.Parse(model.customerModel.ContactID);
                updateOrderModel.TenantID=model.tenantModel.TenantId.Value;
                updateOrderModel.OrderTotal=model.customerModel.CustomerStepModel.OrderTotal;
                updateOrderModel.loyalityPoint=0;
                updateOrderModel.OrderId=int.Parse(model.customerModel.CustomerStepModel.OrderId.ToString());

                updateOrderModel.MenuId=int.Parse(model.customerModel.CustomerStepModel.SelectedAreaId.ToString());
                updateOrderModel.BranchId=(int)model.customerModel.CustomerStepModel.SelectedAreaId;
                updateOrderModel.BranchName="";
                updateOrderModel.TypeChoes=model.customerModel.CustomerStepModel.OrderTypeId;
                updateOrderModel.IsSpecialRequest=false;
                updateOrderModel.SpecialRequest="";
                updateOrderModel.BotLocal=model.customerModel.CustomerStepModel.LangString;
                updateOrderModel.BuyType=model.customerModel.CustomerStepModel.BayType;



                updateOrderModel.IsItemOffer=model.customerModel.CustomerStepModel.IsItemOffer;
                updateOrderModel.ItemOffer=model.customerModel.CustomerStepModel.Discount;



                updateOrderModel.isOrderOfferCost=model.customerModel.CustomerStepModel.IsItemOffer;

                //updateOrderModel.IsDeliveryOffer=false;
                //updateOrderModel.DeliveryOffer=0;

            }
            else
            {
                updateOrderModel.ContactId=int.Parse(model.customerModel.ContactID);
                updateOrderModel.TenantID=model.tenantModel.TenantId.Value;
                updateOrderModel.OrderTotal=model.customerModel.CustomerStepModel.OrderTotal;
                updateOrderModel.loyalityPoint=0;
                updateOrderModel.OrderId=int.Parse(model.customerModel.CustomerStepModel.OrderId.ToString());

                updateOrderModel.MenuId=int.Parse(model.customerModel.CustomerStepModel.LocationId.ToString());
                updateOrderModel.BranchId=updateOrderModel.MenuId;
                updateOrderModel.BranchName="";
                updateOrderModel.TypeChoes=model.customerModel.CustomerStepModel.OrderTypeId;
                updateOrderModel.IsSpecialRequest=false;
                updateOrderModel.SpecialRequest="";
                updateOrderModel.BotLocal=model.customerModel.CustomerStepModel.LangString;

                updateOrderModel.IsItemOffer=false;
                updateOrderModel.ItemOffer=0;
                updateOrderModel.DeliveryOffer=model.customerModel.CustomerStepModel.DeliveryCostBefor;


                updateOrderModel.Address=model.customerModel.CustomerStepModel.Address;
                updateOrderModel.DeliveryCostAfter=model.customerModel.CustomerStepModel.DeliveryCostAfter.Value;
                updateOrderModel.DeliveryCostBefor=model.customerModel.CustomerStepModel.DeliveryCostBefor.Value;
                updateOrderModel.IsPreOrder=model.customerModel.CustomerStepModel.IsPreOrder;
                updateOrderModel.SelectDay=model.customerModel.CustomerStepModel.SelectDay;
                updateOrderModel.SelectTime=model.customerModel.CustomerStepModel.SelectTime;
                updateOrderModel.LocationFrom=model.customerModel.CustomerStepModel.AddressLatLong;
                updateOrderModel.BuyType=model.customerModel.CustomerStepModel.BayType;
    


                updateOrderModel.IsItemOffer=model.customerModel.CustomerStepModel.IsItemOffer;
                updateOrderModel.ItemOffer=model.customerModel.CustomerStepModel.Discount;

                updateOrderModel.IsDeliveryOffer=model.customerModel.CustomerStepModel.IsDeliveryOffer;
                updateOrderModel.DeliveryOffer=model.customerModel.CustomerStepModel.DeliveryCostBefor;

                updateOrderModel.isOrderOfferCost=model.customerModel.CustomerStepModel.IsItemOffer;



            }
            updateOrderModel.loyalityPoint=model.customerModel.CustomerStepModel.TotalPoints;
            var text = _botApis.UpdateOrderAsync(updateOrderModel);
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
                Summary=Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepSelectPayment(BotStepModel model)
        {
            var caption = model.captionDtos;


            var text = "";
            var Summary = "";
            var op1 = "";
            var op2 = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                text = "كيف حابب تدفع";
                op1 = "فيزا";
                op2 = "كاش";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                text = "Please select the payment method:";
                op1 = "visa";
                op2 = "cash";
                Summary = "To change the language, send a # ";
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


            List<Activity> activities = new List<Activity>();

            Activity Bot = new Activity
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
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepCancelOrder(BotStepModel model)
        {
            List<Activity> activities = new List<Activity>();

            if (model.isDelete)
            {
                _botApis.DeleteOrderDraft(model.tenantModel.TenantId.Value, int.Parse(model.customerModel.CustomerStepModel.OrderId.ToString()));
            }

            var caption = model.captionDtos;

            var text = caption.Where(x => x.TextResourceId==11 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            //  var text = "تم الغاء طلبك يمكنك الطلب من جديد";//11
            var Summary = "";
            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
            }
            else
            {
                Summary = "Please send # to return to the main menu";
            }

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


            if (model.tenantModel.IsBotLanguageAr&&model.tenantModel.IsBotLanguageEn)
            {

                activities = step1(model);
                activities.Add(Bot);
                activities.Reverse();
            }
            else
            {

                activities.Add(Bot);
            }
           

            return activities;
        }
        private List<Activity> ChatStepEvaluationQuestionl(BotStepModel model)
        {
            var caption = model.captionDtos;

            var text = model.customerModel.CustomerStepModel.EvaluationQuestionText;
            //  var text = "يرجى ادخال استفسارك ";//6
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
            return activities;
        }

        #endregion


        #region ChatStepTriggers
        private List<Activity> ChatStepTriggersCancel(BotStepModel model)
        {
            var caption = model.captionDtos;

            var text = caption.Where(x => x.TextResourceId==3 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;
            //  var text = "الرجاء ادخال رقم الطلب ";//5
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
                Summary= Summary,
                Locale =   model.customerModel.CustomerStepModel.LangString,
                Attachments = null
            };


            if(model.CancelText!=null)
            {
                Activity Bot2 = new Activity
                {
                    From = new ChannelAccount(model.customerModel.userId, model.customerModel.displayName.Trim()),
                    Text = model.CancelText,
                    Speak= model.CancelText,
                    Type = ActivityTypes.Message,
                    Summary= Summary,
                    Locale =   model.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };
                activities.Add(Bot2);
                return activities;
            }
            activities.Add(Bot);
            return activities;
        }
        private List<Activity> ChatStepTriggersEvaluationQuestionl(BotStepModel model)
        {
            var caption = model.captionDtos;
            var text = caption.Where(x => x.TextResourceId==7 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
            var op1 = "";
            var op2 = "";
            var op3 = "";
            var op4 = "";
            var op5 = "";
            var Summary = "";
            var InputHint = "";



            if (model.customerModel.CustomerStepModel.LangString=="ar")
            {
                op1 = "⭐ ( ضعيف)";
                op2 = "⭐⭐ ( مقبول)";
                op3 = "⭐⭐⭐ ( جيد)";
                op4 = "⭐⭐⭐⭐ ( جيد جدا)";
                op5 = "⭐⭐⭐⭐⭐ ( ممتاز)";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
            }
            else
            {
                op1 = "⭐ ( weak)";
                op2 = "⭐⭐ ( Acceptable)";
                op3 = "⭐⭐⭐ ( Good)";
                op4 = "⭐⭐⭐⭐ ( very good)";
                op5 = "⭐⭐⭐⭐⭐ ( excellent)";
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
                    InputHint=InputHint,
                    Attachments = new List<Attachment>()
                };
                activities.Add(Bot);



            return activities;
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
                    if (model.customerModel.channel=="facebook")
                    {
                        result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.FacebookPageId, model.tenantModel.FacebookAccessToken, model.customerModel.channel, null);
                    }
                    else if (model.customerModel.channel == "instagram")
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



                }

            }
        }

        private void UpdateCustomer(BotStepModel model)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0  && a.userId == model.customerModel.userId);//&& a.TenantId== TenantId
            var Result = itemsCollection.UpdateItemAsync(model.customerModel._self, model.customerModel).Result;

            // BotSendToWhatsApp(Tenant, customer, Bot);
            // var CustomerSendChat = _dbService.UpdateCustomerChat(model.customerModel, model.customerModel.userId, model.customerModel.customerChat.text, "text", model.customerModel.TenantId.Value, 0, "", string.Empty, string.Empty, MessageSenderType.Customer, "");

        }

        private CustomerModel GetCustomer(string id)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0 && a.userId == id);//&& a.TenantId== TenantId
            return customerResult.Result;
        }
    }
}
