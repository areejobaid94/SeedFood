using CampaignSendNew.Model;
using CampaignSendNew.WhatsAppApi.Dto;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CampaignSendNew
{
    public  class NewSendCampaingFun
    {
    //    [FunctionName("NewSendCampaingFun")]
    //    public static async Task Campaign1Async([QueueTrigger("NewSendCampaingFun", Connection = "AzureWebJobsStorage")] string message, ILogger log)
    //    {

    //        CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

    //        var funName = "NewSendCampaingFun";
    //        List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
    //        try
    //        {

    //            if (campaigns.Count()>0)
    //            {
    //                WhatsAppCampaignModel whatsAppCampaignModel2 = new WhatsAppCampaignModel
    //                {
    //                    id = obj.campaignId,
    //                    sentTime = DateTime.UtcNow,
    //                    status = 1 // as sent
    //                };
    //                updateWhatsAppCampaign(whatsAppCampaignModel2, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());

    //                await sendcamp(log, obj, campaigns, "NewSendCampaingFun");


    //            }


    //        }
    //        catch (Exception ex)
    //        {
    //            log.LogInformation($"{ex}");
    //        }

    //    }



        private static async Task sendcamp(ILogger log, CampinQueueNew obj, List<SendCampaignNow> campaigns, string jopName)
        {
            string type = "";
            string mediaUrl = "";
            string msg = prepareMessageTemplateText(obj.messageTemplateModel, out type, out mediaUrl);

            if (campaigns.Any())
            {
                using (var httpClient = new HttpClient())
                {
                    var itemsCollection = new DocumentDBHelper<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection);

                    // Create a SemaphoreSlim to limit the rate to 60 requests per second
                    SemaphoreSlim rateLimitSemaphore = new SemaphoreSlim(1, 1);
                    int requestIntervalInMilliseconds = 1000 / 60; // 60 requests per second -> ~16ms interval

                    foreach (var campaign in campaigns)
                    {
                        foreach (var contacts in campaign.contacts)
                        {
                            Stopwatch stopwatch = Stopwatch.StartNew();

                            try
                            {
                                var teaminboxmsg = msg;

                                // Replace template variables if they exist
                                if (contacts.templateVariables != null)
                                {
                                    if (contacts.templateVariables.VarOne != null)
                                    {
                                        teaminboxmsg = teaminboxmsg.Replace("{{1}}", contacts.templateVariables.VarOne);
                                    }
                                    if (contacts.templateVariables.VarTwo != null)
                                    {
                                        teaminboxmsg = teaminboxmsg.Replace("{{2}}", contacts.templateVariables.VarTwo);
                                    }
                                    if (contacts.templateVariables.VarThree != null)
                                    {
                                        teaminboxmsg = teaminboxmsg.Replace("{{3}}", contacts.templateVariables.VarThree);
                                    }
                                    if (contacts.templateVariables.VarFour != null)
                                    {
                                        teaminboxmsg = teaminboxmsg.Replace("{{4}}", contacts.templateVariables.VarFour);
                                    }
                                    if (contacts.templateVariables.VarFive != null)
                                    {
                                        teaminboxmsg = teaminboxmsg.Replace("{{5}}", contacts.templateVariables.VarFive);
                                    }
                                }

                                PostMessageTemplateModel _postMessageTemplateModel = prepareMessageTemplate(obj.messageTemplateModel, contacts, obj.IsExternal, obj.templateVariables);
                                var postBody = JsonConvert.SerializeObject(_postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                                var postUrl = Constant.WhatsAppApiUrl + obj.D360Key + "/messages";
                                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", obj.AccessToken);

                                await rateLimitSemaphore.WaitAsync(); // Wait for semaphore

                                try
                                {
                                    using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                                    {
                                        var result = await response.Content.ReadAsStringAsync();
                                        var templateResult = JsonConvert.DeserializeObject<WhatsAppMessageTemplateResult>(result);

                                        var campaignss = new CampaignCosmoDBModel()
                                        {
                                            campaignId = obj.campaignId.ToString(),
                                            messagesId = templateResult?.messages?.FirstOrDefault()?.id ?? "",
                                            phoneNumber = contacts.PhoneNumber,
                                            contactName = contacts.ContactName,
                                            msg = teaminboxmsg,
                                            mediaUrl = "", // mediaUrl,
                                            type = "text", // type,
                                            isSent = response.StatusCode == System.Net.HttpStatusCode.OK,
                                            isDelivered = false,
                                            isFailed = response.StatusCode != System.Net.HttpStatusCode.OK,
                                            isRead = false,
                                            isReplied = false,
                                            tenantId = obj.TenantId,
                                            itemType = ContainerItemTypes.Campaign,
                                            sendTime = DateTime.UtcNow,
                                            templateName = obj.messageTemplateModel.name,
                                            campaignName = obj.campaignName,
                                            templateId = obj.templateId
                                        };

                                        if (!obj.IsExternal)
                                        {
                                            campaignss.contactId = contacts.Id.ToString();
                                        }

                                        await itemsCollection.CreateItemAsync(campaignss);
                                    }
                                }
                                finally
                                {
                                    stopwatch.Stop();
                                    TimeSpan timeTaken = stopwatch.Elapsed;

                                    // Ensure a delay to maintain 60 requests per second rate
                                    int delayTime = requestIntervalInMilliseconds - (int)timeTaken.TotalMilliseconds;
                                    if (delayTime > 0)
                                    {
                                        await Task.Delay(delayTime);
                                    }

                                    rateLimitSemaphore.Release(); // Release semaphore
                                }
                            }
                            catch (Exception ex)
                            {
                                log.LogInformation($"{ex}");
                                var campaignss = new CampaignCosmoDBModel()
                                {
                                    campaignId = obj.campaignId.ToString(),
                                    messagesId = "",
                                    phoneNumber = contacts.PhoneNumber,
                                    contactName = contacts.ContactName,
                                    msg = "",
                                    mediaUrl = "", // mediaUrl,
                                    type = "text", // type,
                                    isSent = true,
                                    isDelivered = false,
                                    isFailed = true,
                                    isRead = false,
                                    isReplied = false,
                                    tenantId = obj.TenantId,
                                    itemType = ContainerItemTypes.Campaign,
                                    sendTime = DateTime.UtcNow,
                                    templateName = obj.messageTemplateModel.name,
                                    campaignName = obj.campaignName,
                                    templateId = obj.templateId
                                };
                                await itemsCollection.CreateItemAsync(campaignss);
                            }
                        }
                    }
                }
            }
        }


        private static PostMessageTemplateModel prepareMessageTemplate(MessageTemplateModel objWhatsAppTemplateModel, ListContactToCampin contact, bool IsExternal, TemplateVariablles templateVariables)
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
                        if (objWhatsAppTemplateModel.category=="AUTHENTICATION")
                        {
                            Component component = new Component();


                            if (item.type == "BODY")
                            {

                                component.type="body";
                                component.parameters=new List<Parameter>();
                                component.parameters.Add(new Parameter()
                                {
                                    type="text",
                                    text=contact.templateVariables.VarOne

                                });


                            }
                            else if (item.type == "BUTTONS")
                            {
                                component.type="button";
                                component.sub_type="url";
                                component.index=0;
                                component.parameters=new List<Parameter>();
                                component.parameters.Add(new Parameter()
                                {
                                    type="text",
                                    text=contact.templateVariables.VarOne

                                });


                            }



                            components.Add(component);


                        }
                        else
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
                                        Uri uri = new Uri(objWhatsAppTemplateModel.mediaLink);

                                        // Extract the file name
                                        string fileName = System.IO.Path.GetFileName(uri.LocalPath);

                                        if (fileName.Length<20)
                                        {

                                            document.filename=fileName;
                                        }
                                        else
                                        {
                                            document.filename="fileName";
                                        }



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
                                else if (templateVariables != null)
                                {
                                    if (objWhatsAppTemplateModel.VariableCount >= 1)
                                    {
                                        componentBody.parameters = new List<Parameter>();

                                        parameterBody1.type = "TEXT";
                                        parameterBody1.text = templateVariables.VarOne;
                                        componentBody.parameters.Add(parameterBody1);

                                        if (objWhatsAppTemplateModel.VariableCount >= 2)
                                        {
                                            parameterBody2.type = "TEXT";
                                            parameterBody2.text = templateVariables.VarTwo;
                                            componentBody.parameters.Add(parameterBody2);

                                            if (objWhatsAppTemplateModel.VariableCount >= 3)
                                            {
                                                parameterBody3.type = "TEXT";
                                                parameterBody3.text = templateVariables.VarThree;
                                                componentBody.parameters.Add(parameterBody3);

                                                if (objWhatsAppTemplateModel.VariableCount >= 4)
                                                {
                                                    parameterBody4.type = "TEXT";
                                                    parameterBody4.text = templateVariables.VarFour;
                                                    componentBody.parameters.Add(parameterBody4);

                                                    if (objWhatsAppTemplateModel.VariableCount >= 5)
                                                    {
                                                        parameterBody5.type = "TEXT";
                                                        parameterBody5.text = templateVariables.VarFive;
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

        private static List<SendCampaignNow> GetCampaign(long rowId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_SendCampaignGetFromDB;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@rowId", rowId)
                };
                List<SendCampaignNow> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, Constants.ConnectionString).ToList();
                return model;
            }
            catch
            {
                return new List<SendCampaignNow>();
            }
        }
        private static void updateWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel, int TenantId, int count)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@CampaignId",whatsAppCampaignModel.id)
                    ,new System.Data.SqlClient.SqlParameter("@SentTime",whatsAppCampaignModel.sentTime)
                    ,new System.Data.SqlClient.SqlParameter("@Status",whatsAppCampaignModel.status)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Count",count)
                    ,new System.Data.SqlClient.SqlParameter("@SentCampaignId",Guid.NewGuid())
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static SendCampaignNow MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                SendCampaignNow model = new SendCampaignNow
                {
                    rowId = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                    campaignId = SqlDataHelper.GetValue<long>(dataReader, "CampaignId"),
                    templateId = SqlDataHelper.GetValue<long>(dataReader, "TemplateId"),
                    IsExternal = SqlDataHelper.GetValue<bool>(dataReader, "IsExternalContact"),
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
                return new SendCampaignNow();
            }
        }
        private static string prepareMessageTemplateText(MessageTemplateModel objWhatsAppTemplateModel, out string type, out string mediaUrl)
        {
            try
            {
                string result = string.Empty;
                type = "text";
                mediaUrl = "";
                if (objWhatsAppTemplateModel.components != null)
                {
                    foreach (var item in objWhatsAppTemplateModel.components)
                    {
                        if (item.type.Equals("HEADER"))
                        {


                            type = item.format.ToLower();

                            if (type=="document")
                            {


                                if (!string.IsNullOrEmpty(objWhatsAppTemplateModel.mediaLink))
                                {
                                    if (objWhatsAppTemplateModel.mediaLink.Contains(","))
                                    {

                                        var media = objWhatsAppTemplateModel.mediaLink.Split(",")[1];
                                        try
                                        {
                                            type = "application";
                                            result+=media+"\n\r";

                                            mediaUrl=media;
                                        }
                                        catch
                                        {

                                        }

                                    }
                                    else
                                    {
                                        try
                                        {
                                            type = "application";
                                            result+=objWhatsAppTemplateModel.mediaLink+"\n\r";

                                            mediaUrl=objWhatsAppTemplateModel.mediaLink;
                                        }
                                        catch
                                        {

                                        }

                                    }




                                }
                                else
                                {
                                    try
                                    {
                                        type = "application";
                                        result+=item.example.header_handle[0]+"\n\r";

                                        mediaUrl=item.example.header_handle[0];
                                    }
                                    catch
                                    {

                                    }


                                }



                            }

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
    }
}
