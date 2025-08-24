using CampaignSendNew.Model;
using CampaignSendNew.WhatsAppApi;
using CampaignSendNew.WhatsAppApi.Dto;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Reflection;
using Microsoft.Bot.Connector.DirectLine;

namespace CampaignSendNew
{


    //https://infoseedstoragestg.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/sample-pdf-with-images.pdf
    public static class SendCampaign1
    {
        //[FunctionName("Campaign1")]
        //public static async Task Campaign1Async([QueueTrigger("campaign1", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        // {

        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

        //    var funName = "campaign1";
        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {

        //        if (campaigns.Count()>0)
        //        {
        //            WhatsAppCampaignModel whatsAppCampaignModel2 = new WhatsAppCampaignModel
        //            {
        //                id = obj.campaignId,
        //                sentTime = DateTime.UtcNow,
        //                status = 3 // as sent
        //            };
        //            updateWhatsAppCampaign(whatsAppCampaignModel2, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());


        //            var client = new HttpClient();
        //            var request = new HttpRequestMessage(HttpMethod.Post, "https://startcampign.azurewebsites.net/api/startCampaign");
        //            var content = new StringContent("{\n    \"campaignId\": "+obj.campaignId+"\n}", null, "application/json");
        //            request.Content = content;
        //            var response = await client.SendAsync(request);
        //            response.EnsureSuccessStatusCode();


        //            if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //            {
        //                //WhatsAppCampaignModel whatsAppCampaignModel23 = new WhatsAppCampaignModel
        //                //{
        //                //    id = obj.campaignId,
        //                //    sentTime = DateTime.UtcNow,
        //                //    status = 3 // as sent
        //                //};
        //                //updateWhatsAppCampaign(whatsAppCampaignModel23, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());
        //            }
        //            else
        //            {
        //                WhatsAppCampaignModel whatsAppCampaignModel23 = new WhatsAppCampaignModel
        //                {
        //                    id = obj.campaignId,
        //                    sentTime = DateTime.UtcNow,
        //                    status = 4 // as sent
        //                };
        //                updateWhatsAppCampaign(whatsAppCampaignModel23, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());

        //            }




        //            //     await sendcamp(log, obj, campaigns, "campaign1");


        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        WhatsAppCampaignModel whatsAppCampaignModel23 = new WhatsAppCampaignModel
        //        {
        //            id = obj.campaignId,
        //            sentTime = DateTime.UtcNow,
        //            status = 4 // as sent
        //        };
        //        updateWhatsAppCampaign(whatsAppCampaignModel23, obj.TenantId, campaigns.FirstOrDefault().contacts.Count());
        //        log.LogInformation($"{ex}");
        //    }
           
        //}
        //[FunctionName("Campaign2")]
        //public static async Task Campaign2Async([QueueTrigger("campaign2", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

        //    var funName = "campaign2";
        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {
        //        await sendcamp(log, obj, campaigns, "campaign2");

        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation($"{ex}");
        //    }



        //}
        //[FunctionName("Campaign3")]
        //public static async Task Campaign3Async([QueueTrigger("campaign3", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

      
        //    var funName = "campaign3";
        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {
        //        await sendcamp(log, obj, campaigns, "campaign3");


        //    }
        //    catch (Exception ex)
        //    {
              
        //        log.LogInformation($"{ex}");
        //    }



        //}
        //[FunctionName("Campaign4")]
        //public static async Task Campaign4Async([QueueTrigger("campaign4", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

        //    var funName = "campaign4";

        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {

        //        await sendcamp(log, obj, campaigns, "campaign4");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation($"{ex}");
        //    }



        //}
        //[FunctionName("Campaign5")]
        //public static async Task Campaign5Async([QueueTrigger("campaign5", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);
        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {
        //        await sendcamp(log, obj, campaigns, "campaign5");

        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation($"{ex}");
        //    }

        //}
        //[FunctionName("Campaign6")]
        //public static async Task Campaign6Async([QueueTrigger("campaign6", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

        //    var funName = "campaign6";
        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {

        //        await sendcamp(log, obj, campaigns, "campaign6");

        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation($"{ex}");
        //    }

        //}
        //[FunctionName("Campaign7")]
        //public static async Task Campaign7Async([QueueTrigger("campaign7", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);


        //    var funName = "campaign7";
        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {


        //        await sendcamp(log, obj, campaigns, "campaign7");




        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation($"{ex}");
        //    }


        //}
        //[FunctionName("Campaign8")]
        //public static async Task Campaign8Async([QueueTrigger("campaign8", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);


        //    var funName = "campaign8";

        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {
        //        await sendcamp(log, obj, campaigns, "campaign8");

        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation($"{ex}");
        //    }


        //}
        //[FunctionName("Campaign9")]
        //public static async Task Campaign9Async([QueueTrigger("campaign9", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);
        //    var funName = "campaign9";
        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {

        //        await sendcamp(log, obj, campaigns, "campaign9");

        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation($"{ex}");
        //    }


        //}
        //[FunctionName("Campaign10")]
        //public static async Task Campaign10Async([QueueTrigger("campaign10", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        //{
        //    CampinQueueNew obj = JsonConvert.DeserializeObject<CampinQueueNew>(message);

        //    var funName = "campaign10";
        //    List<SendCampaignNow> campaigns = GetCampaign(obj.rowId);
        //    try
        //    {

        //        await sendcamp(log, obj, campaigns, "campaign10");
        //    }
        //    catch (Exception ex)
        //    {
        //        log.LogInformation($"{ex}");
        //    }

        //}
        private static async Task sendcamp(ILogger log, CampinQueueNew obj, List<SendCampaignNow> campaigns,string jopName)
        {

            


            string type = "";
            string mediaUrl = "";
            string msg = prepareMessageTemplateText(obj.messageTemplateModel, out type ,out mediaUrl);


            List<CampaignCosmoDBModel> vs = new List<CampaignCosmoDBModel>();


            if (campaigns.Any())
            {
                foreach (var campaign in campaigns)
                {

                    using (var httpClient = new HttpClient())
                    {
                       
                        int count = 0;
                        var total = campaign.contacts.Count();


                        TimeSpan targetTimeSpan = TimeSpan.FromHours(1);
                        int totalMessages = campaign.contacts.Count();// 1000;
                        TimeSpan delayBetweenMessages = targetTimeSpan / totalMessages;



                        var itemsCollection = new DocumentDBHelper<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection);




                        foreach (var contacts in campaign.contacts)
                        {

                            contacts.PhoneNumber= contacts.PhoneNumber.Replace("+", "");
                            Stopwatch stopwatch = Stopwatch.StartNew();
                                                        

                            try
                            {
                                //if (obj.messageTemplateModel.category=="AUTHENTICATION")
                                //{

                                //    contacts.templateVariables=new TemplateVariablles() {VarOne= };

                                //}


                                var teaminboxmsg = msg;

                                if (contacts.templateVariables!=null)
                                {

                                    if (contacts.templateVariables.VarOne!=null)
                                    {
                                        teaminboxmsg= teaminboxmsg.Replace("{{1}}", contacts.templateVariables.VarOne);

                                    }
                                    if (contacts.templateVariables.VarTwo!=null)
                                    {
                                        teaminboxmsg= teaminboxmsg.Replace("{{2}}", contacts.templateVariables.VarTwo);

                                    }
                                    if (contacts.templateVariables.VarThree!=null)
                                    {
                                        teaminboxmsg= teaminboxmsg.Replace("{{3}}", contacts.templateVariables.VarThree);

                                    }
                                    if (contacts.templateVariables.VarFour!=null)
                                    {
                                        teaminboxmsg= teaminboxmsg.Replace("{{4}}", contacts.templateVariables.VarFour);

                                    }
                                    if (contacts.templateVariables.VarFive!=null)
                                    {
                                        teaminboxmsg= teaminboxmsg.Replace("{{5}}", contacts.templateVariables.VarFive);

                                    }
                                }


                                PostMessageTemplateModel _postMessageTemplateModel = prepareMessageTemplate(obj.messageTemplateModel, contacts, obj.IsExternal, obj.templateVariables);
                                var postBody = JsonConvert.SerializeObject(_postMessageTemplateModel, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                                var postUrl = Constant.WhatsAppApiUrl + obj.D360Key + "/messages";
                                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", obj.AccessToken);

          

                                using (var response = await httpClient.PostAsync(postUrl, new StringContent(postBody, Encoding.UTF8, "application/json")))
                                {
                                    var result = await response.Content.ReadAsStringAsync();

                                    WhatsAppMessageTemplateResult templateResult = new WhatsAppMessageTemplateResult();
                                    templateResult = JsonConvert.DeserializeObject<WhatsAppMessageTemplateResult>(result);

                                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        CampaignCosmoDBModel campaignss = new CampaignCosmoDBModel()
                                        {
                                            campaignId = obj.campaignId.ToString(),
                                            messagesId = templateResult.messages.FirstOrDefault().id,
                                            phoneNumber = contacts.PhoneNumber,
                                            contactName = contacts.ContactName,
                                            msg = teaminboxmsg,
                                            mediaUrl="",//mediaUrl,
                                            type ="text",// type,
                                            isSent = true,
                                            isDelivered = false,
                                            isFailed = false,
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
                                    else
                                    {
                                        CampaignCosmoDBModel campaignss = new CampaignCosmoDBModel()
                                        {
                                            campaignId = obj.campaignId.ToString(),
                                            messagesId = templateResult.messages.FirstOrDefault().id,
                                            phoneNumber = contacts.PhoneNumber,
                                            contactName = contacts.ContactName,
                                            msg = teaminboxmsg,
                                            mediaUrl="",//mediaUrl,
                                            type ="text",// type,
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
                                        //var itemsCollection = new DocumentDBHelper<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection);
                                        await itemsCollection.CreateItemAsync(campaignss);

                                        try
                                        {
                                            var stcode = "200";
                                            try
                                            {
                                                 stcode = response.StatusCode.ToString();
                                            }
                                            catch
                                            {
                                                 stcode ="400";
                                            }
                                           
                                            AddToWebHookErorrLog(obj.TenantId, stcode, stcode, contacts.PhoneNumber, result);


                                        }
                                        catch
                                        {

                                        }
                                    }
                                }





                            }
                            catch (Exception ex)
                            {
                                CampaignCosmoDBModel campaignss = new CampaignCosmoDBModel()
                                {
                                    campaignId = obj.campaignId.ToString(),
                                    messagesId = "",
                                    phoneNumber = contacts.PhoneNumber,
                                    contactName = contacts.ContactName,
                                    msg = "",
                                    isSent = true,
                                    mediaUrl="",//mediaUrl,
                                    type ="text",// type,
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
                                //var itemsCollection = new DocumentDBHelper<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection);
                                await itemsCollection.CreateItemAsync(campaignss);
                                log.LogInformation($"{ex}");
                            }
                            count++;

                            stopwatch.Stop();
                            TimeSpan timeTaken = stopwatch.Elapsed;


                            // Calculate the remaining time to wait to ensure 1 hour total
                            TimeSpan remainingDelay = delayBetweenMessages - timeTaken;
                            if (remainingDelay > TimeSpan.Zero && campaign.contacts.Count()>=2000)
                            {
                                await Task.Delay(remainingDelay);
                            }


                        }
         
                    }
                }
                //change status campain After sending
                long resultId = UpdateCampaignStatus(obj.rowId);
            }




            List<SendCampaignNow> campaignsL = GetCampaignList(obj.TenantId,obj.campaignId);


            if (campaignsL!=null)
            {
           
                if (campaignsL.Count()>0)
                {

                    CampinQueueNew campinQueueNew = new CampinQueueNew();

                    var JopName = RemoveNumbers(jopName);


                    if (JopName=="campaign11")
                    {



                    }
                    else
                    {

                        campinQueueNew.messageTemplateModel = obj.messageTemplateModel;
                        campinQueueNew.campaignId = obj.campaignId;
                        campinQueueNew.templateId = obj.templateId;
                        campinQueueNew.IsExternal = obj.IsExternal;
                        campinQueueNew.TenantId = obj.TenantId;
                        campinQueueNew.D360Key = obj.D360Key;
                        campinQueueNew.AccessToken = obj.AccessToken;
                        campinQueueNew.functionName = JopName;
                        campinQueueNew.msg = msg;
                        campinQueueNew.type = type;
                        campinQueueNew.contacts = null;
                        campinQueueNew.templateVariables = null;
                        campinQueueNew.campaignName = obj.campaignName;
                        campinQueueNew.rowId = campaignsL.FirstOrDefault().rowId;
                        SetCampinQueueContact(campinQueueNew);

                    }


                }



            }
           
            

        }
        static string RemoveNumbers(string input)
        {
            return Regex.Replace(input, @"\d+", m => (int.Parse(m.Value) + 1).ToString());
        }
        private static void SetCampinQueueContact(CampinQueueNew campinQueueNew)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Constant.AzureStorageAccount);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference(campinQueueNew.functionName);
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           campinQueueNew
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception)
            {
                var Error = JsonConvert.SerializeObject(campinQueueNew);
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
        private static void updateWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel,int TenantId, int count)
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
        private static long UpdateCampaignStatus(long rowId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_CampaignUpdateStatus;
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

        private static List<SendCampaignNow> GetCampaignList(int tenantId,long campaignId)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_SendCampaignListGetFromDB;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId", tenantId),
                    new System.Data.SqlClient.SqlParameter("@campaignId", campaignId)
                };
                List<SendCampaignNow> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, Constants.ConnectionString).ToList();
                return model;
            }
            catch
            {
                return new List<SendCampaignNow>();
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


                            }else if (item.type == "BUTTONS")
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
        #region Mapper
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
        #endregion

        private static int CreateContactDB(ContactDto contactDto)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactInfoJson",JsonConvert.SerializeObject(contactDto))
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Int;
                OutputParameter.ParameterName = "@ContactId";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

                return (int)OutputParameter.Value;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private static void NewaddContactCampaignFun(List<ContactCampaignModel> lstcontactCampaignModel)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsCampaignlBulkAddNew;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@ContactsCampaignJson",JsonConvert.SerializeObject(lstcontactCampaignModel))
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
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                new System.Data.SqlClient.SqlParameter("@ContactsFailedCampaignJson",JsonConvert.SerializeObject(lstSendCampaignFailedModel) )
            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void AddToWebHookErorrLog(int TenantId, string Code, string Details, string Title, string JsonString)
        {
            try
            {

                var SP_Name = "[dbo].[AddToWebHookErorrLog]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId),
                    new System.Data.SqlClient.SqlParameter("@Code",Code),
                     new System.Data.SqlClient.SqlParameter("@Details",Details),
                    new System.Data.SqlClient.SqlParameter("@exp",Title),
                    new System.Data.SqlClient.SqlParameter("@JsonString",JsonString)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

            }
            catch (Exception ex)
            {
                // throw ex;
            }
        }
    }
}
