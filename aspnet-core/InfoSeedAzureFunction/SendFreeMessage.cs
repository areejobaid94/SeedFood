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
using System.Threading;
using System.Threading.Tasks;

namespace InfoSeedAzureFunction
{
    public static class SendFreeMessage
    {

        //[FunctionName("SendFreeMessageFunction")]
        //public static void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = false)] TimerInfo myTimer)
        //{
        //    Sync().Wait();
        //}
        public static async Task Sync()
        {
            try
            {
                var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);

                List<WhatsAppFreeMessageModel>  message = GetScheduledMessages();
 
                if (message.Any())
                {
                    //Hashtable CampaignHashTable = new Hashtable();
                    //WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel();

                    foreach (var item in message)
                    {
                        int tenantID = item.TenantId.Value;
                        Guid guid = Guid.NewGuid();
                        TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.Tenant && a.TenantId == tenantID);
                        await SendAsync(item, tenant,guid);

                    }
                    //foreach (var item in CampaignHashTable.Values)
                    //{
                    //    foreach (var ScheduledMessage in message)
                    //    {
                    //        if (ScheduledMessage.CampaignId == (long)item)
                    //        {
                    //            if (ScheduledMessage.IsLatest && ScheduledMessage.IsRecurrence)
                    //            {
                    //                whatsAppCampaignModel.id = (long)item;
                    //                whatsAppCampaignModel.status = (int)WhatsAppCampaignStatusEnum.Active;
                    //                whatsAppCampaignModel.SentCampaignId = guid;
                    //            }
                    //            if (ScheduledMessage.IsLatest && !ScheduledMessage.IsRecurrence)
                    //            {
                    //                whatsAppCampaignModel.id = (long)item;
                    //                whatsAppCampaignModel.status = (int)WhatsAppCampaignStatusEnum.Sent;
                    //                whatsAppCampaignModel.SentCampaignId = guid;
                    //            }
                    //            updateWhatsAppCampaign(whatsAppCampaignModel);
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private static async Task SendAsync(WhatsAppFreeMessageModel message, TenantModel tenant, Guid guid)
        {
            try
            {
                int pageNumber = 0;
                int pageSize = 10;
                var postBody = string.Empty;

                DataTable tbl = new DataTable();
                tbl.Columns.Add(new DataColumn("PhoneNumber", typeof(string)));
                tbl.Columns.Add(new DataColumn("TenantId", typeof(int)));
                tbl.Columns.Add(new DataColumn("ResultJson", typeof(string)));
                tbl.Columns.Add(new DataColumn("MessageId", typeof(string)));
                tbl.Columns.Add(new DataColumn("ContactId", typeof(int)));
                tbl.Columns.Add(new DataColumn("FreeMessageId", typeof(long)));
                tbl.Columns.Add(new DataColumn("SentCampaignId", typeof(Guid)));

                ConversationSessionEntity contactsSession = new ConversationSessionEntity();
                List<SendCampaignFailedModel> LstSendCampaignFailedModel = new List<SendCampaignFailedModel>();

                contactsSession = getOpenConversationSession(message.TenantId.Value,message.CampaignId, pageNumber, pageSize);

                foreach (var contact in contactsSession.conversationSessionModel)
                {
                    postBody = GetPostBody(message, contact.PhoneNumber, tenant);
                    tbl = await SendToWhatsApp(message, tenant, tbl, postBody, contact, LstSendCampaignFailedModel,guid);
                }

                if (tbl.Rows.Count > 0)
                {
                    AddContactFreeMessage(tbl);
            
                    //if (!CampaignHashTable.ContainsValue(message.CampaignId))
                    //{
                    //    CampaignHashTable.Add("Id", message.CampaignId);
                    //}
                }
                if (LstSendCampaignFailedModel != null && LstSendCampaignFailedModel.Count > 0)
                {
                    UpdateFailedContact(LstSendCampaignFailedModel);

                }

                if (contactsSession.TotalCount > 0)
                {
                    Thread.Sleep(2000);
                    pageNumber = 0;
                    await SendAsync(message, tenant,guid);
                }
                else
                {
                    if (message.IsLatest && message.IsRecurrence)
                    {
                        WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
                        {
                            id = message.CampaignId,
                            status = (int)WhatsAppCampaignStatusEnum.Active,
                            SentCampaignId = guid
                        };
                        updateWhatsAppCampaign(whatsAppCampaignModel);

                    }
                    if (message.IsLatest && !message.IsRecurrence)
                    {
                        WhatsAppCampaignModel whatsAppCampaignModel = new WhatsAppCampaignModel
                        {
                            id = message.CampaignId,
                            status = (int)WhatsAppCampaignStatusEnum.Sent,
                            SentCampaignId = guid
                        };
                        updateWhatsAppCampaign(whatsAppCampaignModel);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private static List<WhatsAppFreeMessageModel> GetScheduledMessages()
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppScheduledFreeMessageGet;

                var sqlParameters = new List<SqlParameter> {
                    

                };

               List<WhatsAppFreeMessageModel> message = new List<WhatsAppFreeMessageModel>();
                message = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapScheduledFreeMessage, Constants.ConnectionString).ToList();

                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        private static void updateWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel)
        {
            try
            {
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppFreeMessageUpdate;

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
        private static void AddContactFreeMessage(DataTable tbl)
        {
            try
            {
                var SP_Name = Constants.Contacts.SP_ContactsFreeMessageBulkAdd;
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@ContactFreeMessageTable",tbl)
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private static async Task<DataTable> SendToWhatsApp(WhatsAppFreeMessageModel message,TenantModel tenant, DataTable tbl, string postBody, ConversationSessionModel contact, List<SendCampaignFailedModel> lstSendCampaignFailedModel,Guid guid)
        {
            try
            {
                WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
                SendToWhatsAppResultModel sendToWhatsAppResultModel = new SendToWhatsAppResultModel();
                sendToWhatsAppResultModel = whatsAppAppService.SendMsgToWhatsApp(postBody, tenant.D360Key, tenant.AccessToken).Result;

                if (sendToWhatsAppResultModel.response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    WhatsAppMessageTemplateResult templateResult = new WhatsAppMessageTemplateResult();
                    templateResult = JsonConvert.DeserializeObject<WhatsAppMessageTemplateResult>(sendToWhatsAppResultModel.content);

                    DataRow dr = tbl.NewRow();
                    dr["PhoneNumber"] = contact.PhoneNumber;
                    dr["TenantId"] = tenant.TenantId;
                    dr["ResultJson"] = JsonConvert.SerializeObject(sendToWhatsAppResultModel.content);
                    dr["MessageId"] = templateResult.messages.FirstOrDefault().id;
                    dr["ContactId"] = contact.ContactId.Value;//test
                    dr["FreeMessageId"] = message.CampaignId;
                    dr["SentCampaignId"] = guid;

                    tbl.Rows.Add(dr);

                    if (message.FreeMessageType != "text")
                    {
                        await UpdateCustomerChatAsync(contact.PhoneNumber, null, message.FreeMessageType, message.FreeMessage, tenant.TenantId.Value, message.UserId.ToString());
                    }
                    else
                    {
                        await UpdateCustomerChatAsync(contact.PhoneNumber, message.FreeMessage, message.FreeMessageType, null, tenant.TenantId.Value, message.UserId.ToString());
                    }
                }
                else
                {

                    SendCampaignFailedModel sendCampaignFailedModel = new SendCampaignFailedModel();

                    sendCampaignFailedModel.TemplateId = 0;
                    sendCampaignFailedModel.TenantId = tenant.TenantId.Value;
                    sendCampaignFailedModel.PhoneNumber = contact.PhoneNumber;
                    sendCampaignFailedModel.ContactId = contact.ContactId.Value;
                    sendCampaignFailedModel.CampaignId = message.Id.Value;
                    sendCampaignFailedModel.SentCampaignId = guid;

                    if (lstSendCampaignFailedModel == null)
                    {
                        lstSendCampaignFailedModel = new List<SendCampaignFailedModel>();
                    }
                    lstSendCampaignFailedModel.Add(sendCampaignFailedModel);
                }
                return tbl;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            

        }
        
        private static string GetPostBody(WhatsAppFreeMessageModel message, string phoneNumber, TenantModel tenant)
        {
            string postBody = string.Empty;
            WhatsAppAppService whatsAppApiService = new WhatsAppAppService();

            PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel();

            if (!string.IsNullOrEmpty(tenant.AccessToken))
            {
                if (message.FreeMessageType == "text")
                {
                    postWhatsAppMessageModel.to = phoneNumber;
                    postWhatsAppMessageModel.type = "text";
                    postWhatsAppMessageModel.text = new PostWhatsAppMessageModel.Text
                    {
                        body = message.FreeMessage
                    };
                    postBody = whatsAppApiService.PrepareTextMessage(postWhatsAppMessageModel);
                }
                else if (message.FreeMessageType.ToLower() == "image")
                {
                    postWhatsAppMessageModel.image = new PostWhatsAppMessageModel.Image();
                    postWhatsAppMessageModel.to = phoneNumber;
                    postWhatsAppMessageModel.image.link = message.FreeMessage;
                    postBody = whatsAppApiService.PrepareImageMessage(postWhatsAppMessageModel, true);
                }
                else if (message.FreeMessageType.ToLower() == "video")
                {
                    postWhatsAppMessageModel.video = new PostWhatsAppMessageModel.Video();
                    postWhatsAppMessageModel.to = phoneNumber;
                    postWhatsAppMessageModel.video.link = message.FreeMessage;
                    postBody = whatsAppApiService.PrepareVideoMessage(postWhatsAppMessageModel, true);
                }
                else
                {
                    postWhatsAppMessageModel.document = new PostWhatsAppMessageModel.Document();
                    postWhatsAppMessageModel.to = phoneNumber;
                    postWhatsAppMessageModel.document.link = message.FreeMessage;
                    postBody = whatsAppApiService.PrepareDocumentMessage(postWhatsAppMessageModel, true);
                }
            }
            return postBody;  
        }
        private static ConversationSessionEntity getOpenConversationSession(int tenantId,long campaignId,int pageNumber = 0, int pageSize = 10)
        {

            try
            {
                ConversationSessionEntity sessionEntity = new ConversationSessionEntity();
                List<ConversationSessionModel> conversations = new List<ConversationSessionModel>();
                var SP_Name = Constants.ConversationSession.SP_ConversationOpenSessionGet;
                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@TenantId", tenantId),
                    new SqlParameter("@CampaignId", campaignId),
                    new SqlParameter("@PageNumber", pageNumber),
                    new SqlParameter("@PageSize", pageSize)
                };

                var OutputParameter = new SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                conversations = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapConversationSession, Constants.ConnectionString).ToList();
                sessionEntity.conversationSessionModel = conversations;
                sessionEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                return sessionEntity;
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
        public static ConversationSessionModel MapConversationSession(IDataReader dataReader)
        {
            try
            {
                ConversationSessionModel _ConversationSessionModel = new ConversationSessionModel();

                _ConversationSessionModel.PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber");
                _ConversationSessionModel.TenantId = SqlDataHelper.GetValue<string>(dataReader, "TenantId");
                _ConversationSessionModel.ContactId = SqlDataHelper.GetValue<string>(dataReader, "ContactId");

                //_ConversationSessionModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
               // _ConversationSessionModel.ConversationId = SqlDataHelper.GetValue<string>(dataReader, "ConversationId");
                //_ConversationSessionModel.ConversationDateTime = SqlDataHelper.GetValue<string>(dataReader, "ConversationDateTime");
               // _ConversationSessionModel.InitiatedBy = SqlDataHelper.GetValue<string>(dataReader, "InitiatedBy");


                return _ConversationSessionModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static WhatsAppFreeMessageModel MapScheduledFreeMessage(IDataReader dataReader)
        {
            try
            {
                WhatsAppFreeMessageModel _WhatsAppFreeMessageModel = new WhatsAppFreeMessageModel();
                _WhatsAppFreeMessageModel.Id = SqlDataHelper.GetValue<long>(dataReader, "Id");
                _WhatsAppFreeMessageModel.UserId = SqlDataHelper.GetValue<long>(dataReader, "CreatedByUserId");
                _WhatsAppFreeMessageModel.FreeMessage = SqlDataHelper.GetValue<string>(dataReader, "FreeMessage");
                _WhatsAppFreeMessageModel.FreeMessageType = SqlDataHelper.GetValue<string>(dataReader, "FreeMessageType");
                _WhatsAppFreeMessageModel.IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive");
                _WhatsAppFreeMessageModel.IsRecurrence = SqlDataHelper.GetValue<bool>(dataReader, "IsRecurrence");
                _WhatsAppFreeMessageModel.IsLatest = SqlDataHelper.GetValue<bool>(dataReader, "IsLatest");
                _WhatsAppFreeMessageModel.SendDateTime = SqlDataHelper.GetValue<DateTime>(dataReader, "SendDateTime");
                _WhatsAppFreeMessageModel.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                _WhatsAppFreeMessageModel.CampaignId = SqlDataHelper.GetValue<DateTime>(dataReader, "CampaignId");


                return _WhatsAppFreeMessageModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
