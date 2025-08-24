using InfoSeedAzureFunction.Model;
using InfoSeedAzureFunction.WhatsAppApi;
using InfoSeedAzureFunction.WhatsAppApi.Dto;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace InfoSeedAzureFunction
{
    public static class SendUTracTemplate
    {

        //[FunctionName("UTracTemplateFunction")]

        ////public static void Run([TimerTrigger("0 5 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        //public static void Run([QueueTrigger("utractemplate-sync", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //  //  WhatsAppFunModel obj = JsonConvert.DeserializeObject<WhatsAppFunModel>(message);

        //    //WhatsAppFunModel obj = new WhatsAppFunModel();
        //    //UTracOrderModel uTracOrderModel = new UTracOrderModel()
        //    //{
        //    //    OrderId = 1,
        //    //    ResturantName = "infoseed",
        //    //    TenantId = 27,
        //    //    UTracTenantId = 27,
        //    //    PhoneNumber = "962786464718",
        //    //    ContactName = "amjad",
        //    //    OrderStatusId = 0,

        //    //};
        //    //obj.uTracOrderModel = uTracOrderModel;
        //    //obj.TenantId = 27;
        //    //obj.templateId = 823;
        //    //obj.campaignId = 867;
        //    //obj.UserId = uTracOrderModel.TenantId + "_" + uTracOrderModel.PhoneNumber;

        //   // Sync(obj, log).Wait();
        //}
        public static async Task Sync(WhatsAppFunModel model, ILogger log)
        {
            try
            {

                var Statistics = getStatistics(model.TenantId);
                decimal? remainingAds = Statistics.RemainingBIConversation + Statistics.RemainingFreeConversation;

                var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.Tenant && a.TenantId == model.TenantId);
                ContactsEntity contactsEntity = new ContactsEntity();
                Hashtable CampaignHashTable = new Hashtable();

                Guid guid = Guid.NewGuid();

                await SendCampaignAsync(log, model, Statistics, tenant, guid);


            }
            catch (Exception ex)
            {
                WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel();

                whatsAppCampaignModel.id = model.campaignId;
                whatsAppCampaignModel.status = (int)WhatsAppCampaignStatusEnum.Sent;
                updateWhatsAppCampaign(log, whatsAppCampaignModel);

                log.LogInformation($"C# Queue Sync function processed: {ex}");
                throw ex;
            }
        }
        private static async Task SendCampaignAsync(ILogger log, WhatsAppFunModel model, GetAllDashboard Statistics, TenantModel tenant, Guid guid)
        {
            try
            {
                MessageTemplateModel objWhatsAppTemplateModel = getTemplateById(log, model.templateId);
                var whatsAppTemplate = await getTemplateByWhatsAppId(log, tenant, objWhatsAppTemplateModel.id);

                if (whatsAppTemplate != null)
                {
                    WhatsAppContactsDto contact = new WhatsAppContactsDto();
                    TemplateVariables variables = new TemplateVariables();
                    variables.VarOne = model.uTracOrderModel.ContactName;
                    variables.VarTwo = model.uTracOrderModel.OrderId.ToString();
                    variables.VarThree = model.uTracOrderModel.ResturantName;
                    contact.PhoneNumber = model.uTracOrderModel.PhoneNumber;
                    contact.ContactName = model.uTracOrderModel.ContactName;
                    contact.templateVariables = variables;

                    if (objWhatsAppTemplateModel.name == whatsAppTemplate.name)
                    {
                        objWhatsAppTemplateModel.components = whatsAppTemplate.components;
                    }

                    DataTable tbl = new DataTable();
                    tbl.Columns.Add(new DataColumn("PhoneNumber", typeof(string)));
                    tbl.Columns.Add(new DataColumn("TenantId", typeof(int)));
                    tbl.Columns.Add(new DataColumn("CampaignId", typeof(long)));
                    tbl.Columns.Add(new DataColumn("ResultJson", typeof(string)));
                    tbl.Columns.Add(new DataColumn("MessageId", typeof(string)));
                    tbl.Columns.Add(new DataColumn("ContactId", typeof(int)));
                    tbl.Columns.Add(new DataColumn("TemplateId", typeof(long)));
                    tbl.Columns.Add(new DataColumn("SentCampaignId", typeof(Guid)));
                    tbl.Columns.Add(new DataColumn("MessageRate", typeof(decimal)));


                    string type;
                    string msg = prepareMessageTemplateText(objWhatsAppTemplateModel, out type);
                    List<SendCampaignFailedModel> LstSendCampaignFailedModel = new List<SendCampaignFailedModel>();

                    PostMessageTemplateModel _postMessageTemplateModel = prepareMessageTemplate(objWhatsAppTemplateModel, contact);
                    var postBody = JsonConvert.SerializeObject(_postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                    tbl = await SendTemplateToWhatsApp(log, tenant, postBody, model.campaignId, _postMessageTemplateModel.to, contact.Id, tbl, msg, type, objWhatsAppTemplateModel.mediaLink, model.TenantId, model.UserId, model.templateId, LstSendCampaignFailedModel, guid);
                        

                    if (tbl.Rows.Count > 0)
                    {
                        addContactCampaign(tbl);

                        decimal? UsageBIRate = 0, UsageFreeRate = 0;
                        foreach (DataRow row in tbl.Rows)
                        {
                            CoutryTelCodeModel countryCode = getCoutryBIRate(row["phoneNumber"].ToString());
                            if (Statistics.RemainingFreeConversation > countryCode.Rate && Statistics.RemainingBIConversation <= countryCode.Rate)
                            {
                                UsageFreeRate += 1;
                                Statistics.RemainingFreeConversation = Statistics.RemainingFreeConversation - UsageFreeRate;
                            }
                            if (Statistics.RemainingBIConversation > countryCode.Rate)
                            {
                                UsageBIRate += countryCode.Rate;
                                Statistics.RemainingBIConversation = Statistics.RemainingBIConversation - UsageBIRate;
                            }
                        }
                        UpdateBIConversation(model.TenantId, UsageBIRate, UsageFreeRate);

                        WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel();
                        whatsAppCampaignModel.id = model.campaignId;
                        whatsAppCampaignModel.sentTime = DateTime.UtcNow;
                        whatsAppCampaignModel.status = (int)WhatsAppCampaignStatusEnum.Sent;
                        whatsAppCampaignModel.SentCampaignId = guid;
                        updateWhatsAppCampaign(log, whatsAppCampaignModel);
                    }

                    if (LstSendCampaignFailedModel != null && LstSendCampaignFailedModel.Count > 0)
                    {
                        UpdateFailedContact(LstSendCampaignFailedModel);

                    }
                }
                else
                {
                    WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel();
                    whatsAppCampaignModel.id = model.campaignId;
                    whatsAppCampaignModel.sentTime = DateTime.UtcNow;
                    whatsAppCampaignModel.status = (int)WhatsAppCampaignStatusEnum.Sent;
                    whatsAppCampaignModel.SentCampaignId = guid;
                    updateWhatsAppCampaign(log, whatsAppCampaignModel);
                }

            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Queue SendCampaignAsync function processed: {ex}");
            }
        }
        private static async Task<DataTable> SendTemplateToWhatsApp(
                ILogger log, TenantModel tenant, string postBody, long campaignId, string phoneNumber, int? ContactId,
                DataTable tbl, string msg, string type, string mediaUrl, int TenantId, string UserId,
                long templateId,List<SendCampaignFailedModel> lstSendCampaignFailedModel, Guid guid
            )
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var postUrl = Constant.WhatsAppApiUrl + tenant.D360Key + "/messages";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                    using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                    {
                        var result = await response.Content.ReadAsStringAsync();

                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            WhatsAppMessageTemplateResult templateResult = new WhatsAppMessageTemplateResult();
                            templateResult = JsonConvert.DeserializeObject<WhatsAppMessageTemplateResult>(result);

                            DataRow dr = tbl.NewRow();
                            dr["TenantId"] = tenant.TenantId;
                            dr["CampaignId"] = campaignId;
                            dr["PhoneNumber"] = phoneNumber;
                            dr["ResultJson"] = JsonConvert.SerializeObject(result);
                            dr["MessageId"] = templateResult.messages.FirstOrDefault().id;
                            dr["ContactId"] = 0;
                            dr["TemplateId"] = templateId;
                            dr["SentCampaignId"] = guid;
                            dr["MessageRate"] = getCoutryBIRate(phoneNumber).Rate;

                            tbl.Rows.Add(dr);

                            
                            await UpdateCustomerChatAsync(phoneNumber, msg, type, mediaUrl, TenantId, UserId);
                        }
                        else
                        {
                            SendCampaignFailedModel sendCampaignFailedModel = new SendCampaignFailedModel();
                            sendCampaignFailedModel.TenantId = tenant.TenantId.Value;
                            sendCampaignFailedModel.CampaignId = campaignId;
                            sendCampaignFailedModel.PhoneNumber = phoneNumber;
                            sendCampaignFailedModel.ContactId = ContactId.Value;
                            sendCampaignFailedModel.TemplateId = templateId;
                            sendCampaignFailedModel.SentCampaignId = guid;

                            if (lstSendCampaignFailedModel == null)
                            {
                                lstSendCampaignFailedModel = new List<SendCampaignFailedModel>();
                            }
                            lstSendCampaignFailedModel.Add(sendCampaignFailedModel);
                        }
                        return tbl;
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Queue SendCampaignAsync function processed: {ex}");
                return tbl;
            }
        }
        private static void updateWhatsAppCampaign(ILogger log, WhatsAppCampaignModel whatsAppCampaignModel)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignUpdate;

                var sqlParameters = new List<SqlParameter> {

                     new SqlParameter("@CampaignId",whatsAppCampaignModel.id)
                    ,new SqlParameter("@SentTime",DateTime.UtcNow)
                    ,new SqlParameter("@Status",whatsAppCampaignModel.status)
                    ,new SqlParameter("@SentCampaignId",whatsAppCampaignModel.SentCampaignId)
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Queue updateWhatsAppCampaign function processed: {ex}");
                throw ex;
            }
        }
        private static void UpdateBIConversation(int TenantId, decimal? usageBI, decimal? usageFree)
        {
            try
            {
                var SP_Name = Constants.Dashboard.SP_ConversationMeasurementBIUpdate;
                var sqlParameters = new List<SqlParameter> {

                    new SqlParameter("@UsageBi",usageBI),
                    new SqlParameter("@UsageFree",usageFree),
                    new SqlParameter("@TenantId", TenantId)

                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        private static void addContactCampaign(DataTable tbl)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsCampaignlBulkAdd;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@ContactCampaignTable",tbl)
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private static void UpdateFailedContact(List<SendCampaignFailedModel> lstSendCampaignFailedModel)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_ContactsFailedCampaignBulkAdd;

                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@ContactsFailedCampaignJson",JsonConvert.SerializeObject(lstSendCampaignFailedModel) )
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static async Task UpdateCustomerChatAsync(string phoneNumber, string msg, string type, string mediaUrl, int TenantId, string UserId)
        {
            string userId = TenantId + "_" + phoneNumber;
            //   var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);


            CustomerChat customerChat = new CustomerChat()
            {
                messageId = Guid.NewGuid().ToString(),
                TenantId = TenantId,
                userId = userId,
                text = msg,
                type = type,
                CreateDate = DateTime.Now,
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = mediaUrl,
                UnreadMessagesCount = 0,
                agentName = "admin",
                agentId = UserId,
            };

            var itemsCollection2 = new DocumentDBHelper<CustomerChat>(CollectionTypes.ItemsCollection);
            await itemsCollection2.CreateItemAsync(customerChat);
        }
        private static PostMessageTemplateModel prepareMessageTemplate(MessageTemplateModel objWhatsAppTemplateModel, WhatsAppContactsDto contact)
        {
            try
            {
                PostMessageTemplateModel postMessageTemplateModel = new PostMessageTemplateModel();
                WhatsAppTemplateModel whatsAppTemplateModel = new WhatsAppTemplateModel();
                WhatsAppLanguageModel whatsAppLanguageModel = new WhatsAppLanguageModel();

                List<Component> components = new List<Component>();
                Component componentHeader = new Component();
                Parameter parameterHeader = new Parameter();

                Component componentBody = new Component();
                Parameter parameterBody1 = new Parameter();
                Parameter parameterBody2 = new Parameter();
                Parameter parameterBody3 = new Parameter();
                Parameter parameterBody4 = new Parameter();
                Parameter parameterBody5 = new Parameter();

                ImageTemplate image = new ImageTemplate();
                VideoTemplate video = new VideoTemplate();
                DocumentTemplate document = new DocumentTemplate();


                if (objWhatsAppTemplateModel.components != null)
                {
                    components = new List<Component>();
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type == "HEADER")
                        {
                            componentHeader.type = item.type;
                            parameterHeader.type = item.format.ToLower();
                            if (item.format == "IMAGE")
                            {
                                image.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.image = image;
                                componentHeader.parameters = new List<Parameter>();
                                componentHeader.parameters.Add(parameterHeader);
                            }
                            if (item.format == "VIDEO")
                            {
                                video.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.video = video;
                                componentHeader.parameters = new List<Parameter>();

                                componentHeader.parameters.Add(parameterHeader);

                            }
                            if (item.format == "DOCUMENT")
                            {
                                document.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.document = document;

                                componentHeader.parameters = new List<Parameter>();
                                componentHeader.parameters.Add(parameterHeader);

                            }
                            components.Add(componentHeader);

                        }
                        if (item.type == "BODY")
                        {
                            if (contact.templateVariables != null)
                            {
                                if (objWhatsAppTemplateModel.VariableCount >= 1)
                                {
                                    componentBody.parameters = new List<Parameter>();

                                    parameterBody1.type = "TEXT";
                                    parameterBody1.text = contact.templateVariables.VarOne;
                                    componentBody.parameters.Add(parameterBody1);

                                    if (objWhatsAppTemplateModel.VariableCount >= 2)
                                    {
                                        parameterBody2.type = "TEXT";
                                        parameterBody2.text = contact.templateVariables.VarTwo;
                                        componentBody.parameters.Add(parameterBody2);

                                        if (objWhatsAppTemplateModel.VariableCount >= 3)
                                        {
                                            parameterBody3.type = "TEXT";
                                            parameterBody3.text = contact.templateVariables.VarThree;
                                            componentBody.parameters.Add(parameterBody3);

                                            if (objWhatsAppTemplateModel.VariableCount >= 4)
                                            {
                                                parameterBody4.type = "TEXT";
                                                parameterBody4.text = contact.templateVariables.VarFour;
                                                componentBody.parameters.Add(parameterBody4);

                                                if (objWhatsAppTemplateModel.VariableCount >= 5)
                                                {
                                                    parameterBody5.type = "TEXT";
                                                    parameterBody5.text = contact.templateVariables.VarFive;
                                                    componentBody.parameters.Add(parameterBody5);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                            componentBody.type = item.type;

                            components.Add(componentBody);
                        }
                    }

                }
                whatsAppLanguageModel.code = objWhatsAppTemplateModel.language;
                whatsAppTemplateModel.language = whatsAppLanguageModel;
                whatsAppTemplateModel.name = objWhatsAppTemplateModel.name;
                whatsAppTemplateModel.components = components;
                postMessageTemplateModel.template = whatsAppTemplateModel;
                postMessageTemplateModel.to = contact.PhoneNumber;
                //postMessageTemplateModel.to = "962786464718";
                postMessageTemplateModel.messaging_product = "whatsapp";
                postMessageTemplateModel.type = "template";
                return postMessageTemplateModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private static string prepareMessageTemplateText(MessageTemplateModel objWhatsAppTemplateModel, out string type)
        {
            try
            {
                string result = string.Empty;
                type = "text";
                if (objWhatsAppTemplateModel.components != null)
                {
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type.Equals("HEADER"))
                        {
                            type = item.format.ToLower();
                        }
                        result += item.text;

                    }

                }

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private static GetAllDashboard getStatistics(int TenantId)
        {
            try
            {
                GetAllDashboard statistics = new GetAllDashboard();
                var SP_Name = Constants.Dashboard.SP_ConversationMeasurementsGet;
                var sqlParameters = new List<SqlParameter> {
                     new SqlParameter("@Year",DateTime.Now.Year)
                    ,new SqlParameter("@Month",DateTime.Now.Month)
                    ,new SqlParameter("@TenantId",TenantId)
                };
                statistics = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapDashboardStatistic, Constants.ConnectionString).FirstOrDefault();
                statistics.RemainingUIConversation = statistics.TotalUIConversation - statistics.TotalUsageUIConversation;
                statistics.RemainingBIConversation = statistics.TotalBIConversation - statistics.TotalUsageBIConversation;
                statistics.RemainingFreeConversation = statistics.TotalFreeConversationWA - statistics.TotalUsageFreeConversation;

                return statistics;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static bool IsExistingContacts(object myObject)
        {
            foreach (PropertyInfo pi in myObject.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string) || pi.PropertyType == typeof(bool) || pi.PropertyType == typeof(int) || pi.PropertyType == typeof(long))
                {
                    string value = (string)pi.GetValue(myObject);
                    if (!string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static GetAllDashboard MapDashboardStatistic(IDataReader dataReader)
        {
            GetAllDashboard GetAllDashboard = new GetAllDashboard();
            GetAllDashboard.TotalOfAllContact = SqlDataHelper.GetValue<int>(dataReader, "TotalOfAllContact");
            GetAllDashboard.TotalOfOrders = SqlDataHelper.GetValue<int>(dataReader, "TotalOfOrders");
            GetAllDashboard.Bandel = SqlDataHelper.GetValue<int>(dataReader, "ConversationBundle");
            GetAllDashboard.RemainingConversation = SqlDataHelper.GetValue<int>(dataReader, "RemainingConversation");
            GetAllDashboard.TotalOfRating = SqlDataHelper.GetValue<double>(dataReader, "TotalOfRating");

            GetAllDashboard.TotalFreeConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalFreeConversationWA");
            GetAllDashboard.TotalUsageFreeConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeConversationWA");
            GetAllDashboard.TotalUsageFreeUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeUIWA");
            GetAllDashboard.TotalUsageFreeBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeBIWA");

            GetAllDashboard.TotalUsagePaidConversationWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidConversationWA");
            GetAllDashboard.TotalUsagePaidUIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidUIWA");
            GetAllDashboard.TotalUsagePaidBIWA = SqlDataHelper.GetValue<int>(dataReader, "TotalUsagePaidBIWA");

            GetAllDashboard.TotalUsageFreeConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageFreeConversation");
            GetAllDashboard.TotalUIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUIConversation");
            GetAllDashboard.TotalUsageUIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageUIConversation");
            GetAllDashboard.TotalBIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalBIConversation");
            GetAllDashboard.TotalUsageBIConversation = SqlDataHelper.GetValue<int>(dataReader, "TotalUsageBIConversation");

            //GetAllDashboard.RemainingConversationWA = SqlDataHelper.GetValue<int>(dataReader, "RemainingConversationWA");
            return GetAllDashboard;
        }
        private static MessageTemplateModel getTemplateById(ILogger log, long templateId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesGetById;
                MessageTemplateModel objWhatsAppTemplateModel = new MessageTemplateModel();
                var sqlParameters = new List<SqlParameter> { new SqlParameter("@TemplateId", templateId) };
                objWhatsAppTemplateModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapTemplate, Constants.ConnectionString).FirstOrDefault();
                return objWhatsAppTemplateModel;
            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Queue getTemplateById processed: {ex}");
                throw ex;
            }
        }
        private static async Task<MessageTemplateModel> getTemplateByWhatsAppId(ILogger log, TenantModel tenant, string templateId)
        {
            using (var httpClient = new HttpClient())
            {
                var postUrl = Constant.WhatsAppApiUrl + templateId;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                using (var response = await httpClient.GetAsync(postUrl))
                {
                    using (var content = response.Content)
                    {
                        try
                        {
                            var WhatsAppTemplate = await content.ReadAsStringAsync();
                            MessageTemplateModel objWhatsAppTemplate = new MessageTemplateModel();
                            objWhatsAppTemplate = JsonConvert.DeserializeObject<MessageTemplateModel>(WhatsAppTemplate);
                            return objWhatsAppTemplate;
                        }
                        catch (Exception ex)
                        {
                            log.LogInformation($"C# Queue getTemplateByWhatsAppId processed: {ex}");
                            throw ex;
                        }


                    }
                }
            }
        }
        private static async Task<WhatsAppMessageTemplateModel> getWhatsAppMessageTemplate(ILogger log, TenantModel tenant)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var postUrl = Constant.WhatsAppApiUrl + tenant.WhatsAppAccountID + "/message_templates?limit=250";
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                    using (var response = await httpClient.GetAsync(postUrl))
                    {
                        using (var content = response.Content)
                        {
                            var WhatsAppTemplate = await content.ReadAsStringAsync();
                            WhatsAppMessageTemplateModel objWhatsAppTemplate = new WhatsAppMessageTemplateModel();
                            objWhatsAppTemplate = JsonConvert.DeserializeObject<WhatsAppMessageTemplateModel>(WhatsAppTemplate);
                            return objWhatsAppTemplate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Queue getWhatsAppMessageTemplate processed: {ex}");
                throw ex;
            }

        }


        #region Contact

        private static ContactsEntity getFilterContacts(ILogger log, WhatsAppContactsDto contacts, int TenantId)
        {
            try
            {
                if (contacts.tenantId == null)
                {
                    contacts.tenantId = TenantId;
                }

                ContactsEntity contactsEntity = new ContactsEntity();

                List<WhatsAppContactsDto> lstContacts = new List<WhatsAppContactsDto>();
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppContactFilterGet;
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@phone",contacts.PhoneNumber)
                   ,new SqlParameter("@contactName",contacts.ContactName)
                   ,new SqlParameter("@countryCode",contacts.CountryCode)
                   ,new SqlParameter("@city",contacts.City)
                   ,new SqlParameter("@branch",contacts.Branch)
                   ,new SqlParameter("@joiningFrom",contacts.JoiningFrom)
                   ,new SqlParameter("@joiningTo",contacts.JoiningTo)
                   ,new SqlParameter("@orderTimeFrom",contacts.OrderTimeFrom )
                   ,new SqlParameter("@orderTimeTo", contacts.OrderTimeTo)
                   ,new SqlParameter("@totalSessions",contacts.TotalSessions)
                   ,new SqlParameter("@totalOrderMin",contacts.TotalOrderMin)
                   ,new SqlParameter("@totalOrderMax",contacts.TotalOrderMax)

                   ,new SqlParameter("@interestedOfOne",contacts.InterestedOfOne)
                   ,new SqlParameter("@interestedOfTwo",contacts.InterestedOfTwo)
                   ,new SqlParameter("@interestedOfThree",contacts.InterestedOfThree)

                   ,new SqlParameter("@isOpt",contacts.IsOpt)


                   ,new SqlParameter("@TemplateId",contacts.TemplateId)

                   ,new SqlParameter("@PageNumber",contacts.pageNumber)
                   ,new SqlParameter("@PageSize",contacts.pageSize)
                   ,new SqlParameter("@TenantId",contacts.tenantId)


                };
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                var OutputParameter2 = new SqlParameter();
                OutputParameter2.SqlDbType = SqlDbType.BigInt;
                OutputParameter2.ParameterName = "@TotalOptOut";
                OutputParameter2.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter2);

                lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapFilterContacts, Constants.ConnectionString).ToList();


                contactsEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                contactsEntity.TotalOptOut = Convert.ToInt32(OutputParameter2.Value);
                contactsEntity.contacts = lstContacts;
                return contactsEntity;
            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Queue getFilterContacts processed: {ex}");
                throw ex;
            }
        }
        private static ContactsEntity getExternalContacts(ILogger log, long campaignId, long templateId, int? pageNumber = 0, int? pageSize = int.MaxValue, int? tenantId = null)
        {
            try
            {
                ContactsEntity contactsEntity = new ContactsEntity();

                List<WhatsAppContactsDto> lstContacts = new List<WhatsAppContactsDto>();
                var SP_Name = Constants.Contacts.SP_ContactsExternalByCampaignGet;

                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@PageSize",pageSize)
                   ,new SqlParameter("@PageNumber",pageNumber)
                   ,new SqlParameter("@TenantId",tenantId)
                   ,new SqlParameter("@CampaignId",campaignId)
                   ,new SqlParameter("@TemplateId",templateId)
                };
                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                lstContacts = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 MapExternalContacts, Constants.ConnectionString).ToList();


                contactsEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                contactsEntity.contacts = lstContacts;
                return contactsEntity;
            }
            catch (Exception ex)
            {
                log.LogInformation($"C# Queue getExternalContacts processed: {ex}");
                throw ex;
            }
        }
        private static int getCountFilterContacts(int tenantId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppContactCountFilterGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)


                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);



                return Convert.ToInt32(OutputParameter.Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        private static MessageTemplateModel getTemplateByWhatsAppId(long templateId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppTemplates.SP_TemplateGetByWhatsAppId;
                MessageTemplateModel objWhatsAppTemplateModel = new MessageTemplateModel();
                var sqlParameters = new List<SqlParameter> { new SqlParameter("@TemplateId", templateId) };
                objWhatsAppTemplateModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapTemplate, Constants.ConnectionString).FirstOrDefault();
                return objWhatsAppTemplateModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private static CoutryTelCodeModel getCoutryBIRate(string phone)
        {
            List<CoutryTelCodeModel> TelCodes = new List<CoutryTelCodeModel>
                   {
                       new CoutryTelCodeModel("93", "AF",1),
                       new CoutryTelCodeModel("355", "AL",1),
                       new CoutryTelCodeModel("213", "DZ",2),
                       new CoutryTelCodeModel("1-684", "AS",1),
                       new CoutryTelCodeModel("376", "AD",1),
                       new CoutryTelCodeModel("244", "AO",1),
                       new CoutryTelCodeModel("1-264", "AI",1),
                       new CoutryTelCodeModel("672", "AQ",1),
                       new CoutryTelCodeModel("1-268", "AG",1),
                       new CoutryTelCodeModel("54", "AR",1),
                       new CoutryTelCodeModel("374", "AM",1),
                       new CoutryTelCodeModel("297", "AW",1),
                       new CoutryTelCodeModel("61", "AU",1),
                       new CoutryTelCodeModel("43", "AT",(decimal)1.5),
                       new CoutryTelCodeModel("994", "AZ",1),
                       new CoutryTelCodeModel("1-242", "BS",1),
                       new CoutryTelCodeModel("973", "BH",1),
                       new CoutryTelCodeModel("880", "BD",1),
                       new CoutryTelCodeModel("1-246", "BB",1),
                       new CoutryTelCodeModel("375", "BY",(decimal)1.5),
                       new CoutryTelCodeModel("32", "BE",2),
                       new CoutryTelCodeModel("501", "BZ",1),
                       new CoutryTelCodeModel("229", "BJ",1),
                       new CoutryTelCodeModel("1-441", "BM",1),
                       new CoutryTelCodeModel("975", "BT",1),
                       new CoutryTelCodeModel("591", "BO",1),
                       new CoutryTelCodeModel("387", "BA",1),
                       new CoutryTelCodeModel("267", "BW",1),
                       new CoutryTelCodeModel("55", "BR",1),
                       new CoutryTelCodeModel("246", "IO",1),
                       new CoutryTelCodeModel("1-284", "VG",1),
                       new CoutryTelCodeModel("673", "BN",1),
                       new CoutryTelCodeModel("359", "BG",(decimal)1.5),
                       new CoutryTelCodeModel("226", "BF",1),
                       new CoutryTelCodeModel("257", "BI",1),
                       new CoutryTelCodeModel("855", "KH",1),
                       new CoutryTelCodeModel("237", "CM",1),
                       new CoutryTelCodeModel("1", "CA",(decimal)0.5),
                       new CoutryTelCodeModel("238", "CV",1),
                       new CoutryTelCodeModel("1-345", "KY",1),
                       new CoutryTelCodeModel("236", "CF",1),
                       new CoutryTelCodeModel("235", "TD",1),
                       new CoutryTelCodeModel("56", "CL",1),
                       new CoutryTelCodeModel("86", "CN",1),
                       new CoutryTelCodeModel("61", "CX",1),
                       new CoutryTelCodeModel("61", "CC",1),
                       new CoutryTelCodeModel("57", "CO",(decimal)0.5),
                       new CoutryTelCodeModel("269", "KM",1),
                       new CoutryTelCodeModel("682", "CK",1),
                       new CoutryTelCodeModel("506", "CR",1),
                       new CoutryTelCodeModel("385", "HR",(decimal)1.5),
                       new CoutryTelCodeModel("53", "CU",1),
                       new CoutryTelCodeModel("599", "CW",1),
                       new CoutryTelCodeModel("357", "CY",1),
                       new CoutryTelCodeModel("420", "CZ",(decimal)1.5),
                       new CoutryTelCodeModel("243", "CD",1),
                       new CoutryTelCodeModel("45", "DK",2),
                       new CoutryTelCodeModel("253", "DJ",1),
                       new CoutryTelCodeModel("1-767", "DM",1),
                       new CoutryTelCodeModel("1-809", "DO",1),
                       new CoutryTelCodeModel("1-829", "DO",1),
                       new CoutryTelCodeModel("1-849", "DO",1),
                       new CoutryTelCodeModel("670", "TL",1),
                       new CoutryTelCodeModel("593", "EC",1),
                       new CoutryTelCodeModel("20", "EG",(decimal)1.5),
                       new CoutryTelCodeModel("503", "SV",1),
                       new CoutryTelCodeModel("240", "GQ",1),
                       new CoutryTelCodeModel("291", "ER",1),
                       new CoutryTelCodeModel("372", "EE",(decimal)1.5),
                       new CoutryTelCodeModel("251", "ET",1),
                       new CoutryTelCodeModel("500", "FK",1),
                       new CoutryTelCodeModel("298", "FO",1),
                       new CoutryTelCodeModel("679", "FJ",1),
                       new CoutryTelCodeModel("358", "FI",1),
                       new CoutryTelCodeModel("33", "FR",2),
                       new CoutryTelCodeModel("689", "PF",1),
                       new CoutryTelCodeModel("241", "GA",1),
                       new CoutryTelCodeModel("220", "GM",1),
                       new CoutryTelCodeModel("995", "GE",1),
                       new CoutryTelCodeModel("49", "DE",2),
                       new CoutryTelCodeModel("233", "GH",1),
                       new CoutryTelCodeModel("350", "GI",1),
                       new CoutryTelCodeModel("30", "GR",1),
                       new CoutryTelCodeModel("299", "GL",(decimal)0.5),
                       new CoutryTelCodeModel("1-473", "GD",1),
                       new CoutryTelCodeModel("1-671", "GU",1),
                       new CoutryTelCodeModel("502", "GT",1),
                       new CoutryTelCodeModel("44-1481", "GG",1),
                       new CoutryTelCodeModel("224", "GN",1),
                       new CoutryTelCodeModel("245", "GW",1),
                       new CoutryTelCodeModel("592", "GY",1),
                       new CoutryTelCodeModel("509", "HT",1),
                       new CoutryTelCodeModel("504", "HN",1),
                       new CoutryTelCodeModel("852", "HK",1),
                       new CoutryTelCodeModel("36", "HU",(decimal)1.5),
                       new CoutryTelCodeModel("354", "IS",1),
                       new CoutryTelCodeModel("91", "IN",(decimal)0.5),
                       new CoutryTelCodeModel("62", "ID",(decimal)0.5),
                       new CoutryTelCodeModel("98", "IR",1),
                       new CoutryTelCodeModel("964", "IQ",1),
                       new CoutryTelCodeModel("353", "IE",1),
                       new CoutryTelCodeModel("44-1624", "IM",1),
                       new CoutryTelCodeModel("972", "IL",(decimal)0.5),
                       new CoutryTelCodeModel("39", "IT",1),
                       new CoutryTelCodeModel("225", "CI",1),
                       new CoutryTelCodeModel("1-876", "JM",1),
                       new CoutryTelCodeModel("81", "JP",1),
                       new CoutryTelCodeModel("44-1534", "JE",1),
                       new CoutryTelCodeModel("962", "JO",1),
                       new CoutryTelCodeModel("7", "KZ",1),
                       new CoutryTelCodeModel("254", "KE",1),
                       new CoutryTelCodeModel("686", "KI",1),
                       new CoutryTelCodeModel("383", "XK",1),
                       new CoutryTelCodeModel("965", "KW",1),
                       new CoutryTelCodeModel("996", "KG",1),
                       new CoutryTelCodeModel("856", "LA",1),
                       new CoutryTelCodeModel("371", "LV",1),
                       new CoutryTelCodeModel("961", "LB",1),
                       new CoutryTelCodeModel("266", "LS",1),
                       new CoutryTelCodeModel("231", "LR",1),
                       new CoutryTelCodeModel("218", "LY",2),
                       new CoutryTelCodeModel("423", "LI",1),
                       new CoutryTelCodeModel("370", "LT",1),
                       new CoutryTelCodeModel("352", "LU",1),
                       new CoutryTelCodeModel("853", "MO",1),
                       new CoutryTelCodeModel("389", "MK",1),
                       new CoutryTelCodeModel("261", "MG",1),
                       new CoutryTelCodeModel("265", "MW",1),
                       new CoutryTelCodeModel("60", "MY",1),
                       new CoutryTelCodeModel("960", "MV",1),
                       new CoutryTelCodeModel("223", "ML",1),
                       new CoutryTelCodeModel("356", "MT",1),
                       new CoutryTelCodeModel("692", "MH",1),
                       new CoutryTelCodeModel("222", "MR",1),
                       new CoutryTelCodeModel("230", "MU",1),
                       new CoutryTelCodeModel("262", "YT",1),
                       new CoutryTelCodeModel("52", "MX",(decimal)0.5),
                       new CoutryTelCodeModel("691", "FM",1),
                       new CoutryTelCodeModel("373", "MD",(decimal)1.5),
                       new CoutryTelCodeModel("377", "MC",1),
                       new CoutryTelCodeModel("976", "MN",1),
                       new CoutryTelCodeModel("382", "ME",1),
                       new CoutryTelCodeModel("1-664", "MS",1),
                       new CoutryTelCodeModel("212", "MA",1),
                       new CoutryTelCodeModel("258", "MZ",1),
                       new CoutryTelCodeModel("95", "MM",1),
                       new CoutryTelCodeModel("264", "NA",1),
                       new CoutryTelCodeModel("674", "NR",1),
                       new CoutryTelCodeModel("977", "NP",1),
                       new CoutryTelCodeModel("31", "NL",1),
                       new CoutryTelCodeModel("599", "AN",1),
                       new CoutryTelCodeModel("687", "NC",1),
                       new CoutryTelCodeModel("64", "NZ",1),
                       new CoutryTelCodeModel("505", "NI",1),
                       new CoutryTelCodeModel("227", "NE",1),
                       new CoutryTelCodeModel("234", "NG",1),
                       new CoutryTelCodeModel("683", "NU",1),
                       new CoutryTelCodeModel("850", "KP",1),
                       new CoutryTelCodeModel("1-670", "MP",1),
                       new CoutryTelCodeModel("47", "NO",1),
                       new CoutryTelCodeModel("968", "OM",1),
                       new CoutryTelCodeModel("92", "PK",1),
                       new CoutryTelCodeModel("680", "PW",1),
                       new CoutryTelCodeModel("970", "PS",1),
                       new CoutryTelCodeModel("507", "PA",1),
                       new CoutryTelCodeModel("675", "PG",1),
                       new CoutryTelCodeModel("595", "PY",1),
                       new CoutryTelCodeModel("51", "PE",1),
                       new CoutryTelCodeModel("63", "PH",1),
                       new CoutryTelCodeModel("64", "PN",1),
                       new CoutryTelCodeModel("48", "PL",(decimal)1.5),
                       new CoutryTelCodeModel("351", "PT",1),
                       new CoutryTelCodeModel("1-787", "PR",1),
                       new CoutryTelCodeModel("1-939", "PR",1),
                       new CoutryTelCodeModel("974", "QA",1),
                       new CoutryTelCodeModel("242", "CG",1),
                       new CoutryTelCodeModel("262", "RE",1),
                       new CoutryTelCodeModel("40", "RO",(decimal)1.5),
                       new CoutryTelCodeModel("7", "RU",1),
                       new CoutryTelCodeModel("250", "RW",1),
                       new CoutryTelCodeModel("590", "BL",1),
                       new CoutryTelCodeModel("290", "SH",1),
                       new CoutryTelCodeModel("1-869", "KN",1),
                       new CoutryTelCodeModel("1-758", "LC",1),
                       new CoutryTelCodeModel("590", "MF",1),
                       new CoutryTelCodeModel("508", "PM",(decimal)0.5),
                       new CoutryTelCodeModel("1-784", "VC",1),
                       new CoutryTelCodeModel("685", "WS",1),
                       new CoutryTelCodeModel("378", "SM",1),
                       new CoutryTelCodeModel("239", "ST",1),
                       new CoutryTelCodeModel("966", "SA",(decimal)0.5),
                       new CoutryTelCodeModel("221", "SN",1),
                       new CoutryTelCodeModel("381", "RS",1),
                       new CoutryTelCodeModel("248", "SC",1),
                       new CoutryTelCodeModel("232", "SL",1),
                       new CoutryTelCodeModel("65", "SG",1),
                       new CoutryTelCodeModel("1-721", "SX",1),
                       new CoutryTelCodeModel("421", "SK",(decimal)1.5),
                       new CoutryTelCodeModel("386", "SI",1),
                       new CoutryTelCodeModel("677", "SB",1),
                       new CoutryTelCodeModel("252", "SO",1),
                       new CoutryTelCodeModel("27", "ZA",(decimal)0.5),
                       new CoutryTelCodeModel("82", "KR",1),
                       new CoutryTelCodeModel("211", "SS",2),
                       new CoutryTelCodeModel("34", "ES",1),
                       new CoutryTelCodeModel("94", "LK",1),
                       new CoutryTelCodeModel("249", "SD",2),
                       new CoutryTelCodeModel("597", "SR",1),
                       new CoutryTelCodeModel("47", "SJ",1),
                       new CoutryTelCodeModel("268", "SZ",1),
                       new CoutryTelCodeModel("46", "SE",1),
                       new CoutryTelCodeModel("41", "CH",1),
                       new CoutryTelCodeModel("963", "SY",1),
                       new CoutryTelCodeModel("886", "TW",1),
                       new CoutryTelCodeModel("992", "TJ",1),
                       new CoutryTelCodeModel("255", "TZ",1),
                       new CoutryTelCodeModel("66", "TH",1),
                       new CoutryTelCodeModel("228", "TG",1),
                       new CoutryTelCodeModel("690", "TK",1),
                       new CoutryTelCodeModel("676", "TO",1),
                       new CoutryTelCodeModel("1-868", "TT",1),
                       new CoutryTelCodeModel("216", "TN",2),
                       new CoutryTelCodeModel("90", "TR",(decimal)0.5),
                       new CoutryTelCodeModel("993", "TM",1),
                       new CoutryTelCodeModel("1-649", "TC",1),
                       new CoutryTelCodeModel("688", "TV",1),
                       new CoutryTelCodeModel("1-340", "VI",1),
                       new CoutryTelCodeModel("256", "UG",1),
                       new CoutryTelCodeModel("380", "UA",(decimal)1.5),
                       new CoutryTelCodeModel("971", "AE",(decimal)0.5),
                       new CoutryTelCodeModel("44", "GB",1),
                       new CoutryTelCodeModel("1", "US",(decimal)0.5),
                       new CoutryTelCodeModel("598", "UY",1),
                       new CoutryTelCodeModel("998", "UZ",1),
                       new CoutryTelCodeModel("678", "VU",1),
                       new CoutryTelCodeModel("379", "VA",1),
                       new CoutryTelCodeModel("58", "VE",1),
                       new CoutryTelCodeModel("84", "VN",1),
                       new CoutryTelCodeModel("681", "WF",1),
                       new CoutryTelCodeModel("212", "EH",1),
                       new CoutryTelCodeModel("967", "YE",1),
                       new CoutryTelCodeModel("260", "ZM",1),
                       new CoutryTelCodeModel("263", "ZW",1),
            };
            CoutryTelCodeModel result = TelCodes.Where(x => phone.StartsWith(x.Pfx)).FirstOrDefault();
            return result;

        }
        #region Mapper
        private static WhatsAppContactsDto MapFilterContacts(IDataReader dataReader)
        {
            WhatsAppContactsDto contacts = new WhatsAppContactsDto();
            contacts.Id = SqlDataHelper.GetValue<int>(dataReader, "id");
            contacts.ContactName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            contacts.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            contacts.CustomerOPT = SqlDataHelper.GetValue<string>(dataReader, "CustomerOPT");

            return contacts;
        }

        public static MessageTemplateModel MapTemplate(IDataReader dataReader)
        {
            MessageTemplateModel _MessageTemplateModel = new MessageTemplateModel();
            _MessageTemplateModel.name = SqlDataHelper.GetValue<string>(dataReader, "Name");
            _MessageTemplateModel.language = SqlDataHelper.GetValue<string>(dataReader, "Language");
            _MessageTemplateModel.category = SqlDataHelper.GetValue<string>(dataReader, "Category");

            var components = SqlDataHelper.GetValue<string>(dataReader, "Components");
            var options = new JsonSerializerOptions { WriteIndented = true };

            _MessageTemplateModel.components = System.Text.Json.JsonSerializer.Deserialize<List<WhatsAppComponentModel>>(components, options);
            _MessageTemplateModel.id = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppTemplateId");
            _MessageTemplateModel.LocalTemplateId = SqlDataHelper.GetValue<long>(dataReader, "Id");
            _MessageTemplateModel.mediaType = SqlDataHelper.GetValue<string>(dataReader, "MediaType");
            _MessageTemplateModel.mediaLink = SqlDataHelper.GetValue<string>(dataReader, "MediaLink");
            _MessageTemplateModel.isDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted");
            _MessageTemplateModel.VariableCount = SqlDataHelper.GetValue<int>(dataReader, "VariableCount");

            return _MessageTemplateModel;
        }
        public static WhatsAppContactsDto MapExternalContacts(IDataReader dataReader)
        {
            WhatsAppContactsDto contacts = new WhatsAppContactsDto();
            contacts.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            contacts.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");
            contacts.ContactName = SqlDataHelper.GetValue<string>(dataReader, "ContactName");
            contacts.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");


            var templateVariables = SqlDataHelper.GetValue<string>(dataReader, "TemplateVariables");
            var options = new JsonSerializerOptions { WriteIndented = true };
            contacts.templateVariables = System.Text.Json.JsonSerializer.Deserialize<TemplateVariables>(templateVariables, options);

            return contacts;
        }

        public static ConversationSessionModel MapConversationSession(IDataReader dataReader)
        {
            try
            {
                ConversationSessionModel _ConversationSessionModel = new ConversationSessionModel();
                _ConversationSessionModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                _ConversationSessionModel.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                _ConversationSessionModel.ConversationId = SqlDataHelper.GetValue<string>(dataReader, "ConversationId");
                _ConversationSessionModel.ConversationDateTime = SqlDataHelper.GetValue<string>(dataReader, "ConversationDateTime");
                _ConversationSessionModel.InitiatedBy = SqlDataHelper.GetValue<string>(dataReader, "InitiatedBy");
                _ConversationSessionModel.TenantId = SqlDataHelper.GetValue<string>(dataReader, "TenantId");


                return _ConversationSessionModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static WhatsAppCampaignModel MapCampaign(IDataReader dataReader)
        {
            try
            {
                WhatsAppCampaignModel _WhatsAppCampaignModel = new WhatsAppCampaignModel();
                _WhatsAppCampaignModel.id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                _WhatsAppCampaignModel.title = SqlDataHelper.GetValue<string>(dataReader, "Title");
                _WhatsAppCampaignModel.language = SqlDataHelper.GetValue<string>(dataReader, "Language");
                _WhatsAppCampaignModel.sender = SqlDataHelper.GetValue<string>(dataReader, "Sender");
                _WhatsAppCampaignModel.sent = SqlDataHelper.GetValue<long>(dataReader, "Sent");
                _WhatsAppCampaignModel.read = SqlDataHelper.GetValue<long>(dataReader, "NumberOfRead");
                _WhatsAppCampaignModel.delivered = SqlDataHelper.GetValue<long>(dataReader, "NumberOfDelivered");
                _WhatsAppCampaignModel.templateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId");
                _WhatsAppCampaignModel.status = SqlDataHelper.GetValue<int>(dataReader, "Status");
                _WhatsAppCampaignModel.fromNumber = SqlDataHelper.GetValue<long>(dataReader, "FromNumber");
                _WhatsAppCampaignModel.sentTime = SqlDataHelper.GetValue<DateTime>(dataReader, "SentTime");



                return _WhatsAppCampaignModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static WhatsAppContactsDto MapScheduledCampaign(IDataReader dataReader)
        {
            WhatsAppContactsDto contacts = new WhatsAppContactsDto();
            var contactsDetails = SqlDataHelper.GetValue<string>(dataReader, "Contacts");

            var options = new JsonSerializerOptions { WriteIndented = true };

            contacts = System.Text.Json.JsonSerializer.Deserialize<WhatsAppContactsDto>(contactsDetails, options);
            return contacts;
        }
        #endregion

    }
}
