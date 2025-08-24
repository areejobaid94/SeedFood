using Abp.Application.Services.Dto;
using Abp.Runtime.Caching;
using Framework.Data;
using Infoseed.MessagingPortal.BotAPI.Interfaces;
using Infoseed.MessagingPortal.BotAPI.Models;
using Infoseed.MessagingPortal.BotAPI.Models.BotModel;
using Infoseed.MessagingPortal.BotFlow;
using Infoseed.MessagingPortal.BotFlow.Dtos;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.SocketIOClient;
using Infoseed.MessagingPortal.Teams.Dto;
using Infoseed.MessagingPortal.Web.Controllers;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.Web.Sunshine;
using Infoseed.MessagingPortal.WhatsApp;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.BotFlow.Dtos.GetBotFlowForViewDto;
using Attachment = Microsoft.Bot.Connector.DirectLine.Attachment;
using CustomerStepModel = Infoseed.MessagingPortal.Web.Models.Sunshine.CustomerStepModel;

namespace Infoseed.MessagingPortal.BotAPI.BotService
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlowsChatBot : MessagingPortalControllerBase
    {
        private readonly IDocumentClient _IDocumentClient;
        private readonly IDBService _dbService;
        private readonly ICacheManager _cacheManager;
        public IBotApis _botApis;
        public IWhatsAppMessageTemplateAppService _whatsAppMessageTemplateAppService;
        public IBotFlowAppService _botFlowAppService;

        public static Dictionary<string, bool> MessagesSent { get; set; }
        public FlowsChatBot(ICacheManager cacheManager, IDBService dbService, IDocumentClient IDocumentClient, IBotApis botApis, IWhatsAppMessageTemplateAppService whatsAppMessageTemplateAppService, IBotFlowAppService botFlowAppService)
        {

            _cacheManager=cacheManager;
            _dbService=dbService;
            _IDocumentClient=IDocumentClient;
            _botApis=botApis;
            _whatsAppMessageTemplateAppService=whatsAppMessageTemplateAppService;
            _botFlowAppService=botFlowAppService;
        }


        [Route("FlowsBotDeleteCache")]
        [HttpGet]
        public string FlowsBotDeleteCache(int TenantId)
        {
            var model = _botApis.GetTenantAsync(TenantId).Result;
            _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + TenantId.ToString());
            _cacheManager.GetCache("CacheTenant").Remove(model.D360Key);

            MessagesSent=null;

            return "Done";

        }
        [Route("FlowsBotStart")]
        [HttpGet]
        public List<Activity> FlowsBotStart(int TenantId, string CustomerPhoneNumber, string text)
        {
            var Customer = GetCustomer(TenantId+"_"+CustomerPhoneNumber);//Get  Customer
            Customer.customerChat.text=text;
            return FlowsBotMessageHandler(Customer);

        }
        [Route("testA")]
        [HttpGet]
        public string testA(string model)
        {

           return GetFileTypeFromUrl(model);

        }

        [Route("FlowsBotMessageHandler")]
        [HttpPost]
        public List<Activity> FlowsBotMessageHandler(CustomerModel model)
        {


            if (model.CustomerStepModel.UserParmeter!=null)
            {

                try
                {
                    var FirstMessage = model.CustomerStepModel.UserParmeter["FirstMessage"];
                    var FirstMessageDate = model.CustomerStepModel.UserParmeter.ContainsKey("FirstMessageDate")
                        ? model.CustomerStepModel.UserParmeter["FirstMessageDate"]
                        : null;

                    if (string.IsNullOrEmpty(FirstMessage))
                    {

                        model.CustomerStepModel.UserParmeter.Remove("FirstMessage");
                        model.CustomerStepModel.UserParmeter.Add("FirstMessage", model.customerChat.text);
                    }
                    if (string.IsNullOrEmpty(FirstMessageDate))
                    {
                        model.CustomerStepModel.UserParmeter.Add("FirstMessageDate", DateTime.Now.ToString("yyyy/MM/dd, hh:mm tt"));
                    }
                }
                catch
                {

                    model.CustomerStepModel.UserParmeter.Remove("FirstMessage");
                    model.CustomerStepModel.UserParmeter.Add("FirstMessage", model.customerChat.text);

                    model.CustomerStepModel.UserParmeter.Remove("FirstMessageDate");
                    model.CustomerStepModel.UserParmeter.Add("FirstMessageDate", DateTime.Now.ToString("yyyy/MM/dd, hh:mm tt"));
                }

                try
                {
                    model.CustomerStepModel.UserParmeter["LastMessage"] = model.customerChat.text;

                }
                catch
                {
                    model.CustomerStepModel.UserParmeter["LastMessage"] = model.customerChat.text;

                }


                model.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                model.CustomerStepModel.UserParmeter.Add("PhoneNumber", model.phoneNumber);

                model.CustomerStepModel.UserParmeter.Remove("ContactID");
                model.CustomerStepModel.UserParmeter.Add("ContactID", model.ContactID);

                model.CustomerStepModel.UserParmeter.Remove("TenantId");
                model.CustomerStepModel.UserParmeter.Add("TenantId", model.TenantId.Value.ToString());


            }
            else
            {
                model.CustomerStepModel.UserParmeter=new Dictionary<string, string>();

                model.CustomerStepModel.UserParmeter.Add("PhoneNumber", model.phoneNumber);
                model.CustomerStepModel.UserParmeter.Add("ContactID", model.ContactID);
                model.CustomerStepModel.UserParmeter.Add("TenantId", model.TenantId.Value.ToString());


                model.CustomerStepModel.UserParmeter.Add("FirstMessage", model.customerChat.text);
                model.CustomerStepModel.UserParmeter.Add("FirstMessageDate", DateTime.Now.ToString("yyyy/MM/dd, hh:mm tt"));


                model.CustomerStepModel.UserParmeter.Remove("LastMessage");
                model.CustomerStepModel.UserParmeter.Add("LastMessage", model.customerChat.text);


            }




            BotStepModel botStepModel = new BotStepModel();

            TenantModel Tenant = new TenantModel();
            List<CaptionDto> captionDtos = new List<CaptionDto>();

            CacheFun(model, out Tenant, out captionDtos);

            botStepModel.customerModel=model;
            botStepModel.captionDtos=captionDtos;
            botStepModel.tenantModel=Tenant;



            //if (botStepModel.customerModel.customerChat.text=="#")
            //{
            //    botStepModel.customerModel.IsHumanhandover=false;
            //}

            if (botStepModel.customerModel.IsHumanhandover && !botStepModel.tenantModel.isReplyAfterHumanHandOver)
            {
                return new List<Activity>();
            }

            botStepModel.customerModel.customerChat.text=botStepModel.customerModel.customerChat.text.Replace("https://maps.google.com/?q=", "");
            if (botStepModel.customerModel.customerChat.text==null)
            {
                botStepModel.customerModel.customerChat.text="#";
            }
            if (IsNumeric(botStepModel.customerModel.customerChat.text))
            {
                botStepModel.customerModel.customerChat.text = ConvertArabicDigitsToAscii(botStepModel.customerModel.customerChat.text);
            }

               



            var keyword = _whatsAppMessageTemplateAppService.KeyWordGetByKey(model.TenantId.Value, botStepModel.customerModel.customerChat.text.ToLower().Trim());
                if (keyword!=null)
                {
                    botStepModel.customerModel.CustomerStepModel.ChatStepId=int.Parse(keyword.triggersBotId.ToString());
                }

            

            if (botStepModel.customerModel.customerChat.text.Trim()!="start"&&botStepModel.customerModel.customerChat.text!="ايقاف عمليات الترويج"&&botStepModel.customerModel.customerChat.text!="Stop promotions"&&botStepModel.customerModel.customerChat.text!="clean"&&botStepModel.customerModel.customerChat.text!="EvaluationQuestion"&&(botStepModel.customerModel.CustomerStepModel.ChatStepId==-1||botStepModel.customerModel.CustomerStepModel.ChatStepId==1||botStepModel.customerModel.CustomerStepModel.ChatStepId==0))
            {
                botStepModel.customerModel.customerChat.text="#";

            }

            if(string.IsNullOrEmpty(botStepModel.customerModel.customerChat.text)&& botStepModel.customerModel.customerChat.type=="text")
            {
                botStepModel.customerModel.customerChat.text="#";
            }
            if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="clean")
            {
                botStepModel.customerModel.customerChat.text="#";
                botStepModel.customerModel.CustomerStepModel.UserParmeter=new Dictionary<string, string>();
            }


            List<Activity> Bot = new List<Activity>();


            bool isEndDialog = false;




            //if (MessagesSent == null)
            //    MessagesSent = new Dictionary<string, bool>();

           // MessagesSent.TryAdd(model.userId, false);


            //if (!MessagesSent[model.userId])
            //{
                try
                {
                   // MessagesSent[model.userId] = true;
                    if (botStepModel.customerModel.customerChat.text.Trim()=="EvaluationQuestion")
                    {

                        Bot =ChatStepTriggersEvaluationQuestionl(botStepModel);
                        botStepModel.customerModel.CustomerStepModel.EvaluationsReat="EvaluationQuestion";
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=99;

                    }
                    else if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="start")
                    {
                        CustomerBehaviourModel customerBehaviourModel = new CustomerBehaviourModel()
                        {
                            ContactId=int.Parse(botStepModel.customerModel.ContactID),
                            TenantID=botStepModel.tenantModel.TenantId,
                            Start=true,
                            Stop=false,

                        };
                        _botApis.UpdateCustomerBehavior(customerBehaviourModel);

                        botStepModel.customerModel.CustomerOPT=2;
                        botStepModel.customerBehaviourModel=customerBehaviourModel;

                        Bot =ChatStepTriggersUpdateCustomerBehavior(botStepModel);
                    }
                    else if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="stop")
                    {
                        CustomerBehaviourModel customerBehaviourModel = new CustomerBehaviourModel()
                        {
                            ContactId=int.Parse(botStepModel.customerModel.ContactID),
                            TenantID=botStepModel.tenantModel.TenantId,
                            Start=false,
                            Stop=true,

                        };
                        _botApis.UpdateCustomerBehavior(customerBehaviourModel);
                        botStepModel.customerModel.CustomerOPT=1;
                        botStepModel.customerBehaviourModel=customerBehaviourModel;
                        Bot=ChatStepTriggersUpdateCustomerBehavior(botStepModel);
                    }
                    else if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="stop promotions")
                    {
                        CustomerBehaviourModel customerBehaviourModel = new CustomerBehaviourModel()
                        {
                            ContactId=int.Parse(botStepModel.customerModel.ContactID),
                            TenantID=botStepModel.tenantModel.TenantId,
                            Start=false,
                            Stop=true,

                        };
                        _botApis.UpdateCustomerBehavior(customerBehaviourModel);
                        botStepModel.customerModel.CustomerOPT=1;
                        botStepModel.customerBehaviourModel=customerBehaviourModel;
                        Bot=ChatStepTriggersUpdateCustomerBehavior(botStepModel);
                    }
                    else if (botStepModel.customerModel.customerChat.text.ToLower().Trim()=="ايقاف عمليات الترويج")
                    {
                        CustomerBehaviourModel customerBehaviourModel = new CustomerBehaviourModel()
                        {
                            ContactId=int.Parse(botStepModel.customerModel.ContactID),
                            TenantID=botStepModel.tenantModel.TenantId,
                            Start=false,
                            Stop=true,
               
                        };
                        _botApis.UpdateCustomerBehavior(customerBehaviourModel);
                        botStepModel.customerModel.CustomerOPT=1;
                        botStepModel.customerBehaviourModel=customerBehaviourModel;
                        Bot=ChatStepTriggersUpdateCustomerBehavior(botStepModel);
                    }
                    else
                    {//in here 
                        BotFlowAsync(model, botStepModel, ref Bot ,ref isEndDialog);
                    }

                if (isEndDialog)
                {
                    isEndDialog=false;
                    BotFlowAsync(model, botStepModel, ref Bot, ref isEndDialog);

                }
              
                   

                    botStepModel.Bot=Bot;
                    UpdateCustomer(botStepModel);//update  Customer
                    var x = BotSendToWhatsApp(botStepModel, botStepModel.customerModel.customerChat.text);
                   // MessagesSent[model.userId] = false;

                }
                catch
                {
                   // MessagesSent[model.userId] = false;

                }



            //}




            return Bot;
        }

        private static async Task AddToAI(string questions ,string answer,string name)
        {

            QNAAdd qNAAdd = new QNAAdd();
            List<QNAAdd.Class1> class1 = new List<QNAAdd.Class1>();
            QNAAdd.Class1 class11 = new QNAAdd.Class1();

            class11.op="add";

            List<string> strings = new List<string>();
            strings.Add(questions);

            class11.value=new QNAAdd.Value()
            {
                answer=answer,
                questions=strings.ToArray(),
                dialog=new QNAAdd.Dialog() { isContextOnly=false },
                metadata=new QNAAdd.Metadata(),
                source= "FromHasanBotFlow"


            };

            class1.Add(class11);
           var sss=class1.ToArray();
            var qNAAddstring = JsonConvert.SerializeObject(sss);
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Patch, "https://qnasupportinfoseed.cognitiveservices.azure.com/language/query-knowledgebases/projects/infoseedA/qnas?api-version=2021-10-01");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "fc14e8bb1aef4111917424d1b5f78632");
            var content = new StringContent(qNAAddstring, null, "application/json");
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private List<Activity> ChatStepTriggersEvaluationQuestionl(BotStepModel model)
        {
            var caption = model.captionDtos;
            var text = model.tenantModel.EvaluationText;// caption.Where(x => x.TextResourceId==7 && x.LanguageBotId==model.customerModel.CustomerStepModel.LangId).FirstOrDefault().Text;//14
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
                op1 = "⭐ ( ضعيف)";
                op2 = "⭐⭐ ( مقبول)";
                op3 = "⭐⭐⭐ ( جيد)";
                op4 = "⭐⭐⭐⭐ ( جيد جدا)";
                op5 = "⭐⭐⭐⭐⭐ ( ممتاز)";
                Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                InputHint="الرجاء الاختيار";
                //op1 = "⭐ ( weak)";
                //op2 = "⭐⭐ ( Acceptable)";
                //op3 = "⭐⭐⭐ ( Good)";
                //op4 = "⭐⭐⭐⭐ ( very good)";
                //op5 = "⭐⭐⭐⭐⭐ ( excellent)";
                //Summary = "To change the language, send a #";
                //InputHint="Please Select";
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
        private void BotFlowAsync(CustomerModel model, BotStepModel botStepModel, ref List<Activity> Bot ,ref bool isEndDialog)
        {
            isEndDialog = false;
            GetBotModelFlowForViewDto botflowModels = new GetBotModelFlowForViewDto();
            bool istemp = false;
            if (model.channel=="facebook")
            {
     
                    botflowModels = _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.isPublished && x.StatusId!=4 &&x.BotChannel=="facebook").FirstOrDefault();
 
            }
            else if (model.channel=="instagram")
            {

                botflowModels = _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.isPublished && x.StatusId!=4 &&x.BotChannel=="instagram").FirstOrDefault();

            }
            else
            {
                if (model.IsTemplateFlow)
                {
                    TimeSpan timeSpan = DateTime.UtcNow - model.TemplateFlowDate.Value;
                    int totalHours = (int)Math.Ceiling(timeSpan.TotalHours);


                    if (totalHours <= 24)
                    {


                        botflowModels = _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.isPublished && x.StatusId==4&&x.BotChannel=="whatsapp").FirstOrDefault();
                        if (botflowModels!=null)
                        {
                            if (botflowModels.getBotFlowForViewDto.FirstOrDefault().templateId!=model.templateId)
                            {
                                var id = _botFlowAppService.GetBotFlowsById(long.Parse(model.templateId)).Result;
                                botflowModels = _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.isPublished && x.StatusId==4 && x.getBotFlowForViewDto.FirstOrDefault().templateId==id.ToString()&&x.BotChannel=="whatsapp").FirstOrDefault();

                                if (botflowModels==null)
                                {
                                    botflowModels = _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.isPublished && x.StatusId!=4&&x.BotChannel=="whatsapp").FirstOrDefault();

                                }

                            }
                            else
                            {
                                //botStepModel.customerModel.CustomerStepModel.ChatStepId= botflowModels.getBotFlowForViewDto.FirstOrDefault()..childId.Value;
                                istemp=true;

                            }

                        }
                        else
                        {
                            botflowModels = _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.isPublished && x.StatusId!=4&&x.BotChannel=="whatsapp").FirstOrDefault();


                        }




                    }
                    else
                    {
                        botflowModels = _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.isPublished && x.StatusId!=4&&x.BotChannel=="whatsapp").FirstOrDefault();
                        model.IsTemplateFlow=false;
                    }


                }
                else
                {
                    botflowModels = _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.isPublished && x.StatusId!=4&&x.BotChannel=="whatsapp").FirstOrDefault();


                }
            }
     
           // botflowModels= _botFlowAppService.GetAllBotFlows(model.TenantId.Value).Result.Items.ToList().Where(x => x.Id==293 && x.StatusId!=4).FirstOrDefault();
            botStepModel.customerModel.CustomerStepModel.LangString="en";
            botStepModel.customerModel.CustomerStepModel.LangId=2;


            var flow = botflowModels.getBotFlowForViewDto.ToList();

            if (botStepModel.customerModel.CustomerStepModel.ChatStepId==-1)
            {
                var firstitem = flow.Where(x => x.isNodeRoot.Value).FirstOrDefault();
                if (firstitem!=null)
                {
                    if (firstitem.childId==null)
                    {

                        if (firstitem.content.dtoContent.FirstOrDefault().childId!=null)
                        {
                            botStepModel.customerModel.CustomerStepModel.ChatStepId= firstitem.content.dtoContent.FirstOrDefault().childId.Value;

                        }

                    }
                    else
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId= firstitem.childId.Value;


                    }

                }

            }

            if (botStepModel.customerModel.customerChat.text.Trim()=="#" || model.IsReStart)
            {
                model.IsReStart=false;
                var firstitem = flow.Where(x => x.isNodeRoot.Value).FirstOrDefault();
                if (firstitem!=null)
                {


                        botStepModel.customerModel.CustomerStepModel.ChatStepId= firstitem.id.Value;
                        try
                        {
                            botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId= firstitem.parentIndex[0];

                        }
                        catch
                        {

                        }
                    
                    
                    botStepModel.customerModel.getBotFlowForViewDto=null;
                    //botStepModel.customerModel.CustomerStepModel.UserParmeter=new Dictionary<string, string>();
                }

            }


            //check the user send 

            var nextstepId = 0;
            var PervoiusstepId = 0;

            if (botStepModel.customerModel.getBotFlowForViewDto!=null)
            {

                
                if (!string.IsNullOrEmpty(botStepModel.customerModel.getBotFlowForViewDto.parameter))
                {
   

                    if (botStepModel.customerModel.getBotFlowForViewDto.parameter=="Name")
                    {
                        botStepModel.customerModel.displayName=botStepModel.customerModel.customerChat.text.Trim();
                        _botApis.UpdaateDisplayName(botStepModel.customerModel.ContactID, botStepModel.customerModel.customerChat.text.Trim());

                    }
                    if (botStepModel.customerModel.getBotFlowForViewDto.parameter.Trim()=="Location")
                    {
                        botStepModel.customerModel.Description=botStepModel.customerModel.customerChat.text.Trim();
                        botStepModel.customerModel.Website=botStepModel.customerModel.customerChat.text.Trim();
                        _botApis.UpdaateLocation(botStepModel.customerModel.ContactID, botStepModel.customerModel.customerChat.text.Trim());

                    }
                    if (botStepModel.customerModel.getBotFlowForViewDto.type == "Cataloge multity Product"  || botStepModel.customerModel.getBotFlowForViewDto.type == "Cataloge Single Product")
                    {
                        if (!botStepModel.customerModel.customerChat.text.StartsWith("📦 New Order:"))
                        {
                            botStepModel.customerModel.CustomerStepModel.ChatStepId = botStepModel.customerModel.getBotFlowForViewDto.id.Value;
                            try
                            {
                                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = botStepModel.customerModel.getBotFlowForViewDto.parentIndex[0];
                            }
                            catch
                            {

                            }

                        }

                    }
                    if (botStepModel.customerModel.getBotFlowForViewDto.type=="ScheduleNode" ||botStepModel.customerModel.getBotFlowForViewDto.type=="Branches" || botStepModel.customerModel.getBotFlowForViewDto.type == "BranchesEN" || botStepModel.customerModel.getBotFlowForViewDto.type=="Reply buttons" || botStepModel.customerModel.getBotFlowForViewDto.type=="List options"|| botStepModel.customerModel.getBotFlowForViewDto.type=="Language")
                    {
                        int intValue;
                        if (int.TryParse(botStepModel.customerModel.customerChat.text.Trim(), out intValue) && intValue<=botStepModel.customerModel.getBotFlowForViewDto.content.dtoContent.Length)
                        {

                            var xx = botStepModel.customerModel.getBotFlowForViewDto.content.dtoContent[intValue-1];
                            // String is a valid integer
                            botStepModel.customerModel.CustomerStepModel.ChatStepId=xx.childId.Value;
                            try
                            {
                                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=xx.parentIndex[0];

                            }
                            catch
                            {

                            }

                            botStepModel.customerModel.customerChat.text=xx.valueEn;
                            //TODO : add parm to contact 

                            botStepModel.customerModel.CustomerStepModel.UserParmeter.Remove(botStepModel.customerModel.getBotFlowForViewDto.parameter);

                            if (model.getBotFlowForViewDto.type == "Branches" || model.getBotFlowForViewDto.type == "BranchesEN")
                            {
                                botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, xx.branchID);

                                //model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                            }
                            else
                            {
                                botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, botStepModel.customerModel.customerChat.text.Trim());

                            }


                        }
                        else
                        {
                            // String is not a valid integer
                            var istrue = false;
                            foreach (var xx in botStepModel.customerModel.getBotFlowForViewDto.content.dtoContent)
                            {


                                if (xx.valueEn.Trim()==botStepModel.customerModel.customerChat.text.Trim() || xx.valueAr.Trim()==botStepModel.customerModel.customerChat.text.Trim())
                                {
                                    if (botStepModel.customerModel.getBotFlowForViewDto.type=="Language")
                                    {

                                        if (xx.valueEn=="العربية"||xx.valueAr=="العربية")
                                        {
                                            botStepModel.customerModel.CustomerStepModel.LangId=1;
                                            botStepModel.customerModel.CustomerStepModel.LangString="ar";
                                        }

                                        if (xx.valueEn=="English"||xx.valueAr=="English")
                                        {
                                            botStepModel.customerModel.CustomerStepModel.LangId=2;
                                            botStepModel.customerModel.CustomerStepModel.LangString="en";
                                        }




                                    }
                                    botStepModel.customerModel.CustomerStepModel.ChatStepId=xx.childId.Value;
                                    try
                                    {
                                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=xx.parentIndex[0];

                                    }
                                    catch
                                    {

                                    }
                                    //TODO : add parm to contact 
                                    istrue =true;
                                    botStepModel.customerModel.CustomerStepModel.UserParmeter.Remove(botStepModel.customerModel.getBotFlowForViewDto.parameter);

                                    if (model.getBotFlowForViewDto.type == "Branches" || model.getBotFlowForViewDto.type == "BranchesEN")
                                    {
                                        botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.getBotFlowForViewDto.content.dtoContent.Where(x => x.valueAr == botStepModel.customerModel.customerChat.text.Trim() || x.valueEn == botStepModel.customerModel.customerChat.text.Trim()).FirstOrDefault().branchID);

                                        //model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                                    }
                                    else
                                    {
                                        botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, botStepModel.customerModel.customerChat.text.Trim());

                                    }


                                }

                            }
                            if (!istrue)
                            {
                                botStepModel.customerModel.CustomerStepModel.ChatStepId=botStepModel.customerModel.getBotFlowForViewDto.id.Value;
                                try
                                {
                                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=botStepModel.customerModel.getBotFlowForViewDto.parentIndex[0];

                                }
                                catch
                                {

                                }

                            }
                        }


                    



                    }
                    else
                    {
                        //botStepModel.customerModel.CustomerStepModel.ChatStepId=botStepModel.customerModel.getBotFlowForViewDto.childIndex.Value;
                        //botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=botStepModel.customerModel.getBotFlowForViewDto.parentIndex[0];
                        //TODO : add parm to contact 
                        botStepModel.customerModel.CustomerStepModel.UserParmeter.Remove(botStepModel.customerModel.getBotFlowForViewDto.parameter);

                        if (model.getBotFlowForViewDto.type=="Branches" || model.getBotFlowForViewDto.type == "BranchesEN")
                        {
                            botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.getBotFlowForViewDto.content.dtoContent.Where(x => x.valueAr==botStepModel.customerModel.customerChat.text.Trim() || x.valueEn==botStepModel.customerModel.customerChat.text.Trim()).FirstOrDefault().branchID);

                            //model.BotTestStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, model.text.Trim());

                        }
                        else
                        {


                            if (botStepModel.customerModel.getBotFlowForViewDto.parameterType=="JSON"&&botStepModel.customerModel.attachments.Count()>0)
                            {
                                var RetrievingMedia = RetrievingMediaAsync(botStepModel.customerModel.attachments[0].Name, botStepModel.tenantModel.AccessToken).Result;

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


                                var type = RetrievingMedia.mime_type.Split("/")[0];
                                AzureBlobProvider azureBlobProvider = new AzureBlobProvider();

                                AttachmentContent attachmentContent = new AttachmentContent()
                                {
                                    Content = RetrievingMedia.contentByte,
                                    Extension = extention,
                                    MimeType = RetrievingMedia.mime_type,
                                    AttacmentName = RetrievingMedia.id.ToString()
                                };

                                var url = azureBlobProvider.Save(attachmentContent).Result;
                                botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, url);


                            }
                            else
                            {

                                var type = "Text";
                                try
                                {
                                    type= botParameterGetAll(model.TenantId.Value).Result.Items.Where(x => x.Name==model.getBotFlowForViewDto.parameter).First().Format;

                                }
                                catch
                                {


                                }

                                if (type=="Number")
                                {
                                    //checked
                                    bool isNumber = double.TryParse(botStepModel.customerModel.customerChat.text.Trim(), out double resultdouble);
                                    bool isInteger = int.TryParse(botStepModel.customerModel.customerChat.text.Trim(), out int resultint);

                                    if (isNumber || isInteger)
                                    {
                                        botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, botStepModel.customerModel.customerChat.text.Trim());



                                    }
                                    else
                                    {
                                        botStepModel.customerModel.CustomerStepModel.ChatStepId=botStepModel.customerModel.getBotFlowForViewDto.id.Value;
                                        try
                                        {
                                            botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=botStepModel.customerModel.getBotFlowForViewDto.parentIndex[0];

                                        }
                                        catch
                                        {


                                        }

                                    }
                                }
                                else if (botStepModel.customerModel.getBotFlowForViewDto.parameterType=="JSON")
                                {
                                    botStepModel.customerModel.CustomerStepModel.ChatStepId=botStepModel.customerModel.getBotFlowForViewDto.id.Value;
                                    try
                                    {
                                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=botStepModel.customerModel.getBotFlowForViewDto.parentIndex[0];

                                    }
                                    catch
                                    {

                                    }

                                }
                                else
                                {
                                    botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(model.getBotFlowForViewDto.parameter, botStepModel.customerModel.customerChat.text.Trim());

                                }






                            }

                        }
                    }


                    botStepModel.customerModel.getBotFlowForViewDto=null;
                }







            }




            if (botStepModel.customerModel.CustomerStepModel.EvaluationsReat=="EvaluationQuestion")
            {


                _botApis.CreateEvaluations(model.TenantId.Value, botStepModel.customerModel.phoneNumber, botStepModel.customerModel.displayName, botStepModel.customerModel.customerChat.text, botStepModel.customerModel.CustomerStepModel.OrderNumber.ToString(), botStepModel.customerModel.customerChat.text);


                botStepModel.customerModel.CustomerStepModel.ChatStepId=-1;
                botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=0;
                botStepModel.customerModel.customerChat.text="#";
                botStepModel.customerModel.CustomerStepModel.EvaluationsReat=null;

                //////
                var text = "شكرا لتقييم خدماتنا";// botStepModel.tenantModel.EvaluationText;
                //  var text = "يرجى ادخال استفسارك ";//6
                var Summary = "";
                if (botStepModel.customerModel.CustomerStepModel.LangString=="ar")
                {
                    Summary = "الرجاء ارسال # للعودة للقائمة الرئسية";
                }
                else
                {
                    Summary = "Please send # to return to the main menu";
                }
                List<Activity> activities = new List<Activity>();
                Activity Bot2 = new Activity
                {
                    From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                    Text = text,
                    Speak= text,
                    Type = ActivityTypes.Message,
                    Summary = Summary,
                    Locale =   botStepModel.customerModel.CustomerStepModel.LangString,
                    Attachments = null
                };

                activities.Add(Bot2);
                Bot=activities;

            }
            else
            {


                nextstepId = 0;
                PervoiusstepId = 0;
                bool isButton = false;
                bool iskeyword=false;


                int count = 0;
                while (!isButton)
                {

                    if (count>=150)
                    {
                        break;
                    }

                    //key word 
                    if (!iskeyword)
                    {     
                        var keyword = _whatsAppMessageTemplateAppService.KeyWordGetByKey(model.TenantId.Value, botStepModel.customerModel.customerChat.text.ToLower().Trim());
                        if (keyword!=null && botflowModels.FlowName==keyword.action)
                        {
                            botStepModel.customerModel.CustomerStepModel.ChatStepId=int.Parse(keyword.triggersBotId.ToString());
                            iskeyword=true;

                            keyword.KeyUse=keyword.KeyUse+1;
                            keyword.tenantId=botStepModel.customerModel.TenantId.Value;
                            _whatsAppMessageTemplateAppService.KeyWordUpdate(keyword);
                        }

                    }
              


                    var fw = flow.Where(x => x.id==botStepModel.customerModel.CustomerStepModel.ChatStepId).FirstOrDefault();
                    if (fw==null)
                    {

                        fw=flow.Where(x => x.isNodeRoot.Value).FirstOrDefault();
                        isButton=true;
                    }
                    if(fw.captionAr=="WhatsApp template")
                    {
                        if (fw.childId==null )//if buttons
                        {
                            botStepModel.customerModel.CustomerStepModel.ChatStepId= fw.content.dtoContent[0].childId.Value;
                            fw = flow.Where(x => x.id==botStepModel.customerModel.CustomerStepModel.ChatStepId).FirstOrDefault();

                        }
                        else
                        {
                            botStepModel.customerModel.CustomerStepModel.ChatStepId= fw.childId.Value;
                            fw = flow.Where(x => x.id==botStepModel.customerModel.CustomerStepModel.ChatStepId).FirstOrDefault();

                        }
                        
                    }



                    //one time 
                    bool isfound = false;
                    if (fw.isOneTimeQuestion && !string.IsNullOrEmpty(fw.parameter))
                    {
                        try
                        {
                            if (botStepModel.customerModel.CustomerStepModel.UserParmeter.ContainsKey(fw.parameter))
                            {
                                //  Key  exists in the dictionary

                                var x = botStepModel.customerModel.CustomerStepModel.UserParmeter[fw.parameter];
                                if (!string.IsNullOrEmpty(x))
                                {
                                    try
                                    {
                                        if (fw.content!=null)
                                        {
                                            if (fw.content.dtoContent.Count()>0)
                                            {
                                                botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.content.dtoContent.FirstOrDefault().childId.Value;
                                            }
                                            else
                                            {

                                                botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                                            }


                                        }
                                        else
                                        {

                                            botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                                        }

                                    }
                                    catch
                                    {

                                        if (fw.childId!=null)
                                        {
                                            botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;

                                        }
                                        else
                                        {
                                            break;

                                        }
                                    }

                                    continue;
                                }
                            }

                          
                        }
                        catch
                        {

                            botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                            continue;

                        }


                    }

                    //end one time




                    TypeMassgeFun(botStepModel, Bot, ref nextstepId, ref PervoiusstepId, ref isButton, fw, isfound);





                    if (fw.type=="End")
                    {

                        isEndDialog=true;

                    }
                   
                    if (isButton && isfound)
                    {
                 
                        isButton=false;
                    }
                 
                    if (isButton|| fw.childIndex==-1)
                    {
                 
                        break;
                    }

                    

           
                    count++;



                }
            }




            
        }
        private static bool IsNumeric(string input)
        {
            // Normalize the string to replace Arabic-Indic digits with ASCII digits
            input = ConvertArabicDigitsToAscii(input);

            // Check if the resulting string can be parsed to an integer
            return long.TryParse(input, out _);
        }
        private static string ConvertArabicDigitsToAscii(string input)
        {
            string[] arabicDigits = { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };
            for (int i = 0; i < arabicDigits.Length; i++)
            {
                input = input.Replace(arabicDigits[i], i.ToString());
            }
            return input;
        }
        private void TypeMassgeFun(BotStepModel botStepModel, List<Activity> Bot, ref int nextstepId, ref int PervoiusstepId, ref bool isButton, GetBotFlowForViewDto fw,  bool isfound)
        {
            if (fw!=null)
            {
                // Define the regular expression pattern
                string pattern = @"\$(\w+)";

                if (fw.type=="whatsApp template")
                {
                    botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.id.Value;
                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                }
                if (fw.type=="Send Message")
                {
                    var Summary = "";
                    var text = "";
                    if (botStepModel.customerModel.CustomerStepModel.LangString=="ar")
                    {
                        text = fw.captionAr;
                        Summary =fw.footerTextAr;// "الرجاء ارسال # للعودة للقائمة الرئسية";
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary =fw.footerTextEn;// "Please send # to return to the main menu";
                    }


                 
                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        result=result.Replace(")", "");
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                            var textModel= GetTextFromModel(result, diVAlue);
                            text= text.Replace("$"+result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                            text= text.Replace("$"+result, diVAlue);

                        }



                 


                    }

                    if (text.Contains("IMG("))
                    {

                        text=text.Replace("IMG(", "").Replace(")", "");
                        fw.urlImage.url=text;
                    }



         
                    List<Activity> activities = new List<Activity>();
                    Activity Botf = new Activity();


                    if (fw.urlImageArray!=null && fw.urlImageArray.Length>0)
                    {
                        foreach (var activity in fw.urlImageArray)
                        {
                            string img = activity.url;
                            string fileType = GetFileTypeFromUrl(img);
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = img,
                                Speak= activity.name,
                                Type = ActivityTypes.Message,
                                InputHint=fileType,
                                Summary = Summary,
                                Locale =   botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };
                            if (!isfound) 
                            Bot.Add(Botf);

                        }

                    }
                    else
                    {

                        if (fw.urlImage!=null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            string img = fw.urlImage.url;
                            string fileType = GetFileTypeFromUrl(img);
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = img,
                                Speak= fw.urlImage.name,
                                Type = ActivityTypes.Message,
                                InputHint=fileType,
                                Summary = Summary,
                                Locale =   botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };

                            if (!isfound)
                             Bot.Add(Botf);
                        }
                        else
                        {
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = text,
                                Speak= text,
                                Type = ActivityTypes.Message,
                                Summary = Summary,
                                Locale =   botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };
                            if (!isfound)
                                Bot.Add(Botf);
                        }
                    }





                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=-1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                    }
                    



                }
                if (fw.type=="Reply buttons")
                {
                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();


                    string img = null;
                    var InputHint = "";

                    if (fw.urlImageArray!=null && fw.urlImageArray.Length>0)
                    {

                        img=fw.urlImageArray.FirstOrDefault().url;



                    }
                    else
                    {

                        if (fw.urlImage!=null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            img=fw.urlImage.url;

                        }



                    }
                    try
                    {
                        if (string.IsNullOrEmpty(fw.InputHint))
                        {
                            fw.InputHint=null;
                        }

                    }
                    catch
                    {
                        fw.InputHint=null;
                    }




                    foreach (var bt in fw.content.dtoContent)
                    {

                        if (botStepModel.customerModel.CustomerStepModel.LangString=="ar")
                        {
                            cardActions.Add(new CardAction()
                            {
                                Title=bt.valueAr,
                                Value=bt.valueAr,
                                Image=img
                            });
                            Buttons.Add(bt.valueAr);
                        }
                        else
                        {

                            cardActions.Add(new CardAction()
                            {
                                Title=bt.valueEn,
                                Value=bt.valueEn,
                                Image=img
                            });
                            Buttons.Add(bt.valueEn);
                        }


                    }
                    var Summary = "";
                    var text = "";
                    if (botStepModel.customerModel.CustomerStepModel.LangString=="ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                    }

                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text= text.Replace("$"+result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                            text= text.Replace("$"+result, diVAlue);

                        }

                    }
                    Activity Botf = new Activity();

                    /////546456///

                    Botf = new Activity
                    {
                        From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                        Text = text,
                        // Speak= text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        InputHint=fw.InputHint,
                        Locale =  botStepModel.customerModel.CustomerStepModel.LangString,
                        Attachments = null
                    };



                    //isButton=true;
                    botStepModel.customerModel.customerChat.IsButton=true;
                    botStepModel.customerModel.customerChat.Buttons=Buttons;
                    if (!isfound)
                        Bot.Add(Botf);
                   // botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                   // botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                }
                if (fw.type == "List options" || fw.type == "Branches" || fw.type == "BranchesEN")
                {
                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();
                    botStepModel.customerModel.CustomerStepModel.LangString = "en";

                    string img = null;
                    var InputHint = "";

                    if (fw.urlImageArray != null && fw.urlImageArray.Length > 0)
                    {

                        img = fw.urlImageArray.FirstOrDefault().url;



                    }
                    else
                    {

                        if (fw.urlImage != null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            img = fw.urlImage.url;

                        }



                    }
                    try
                    {
                        if (string.IsNullOrEmpty(fw.InputHint))
                        {
                            fw.InputHint = null;
                        }

                    }
                    catch
                    {
                        fw.InputHint = null;
                    }
                    if (fw.type == "Branches")
                    {
                        botStepModel.customerModel.CustomerStepModel.LangString = "ar";
                    }
                    else if (fw.type == "BranchesEN")
                    {
                        botStepModel.customerModel.CustomerStepModel.LangString = "en";
                    }

                    foreach (var bt in fw.content.dtoContent)
                        {

                            if (botStepModel.customerModel.CustomerStepModel.LangString == "ar")
                            {
                                cardActions.Add(new CardAction()
                                {
                                    Title = bt.valueAr,
                                    Value = bt.valueAr,
                                    Image = img
                                });
                                Buttons.Add(bt.valueAr);
                            }
                            else
                            {

                                cardActions.Add(new CardAction()
                                {
                                    Title = bt.valueEn,
                                    Value = bt.valueEn,
                                    Image = img
                                });
                                Buttons.Add(bt.valueEn);
                            }


                        }
                    var Summary = "";
                    var text = "";
                    try
                    {
                        if (string.IsNullOrEmpty(fw.InputHint))
                        {
                            fw.InputHint = "Please check";
                        }

                    }
                    catch
                    {
                        fw.InputHint = "Please check";
                    }


                    if (botStepModel.customerModel.CustomerStepModel.LangString == "ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                        InputHint = fw.InputHint;// "الرجاء الاختيار";
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                        InputHint = fw.InputHint; //"Please check";
                    }

                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text = text.Replace("$" + result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                            text = text.Replace("$" + result, diVAlue);

                        }

                    }


                    List<Attachment> ss = new List<Attachment>();
                    Activity Botf = new Activity();
                    if (fw.isAdvance && botStepModel.customerModel.channel.ToLower() != "facebook" && botStepModel.customerModel.channel.ToLower() != "instagram")
                    {
                        InputHint = "منطقتك";
                        if (fw.urlImageArray.Length > 0)
                        {
                            img = fw.urlImageArray[0].url;


                        }

                        Botf = new Activity
                        {
                            From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                            Text = text,
                            Speak = img,
                            Type = ActivityTypes.Message,
                            SuggestedActions = new SuggestedActions() { Actions = cardActions },
                            Summary = Summary,
                            Locale = botStepModel.customerModel.CustomerStepModel.LangString,
                            InputHint = InputHint,
                            Attachments = ss
                        };


                    }
                    else
                    {

                        Botf = new Activity
                        {
                            From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                            Text = text,
                            // Speak= text,
                            Type = ActivityTypes.Message,
                            SuggestedActions = new SuggestedActions() { Actions = cardActions },
                            Summary = Summary,
                            Locale = botStepModel.customerModel.CustomerStepModel.LangString,
                            InputHint = InputHint,
                            Attachments = ss
                        };
                    }



                    //isButton=true;
                    botStepModel.customerModel.customerChat.IsButton = true;
                    botStepModel.customerModel.customerChat.Buttons = Buttons;
                    if (!isfound)
                        Bot.Add(Botf);
                    // botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                    // botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                }

                if (fw.type=="Language")
                {
                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();

                    string img = null;
                    if (!string.IsNullOrEmpty(fw.urlImage.url))
                    {
                        img=fw.urlImage.url;

                    }
                    foreach (var bt in fw.content.dtoContent)
                    {

                        if (botStepModel.customerModel.CustomerStepModel.LangString=="ar")
                        {
                            cardActions.Add(new CardAction()
                            {
                                Title=bt.valueAr,
                                Value=bt.valueAr,
                                Image=img
                            });
                            Buttons.Add(bt.valueAr);
                        }
                        else
                        {

                            cardActions.Add(new CardAction()
                            {
                                Title=bt.valueEn,
                                Value=bt.valueEn,
                                Image=img
                            });
                            Buttons.Add(bt.valueEn);
                        }


                    }
                    var Summary = "";
                    var text = "";
                    if (botStepModel.customerModel.CustomerStepModel.LangString=="ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                    }


                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text= text.Replace("$"+result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                            text= text.Replace("$"+result, diVAlue);

                        }






                    }
                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                        Text = text,
                        Speak= text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =  botStepModel.customerModel.CustomerStepModel.LangString,
                        Attachments = null
                    };


                    //isButton=true;
                    botStepModel.customerModel.customerChat.IsButton=true;
                    botStepModel.customerModel.customerChat.Buttons=Buttons;
                    if (!isfound)
                        Bot.Add(Botf);

                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=-1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                    }

                }
                if (fw.type=="Delay")
                {
                  

                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                        Text = (fw.dilationTime*1000).ToString(),
                        Speak= (fw.dilationTime*1000).ToString(),
                        Type = ActivityTypes.Ping,
                        Summary=(fw.dilationTime*1000).ToString(),
                        Locale =  botStepModel.customerModel.CustomerStepModel.LangString,
                        Attachments = null
                    };
                    if (!isfound)
                        Bot.Add(Botf);

                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=-1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                    }




                }
                if (fw.type=="Human handover")
                {
                    var text = "";
                    var Summary = "";
                    if (fw.actionBlock=="request")
                    {

                        var info = "";
                        var listparm = fw.parameter.Split(",");

                        foreach(var item in listparm)
                        {                              
                                try
                                {                                   
                                    var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[item];                           
                                    info=info+diVAlue+",";

                                }
                                catch
                                {

                                }

                        }

                        UpdateSaleOfferModel updateSaleOfferModel = new UpdateSaleOfferModel();


                        if (!string.IsNullOrEmpty(fw.listOfTeams))
                        {


                            var teams = teamsGetAll("", 0, 100000);


                            var list = "";


                            foreach(var te in teams.TeamsDtoModel)
                            {

                                if (fw.listOfTeams.Contains(te.Id.ToString()))
                                {

                                    list=list+","+te.UserIds;

                                }


                            }
                            string result = string.Join(",", list.Split(',').Distinct());


                            updateSaleOfferModel.UserIds=result;
                            fw.listOfUsers=result;


                        }
                        else
                        {


                            updateSaleOfferModel.UserIds=fw.listOfUsers;

                            fw.listOfUsers=fw.listOfUsers;
                        }




                        updateSaleOfferModel.PhoneNumber=botStepModel.customerModel.phoneNumber;
                        updateSaleOfferModel.TenantID=botStepModel.tenantModel.TenantId.Value;
                        updateSaleOfferModel.ContactName=botStepModel.customerModel.displayName;
                        updateSaleOfferModel.ContactId=int.Parse(botStepModel.customerModel.ContactID);
                        updateSaleOfferModel.information=info;



                         _botApis.SendNewPrescription(updateSaleOfferModel);


                        //get list user email fw.listOfUsers
                        if(botStepModel.customerModel.TenantId == 282|| botStepModel.customerModel.TenantId == 440|| botStepModel.customerModel.TenantId == 599)
                        {

                            _botApis.SendEmailAsync("", "request", info, fw.listOfUsers);
                        }


                    }
                    else
                    {

                        var info = "";
                        var listparm = fw.parameter.Split(",");






                        if (!string.IsNullOrEmpty(fw.listOfTeams))
                        {

                            var teams = teamsGetAll("", 0, 100000, botStepModel.tenantModel.TenantId.Value);


                            var list = "";


                            foreach (var te in teams.TeamsDtoModel)
                            {

                                if (fw.listOfTeams.Contains(te.Id.ToString()))
                                {

                                    list=list+","+te.UserIds;

                                }


                            }
                            string result = string.Join(",", list.Split(',').Distinct());


                            fw.listOfUsers=result;


                        }
                        else
                        {

                            fw.listOfUsers=fw.listOfUsers;
                        }







                        foreach (var item in listparm)
                        {
                            try
                            {
                                var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[item];
                                info=info+diVAlue+",";

                            }
                            catch
                            {

                            }

                        }

                        text = _botApis.UpdateNewLiveChatAsync(botStepModel.tenantModel.TenantId, botStepModel.customerModel.phoneNumber, "", "", 0, fw.listOfUsers, info).Result;

                        //get list user email  fw.listOfUsers

                        //get list user email fw.listOfUsers
                        if (botStepModel.customerModel.TenantId == 282 ||botStepModel.customerModel.TenantId == 440 || botStepModel.customerModel.TenantId == 599)
                        {
                             _botApis.SendEmailAsync("", "Tickets", info, fw.listOfUsers);
                        }
                        botStepModel.customerModel.IsHumanhandover=true;

                    }



                    if (text=="")
                    {
                        if (botStepModel.customerModel.CustomerStepModel.LangString=="ar")
                        {
                            text = fw.captionAr;
                            Summary =fw.footerTextAr;// "الرجاء ارسال # للعودة للقائمة الرئسية";
                        }
                        else
                        {
                            text = fw.captionEn;
                            Summary =fw.footerTextEn;// "Please send # to return to the main menu";
                        }
                    }
                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text= text.Replace("$"+result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                            text= text.Replace("$"+result, diVAlue);

                        }


                    }
                    List<Activity> activities = new List<Activity>();
                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                        Text = text,
                        Speak= text,
                        Type = ActivityTypes.Message,
                        Summary = Summary,
                        Locale =   botStepModel.customerModel.CustomerStepModel.LangString,
                        Attachments = null
                    };
                    if (!isfound)
                        Bot.Add(Botf);

                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=-1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                    }

                }
                if (fw.type=="Jump")
                {

                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.jumpId;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.jumpId;
              
                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=-1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=-1;


                    }

                    fw.childIndex=fw.jumpId;
                    //botStepModel.BotTestStepModel.ChatStepId=fw.childId.Value;
                    //botStepModel.BotTestStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                }

                if (fw.type=="Http request")
                {

                    var client = new HttpClient();
                    var mathod = HttpMethod.Get;

                    if (fw.request.httpMethod=="POST")
                    {
                        mathod = HttpMethod.Post;

                    }
                    int count = fw.request.url.Count(c => c == '$');
                    for (int i = 0; i<count; i++)
                    {
                        // Use Regex.Match to find the first match
                        Match match = Regex.Match(fw.request.url, pattern);
                        if (match.Success)
                        {
                            // Extract the word after '$' using the captured group
                            try
                            {
                                string wordAfterDollar = match.Groups[1].Value;
                                var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[wordAfterDollar];
                                //if (diVAlue.Contains("$"))
                                //{

                                //    count++;
                                //}
                                diVAlue=diVAlue.Replace("$", "");
                                fw.request.url= fw.request.url.Replace("$"+wordAfterDollar, diVAlue);
                                //fw.request.url= text.Replace("$", "");

                            }
                            catch
                            {

                            }

                        }


                    }
                    fw.request.url= fw.request.url.Replace("$", "");


                    var request = new HttpRequestMessage(mathod, fw.request.url);
                

                    if (fw.request.httpMethod=="POST")
                    {
                        if (!string.IsNullOrEmpty(fw.request.token))
                        {
                            request.Headers.Add("Authorization", "Bearer "+fw.request.token);

                        }
                        string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                        MatchCollection matches = Regex.Matches(fw.request.body, pattern3);
                        foreach (Match match in matches)
                        {
                            string result = match.Value;
                            if (result.Contains(".") || result.Contains("["))
                            {
                                var word = result.Split(".")[0];
                                var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                                var textModel = GetTextFromModel(result, diVAlue);
                                fw.request.body= fw.request.body.Replace("$"+result, textModel);

                            }
                            else
                            {
                                var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                                fw.request.body= fw.request.body.Replace("$"+result, diVAlue);

                            }


                        }
                        var body = fw.request.body;//.Replace("\"", "\\\"");
                        var content = new StringContent(body, null, fw.request.contentType);
                        request.Content = content;
                    }


                    var response = client.SendAsync(request).Result;
                    response.EnsureSuccessStatusCode();

                    try
                    {
                        var rwz2 = JsonConvert.DeserializeObject<ResponseHttpModel>(response.Content.ReadAsStringAsync().Result);
                        if (rwz2!=null &&!string.IsNullOrEmpty( rwz2.result))
                        {
                            botStepModel.customerModel.CustomerStepModel.UserParmeter.Remove(fw.parameter);
                            botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(fw.parameter, rwz2.result);
                        }
                        else
                        {
                            botStepModel.customerModel.CustomerStepModel.UserParmeter.Remove(fw.parameter);
                            botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(fw.parameter, response.Content.ReadAsStringAsync().Result);
                        }
                       
                    }
                    catch
                    {
                        string responseContent =  response.Content.ReadAsStringAsync().Result;

                        // Parse the response content to a JObject
                        JObject jsonResponse = JObject.Parse(responseContent);

                        // Extract the "result" property
                        JObject result = (JObject)jsonResponse["result"];

                        // Serialize the "result" object back to a formatted JSON string
                        string formattedJson = JsonConvert.SerializeObject(result, Formatting.Indented);


                        botStepModel.customerModel.CustomerStepModel.UserParmeter.Remove(fw.parameter);
                        botStepModel.customerModel.CustomerStepModel.UserParmeter.Add(fw.parameter, formattedJson);

                    }
                  
                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentId[0];

                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId=-1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=-1;


                    }

                    ///get customer
                    try
                    {
                        if (fw.request.url.Contains("EndDialog"))
                        {
                          //  var Customer = GetCustomer(botStepModel.customerModel.userId);//Get  Customer

                            botStepModel.customerModel.IsTemplateFlow = false;
                            botStepModel.customerModel.templateId = "";
                            botStepModel.customerModel.CampaignId="";
                            botStepModel.customerModel.TemplateFlowDate=null;

                          //  botStepModel.customerModel=Customer;
                        }

                    }
                    catch
                    {

                    }
            

                }
                if (fw.type=="Condition")
                {
                    if (fw.conditional!=null)
                    {


                        bool rez = false;
                        foreach (var op in fw.conditional.conditionList)
                        {





                            // Use Regex.Match to find the first match
                            Match match = Regex.Match(op.op1, pattern);
                            if (match.Success)
                            {
                                // Extract the word after '$' using the captured group
                                try
                                {
                                    string wordAfterDollar = match.Groups[1].Value;
                                    var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[wordAfterDollar];
                                    op.op1= op.op1.Replace(wordAfterDollar, diVAlue);
                                    op.op1= op.op1.Replace("$", "");

                                }
                                catch
                                {

                                }

                            }
                            Match match2 = Regex.Match(op.op2, pattern);
                            if (match2.Success)
                            {
                                // Extract the word after '$' using the captured group
                                try
                                {
                                    string wordAfterDollar = match2.Groups[1].Value;
                                    var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[wordAfterDollar];
                                    op.op2= op.op2.Replace(wordAfterDollar, diVAlue);
                                    op.op2= op.op2.Replace("$", "");

                                }
                                catch
                                {

                                }

                            }

                            switch (op.operation)
                            {
                                case "Less tham":
                                    if (decimal.Parse(op.op1) < decimal.Parse(op.op2))
                                    {
                                        rez=true;

                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Less or equal to":
                                    if (decimal.Parse(op.op1) <= decimal.Parse(op.op2))
                                    {
                                        rez=true;


                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Greater than":
                                    if (decimal.Parse(op.op1) > decimal.Parse(op.op2))
                                    {

                                        rez=true;

                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Equal to":

                                    if (op.op1 is string)
                                    {
                                        if (op.op1.Equals(op.op2))
                                        {
                                            rez=true;


                                        }
                                        else
                                        {
                                            rez=false;

                                        }
                                    }
                                    else
                                    {
                                        if (decimal.Parse(op.op1) == decimal.Parse(op.op2))
                                        {

                                            rez=true;

                                        }
                                        else
                                        {
                                            rez=false;

                                        }
                                    }

                                    break;
                                case "Not equal to":
                                    if (decimal.Parse(op.op1) != decimal.Parse(op.op2))
                                    {
                                        rez=true;


                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Contains":
                                    if (op.op1.Contains(op.op2))
                                    {
                                        rez=true;

                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;
                                case "Does not contain":
                                    if (!op.op1.Contains(op.op2))
                                    {
                                        rez=true;

                                    }
                                    else
                                    {
                                        rez=false;

                                    }
                                    break;


                            }


                        }


                        if (rez)
                        {
                            foreach (var xx in fw.content.dtoContent)
                            {

                                if (xx.valueEn.Trim() =="Yes" || xx.valueAr.Trim() =="Yes")
                                {
                                    botStepModel.customerModel.CustomerStepModel.ChatStepId = xx.childId.Value;
                                    fw.childIndex=xx.childId.Value;
                                    try
                                    {
                                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = xx.parentIndex[0];

                                    }
                                    catch
                                    {

                                    }
                                }


                            }
                        }
                        else
                        {
                            foreach (var xx in fw.content.dtoContent)
                            {

                                if (xx.valueEn.Trim() =="No" || xx.valueAr.Trim() =="No")
                                {
                                    botStepModel.customerModel.CustomerStepModel.ChatStepId = xx.childId.Value;
                                    fw.childIndex=xx.childId.Value;
                                    try
                                    {
                                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = xx.parentIndex[0];

                                    }
                                    catch
                                    {

                                    }
                                }


                            }

                        }
                        isButton=false;

                    }

               
                }
                if (fw.type=="ScheduleNode")
                {

                    var Summary = "";
                    var text = "";
                    var InputHint = "";

                    var lang = "ar";

                    try
                    {
                        if (string.IsNullOrEmpty(fw.InputHint))
                        {
                            fw.InputHint="Please check";
                        }

                    }
                    catch
                    {
                        fw.InputHint="Please check";
                    }


                    if (fw.InputHint.Contains("AR-API("))
                    {
                        fw.InputHint=fw.InputHint.Replace("AR-", "");
                        lang = "ar";

                    }

                    if (fw.InputHint.Contains("EN-API("))
                    {
                        fw.InputHint=fw.InputHint.Replace("EN-", "");
                        lang = "en";

                    }




                    List<CardAction> cardActions = new List<CardAction>();
                    List<string> Buttons = new List<string>();

                    string img = null;
                    if (!string.IsNullOrEmpty(fw.urlImage.url))
                    {
                        img=fw.urlImage.url;

                    }

                    if (fw.InputHint.Contains("API2("))
                    {
                        fw.InputHint=fw.InputHint.Replace("API2(", "").Replace(")", "");
                        var parm = fw.InputHint.Replace("$", "");
                        var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[parm];

                        Buttons=diVAlue.Split(",").ToList();

                        if (lang=="ar")
                        {
                            InputHint="منطقتك";
                            fw.InputHint="منطقتك";
                        }
                        else
                        {
                            InputHint="منطقتك";
                            fw.InputHint="منطقتك";
                        }

                        Buttons.RemoveAll(item => string.IsNullOrEmpty(item));
                        foreach (var bin in Buttons)
                        {
                            cardActions.Add(new CardAction
                            {
                                Title = bin,
                                Value = bin,
                                Image = img
                            });

                        }

            

                    }
                    else if (fw.InputHint.Contains("API("))
                    {
                        fw.InputHint=fw.InputHint.Replace("API(", "").Replace(")", "");
                        var parm = fw.InputHint.Replace("$", "");                   
                        var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[parm];

                        Buttons=diVAlue.Split(",").ToList();

                        if (lang=="ar")
                        {
                            InputHint="الرجاء الاختيار";
                            fw.InputHint="الرجاء الاختيار";
                        }
                        else
                        {
                            InputHint="Please Check";
                            fw.InputHint="Please Check";
                        }
                  
                        Buttons.RemoveAll(item => string.IsNullOrEmpty(item));

                        var Buttons2 = new List<string>();
                        var buttonMappings = new Dictionary<string, string>
                        {
                             { "Tania 200ml(1 Carton - 48 Bottles)","200ml*48-21.8SR" },
                             { "Tania 200ml (1 Carton - 40 Bottles)","200ml*40-19.26SR" },
                             {  "Tania 330ml(1 Carton - 40 Bottles)","330ml*40-21.8SR" },
                             {  "Tania 3.8L(1 Carton - 4 Bottles)","3.8L*4-16.1SR"},
                             {  "Tania 5L(1 Carton - 4 Bottles)" ,"5L*4-18.09SR"},
                             {  "Tania 1500ml(1 Shrink - 6 Bottles)","1500ml*6-8.98SR" },
                             {  "Tania 330ml (1 Shrink - 20 Bottles)","330ml*20-10.35SR" },
                             {  "Tania 5G Refill(18.9 Liters)","5GRefill-8.5SR" },
                             {  "Tania 5G New(18.9 Liters)","5GNew-13.8SR" },
                             {  "Mocabat ICE(400ml*8)","MocabatICE-12.08SR" },
                             {  "Tania 600 ml (1 Carton - 24 Bottles )" ,"600ml*24-18.18SR"},

                             {  "تانيا 200 مل(1 كرتون - 48 عبوة)" ,"200 مل*48-21.8رس"},
                             { "تانيا 200 مل (1 كرتون - 40 عبوة)" , "200مل*40-19.26رس"},
                             {  "تانيا 330 مل(1 كرتون - 40 عبوة)","330مل*40-21.8رس" },
                             { "تانيا 3.8 لتر(1 كرتون - 4 عبوة)" , "3.8لتر*4-16.1رس"},
                             {  "تانيا 5 لتر(1 كرتون - 4 عبوة)" ,"5لتر*4-18.09رس"},
                             { "تانيا 1500 مل(1 شرنك - 6 عبوة)" ,"1500مل*6-8.98رس"},
                             {  "تانيا 330 مل (1 شرنك - 20 عبوة)" ,"330مل*20-10.35رس"},
                             {  "تانيا 5 جالون - جديد","5جالون-جديد-13.8رس" },
                             {  "تانيا 5 جالون - إعادة تعبئة" ,"5جالون-تعبئة-8.5رس"},
                             {  "مكعبات ثلج(1 كرتون - 8 شريحة)","مكعباتثلج-12.08رس" },
                             {  "تانيا 600 مل(1 كرتون - 24 عبوة)" ,"600مل*24-18.18رس"}
                        };
                        foreach (var bin in Buttons)
                        {
                            try
                            {
                                if (buttonMappings.TryGetValue(bin, out var mappedValue))
                                {
                                    Buttons2.Add(mappedValue);
                                    cardActions.Add(new CardAction
                                    {
                                        Title = mappedValue,
                                        Value = mappedValue,
                                        Image = img
                                    });
                                }
                                else
                                {
                                    // If no mapping is found, use the original value
                                    Buttons2.Add(bin);
                                    cardActions.Add(new CardAction
                                    {
                                        Title = bin,
                                        Value = bin,
                                        Image = img
                                    });
                                }
                            }
                            catch
                            {
                                Buttons2.Add(bin);
                                cardActions.Add(new CardAction
                                {
                                    Title = bin,
                                    Value = bin,
                                    Image = img
                                });

                            }
                           
                        }

                        // Replace the original Buttons list with the updated Buttons2 list
                        Buttons=new List<string>();
                        Buttons = Buttons2;
   


                    }
                    else
                    {

                        if (fw.schedule.isData)
                        {
                            if (fw.schedule.isNow)
                            {
                                DateTime dateTime = DateTime.UtcNow;

                                for (var i = 0; i<fw.schedule.numberButton; i++)
                                {
                                    var date = dateTime.AddDays(i).ToString("M/dd/yyyy");


                                    var dayadd = dateTime.AddDays(i);


                                    CultureInfo arabicCulture = new CultureInfo(lang);
                                    string dayNameInArabic = dayadd.ToString("dddd", arabicCulture);

                                    if (!fw.schedule.unavailableDate.Contains(date))
                                    {

                                        cardActions.Add(new CardAction()
                                        {
                                            Title=date+dayNameInArabic,
                                            Value=date+dayNameInArabic,
                                            Image=img
                                        });
                                        Buttons.Add(date+dayNameInArabic);
                                    }
                                    else
                                    {
                                        fw.schedule.numberButton=fw.schedule.numberButton+1;
                                    }

                                }



                            }
                            else
                            {
                                TimeSpan duration = fw.schedule.startDate.Value-fw.schedule.endDate.Value;

                                // Extract the number of days from the time span
                                int numberOfDays = (int)duration.TotalDays;
                                if (numberOfDays<0)
                                {
                                    numberOfDays=numberOfDays*-1;

                                }

                                if (numberOfDays<=10)
                                {
                                    for (var i = 0; i<numberOfDays; i++)
                                    {
                                        var date = fw.schedule.startDate.Value.AddDays(i).ToString("M/dd/yyyy");

                                        if (!fw.schedule.unavailableDate.Contains(date))
                                        {

                                            cardActions.Add(new CardAction()
                                            {
                                                Title=date,
                                                Value=date,
                                                Image=img
                                            });
                                            Buttons.Add(date);
                                        }
                                        else
                                        {
                                            numberOfDays=numberOfDays+1;
                                        }

                                    }

                                }




                            }


                        }
                        else
                        {
                            if (fw.schedule.isNow)
                            {
                                DateTime dateTime = DateTime.UtcNow;

                                dateTime=dateTime.AddHours(6);

                                for (var i = 0; i<fw.schedule.numberButton; i++)
                                {
                                    var time = dateTime.AddHours(i).ToString("hh:00 tt");

                                    if (!fw.schedule.unavailableDate.Contains(time))
                                    {

                                        cardActions.Add(new CardAction()
                                        {
                                            Title=time,
                                            Value=time,
                                            Image=img
                                        });
                                        Buttons.Add(time);
                                    }
                                    else
                                    {
                                        fw.schedule.numberButton=fw.schedule.numberButton+1;
                                    }

                                }


                            }
                            else
                            {
                                TimeSpan duration = fw.schedule.startDate.Value.AddHours(3) - fw.schedule.endDate.Value.AddHours(3);

                                fw.schedule.startDate = fw.schedule.startDate.Value.AddHours(3);
                                // Extract the number of days from the time span
                                int numberOfDays = (int)duration.TotalHours;
                                if (numberOfDays<0)
                                {
                                    numberOfDays=numberOfDays*-1;

                                }

                                if (numberOfDays<=10)
                                {
                                    for (var i = 0; i<numberOfDays; i++)
                                    {
                                        var time = fw.schedule.startDate.Value.AddHours(i).ToString("hh:mm tt");

                                        if (!fw.schedule.unavailableDate.Contains(time))
                                        {

                                            cardActions.Add(new CardAction()
                                            {
                                                Title=time,
                                                Value=time,
                                                Image=img
                                            });
                                            Buttons.Add(time);
                                        }
                                        else
                                        {
                                            numberOfDays=numberOfDays+1;
                                        }

                                    }

                                }
                            }

                        }

                    }

                 

                    List<GetBotFlowForViewDto.Dtocontent> lst=new List<GetBotFlowForViewDto.Dtocontent>();
                    
                    foreach (var bt in Buttons)
                    {
                        GetBotFlowForViewDto.Dtocontent op = new GetBotFlowForViewDto.Dtocontent();
                        op.valueAr=bt;
                        op.valueEn=bt;
                        op.childId=fw.childId;
                        lst.Add(op);
                    }

                    fw.content.dtoContent= lst.ToArray();
                   


                    if (botStepModel.customerModel.CustomerStepModel.LangString=="ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                        InputHint=fw.InputHint;// "الرجاء الاختيار";
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                        InputHint=fw.InputHint; //"Please check";
                    }
                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text= fw.request.body.Replace("$"+result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                            text= fw.request.body.Replace("$"+result, diVAlue);

                        }


                    }
                   
                    List<Attachment> ss = new List<Attachment>();
                    Activity Botf = new Activity
                    {
                        From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                        Text = text,
                        Type = ActivityTypes.Message,
                        SuggestedActions=new SuggestedActions() { Actions=cardActions },
                        Summary=Summary,
                        Locale =  botStepModel.customerModel.CustomerStepModel.LangString,
                        InputHint=InputHint,
                        Attachments = ss
                    };

                    botStepModel.customerModel.customerChat.IsButton=true;
                    botStepModel.customerModel.customerChat.Buttons=Buttons;
                    if (!isfound)
                        Bot.Add(Botf);
                }
                if (fw.type=="Set Parameter")
                {
                    if (fw.parameterList!=null)
                    {

                        bool rez = false;
                        foreach (var op in fw.parameterList)
                        {

                            try
                            {
                                string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character



                                if (op.val.Contains("Push"))
                                {
                                    var val = op.val.Replace("Push(", "").Replace(")", "");

                                    // Regex pattern to dynamically match all key-value pairs
                                     pattern = "\\\"([a-zA-Z0-9_]+)\\\":\\$([a-zA-Z0-9_]+)";

                                    // Match the regex pattern
                                    MatchCollection matches = Regex.Matches(val, pattern);

                                    if (matches.Count > 0)
                                    {
                                        Console.WriteLine("Extracted Values:");

                                        // Loop through all matches and print key-value pairs
                                        foreach (Match match in matches)
                                        {
                                            string key0 = match.Groups[1].Value;   // The key (e.g., "product_id")
                                            string value0 = match.Groups[2].Value; // The value (e.g., "product_id")
                                            string result = value0;

                                            if (result.Contains(".") || result.Contains("["))
                                            {
                                                var word = result.Split(".")[0];
                                                var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                                                var textModel = GetTextFromModel(result, diVAlue);
                                                op.val= op.val.Replace("$"+result, textModel);

                                            }
                                            else
                                            {
                                                var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                                                op.val= op.val.Replace("$"+result, diVAlue);

                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("No matches found!");
                                    }



                                }
                                else if (op.val.Contains("+")||op.val.Contains("-"))
                                {

                                    var xx=op.val.Replace("+", " ").Replace("-", "");

                                    MatchCollection matches = Regex.Matches(xx, pattern3);
                                    foreach (Match match in matches)
                                    {
                                        string result = match.Value;
                                        if (result.Contains(".") || result.Contains("["))
                                        {
                                            var word = result.Split(".")[0];
                                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                                            var textModel = GetTextFromModel(result, diVAlue);
                                            op.val= op.val.Replace("$"+result, textModel);

                                        }
                                        else
                                        {
                                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                                            op.val= op.val.Replace("$"+result, diVAlue);

                                        }
                                    }



                                    try
                                    {
                                        var result = new DataTable().Compute(op.val, null);
                                        Console.WriteLine($"The result is: {result}");

                                        op.val=result.ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"Error evaluating expression: {ex.Message}");
                                    }


                                   

                                }
                                else
                                {
                                    MatchCollection matches = Regex.Matches(op.val, pattern3);
                                    foreach (Match match in matches)
                                    {
                                        string result = match.Value;
                                        if (result.Contains(".") || result.Contains("["))
                                        {
                                            var word = result.Split(".")[0];
                                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                                            var textModel = GetTextFromModel(result, diVAlue);
                                            op.val= op.val.Replace("$"+result, textModel);

                                        }
                                        else
                                        {
                                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                                            op.val= op.val.Replace("$"+result, diVAlue);

                                        }






                                    }


                                }
                                




                                var key = op.par.Replace("$", "");

                                if (op.val.Contains("Push"))
                                {

                                    botStepModel.customerModel.CustomerStepModel.UserParmeter[key]=botStepModel.customerModel.CustomerStepModel.UserParmeter[key]+","+op.val.Replace("Push(", "").Replace(")", "");

                                }
                                else
                                {


                                    if (op.val.Contains("cls"))
                                    {
                                        botStepModel.customerModel.CustomerStepModel.UserParmeter[key]="";
                                    }
                                    else
                                    {
                                        botStepModel.customerModel.CustomerStepModel.UserParmeter[key]=op.val;

                                    }
                                    
                                    
                                    
                                    
                                    
                                }



                               


                                try
                                {
                                    botStepModel.customerModel.CustomerStepModel.ChatStepId=fw.childId.Value;
                                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];
                                }
                                catch
                                {
                                    botStepModel.customerModel.CustomerStepModel.ChatStepId=-1;
                                    botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId=fw.parentIndex[0];

                                }
                            }
                            catch
                            {


                            }

                        }

                        isButton=false;

                    }


                }
                if (fw.type=="End")
                {
                    try
                    {


                     

                            botStepModel.customerModel.IsTemplateFlow = false;
                            botStepModel.customerModel.templateId = "";
                            botStepModel.customerModel.CampaignId="";
                            botStepModel.customerModel.TemplateFlowDate=null;

                        

                    }
                    catch
                    {

                    }

                }
                if (fw.type == "Cataloge multity Product")
                {
                    var Summary = "";
                    var text = "";
                    if (botStepModel.customerModel.CustomerStepModel.LangString == "ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;// "الرجاء ارسال # للعودة للقائمة الرئسية";
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;// "Please send # to return to the main menu";
                    }



                    string pattern3 = @"(?<=\$)[^\s]+"; // Pattern to match anything after '$' until a space character

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        result = result.Replace(")", "");
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text = text.Replace("$" + result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                            text = text.Replace("$" + result, diVAlue);

                        }






                    }

                    if (text.Contains("IMG("))
                    {

                        text = text.Replace("IMG(", "").Replace(")", "");
                        fw.urlImage.url = text;
                    }




                    List<Activity> activities = new List<Activity>();
                    Activity Botf = new Activity();


                    if (fw.urlImageArray != null && fw.urlImageArray.Length > 0)
                    {
                        foreach (var activity in fw.urlImageArray)
                        {
                            string img = activity.url;
                            string fileType = GetFileTypeFromUrl(img);
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = img,
                                Speak = activity.name,
                                Type = "Cataloge_multity_Product",
                                InputHint = fileType,
                                Summary = Summary,
                                Locale = botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };
                            if (!isfound)
                                Bot.Add(Botf);

                        }

                    }
                    else
                    {

                        if (fw.urlImage != null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            string img = fw.urlImage.url;
                            string fileType = GetFileTypeFromUrl(img);
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = img,
                                Speak = fw.urlImage.name,
                                Type = "Cataloge_multity_Product",
                                InputHint = fileType,
                                Summary = Summary,
                                Locale = botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };

                            if (!isfound)
                                Bot.Add(Botf);
                        }
                        else
                        {
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = text,
                                Speak = text,
                                Type = "Cataloge_multity_Product",
                                Summary = Summary,
                                Locale = botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };
                            if (!isfound)
                                Bot.Add(Botf);
                        }
                    }
                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId = fw.childId.Value;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId = -1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = fw.parentIndex[0];

                    }
                }
                if (fw.type == "Cataloge Single Product")
                {
                    var Summary = "";
                    var text = "";
                    if (botStepModel.customerModel.CustomerStepModel.LangString == "ar")
                    {
                        text = fw.captionAr;
                        Summary = fw.footerTextAr;
                    }
                    else
                    {
                        text = fw.captionEn;
                        Summary = fw.footerTextEn;
                    }
                    string pattern3 = @"(?<=\$)[^\s]+";

                    MatchCollection matches = Regex.Matches(text, pattern3);
                    foreach (Match match in matches)
                    {
                        string result = match.Value;
                        result = result.Replace(")", "");
                        if (result.Contains(".") || result.Contains("["))
                        {
                            var word = result.Split(".")[0];
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[word];
                            var textModel = GetTextFromModel(result, diVAlue);
                            text = text.Replace("$" + result, textModel);

                        }
                        else
                        {
                            var diVAlue = botStepModel.customerModel.CustomerStepModel.UserParmeter[result];
                            text = text.Replace("$" + result, diVAlue);

                        }

                    }

                    if (text.Contains("IMG("))
                    {

                        text = text.Replace("IMG(", "").Replace(")", "");
                        fw.urlImage.url = text;
                    }
                    List<Activity> activities = new List<Activity>();
                    Activity Botf = new Activity();

                    if (fw.urlImageArray != null && fw.urlImageArray.Length > 0)
                    {
                        foreach (var activity in fw.urlImageArray)
                        {
                            string img = activity.url;
                            string fileType = GetFileTypeFromUrl(img);
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = img,
                                Speak = activity.name,
                                Type = "Cataloge_Single_Product",
                                InputHint = fileType,
                                Summary = Summary,
                                Locale = botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };
                            if (!isfound)
                                Bot.Add(Botf);
                        }
                    }
                    else
                    {

                        if (fw.urlImage != null && !string.IsNullOrEmpty(fw.urlImage.url))
                        {
                            string img = fw.urlImage.url;
                            string fileType = GetFileTypeFromUrl(img);
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = img,
                                Speak = fw.urlImage.name,
                                Type = "Cataloge_Single_Product",
                                InputHint = fileType,
                                Summary = Summary,
                                Locale = botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };

                            if (!isfound)
                                Bot.Add(Botf);
                        }
                        else
                        {
                            Botf = new Activity
                            {
                                From = new ChannelAccount(botStepModel.customerModel.userId, botStepModel.customerModel.displayName.Trim()),
                                Text = text,
                                Speak = text,
                                Type = "Cataloge_Single_Product",
                                Summary = Summary,
                                Locale = botStepModel.customerModel.CustomerStepModel.LangString,
                                Attachments = null
                            };
                            if (!isfound)
                                Bot.Add(Botf);
                        }
                    }
                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId = fw.childId.Value;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId = -1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = fw.parentIndex[0];

                    }
                }


                if (fw.type == "Integration")
                {

                    if (fw.googleSheetIntegration.IntegrationType == "googleSheets")
                    {
                        if (fw.googleSheetIntegration.GoogleSheetAction == "insertRow")
                        {
                            var parameters = fw.googleSheetIntegration.Parameters;
                            Dictionary<string, string> resultDict = new Dictionary<string, string>();

                            foreach (string parameter in parameters)
                            {
                                if (string.IsNullOrEmpty(parameter))
                                    continue;

                                resultDict[parameter] = botStepModel.customerModel.CustomerStepModel.UserParmeter.TryGetValue(parameter, out var val) ? val : "";
                            }
                            Dictionary<string, string> row = new Dictionary<string, string>();
                            var worksheetColumns = fw.googleSheetIntegration.WorksheetColumns;

                            for (int i = 0; i < worksheetColumns.Count && i < parameters.Count; i++)
                            {
                                string col = worksheetColumns[i];
                                string param = parameters[i];

                                if (string.IsNullOrEmpty(col) || string.IsNullOrEmpty(param))
                                    continue;

                                string value = resultDict.TryGetValue(param, out var v) ? v : "";
                                row[col] = value;
                            }

                            var body = new InsertSheetRowDto()
                            {
                                spreadsheetId = fw.googleSheetIntegration.SpreadSheetId,
                                tenantId = fw.googleSheetIntegration.TenantId,
                                sheetName = fw.googleSheetIntegration.WorkSheet,
                                row = row
                            };

                            var result = _botApis.InsertRow(body).Result;

                        }
                        if (fw.googleSheetIntegration.GoogleSheetAction == "getRowByValue")
                        {
                            var lookupValue = fw.googleSheetIntegration.LookupValue;
                            var filterValue = botStepModel.customerModel.CustomerStepModel.UserParmeter.TryGetValue(lookupValue, out var val) ? val : "";

                            var spreadsheetId = fw.googleSheetIntegration.SpreadSheetId;
                            var sheetName = fw.googleSheetIntegration.WorkSheet;
                            var lookUpColumn = fw.googleSheetIntegration.LookupColumn;
                            var tenantId = fw.googleSheetIntegration.TenantId;

                            var result = _botApis.GetSheetValues(spreadsheetId, sheetName, lookUpColumn, filterValue, tenantId).Result;

                            if (result != null && result.Count > 1)
                            {
                                var headers = result.ElementAtOrDefault(0)?.Select(x => x?.ToString()).ToList() ?? new List<string>();
                                var values = result.ElementAtOrDefault(1)?.Select(x => x?.ToString()).ToList() ?? new List<string>();

                                var parameters = fw.googleSheetIntegration.Parameters ?? new List<string>();
                                var worksheetColumns = fw.googleSheetIntegration.WorksheetColumns ?? new List<string>();

                                var paramToValue = worksheetColumns
                                    .Select((col, i) => new
                                    {
                                        Param = parameters.ElementAtOrDefault(i),
                                        Header = col
                                    })
                                    .Where(x => !string.IsNullOrWhiteSpace(x.Param) && !string.IsNullOrWhiteSpace(x.Header))
                                    .Select(x => new
                                    {
                                        x.Param,
                                        Value = values.ElementAtOrDefault(headers.IndexOf(x.Header)) ?? ""
                                    })
                                    .ToDictionary(x => x.Param!, x => x.Value!);

                                foreach (var kvp in paramToValue)
                                {
                                    string parameter = kvp.Key;
                                    string value = kvp.Value;
                                    botStepModel.customerModel.CustomerStepModel.UserParmeter[parameter] = value;
                                }

                            }
                            else
                            {
                                // no rows found >> save null into infoseed variables
                                var parameters = fw.googleSheetIntegration.Parameters ?? new List<string>();

                                foreach (var parameter in parameters)
                                {
                                    if (parameter == null)
                                        continue;
                                    botStepModel.customerModel.CustomerStepModel.UserParmeter[parameter] = null!;
                                }
                            }

                        }
                    }
                    try
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId = fw.childId.Value;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = fw.parentIndex[0];
                    }
                    catch
                    {
                        botStepModel.customerModel.CustomerStepModel.ChatStepId = -1;
                        botStepModel.customerModel.CustomerStepModel.ChatStepPervoiusId = fw.parentIndex[0];

                    }

                }

                if (!string.IsNullOrEmpty(fw.parameter) && fw.type!="Http request")
                {
                    isButton=true;
                    botStepModel.customerModel.getBotFlowForViewDto=fw;        
                 
                }
           

            }
        }

        private  string GetTextFromModel(string parm, string result)
        {
            try
            {

                JObject jsonObject = JObject.Parse(result);

                string afterDot = "";
                string after2Dot = "";

                // Split the string by dot
                string[] parts = parm.Split('.');

                // Check if there are at least two parts
                if (parts.Length >= 2)
                {
                    afterDot = parts[1];
                }
                if (parts.Length >= 3)
                {
                    after2Dot = parts[2];
                }


                if (parm.Contains("["))//array
                {


                    string patternarr = @"\[(\d+)\]";
                    int index = -1;
                    // Match the pattern in the input string
                    Match match2 = Regex.Match(parm, patternarr);
                    // Check if a match is found
                    if (match2.Success)
                    {
                        // Get the captured number
                        index = int.Parse(match2.Groups[1].Value); //index

                    }

                    afterDot=afterDot.Replace("["+index+"]", "");

                    JArray fuelPrices = (JArray)jsonObject[afterDot];


                    // Check if the index is within bounds
                    if (index >= 0 && index < fuelPrices.Count)
                    {
                        if (!string.IsNullOrEmpty(after2Dot))
                        {
                            JObject jsonObject2 = JObject.Parse(fuelPrices[index].ToString());

                            JToken propertyValue = jsonObject2[after2Dot];

                            if (propertyValue != null)
                            {
                                return propertyValue.ToString();
                            }
                            else
                            {
                                return "Invalid index";
                            }

                        }
                        else
                        {
                            return fuelPrices[index].ToString();
                        }


                    }
                    else
                    {
                        // Index is out of bounds
                        // return "Invalid index";
                        return "Invalid index";
                    }


                }
                else
                {

                    // Check if the JSON object contains the specified property
                    JToken propertyValue = jsonObject[afterDot];
                    if (propertyValue != null)
                    {
                        return propertyValue.ToString();
                    }
                    else
                    {
                        return "Invalid index";
                    }
                }




            }
            catch (Exception ex)
            {
                return "Invalid index";
            }
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

        #region private
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
        private async Task BotSendToWhatsApp(BotStepModel model ,string textfromuser)
        {
            //     if(model.Bot.GetType==)
            CatalogTemplateDto catalogTemplateDto =new CatalogTemplateDto();
            if (model.customerModel?.getBotFlowForViewDto != null)
            {
                if (model.customerModel.getBotFlowForViewDto.catalogTemplateDto != null)
                {
                     catalogTemplateDto = model.customerModel.getBotFlowForViewDto.catalogTemplateDto;
                }
            }

            List<Activity> botListMessages = new List<Activity>();
            //model.catalogTemplateDto.

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
                List<PostWhatsAppMessageModel> lstpostWhatsAppMessageModel = new List<PostWhatsAppMessageModel>();

                if (msgBot.Type == "Cataloge_multity_Product" || msgBot.Type == "Cataloge_Single_Product")
                {
                    var type = msgBot.Type;
                    catalogTemplateDto = model.customerModel.getBotFlowForViewDto.catalogTemplateDto;
                    lstpostWhatsAppMessageModel = await whatsAppAppService.BotChatWithCustomer(msgBot, model.customerModel.phoneNumber, model.tenantModel.botId);
                    foreach (var msg in lstpostWhatsAppMessageModel)
                    {
                        msg.type = type;
                    }
                }
                else
                {
                    catalogTemplateDto = null;
                    lstpostWhatsAppMessageModel = await whatsAppAppService.BotChatWithCustomer(msgBot, model.customerModel.phoneNumber, model.tenantModel.botId);
                }


                foreach (var postWhatsAppMessageModel in lstpostWhatsAppMessageModel)
                {

                    if (msgBot.Type=="ping")
                    {
                        await Task.Delay(int.Parse(msgBot.Text));
                        continue;
                    }
                    string result = null;
                    if (model.customerModel.channel.ToLower()=="facebook")
                    {
                        result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.FacebookPageId, model.tenantModel.FacebookAccessToken, model.customerModel.channel , catalogTemplateDto);
                      

                    }
                    else if (model.customerModel.channel.ToLower()== "instagram")
                    {
                        result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.InstagramId, model.tenantModel.InstagramAccessToken, model.customerModel.channel , catalogTemplateDto);
                    }
                    else
                    {
                        result = await new WhatsAppAppService().postToFBNew(postWhatsAppMessageModel, model.tenantModel.D360Key, model.tenantModel.AccessToken, model.customerModel.channel , catalogTemplateDto);
                    }

                    
                    if (result!=null)
                    
                    {

 

                        WhatsAppContent model2 = new WhatsAppAppService().PrepareMessageContent(msgBot, model.tenantModel.botId, model.customerModel.userId, model.customerModel.TenantId.Value, model.customerModel.ConversationId);
                        try
                        {
                            model2.Buttons=model.customerModel.customerChat.Buttons;
                            model2.IsButton=model.customerModel.customerChat.IsButton;

                            if (msgBot.SuggestedActions!=null)
                            {
                                foreach (var action in msgBot.SuggestedActions.Actions)
                                {
                                    if(!string.IsNullOrEmpty(action.Image))
                                    {
                                        model2.mediaUrl=action.Image;
                                        break;
                                    }

                                }

                            }
                            

                        }
                        catch
                        {
                            model2.Buttons=new List<string>();
                            model2.IsButton=false;
                        }



                    
                        try
                        {


                            if (true)
                            {
                                var modelR = JsonConvert.DeserializeObject<FacebookAppResult>(result);
                                model2.conversationID = modelR.message_id;


                            }
                            else
                            {
                                var modelR = JsonConvert.DeserializeObject<WhatsAppResult>(result);
                                model2.conversationID = modelR.messages.FirstOrDefault().id;

                            }



                            var CustomerChat = _dbService.UpdateCustomerChatWhatsAppAPI(model2);

                            model.customerModel.customerChat = CustomerChat;
                            //try
                            //{
                            //    await AddToAI(textfromuser, CustomerChat.text, model.tenantModel.TenantId.ToString()+"_"+model.tenantModel.PhoneNumberId);

                            //}
                            //catch
                            //{

                            //}



                            SocketIOManager.SendChat(model.customerModel, model.customerModel.TenantId.Value);
                        }
                        catch(Exception ex)
                        {

                        }
        
                    }



                }

            }
        }

        private void UpdateCustomer(BotStepModel model)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == 0  && a.userId == model.customerModel.userId);//&& a.TenantId== TenantId

            if (model.customerModel.CustomerStepModel.ChatStepId==-1)
            {
                model.customerModel.IsReStart=true;
                //model.IsReStart=true;
            }

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

        #endregion

        private static async Task<WhatsAppAttachmentModel> RetrievingMediaAsync(string mediaId, string fbToken)
        {

            WhatsAppAttachmentModel attachmentModel = new WhatsAppAttachmentModel();


            using (var httpClient = new HttpClient())
            {


                var FBUrl = "https://graph.facebook.com/v17.0/" + mediaId;
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


        private async Task<PagedResultDto<BotParameterModel>> botParameterGetAll(int TenantId)
        {
            try
            {
                List<BotParameterModel> botParameterModel = new List<BotParameterModel>();

                int totalCountOut = 0;
                var SP_Name = Constants.BotFlows.SP_BotParametrsGetAll;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@TotalCount",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                botParameterModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertBotParametrsDto, AppSettingsModel.ConnectionStrings).ToList();
                totalCountOut = (int)OutputParameter.Value;

                return new PagedResultDto<BotParameterModel>(totalCountOut, botParameterModel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private TeamsModel teamsGetAll(string searchTerm = "", int? pageNumber = 0, int? pageSize = 10, int tenantId = 0)
        {
            TeamsModel TeamsModel = new TeamsModel();
            try
            {
                if (!string.IsNullOrEmpty(searchTerm) && searchTerm != "")
                    searchTerm = searchTerm.ToLower();

                var SP_Name = Constants.Teams.SP_TeamsGetAll;


                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId) ,
                    new System.Data.SqlClient.SqlParameter("@pageNumber",pageNumber) ,
                    new System.Data.SqlClient.SqlParameter("@pageSize",pageSize) ,
                    new System.Data.SqlClient.SqlParameter("@searchTerm",searchTerm)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@totalOutBut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                TeamsModel.TeamsDtoModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTeams, AppSettingsModel.ConnectionStrings).ToList();


                TeamsModel.state = 2;
                TeamsModel.message = "OK";
                TeamsModel.total = OutputParameter.Value != DBNull.Value ? Convert.ToInt64(OutputParameter.Value) : 0;
                if (TeamsModel.total == 0)
                {
                    TeamsModel.state = 2;
                    TeamsModel.message = "No results found";
                }
                return TeamsModel;
            }
            catch (Exception ex)
            {
                TeamsModel.state = -1;
                TeamsModel.message = ex.Message;
                return TeamsModel;
            }
        }
    }
}
