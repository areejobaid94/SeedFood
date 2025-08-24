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
using TargetReachModel = InfoSeedAzureFunction.Model.TargetReachModel;
using InfoSeedAzureFunction.AppFunEntities;

namespace InfoSeedAzureFunction
{
    public static class SendGlamera
    {

        //[FunctionName("SendGlamera")]
        //public static void SendGlameraF([QueueTrigger("targetreach-sync", Connection = "AzureWebJobsStorage")] string message, ILogger log)

        //{
        //    List<TargetReachModel> obj = JsonConvert.DeserializeObject<List<TargetReachModel>>(message);

        //    Sync(obj, log).Wait();
        //}
        public static async Task Sync(List<TargetReachModel> model, ILogger log)
        {
            try
            {
           

                var tenantId = model.FirstOrDefault().TenantId;
                var templateName = model.FirstOrDefault().TemplateName;
                var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
                TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.Tenant && a.TenantId == tenantId);
                ContactsEntity contactsEntity = new ContactsEntity();
                Hashtable CampaignHashTable = new Hashtable();

                Guid guid = Guid.NewGuid();
                MessageTemplateModel objWhatsAppTemplateModel = getTemplateByName(log, templateName, tenantId);
                var whatsAppTemplate = await getTemplateByWhatsAppId(log, tenant, objWhatsAppTemplateModel.id);
                List<TargetReachModel> lstTargetReachModel = new List<TargetReachModel>();


                await NewSendCampaignFunAsync(log, model, tenant, objWhatsAppTemplateModel);

            }
            catch (Exception ex)
            {

                log.LogInformation($"C# Queue Sync function processed: {ex}");
                throw ex;
            }
          
        } /*****/
        private static async Task NewSendCampaignFunAsync(ILogger log, List<TargetReachModel> model, TenantModel tenant, MessageTemplateModel objWhatsAppTemplateModel)
        {
            try
            {

                WhatsAppContactsDto contact = new WhatsAppContactsDto();
                TemplateVariables variables = new TemplateVariables();
                variables.VarOne = model.FirstOrDefault().Message;

                contact.Id = model.FirstOrDefault().ContactId;
                contact.PhoneNumber = model.FirstOrDefault().PhoneNumber;
                contact.ContactName = model.FirstOrDefault().CustomerName;
                contact.templateVariables = variables;

                string type;
                string msg = prepareMessageTemplateText(objWhatsAppTemplateModel, model.FirstOrDefault().Message, out type);
                List<SendCampaignFailedModel> LstSendCampaignFailedModel = new List<SendCampaignFailedModel>();

                PostMessageTemplateModel _postMessageTemplateModel = prepareMessageTemplate(objWhatsAppTemplateModel, contact);
                var postBody = JsonConvert.SerializeObject(_postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });



                await NewSendTemplateToWhatsAppFun(log, tenant, postBody);


            

            }
            catch (Exception ex)
            {
               
                log.LogInformation($"C# Queue SendCampaignAsync function processed: {ex}");
            }
        }
        private static string prepareMessageTemplateText(MessageTemplateModel objWhatsAppTemplateModel, string message, out string type)
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
                        result = result + " " + item.text;

                    }

                }
                result = result.Replace("{{1}}", message);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }    
        private static async Task<ContactCampaignModel> NewSendTemplateToWhatsAppFun(ILogger log, TenantModel tenant, string postBody)
        {
            try
            {
                ContactCampaignModel contactCampaignModel = new ContactCampaignModel();

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
                           
                        }
                        else
                        {

                            
                        }
                        return contactCampaignModel;
                    }

                }
            }
            catch (Exception ex)
            {
                ContactCampaignModel contactCampaignModel = new ContactCampaignModel();
                log.LogInformation($"C# Queue SendCampaignAsync function processed: {ex}");
                return contactCampaignModel;
            }
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
        private static MessageTemplateModel getTemplateByName(ILogger log, string templateName, int tenantId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppTemplates.SP_WhatsAppTemplateByNameGet;
                MessageTemplateModel objWhatsAppTemplateModel = new MessageTemplateModel();
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@TenantId", tenantId),
                    new SqlParameter("@TemplateName", templateName)
                };
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
        #region Mapper
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
        #endregion

    }
}
