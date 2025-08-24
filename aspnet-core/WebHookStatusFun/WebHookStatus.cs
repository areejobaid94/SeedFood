

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebHookStatusFun.Model;
using WebHookStatusFun.Models;

namespace WebHookStatusFun
{


    public class WebHookStatus
    {

        [FunctionName("webhookstatusfuntest")]
        public static async Task WebHookStatusFun([QueueTrigger("webhookstatusfuntest", Connection = "AzureWebJobsStorage")] string message,  ILogger log)
        {
            try
            {
                WebHookModel model = JsonConvert.DeserializeObject<WebHookModel>(message);
                await StatAsync(model);
            }
            catch (System.Exception ex)
            {
                log.LogError(ex, "Failed to process webhook message.");
            }
        }




        private static async Task StatAsync(WebHookModel model)
        {
    
            TenantModel Tenant = new TenantModel();
            WhatsAppModel jsonData = new WhatsAppModel();
            var userId = "";
            jsonData=model.whatsApp;
            Tenant=model.tenant;

            var phoneNumberId = jsonData.Entry[0].Changes[0].Value.Metadata.phone_number_id;

            var jsonResult = JsonConvert.SerializeObject(jsonData, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var MassageId = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id;
    

            if (Tenant.IsBundleActive)
            {
                string PhoneNumber = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id;
                var name = PhoneNumber;
                if (jsonData.Entry[0].Changes[0].Value.Contacts != null)
                {

                    name = jsonData.Entry[0].Changes[0].Value.Contacts[0].Profile.Name;
                }
                userId = (Tenant.TenantId + "_" + PhoneNumber).ToString();
                var status = jsonData.Entry[0].Changes[0].Value.statuses[0].status;


                var itemsCollection3 = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);

                var customerResult3 = itemsCollection3.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
                var Customer3 = await customerResult3;



                if (status == "sent")
                {
                    if (Customer3 == null)
                    {

                        Customer3 = CreateNewCustomer(PhoneNumber, name, "text", Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                        Customer3.CustomerStepModel.UserParmeter.Remove("ContactID");
                        Customer3.CustomerStepModel.UserParmeter.Add("ContactID", Customer3.ContactID.ToString());
                        Customer3.CustomerStepModel.UserParmeter.Remove("TenantId");
                        Customer3.CustomerStepModel.UserParmeter.Add("TenantId", Customer3.TenantId.ToString());
                        Customer3.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                        Customer3.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer3.phoneNumber.ToString());
                        Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                        Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName.ToString());
                        Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                        Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");

                        //Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                        //Customer3.expiration_timestamp = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;

                        Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                        Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;


                        var Result = await itemsCollection3.UpdateItemAsync(Customer3._self, Customer3);

                    }
                    else
                    {
                        Customer3.channel = "Whatsapp";

                        Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                        Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;

                        // Customer3.creation_timestamp = int.Parse(jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                        // Customer3.expiration_timestamp = jsonData.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;
                        // Get current UTC time as Unix timestamp
                        //Customer3.creation_timestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                        //// Add 1 day (86400 seconds) to the creation timestamp
                        //Customer3.expiration_timestamp = Customer3.creation_timestamp + 86400;


                        if (Customer3.IsBlock)
                        {
                            return;
                        }
                        Customer3.CustomerStepModel.UserParmeter.Remove("ContactID");
                        Customer3.CustomerStepModel.UserParmeter.Add("ContactID", Customer3.ContactID.ToString());
                        Customer3.CustomerStepModel.UserParmeter.Remove("TenantId");
                        Customer3.CustomerStepModel.UserParmeter.Add("TenantId", Customer3.TenantId.ToString());
                        Customer3.CustomerStepModel.UserParmeter.Remove("PhoneNumber");
                        Customer3.CustomerStepModel.UserParmeter.Add("PhoneNumber", Customer3.phoneNumber.ToString());

                        try
                        {
                            if (!Customer3.CustomerStepModel.UserParmeter.ContainsKey("Name"))
                            {
                                Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                                Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName);
                            }
                        }
                        catch
                        {
                            Customer3.CustomerStepModel.UserParmeter.Remove("Name");
                            Customer3.CustomerStepModel.UserParmeter.Add("Name", Customer3.displayName);
                        }


                        try
                        {
                            if (!Customer3.CustomerStepModel.UserParmeter.ContainsKey("Location"))
                            {
                                Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                                Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                            }
                        }
                        catch
                        {
                            Customer3.CustomerStepModel.UserParmeter.Remove("Location");
                            Customer3.CustomerStepModel.UserParmeter.Add("Location", "No Location");
                        }

                        if (string.IsNullOrEmpty(Customer3.ContactID))
                        {

                            var getCustomer3 = GetCustomerfromDB(PhoneNumber, "", "text", Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                            Customer3.ContactID = getCustomer3.Id.ToString();


                        }


                        var Result = await itemsCollection3.UpdateItemAsync(Customer3._self, Customer3);

                    }


                }


                try
                {

                    if (jsonData.Entry[0].Changes[0].Value.statuses[0].status == "read" || jsonData.Entry[0].Changes[0].Value.statuses[0].status == "failed")
                    {

                        WebHookModel webHookModel = new WebHookModel();
                        webHookModel.tenant = Tenant;
                        webHookModel.whatsApp = jsonData;
                        webHookModel.customer = Customer3;
                        // SetStatusInQueuestg(webHookModel);
                        await TestUpdateMongoDBAsync(webHookModel, itemsCollection3);
                    }
                    else
                    {
                        if (jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToLower() == "marketing" || jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToLower() == "utility" || jsonData.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToUpper() == "AUTHENTICATION")
                        {
                            WebHookModel webHookModel = new WebHookModel();
                            webHookModel.tenant = Tenant;
                            webHookModel.whatsApp = jsonData;
                            webHookModel.customer = Customer3;
                            //SetStatusInQueuestg(webHookModel);
                            await TestUpdateMongoDBAsync(webHookModel, itemsCollection3);


                        }
                    }



                }
                catch
                {

                }


            }


            
        }



        private static async Task<string> TestUpdateMongoDBAsync(WebHookModel model, DocumentDBHelper<CustomerModel> documentCosmoseDB)
        {
            try
            {


                string PhoneNumber = model.whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id;
                var status = model.whatsApp.Entry[0].Changes[0].Value.statuses[0].status;
                var jsonResult = JsonConvert.SerializeObject(model.whatsApp, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                var MassageId = model.whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id;
                var TenantId = model.tenant.TenantId;
                var StatusCode = (status == "failed" ? 400 : 200);
                var FaildDetails = "";
                var DetailsJosn = jsonResult;



 
                // "urlSendCampaignProject": "https://startcampingstgnew.azurewebsites.net/api/startCampaign"



                // Connection string from Azure Cosmos DB for MongoDB (vCore)
                string connectionString = Constant.connectionStringMongoDB;

                // MongoDB database and collection details
                string databaseName = "test";
                string collectionName = "campaignmessages";

                // Initialize the MongoDB client
                var client = new MongoClient(connectionString);

                // Get the database
                var database = client.GetDatabase(databaseName);

                // Get the collection
                var collection = database.GetCollection<CampaginMDRez>(collectionName);



                var filter = Builders<CampaginMDRez>.Filter.Eq(x => x.messageId, MassageId);
                var filterresult = await collection.Find(filter).FirstOrDefaultAsync();


                if (filterresult == null)//Insert
                {
                    // 1. Add a document
                    var newDocument = new CampaginMDRez
                    {
                        tenantId = TenantId.Value,
                        campaignId = "1",
                        phoneNumber = PhoneNumber,
                        messageId = MassageId,
                        status = status,
                        statusCode = StatusCode,
                        failedDetails = "",
                        is_accepted = false,
                        is_delivered = (status == "delivered" ? true : false),
                        is_read = (status == "read" ? true : false),
                        is_sent = (status == "sent" ? true : false),
                        delivered_detailsJson = (status == "delivered" ? DetailsJosn : ""),
                        read_detailsJson = (status == "read" ? DetailsJosn : ""),
                        sent_detailsJson = (status == "sent" ? DetailsJosn : ""),
                    };


                    await collection.InsertOneAsync(newDocument);

                }
                else//update
                {

                    // CampaginMDRez model = JsonConvert.DeserializeObject<CampaginMDRez>(filterresult);

                    if (status == "sent")
                    {

                        var update = Builders<CampaginMDRez>.Update
                            .Set(x => x.status, status)
                            .Set(x => x.is_sent, true)
                            .Set(x => x.sent_detailsJson, jsonResult)
                            .Set(x => x.updatedAt, DateTime.UtcNow);

                        var result = await collection.UpdateOneAsync(filter, update);







                        try
                        {
                            var camp = GetCampaignFun(long.Parse(filterresult.campaignId)).FirstOrDefault();


                            model.customer.templateId = camp.templateId.ToString();
                            model.customer.CampaignId = filterresult.campaignId;
                            model.customer.IsTemplateFlow = true;
                            model.customer.TemplateFlowDate = DateTime.UtcNow;
                            model.customer.getBotFlowForViewDto = new GetBotFlowForViewDto();

                            var Result = await documentCosmoseDB.UpdateItemAsync(model.customer._self, model.customer);



                            string type = "";
                            string mediaUrl = "";
                            string msg = prepareMessageTemplateText(camp.model, out type, out mediaUrl);

                            // show campaign on teamInbox
                            var itemsCollectionCh = new DocumentDBHelper<CustomerChat>(CollectionTypes.ItemsCollection); 
                            CustomerChat customerChat = new CustomerChat()
                            {
                                messageId = MassageId,
                                TenantId = TenantId.Value,
                                userId = model.customer.userId,
                                text = msg,
                                type = "text",
                                CreateDate = DateTime.Now,
                                status = (int)Messagestatus.New,
                                sender = MessageSenderType.TeamInbox,
                                ItemType = InfoSeedContainerItemTypes.ConversationItem,
                                // mediaUrl = campaignCosmo.mediaUrl,
                                UnreadMessagesCount = 0,
                                agentName = "admin",
                                agentId = "",
                            };

                            var Resultchat = await itemsCollectionCh.CreateItemAsync(customerChat);
                        }
                        catch
                        {


                        }





                    }
                    else if (status == "delivered")
                    {


                        var update = Builders<CampaginMDRez>.Update
                            .Set(x => x.status, status)
                            .Set(x => x.is_delivered, true)
                            .Set(x => x.is_sent, true)
                            .Set(x => x.delivered_detailsJson, jsonResult)
                            //   .Set(x => x.sent_detailsJson, jsonResult)
                            .Set(x => x.updatedAt, DateTime.UtcNow);

                        var result = await collection.UpdateOneAsync(filter, update);

                    }
                    else if (status == "read")
                    {


                        var update = Builders<CampaginMDRez>.Update
                            .Set(xx => xx.status, status)
                            .Set(xx => xx.is_read, true)
                            .Set(xx => xx.is_delivered, true)
                            .Set(xx => xx.is_sent, true)
                            .Set(xx => xx.read_detailsJson, jsonResult)
                            // .Set(x => x.delivered_detailsJson, jsonResult)
                            //.Set(x => x.sent_detailsJson, jsonResult)
                            .Set(xx => xx.updatedAt, DateTime.UtcNow);

                        var result = await collection.UpdateOneAsync(filter, update);
                        var x = UpdateCustomerChatStatusNew(model.whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id, TenantId.Value);
                    }
                    else
                    {

                        if (model.whatsApp.Entry[0].Changes[0].Value.statuses[0].Errors.Count > 0)
                        {
                            foreach (var error in model.whatsApp.Entry[0].Changes[0].Value.statuses[0].Errors)
                            {
                                var update = Builders<CampaginMDRez>.Update
                                    .Set(x => x.statusCode, error.code)
                                    .Set(x => x.failedDetails, error.error_data.details.ToString())
                                    .Set(x => x.updatedAt, DateTime.UtcNow);

                                var result = await collection.UpdateOneAsync(filter, update);


                            }
                        }







                    }


                }




            }
            catch (Exception ex)
            {

            }

            return "";
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

                            if (type == "document")
                            {


                                if (!string.IsNullOrEmpty(objWhatsAppTemplateModel.mediaLink))
                                {
                                    if (objWhatsAppTemplateModel.mediaLink.Contains(","))
                                    {

                                        var media = objWhatsAppTemplateModel.mediaLink.Split(',')[1];
                                        try
                                        {
                                            type = "application";
                                            result += media + "\n\r";

                                            mediaUrl = media;
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
                                            result += objWhatsAppTemplateModel.mediaLink + "\n\r";

                                            mediaUrl = objWhatsAppTemplateModel.mediaLink;
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
                                        result += item.example.header_handle[0] + "\n\r";

                                        mediaUrl = item.example.header_handle[0];
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
        private static List<CampaginModel> GetCampaignFun(long CampaignId)
        {
            try
            {
                var SP_Name = "GetSendCampaignNowById";
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CampaignId", CampaignId)
                };
                List<CampaginModel> model = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledCampaign, Constant.ConnectionString).ToList();
                return model;
            }
            catch
            {
                return new List<CampaginModel>();
            }
        }
        private static CampaginModel MapScheduledCampaign(IDataReader dataReader)
        {
            try
            {
                //TenantId, CampaignId, TemplateId, ContactsJson, CreatedDate, UserId, IsExternalContact, JopName, CampaignName, TemplateName, IsSent

                CampaginModel model = new CampaginModel
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
                    IsSent = SqlDataHelper.GetValue<bool>(dataReader, "IsSent"),

                };

                try
                {

                    model.model = System.Text.Json.JsonSerializer.Deserialize<MessageTemplateModel>(SqlDataHelper.GetValue<string>(dataReader, "TemplateJson"));
                }
                catch
                {


                }
                try
                {

                    model.templateVariablles = System.Text.Json.JsonSerializer.Deserialize<TemplateVariablles>(SqlDataHelper.GetValue<string>(dataReader, "TemplateVariables"));
                }
                catch
                {
                    model.templateVariablles = new TemplateVariablles();

                }

                // Deserialize ContactsJson to List<ListContactToCampin>
                model.contacts = System.Text.Json.JsonSerializer.Deserialize<List<ListContactToCampin>>(SqlDataHelper.GetValue<string>(dataReader, "ContactsJson"));

                return model;
            }
            catch
            {
                return new CampaginModel();
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

        private static void UpdateContactConversationId(string ConversationId, string userId, int creation_timestamp, int expiration_timestamp)
        {

            var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);

            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);//&& a.TenantId== TenantId


            if (customerResult.IsCompleted)
            {
                var customer = customerResult.Result;
                if (customer == null)
                {

                }
                else
                {

                    if (customer.ConversationId != ConversationId)
                    {
                        customer.ConversationId = ConversationId;
                        customer.LastConversationStartDateTime = DateTime.Now;//.AddHours(AppSettingsModel.AddHour);

                        customer.expiration_timestamp=expiration_timestamp;
                        customer.creation_timestamp=creation_timestamp;

                        if (customer.creation_timestamp==0 || customer.expiration_timestamp==0)
                        {
                            var model = getConversationSessions(customer.TenantId.Value, customer.phoneNumber, ConversationId);

                            if (model != null && model.expiration_timestamp!=0 &&model.creation_timestamp!=0)
                            {
                                customer.expiration_timestamp=model.expiration_timestamp;
                                customer.creation_timestamp=model.creation_timestamp;
                            }

                        }




                    }
                    else
                    {
                        customer.expiration_timestamp=expiration_timestamp;
                        customer.creation_timestamp=creation_timestamp;


                    }



                    // customer.LastConversationStartDateTime = ConversationId;
                    var Result = itemsCollection.UpdateItemAsync(customer._self, customer).Result;
                }
            }
        }
        private static ConversationSessionsModel getConversationSessions(int TenantId, string PhoneNumber, string ConversationID)
        {
            try
            {
                ConversationSessionsModel ConversationSessionsEntity = new ConversationSessionsModel();
                var SP_Name = "GetConversationSessions";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@ConversationID",ConversationID)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@PhoneNumber",PhoneNumber)
                };

                ConversationSessionsEntity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapConversationSessions, Constants.ConnectionString).FirstOrDefault();


                return ConversationSessionsEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static ConversationSessionsModel MapConversationSessions(IDataReader dataReader)
        {
            try
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = SqlDataHelper.GetValue<int>(dataReader, "creation_timestamp");
                model.expiration_timestamp = SqlDataHelper.GetValue<int>(dataReader, "expiration_timestamp");


                return model;
            }
            catch (Exception)
            {
                ConversationSessionsModel model = new ConversationSessionsModel();
                model.creation_timestamp = 0;
                model.expiration_timestamp = 0;

                return model;
            }
        }

        public static CustomerChat UpdateCustomerChatStatusNew(string messageId, int TenantId)
        {
            try
            {

                var itemsCollection = new DocumentDBHelper<CustomerChat>(CollectionTypes.ItemsCollection);
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.messageId == messageId && a.TenantId == TenantId);
                var Customercaht = customerResult.Result;

                if (Customercaht != null)
                {

                    Customercaht.status = 2;
                    var objCustomer = itemsCollection.UpdateItemAsync(Customercaht._self, Customercaht).Result;
                }

                return Customercaht;


            }
            catch (Exception ex)
            {
                return new CustomerChat();
            }
        }



        public static CustomerModel CreateNewCustomer(string from, string name, string type, string botID, int TenantId, string D360Key)
        {
            //int count = 1;

            string userId = TenantId + "_" + from;
            string displayName = name;
            string phoneNumber = from;



            var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection); 


            var cont = new ContactDto()
            {
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                UserId = userId,
                DisplayName = displayName,
                AvatarUrl = "avatar3",
                CreatorUserId = 1,
                Description = "",
                EmailAddress = "",
                IsDeleted = false,
                CreationTime = DateTime.Now,
                IsLockedByAgent = false,
                IsConversationExpired = false,
                IsBlock = false,
                IsOpen = false,
                LockedByAgentName = "",
                PhoneNumber = phoneNumber,
                SunshineAppID = "",// model.app._id,
                                   //ConversationId = model.statuses.FirstOrDefault().conversation.id,
                Website = "",
                TenantId = TenantId,
                DeletionTime = null,
                DeleterUserId = null,
                ConversationsCount = 1,
                // ConversationId = model.statuses.FirstOrDefault().conversation.id
                channel="Whatsapp"

            };

            //var idcont = InsertContact(cont);
            var idcont = createContact(cont);
            if (string.IsNullOrEmpty(idcont.Group))
            {
                idcont.Group="0";
                idcont.GroupName="";
            }
            var UserParmeter = new Dictionary<string, string>();
            UserParmeter.Add("Name", displayName);
            UserParmeter.Add("PhoneNumber", phoneNumber);
            UserParmeter.Add("Location", "No Location");
            UserParmeter.Add("ContactID", idcont.Id.ToString());
            UserParmeter.Add("TenantId", TenantId.ToString());

            var CustomerModel = new CustomerModel()
            {
                TennantPhoneNumberId=D360Key,
                ConversationsCount = 0,
                ContactID = idcont.Id.ToString(),
                IsComplaint = false,
                userId = userId,
                displayName = displayName,
                avatarUrl = "avatar3",
                type = type,
                D360Key = D360Key,
                CreateDate = DateTime.Now,
                IsLockedByAgent = false,
                LockedByAgentName = botID,
                IsOpen = false,
                agentId = 100000,
                IsBlock = false,
                IsConversationExpired = false,
                CustomerChatStatusID = (int)CustomerChatStatus.Active,
                CustomerStatusID = (int)CustomerStatus.Active,
                LastMessageData = DateTime.Now,
                IsNew = true,
                TenantId = TenantId,
                phoneNumber = phoneNumber,
                UnreadMessagesCount = 1,
                IsNewContact = true,
                IsBotChat = true,
                IsBotCloseChat = false,
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                customerChat = new CustomerChat()
                {
                    CreateDate = DateTime.Now,
                },
                creation_timestamp= 0,
                expiration_timestamp= 0,
                // ConversationId = model.statuses.FirstOrDefault().conversation.id
                CustomerStepModel=new CustomerStepModel() { ChatStepId=-1, ChatStepPervoiusId=0, IsLiveChat=false, UserParmeter=UserParmeter },
                OneTimeQuestionIds=new Dictionary<string, string>(),

                GroupId=long.Parse(idcont.Group),
                GroupName=idcont.GroupName,
                IsHumanhandover=false,
                channel="whatsapp"

            };



            var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;
            //CustomerModel._self = Result.Uri;
            //CustomerModel._rid = Result.ID;





            var itemsCollection2 = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);

            var customerResult = itemsCollection2.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == TenantId);

            CustomerModel = customerResult.Result;



            return CustomerModel;







        }


        private static ContactDto createContact(ContactDto contactDto)
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
                contactDto   = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                 MapContact, Constants.ConnectionString).FirstOrDefault();

                return contactDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static ContactDto MapContact(IDataReader dataReader)
        {
            ContactDto contactDto = new ContactDto();
            contactDto.Id = SqlDataHelper.GetValue<int>(dataReader, "Id");
            contactDto.UserId = SqlDataHelper.GetValue<string>(dataReader, "UserId");
            contactDto.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
            contactDto.Description = SqlDataHelper.GetValue<string>(dataReader, "Description");
            contactDto.EmailAddress = SqlDataHelper.GetValue<string>(dataReader, "EmailAddress");
            contactDto.Website = SqlDataHelper.GetValue<string>(dataReader, "Website");
            contactDto.DisplayName = SqlDataHelper.GetValue<string>(dataReader, "DisplayName");
            contactDto.ContactDisplayName = SqlDataHelper.GetValue<string>(dataReader, "ContactDisplayName");
            contactDto.TakeAwayOrder = SqlDataHelper.GetValue<int>(dataReader, "TakeAwayOrder");
            contactDto.DeliveryOrder = SqlDataHelper.GetValue<int>(dataReader, "DeliveryOrder");
            contactDto.loyalityPoint = SqlDataHelper.GetValue<int>(dataReader, "loyalityPoint");
            contactDto.StreetName = SqlDataHelper.GetValue<string>(dataReader, "StreetName");
            contactDto.BuildingNumber = SqlDataHelper.GetValue<string>(dataReader, "BuildingNumber");
            contactDto.FloorNo = SqlDataHelper.GetValue<string>(dataReader, "FloorNo");
            contactDto.ApartmentNumber = SqlDataHelper.GetValue<string>(dataReader, "ApartmentNumber");
            contactDto.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
            contactDto.IsBlock = SqlDataHelper.GetValue<bool>(dataReader, "IsBlock");

            contactDto.CustomerOPT = SqlDataHelper.GetValue<int>(dataReader, "CustomerOPT");
            contactDto.Branch = SqlDataHelper.GetValue<string>(dataReader, "Branch");
            contactDto.TotalLiveChat = SqlDataHelper.GetValue<int>(dataReader, "TotalLiveChat");
            contactDto.TotalOrder = SqlDataHelper.GetValue<int>(dataReader, "TotalOrders");
            contactDto.TotalRequest = SqlDataHelper.GetValue<int>(dataReader, "TotalRequest");
            contactDto.OrderType = SqlDataHelper.GetValue<int>(dataReader, "OrderType");
            contactDto.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");

            try
            {
                contactDto.Group = SqlDataHelper.GetValue<string>(dataReader, "Group");
                contactDto.GroupName = SqlDataHelper.GetValue<string>(dataReader, "GroupName");
            }
            catch
            {
                contactDto.Group = "0";
                contactDto.GroupName = "";

            }

            return contactDto;
        }

        public static ContactDto GetCustomerfromDB(string from, string name, string type, string botID, int TenantId, string D360Key)
        {


            string userId = TenantId + "_" + from;
            string displayName = name;
            string phoneNumber = from;


            var cont = new ContactDto()
            {
                loyalityPoint = 0,
                TotalOrder = 0,
                TakeAwayOrder = 0,
                DeliveryOrder = 0,
                UserId = userId,
                DisplayName = displayName,
                AvatarUrl = "",
                CreatorUserId = 1,
                Description = "",
                EmailAddress = "",
                IsDeleted = false,
                CreationTime = DateTime.Now,
                IsLockedByAgent = false,
                IsConversationExpired = false,
                IsBlock = false,
                IsOpen = false,
                LockedByAgentName = "",
                PhoneNumber = phoneNumber,
                SunshineAppID = "",// model.app._id,
                                   //ConversationId = model.statuses.FirstOrDefault().conversation.id,
                Website = "",
                TenantId = TenantId,
                DeletionTime = null,
                DeleterUserId = null,
                ConversationsCount = 1,
                // ConversationId = model.statuses.FirstOrDefault().conversation.id


            };
            var idcont = createContact(cont);


            return idcont;







        }
        private static void addToCampaignManagers(List<CampaignManager> campaignManagers)
        {
            try
            {

                string jsonChunk = JsonConvert.SerializeObject(campaignManagers);

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                           {
                               new System.Data.SqlClient.SqlParameter("@CampaignManagersJson", SqlDbType.NVarChar, -1)
                               {
                                   Value = jsonChunk
                               },

                           };

                SqlDataHelper.ExecuteNoneQuery("AddCampaignManagerBulk", sqlParameters.ToArray(), Constants.ConnectionString);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static void updateTicket(int TenantId, string phonenumber, int create, int exp, bool isConvertaionExp)
        {
            try
            {

                var SP_Name = "[dbo].[UpdateAllTiketByContact]";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId),
                    new System.Data.SqlClient.SqlParameter("@phonenumber",phonenumber),
                     new System.Data.SqlClient.SqlParameter("@create",create),
                    new System.Data.SqlClient.SqlParameter("@exp",exp),
                     new System.Data.SqlClient.SqlParameter("@isConvertaionExp",isConvertaionExp)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static CampaignInfoModel GetCampaignName(long CampaignId)
        {
            try
            {
                var SP_Name = "[dbo].[GetCampaignSomInfo]";
                CampaignInfoModel campaignInfoModel = new CampaignInfoModel();

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CampaignId",CampaignId)
                };

                campaignInfoModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapCampignInfo, Constants.ConnectionString).FirstOrDefault();

                return campaignInfoModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static CampaignInfoModel MapCampignInfo(IDataReader dataReader)
        {
            try
            {
                CampaignInfoModel campaignInfoModel = new CampaignInfoModel();

                campaignInfoModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                campaignInfoModel.campaignName = SqlDataHelper.GetValue<string>(dataReader, "Title");
                campaignInfoModel.TemplateName = SqlDataHelper.GetValue<string>(dataReader, "TemplateName");
                campaignInfoModel.UserName = SqlDataHelper.GetValue<string>(dataReader, "UserName");
                campaignInfoModel.Category = SqlDataHelper.GetValue<string>(dataReader, "Category");


                return campaignInfoModel;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void SetConversationMeasurmentsInQueue(ConservationMeasurementMessage message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Constant.AzureStorageAccount);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("conservation-measurements");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception e)
            {

                var Error = JsonConvert.SerializeObject(message);
                // this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
                //
            }


        }

    }
}
