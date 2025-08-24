

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NewFunctionApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NewFunctionApp
{


    public class WebHookStatus
    {
        [FunctionName("webhookstatusfun")]
        public static async Task WebHookStatusFun([QueueTrigger("webhookstatusfun", Connection = "AzureWebJobsStorage")] string message, ILogger log)
        {

            var itemsCollection38 = new DocumentDBHelper<CustomerModel>(CollectionTypes.CustomersCollection);
            var customerResult38888 = await itemsCollection38.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == "27_962779746365" && a.TenantId == 27);




            TenantModel Tenant = new TenantModel();
            WhatsAppModel whatsApp = new WhatsAppModel();
            var userId = "";
           

            WebHookModel model = JsonConvert.DeserializeObject<WebHookModel>(message);
            Tenant=model.tenant;
            whatsApp=model.whatsApp;

            string PhoneNumber = whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().recipient_id;
            var phoneNumberId = whatsApp.Entry[0].Changes[0].Value.Metadata.phone_number_id;
            userId = (Tenant.TenantId + "_" + PhoneNumber).ToString();
            var name = PhoneNumber;
            if (whatsApp.Entry[0].Changes[0].Value.Contacts!=null)
            {

                name = whatsApp.Entry[0].Changes[0].Value.Contacts[0].Profile.Name;
            }
            long creation_timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long expiration_timestamp = DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds();
            bool IsChangeOncampaignCosmo = false;

            try
            {
                if (whatsApp.Entry[0].Changes[0].Value.statuses != null)
                {

                    var from2 = whatsApp.Entry[0].Changes[0].Value.statuses[0].recipient_id; // extract the phone number from the webhook payload

                    try
                    {
                        if (whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower().Equals("failed") || whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower().Equals("deleted") || whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower().Equals("warning"))
                        {
                            try
                            {
                                if (whatsApp.Entry[0].Changes[0].Value.statuses[0].Errors.Count>0)
                                {
                                    foreach (var error in whatsApp.Entry[0].Changes[0].Value.statuses[0].Errors)
                                    {

                                        AddToWebHookErorrLog(Tenant.TenantId.Value, error.code.ToString(), error.error_data.details.ToString(), from2, message);
                                        //Console.WriteLine($"Error Code: {error.Code}");
                                        //Console.WriteLine($"Error Title: {error.Title}");
                                        //Console.WriteLine($"Error Details: {error.Details}");
                                    }
                                }
                                else
                                {

                                    AddToWebHookErorrLog(Tenant.TenantId.Value, whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower(), whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower(), whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower(), message);
                                }
                            }
                            catch
                            {
                                AddToWebHookErorrLog(Tenant.TenantId.Value, whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower(), whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower(), whatsApp.Entry[0].Changes[0].Value.statuses[0].status.ToLower(), message);

                            }



                        }
                    }
                    catch
                    {

                    }


                    if (Tenant.IsBundleActive)
                    {


                        if (whatsApp.Entry[0].Changes[0].Value.statuses[0].status.Equals("sent"))
                        {
                            creation_timestamp = int.Parse(whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().timestamp);
                            expiration_timestamp = whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.expiration_timestamp;
                           // UpdateContactConversationId(whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().conversation.id, userId, int.Parse(creation_timestamp.ToString()), int.Parse(expiration_timestamp.ToString()));

                        }
                        else if (whatsApp.Entry[0].Changes[0].Value.statuses[0].status.Equals("read"))
                        {

                            var x = UpdateCustomerChatStatusNew(whatsApp.Entry[0].Changes[0].Value.statuses.FirstOrDefault().id, Tenant.TenantId.Value);

                        }

                        var campaignCosmoDBModel = new DocumentDBHelper<CampaignCosmoDBModel>(CollectionTypes.ItemsCollection);

                        var campaignCosmoResult = campaignCosmoDBModel.GetItemAsync(a => a.itemType == 5 && a.messagesId == whatsApp.Entry[0].Changes[0].Value.statuses[0].id  && a.tenantId == Tenant.TenantId);
                        var campaignCosmo = campaignCosmoResult.Result;

                        var itemsCollection3 = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
                        var customerResult3 = itemsCollection3.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId && a.TenantId == Tenant.TenantId);
                        var Customer3 = customerResult3.Result;

                        if (Customer3 == null)
                        {

                            if (campaignCosmo!=null)
                            {
                                name=campaignCosmo.contactName;

                            }
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

                            if (campaignCosmo!=null)
                            {
                                Customer3.displayName=campaignCosmo.contactName;
                                Customer3.templateId=campaignCosmo.templateId;
                                Customer3.CampaignId = campaignCosmo.campaignId;
                                Customer3.IsTemplateFlow = true;
                                Customer3.TemplateFlowDate=DateTime.UtcNow;
                                Customer3.getBotFlowForViewDto=new GetBotFlowForViewDto();
                                Customer3.CustomerStepModel=new CustomerStepModel() { ChatStepId=-1, ChatStepPervoiusId=0 };

                                Customer3.creation_timestamp=int.Parse(creation_timestamp.ToString());
                                Customer3.expiration_timestamp=int.Parse(expiration_timestamp.ToString());
                                Customer3.IsConversationExpired=false;


                            }


                            var Result = itemsCollection3.UpdateItemAsync(Customer3._self, Customer3).Result;

                        }
                        else
                        {
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




                            var getCustomer3 = GetCustomerfromDB(PhoneNumber, "", "text", Tenant.botId, Tenant.TenantId.Value, phoneNumberId);
                            Customer3.ContactID=getCustomer3.Id.ToString();

                            if (campaignCosmo!=null)
                            {

                                Customer3.templateId=campaignCosmo.templateId;
                                Customer3.CampaignId = campaignCosmo.campaignId;
                                Customer3.IsTemplateFlow = true;
                                Customer3.TemplateFlowDate=DateTime.UtcNow;
                                Customer3.getBotFlowForViewDto=new GetBotFlowForViewDto();

                                Customer3.creation_timestamp=int.Parse(creation_timestamp.ToString());
                                Customer3.expiration_timestamp=int.Parse(expiration_timestamp.ToString());
                                Customer3.IsConversationExpired=false;
                                updateTicket(Tenant.TenantId.Value, PhoneNumber, Customer3.creation_timestamp, Customer3.expiration_timestamp, false);
                            }

                            var Result = itemsCollection3.UpdateItemAsync(Customer3._self, Customer3).Result;

                        }


                        CampaignInfoModel campaignInfoModel = new CampaignInfoModel();
                        try
                        {
                            if (string.IsNullOrEmpty(Customer3.CampaignId))
                            {
                                Customer3.CampaignId = "";
                            }
                            else
                            {
                                campaignInfoModel = GetCampaignName(long.Parse(Customer3.CampaignId));
                            }

                        }
                        catch
                        {
                            Customer3.CampaignId = "";
                        }


                     
                        if (whatsApp.Entry[0].Changes[0].Value.statuses[0].status.Equals("read"))
                        {
                      
                            try
                            {

                                if (campaignCosmo!=null)
                                {
                                    if (campaignCosmo.campaignId != null && campaignCosmo.campaignId != "")
                                    {
                                        campaignCosmo.isRead = true;
                                        IsChangeOncampaignCosmo = true;
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                               

                            }

                        }
                        else if (whatsApp.Entry[0].Changes[0].Value.statuses[0].status.Equals("delivered"))
                        {
                            if (whatsApp.Entry[0].Changes[0].Value.statuses[0].pricing != null)
                            {
                                if (campaignCosmo!=null)
                                {
                                    if (campaignCosmo.campaignId != null && campaignCosmo.campaignId != "")
                                    {
                                        if (whatsApp.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToLower() == "marketing" || whatsApp.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToLower() == "utility"|| whatsApp.Entry[0].Changes[0].Value.statuses[0].pricing.category.ToUpper() == "AUTHENTICATION")
                                        {
                                            campaignCosmo.isDelivered = true;
                                            IsChangeOncampaignCosmo = true;

                                            var itemsCollectionCh = new DocumentDBHelper<CustomerChat>(CollectionTypes.ItemsCollection);

                                       
                                            CustomerChat customerChat = new CustomerChat()
                                            {
                                                messageId = whatsApp.Entry[0].Changes[0].Value.statuses[0].id,
                                                TenantId = Tenant.TenantId,
                                                userId = userId,
                                                text = campaignCosmo.msg,
                                                type = campaignCosmo.type,
                                                CreateDate = DateTime.Now,
                                                status = (int)Messagestatus.New,
                                                sender = MessageSenderType.TeamInbox,
                                                ItemType= ContainerItemTypes.ConversationItem,
                                                mediaUrl = campaignCosmo.mediaUrl,
                                                UnreadMessagesCount = 0,
                                                agentName = "admin",
                                                agentId = "",
                                            };

                                            var Resultchat = itemsCollectionCh.CreateItemAsync(customerChat).Result;


                                        }
                                    }

                                }

                            }
                        }
                        else if (whatsApp.Entry[0].Changes[0].Value.statuses[0].status.Equals("failed"))
                        {
                            if (campaignCosmo!=null)
                            {
                                if (campaignInfoModel.Category != null && campaignCosmo.campaignId != null && campaignCosmo.campaignId != "")
                                {
                                    if (campaignInfoModel.Category.ToUpper() == "MARKETING" || campaignInfoModel.Category.ToUpper() == "UTILITY"|| campaignInfoModel.Category.ToUpper() == "AUTHENTICATION")
                                    {
                                        campaignCosmo.isFailed = true;
                                        IsChangeOncampaignCosmo = true;
                                    }
                                }

                            }

                        }                      
                        else
                        {

                            if (campaignCosmo!=null)
                            {
                                if (campaignInfoModel.Category != null && campaignCosmo.campaignId != null && campaignCosmo.campaignId != "")
                                {
                                    if (campaignInfoModel.Category.ToUpper() == "MARKETING" || campaignInfoModel.Category.ToUpper() == "UTILITY"|| campaignInfoModel.Category.ToUpper() == "AUTHENTICATION")
                                    {
                                        campaignCosmo.isReplied = true;
                                        IsChangeOncampaignCosmo = true;
                                    }
                                }

                            }

                        }
                        if (IsChangeOncampaignCosmo && campaignCosmo!=null)
                        {
                            campaignCosmo.contactId = Customer3.ContactID;
                            var Result = campaignCosmoDBModel.UpdateItemAsync(campaignCosmo._self, campaignCosmo).Result;
                        }
                    }
                    return;
                }

            }
            catch(Exception ex)
            {

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
        public  static ConversationSessionsModel MapConversationSessions(IDataReader dataReader)
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
                var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.ConversationItem && a.messageId == messageId && a.TenantId == TenantId);
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
                avatarUrl = "",
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

            };



            var Result = itemsCollection.CreateItemAsync(CustomerModel).Result;



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

    }
}
