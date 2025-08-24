using CampaignSendNew.Model;
using Microsoft.Azure.WebJobs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CampaignSendNew.WhatsAppApi.Dto;
using Microsoft.Extensions.Logging;

namespace CampaignSendNew
{
    public static class SendScheduledCampaign1 
    {
        //[FunctionName("SendScheduledCampaign1")]
        //public static void SendScheduledCampaign([TimerTrigger("0 0 * * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign1";
        //    Sync(log, JopName).Wait();
        //}
        //[FunctionName("SendScheduledCampaign10")]
        //public static void SendScheduledCampaign10([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign10";

        //    Sync(log, JopName).Wait();
        //}
        //[FunctionName("SendScheduledCampaign2")]
        //public static void SendScheduledCampaign2([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign2";

        //    Sync(log, JopName).Wait();
        //}
        //[FunctionName("SendScheduledCampaign3")]
        //public static void SendScheduledCampaign3([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign3";

        //    Sync(log, JopName).Wait();
        //}

        //[FunctionName("SendScheduledCampaign4")]
        //public static void SendScheduledCampaign4([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign4";

        //    Sync(log, JopName).Wait();
        //}

        //[FunctionName("SendScheduledCampaign5")]
        //public static void SendScheduledCampaign5([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign5";

        //    Sync(log, JopName).Wait();
        //}

        //[FunctionName("SendScheduledCampaign6")]
        //public static void SendScheduledCampaign6([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign6";

        //    Sync(log, JopName).Wait();
        //}

        //[FunctionName("SendScheduledCampaign7")]
        //public static void SendScheduledCampaign7([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign7";

        //    Sync(log, JopName).Wait();
        //}
        //[FunctionName("SendScheduledCampaign8")]
        //public static void SendScheduledCampaign8([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    string JopName = "SendScheduledCampaign8";

        //    Sync(log, JopName).Wait();
        //}
        //[FunctionName("SendScheduledCampaign9")]
        //public static void SendScheduledCampaign9([TimerTrigger("0 0 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)//"0 */1 * * * *"
        //{
        //    string JopName = "SendScheduledCampaign9";

        //    Sync(log, JopName).Wait();
        //}


        public static async Task Sync(ILogger log,string JopName)
        {
            try
            {
                //string JopName = "SendScheduledCampaign1";
                List<ScheduledCampaign> campaigns = GetScheduledCampaign(JopName);
                if (campaigns.Any())
                {
                    foreach (var campaign in campaigns)
                    {
                        if (campaign.SendTime > DateTime.UtcNow)
                        {
                            continue;
                        }

                        using (var httpClient = new HttpClient())
                        {
                            var tenantInfo = GetTenantInfo(campaign.TenantId);

                            MessageTemplateModel objWhatsAppTemplateModel = getTemplateById(campaign.templateId);
                            MessageTemplateModel templateWA = getTemplateByWhatsId(tenantInfo, objWhatsAppTemplateModel.id).Result;

                            if (templateWA != null && templateWA.status == "APPROVED")
                            {
                                objWhatsAppTemplateModel.components = templateWA.components;

                                string msg = prepareMessageTemplateText(objWhatsAppTemplateModel, out string type);
                            


                                //List<CampaignCosmoDBModel> vs = new List<CampaignCosmoDBModel>();
                                int count = 0;
                                foreach (var contact in campaign.contacts)
                                {
                                    try
                                    {


                                        var teaminboxmsg = msg;

                                        if (contact.templateVariables!=null)
                                        {

                                            if (contact.templateVariables.VarOne!=null)
                                            {
                                                teaminboxmsg= teaminboxmsg.Replace("{{1}}", contact.templateVariables.VarOne);

                                            }
                                            if (contact.templateVariables.VarTwo!=null)
                                            {
                                                teaminboxmsg= teaminboxmsg.Replace("{{2}}", contact.templateVariables.VarTwo);

                                            }
                                            if (contact.templateVariables.VarThree!=null)
                                            {
                                                teaminboxmsg= teaminboxmsg.Replace("{{3}}", contact.templateVariables.VarThree);

                                            }
                                            if (contact.templateVariables.VarFour!=null)
                                            {
                                                teaminboxmsg= teaminboxmsg.Replace("{{4}}", contact.templateVariables.VarFour);

                                            }
                                            if (contact.templateVariables.VarFive!=null)
                                            {
                                                teaminboxmsg= teaminboxmsg.Replace("{{5}}", contact.templateVariables.VarFive);

                                            }
                                        }


                                        PostMessageTemplateModel _postMessageTemplateModel = prepareMessageTemplate(objWhatsAppTemplateModel, contact);
                                        var postBody = JsonConvert.SerializeObject(_postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                                        var postUrl = Constant.WhatsAppApiUrl + tenantInfo.D360Key + "/messages";
                                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenantInfo.AccessToken);

                                        using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                                        {
                                            var result = await response.Content.ReadAsStringAsync();

                                            WhatsAppMessageTemplateResult templateResult = new WhatsAppMessageTemplateResult();
                                            templateResult = JsonConvert.DeserializeObject<WhatsAppMessageTemplateResult>(result);

                                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                            {
                                                CampaignCosmoDBModel campaignCosmoDBModel = new CampaignCosmoDBModel()
                                                {
                                                    campaignId = campaign.campaignId.ToString(),
                                                    messagesId = templateResult.messages.FirstOrDefault().id,
                                                    phoneNumber = contact.PhoneNumber,
                                                    contactName = contact.ContactName,
                                                    msg = teaminboxmsg,
                                                    isSent = true,
                                                    isDelivered = false,
                                                    isFailed = false,
                                                    isRead = false,
                                                    isReplied = false,
                                                    tenantId = tenantInfo.TenantId,
                                                    itemType = ContainerItemTypes.Campaign,
                                                    sendTime = DateTime.UtcNow,
                                                    templateName = objWhatsAppTemplateModel.name,
                                                    campaignName = campaign.campaignName,
                                                    templateId = campaign.templateId
                                                };
                                                if (!campaign.IsExternal)
                                                {
                                                    campaignCosmoDBModel.contactId = contact.Id.ToString();
                                                }
                                                var itemsCollection = new DocumentDBHelper<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection);
                                                await itemsCollection.CreateItemAsync(campaignCosmoDBModel);
                                            }
                                            else
                                            {
                                                CampaignCosmoDBModel campaignCosmoDBModel = new CampaignCosmoDBModel()
                                                {
                                                    campaignId = campaign.campaignId.ToString(),
                                                    messagesId = templateResult.messages.FirstOrDefault().id,
                                                    phoneNumber = contact.PhoneNumber,
                                                    contactName = contact.ContactName,
                                                    msg = teaminboxmsg,
                                                    isSent = true,
                                                    isDelivered = false,
                                                    isFailed = true,
                                                    isRead = false,
                                                    isReplied = false,
                                                    tenantId = tenantInfo.TenantId,
                                                    itemType = ContainerItemTypes.Campaign,
                                                    sendTime = DateTime.UtcNow,
                                                    templateName = objWhatsAppTemplateModel.name,
                                                    campaignName = campaign.campaignName,
                                                    templateId = campaign.templateId
                                                };
                                                var itemsCollection = new DocumentDBHelper<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection);
                                                await itemsCollection.CreateItemAsync(campaignCosmoDBModel);
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CampaignCosmoDBModel campaignCosmoDBModel = new CampaignCosmoDBModel()
                                        {
                                            campaignId = campaign.campaignId.ToString(),
                                            messagesId = "",
                                            phoneNumber = contact.PhoneNumber,
                                            contactName = contact.ContactName,
                                            msg = "",
                                            isSent = true,
                                            isDelivered = false,
                                            isFailed = true,
                                            isRead = false,
                                            isReplied = false,
                                            tenantId = tenantInfo.TenantId,
                                            itemType = ContainerItemTypes.Campaign,
                                            sendTime = DateTime.UtcNow,
                                            templateName = objWhatsAppTemplateModel.name,
                                            campaignName = campaign.campaignName,
                                            templateId = campaign.templateId
                                        };
                                        var itemsCollection = new DocumentDBHelper<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection);
                                        await itemsCollection.CreateItemAsync(campaignCosmoDBModel);
                                        log.LogInformation($"{ex}");
                                    }
                                    count++;
                                }

                                //change status campain After sending
                                long resultId = UpdateCampaignStatus(campaign.Id);

                                #region statistic Campaign
                                WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
                                {
                                    id = campaign.Id,
                                    sentTime = DateTime.UtcNow,
                                    status = 1 // as sent
                                };
                                updateScheduledWhatsAppCampaign(whatsAppCampaignModel, tenantInfo.TenantId, campaign.contacts.Count(), campaign.campaignName);
                                #endregion
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation($"{ex}");
            }
        }
        private static void updateScheduledWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel, int TenantId, int count, string name)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppScheduledCampaignUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@CampaignId",whatsAppCampaignModel.id)
                    ,new System.Data.SqlClient.SqlParameter("@SentTime",whatsAppCampaignModel.sentTime)
                    ,new System.Data.SqlClient.SqlParameter("@Status",whatsAppCampaignModel.status)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Count",count)
                    ,new System.Data.SqlClient.SqlParameter("@SentCampaignId",Guid.NewGuid().ToString())
                    ,new System.Data.SqlClient.SqlParameter("@NameCampaign",name)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static List<ScheduledCampaign> GetScheduledCampaign(string JopName)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_ScheduledCampaignGetFromDB;
                var sqlParameters = new List<SqlParameter> { 
                    new SqlParameter("@DateTime", DateTime.UtcNow) ,
                    new SqlParameter("@JopName", JopName)
                };
                List<ScheduledCampaign> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, Constants.ConnectionString).ToList();
                return model;
            }
            catch
            {
                return new List<ScheduledCampaign>();
            }
        }
        private static PostMessageTemplateModel prepareMessageTemplate(MessageTemplateModel objWhatsAppTemplateModel, ListContactToCampin contact)
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
                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };
                            }
                            if (item.format == "VIDEO")
                            {
                                video.link = objWhatsAppTemplateModel.mediaLink;
                                parameterHeader.video = video;
                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };
                            }
                            if (item.format == "DOCUMENT")
                            {
                                if (objWhatsAppTemplateModel.mediaLink.Contains(","))
                                {
                                    try
                                    {
                                        document.filename=objWhatsAppTemplateModel.mediaLink.Split(",")[0];
                                        document.link =objWhatsAppTemplateModel.mediaLink.Split(",")[1];
                                    }
                                    catch
                                    {
                                        document.link = objWhatsAppTemplateModel.mediaLink;

                                    }

                                }
                                else
                                {

                                    document.link = objWhatsAppTemplateModel.mediaLink;
                                }
                                parameterHeader.document = document;

                                componentHeader.parameters = new List<Parameter>
                                {
                                    parameterHeader
                                };
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
        private static TenantInfoDto GetTenantInfo(int tenantId)
        {
            TenantInfoDto tenantInfoDto = new TenantInfoDto();
            try
            {
                var SP_Name = Constants.Tenant.SP_TenantInfoGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@tenantId",tenantId)
                };

                tenantInfoDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapGetTenantInfo, Constants.ConnectionString).FirstOrDefault();

                return tenantInfoDto;
            }
            catch
            {
                return tenantInfoDto;
            }
        }
        private static MessageTemplateModel getTemplateById(long templateId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplatesGetById;
                MessageTemplateModel objWhatsAppTemplateModel = new MessageTemplateModel();
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> { new System.Data.SqlClient.SqlParameter("@TemplateId", templateId) };
                objWhatsAppTemplateModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapTemplate, Constants.ConnectionString).FirstOrDefault();
                return objWhatsAppTemplateModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static async Task<MessageTemplateModel> getTemplateByWhatsId(TenantInfoDto tenant, string templateId)
        {
            using (var httpClient = new HttpClient())
            {
                var postUrl = Constants.WhatsAppTemplates.WhatsAppApiUrl + templateId;
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tenant.AccessToken);

                using (var response = await httpClient.GetAsync(postUrl))
                {
                    using (var content = response.Content)
                    {
                        try
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                var WhatsAppTemplate = await content.ReadAsStringAsync();
                                MessageTemplateModel objWhatsAppTemplate = new MessageTemplateModel();
                                objWhatsAppTemplate = JsonConvert.DeserializeObject<MessageTemplateModel>(WhatsAppTemplate);
                                return objWhatsAppTemplate;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
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
                        if (item.type.Equals("BUTTONS"))
                        {
                            for (int i = 0; i < item.buttons.Count; i++)
                            {
                                result = result + "\n\r" + (i + 1) + "-" + item.buttons[i].text;
                            }
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
        private static long UpdateCampaignStatus(long rowId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_ScheduledCampaignUpdateStatus;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

                return (OutputParameter.Value != DBNull.Value) ? Convert.ToInt64(OutputParameter.Value) : 0;
            }
            catch
            {
                return 0;
            }
        }
        #region Mapper
        public static ScheduledCampaign MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, SendDateTime, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                ScheduledCampaign model = new ScheduledCampaign
                {
                    Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    campaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId"),
                    templateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId"),
                    IsExternal = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
                    SendTime = SqlDataHelper.GetValue<DateTime>(dataReader, "SendDateTime"),
                    CreatedDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreatedDate"),
                    TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                    UserId = SqlDataHelper.GetValue<long>(dataReader, "UserId"),
                    JopName = SqlDataHelper.GetValue<string>(dataReader, "JopName"),
                    campaignName = SqlDataHelper.GetValue<string>(dataReader, "CampaignName"),
                    templateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName"),
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent")
                };

                // Deserialize ContactsJson to List<ListContactToCampin>
                model.contacts = System.Text.Json.JsonSerializer.Deserialize<List<ListContactToCampin>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new ScheduledCampaign();
            }
        }
        public static TenantInfoDto MapGetTenantInfo(IDataReader dataReader)
        {
            TenantInfoDto model = new TenantInfoDto();
            model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id");
            model.AccessToken = SqlDataHelper.GetValue<string>(dataReader, "AccessToken");
            model.D360Key = SqlDataHelper.GetValue<string>(dataReader, "D360Key");

            return model;
        }
        private static MessageTemplateModel MapTemplate(IDataReader dataReader)
        {
            try
            {
                MessageTemplateModel _MessageTemplateModel = new MessageTemplateModel
                {
                    name = SqlDataHelper.GetValue<string>(dataReader, "Name"),
                    language = SqlDataHelper.GetValue<string>(dataReader, "Language"),
                    category = SqlDataHelper.GetValue<string>(dataReader, "Category"),
                    id = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppTemplateId"),
                    LocalTemplateId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    mediaType = SqlDataHelper.GetValue<string>(dataReader, "MediaType"),
                    mediaLink = SqlDataHelper.GetValue<string>(dataReader, "MediaLink"),
                    isDeleted = SqlDataHelper.GetValue<bool>(dataReader, "IsDeleted"),
                    VariableCount = SqlDataHelper.GetValue<int>(dataReader, "VariableCount"),
                };

                var components = SqlDataHelper.GetValue<string>(dataReader, "Components");
                var options = new JsonSerializerOptions { WriteIndented = true };
                _MessageTemplateModel.components = System.Text.Json.JsonSerializer.Deserialize<List<WhatsAppComponentModel>>(components, options);

                return _MessageTemplateModel;
            }
            catch
            {
                return new MessageTemplateModel();
            }

        }
        #endregion
    }
}
