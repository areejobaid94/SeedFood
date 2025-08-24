using Abp.Web.Models;
using Framework.Data;
using Infoseed.MessagingPortal.InfoSeedParser;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppEnum;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppMediaResult;

namespace Infoseed.MessagingPortal.WhatsApp
{
    public class WhatsAppConversationSessionAppService : MessagingPortalAppServiceBase, IWhatsAppConversationSessionAppService
    {
        private readonly IDocumentClient _IDocumentClient;

        public WhatsAppConversationSessionAppService(IDocumentClient  iDocumentClient)
        {
            _IDocumentClient = iDocumentClient;
        }
        public bool ScheduleValidation(long CampaignId = 0)
        {
            return scheduleValidation(CampaignId);
        }
        public ConversationSessionEntity GetOpenConversationSession(int tenantId)
        {
            return getOpenConversationSession(tenantId);
        }
        public void UpdateActivationCampaign(long messageId)
        {
            updateActivationCampaign(messageId);
        }
        public WhatsAppEntity GetFreeMessage( int pageNumber = 0, int pageSize = 50)
        {
            return getFreeMessage(pageNumber, pageSize);
        }
        public WhatsAppScheduledCampaign GetScheduledCampaignByCampaignId(long messageId)
        {
            return getScheduledCampaignByCampaignId(messageId);
        }
        public WhatsAppFreeMessageModel GetFreeMessageById(long messageId)
        {
            return getFreeMessageById(messageId);
        }
        public long AddFreeMessage(WhatsAppFreeMessageModel message)
        {
            return addFreeMessage(message);
        }
        public bool DeleteFreeMessage(long messageId)
        {
            return deleteFreeMessage(messageId);
        }
        public Task<long> SendFreeMessageToOpenConversation(long messageId, int tenantId)
        {
            return sendMessageToOpenConversationAsync(messageId,tenantId);
        }
        public long ScheduleMessage(WhatsAppScheduledCampaign scheduledCampaign)
        {
            if (scheduleValidation(scheduledCampaign.CampaignId))
            {
                return scheduleMessage(scheduledCampaign);
            }
            else
            {
                return -1;

            }
        }
        
     
        [HttpPost]
        [DontWrapResult]
        public Task<WhatsAppHeaderUrl> GetInfoSeedUrlFile([FromForm] UploadFileModel file)
        {
            return getInfoSeedUrlFile(file);
        }



        #region private Methods

        private WhatsAppEntity getFreeMessage( int pageNumber = 0, int pageSize = 50)
        {
            try
            {
                WhatsAppEntity MessageEntity = new WhatsAppEntity();
                int tenantId = AbpSession.TenantId.Value;
                List<WhatsAppFreeMessageModel> lstWhatsAppMessageConversationModel = new List<WhatsAppFreeMessageModel>();
                var SP_Name = Constants.ConversationSession.SP_WhatsAppFreeMessageGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)
                   ,new System.Data.SqlClient.SqlParameter("@PageNumber",pageNumber)
                   ,new System.Data.SqlClient.SqlParameter("@PageSize",pageSize)

                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                lstWhatsAppMessageConversationModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapWhatsAppFreeMessage, AppSettingsModel.ConnectionStrings).ToList();

                MessageEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);
                MessageEntity.lstWhatsAppFreeMessageModel = lstWhatsAppMessageConversationModel;
                return MessageEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private WhatsAppFreeMessageModel getFreeMessageById(long messageId)
        {
            try
            {
                WhatsAppEntity MessageEntity = new WhatsAppEntity();
                int tenantId = AbpSession.TenantId.Value;
                WhatsAppFreeMessageModel messageConversation = new WhatsAppFreeMessageModel();
                var SP_Name = Constants.ConversationSession.SP_WhatsAppFreeMessageGetById;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@MessageId",messageId)
                   ,new System.Data.SqlClient.SqlParameter("@TenantId",tenantId)


                };

                messageConversation = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapWhatsAppFreeMessage, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                return messageConversation;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private WhatsAppScheduledCampaign getScheduledCampaignByCampaignId(long campaignId)
        {
            try
            {
                WhatsAppScheduledCampaign scheduledCampaign = new WhatsAppScheduledCampaign();
                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppScheduledCampaignByCampaignIdGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                    new System.Data.SqlClient.SqlParameter("@CampaignId",campaignId)

                };

                scheduledCampaign = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapScheduledCampaign, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                return scheduledCampaign;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void updateActivationCampaign(long messageId)
        {
            try
            {

                var SP_Name = Constants.WhatsAppCampaign.SP_WhatsAppCampaignActivationUpdate;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                     new System.Data.SqlClient.SqlParameter("@CampaignId",messageId)
                    ,new System.Data.SqlClient.SqlParameter("@CampaignStatus",WhatsAppCampaignStatusEnum.InActive)
                    ,new System.Data.SqlClient.SqlParameter("@IsActive",false)
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private long addFreeMessage(WhatsAppFreeMessageModel message)
        {
            try
            {
                if (message.FreeMessage != null)
                {
                    var SP_Name = Constants.ConversationSession.SP_WhatsAppFreeMessageAdd;
                    var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                    {
                         new System.Data.SqlClient.SqlParameter("@FreeMessage",message.FreeMessage)
                        ,new System.Data.SqlClient.SqlParameter("@FreeMessageType",message.FreeMessageType)
                        ,new System.Data.SqlClient.SqlParameter("@TenantId",message.TenantId)
                        ,new System.Data.SqlClient.SqlParameter("@CampaignType",(int)WhatsAppCampaignTypeEnum.FreeMessage)
                        ,new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId)
                        ,new System.Data.SqlClient.SqlParameter("@CreatedDate",DateTime.UtcNow)
                    };

                    var OutputParameter = new System.Data.SqlClient.SqlParameter();
                    OutputParameter.SqlDbType = System.Data.SqlDbType.BigInt;
                    OutputParameter.ParameterName = "@MessageId";
                    OutputParameter.Direction = System.Data.ParameterDirection.Output;


                    sqlParameters.Add(OutputParameter);


                    SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                 AppSettingsModel.ConnectionStrings);

                    return (long)OutputParameter.Value;
                }
                else
                {
                    return -1;
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool deleteFreeMessage(long messageId)
        {
            try
            {
                var SP_Name = Constants.ConversationSession.SP_WhatsAppFreeMessageDelete;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@MessageId",messageId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
                return (bool)OutputParameter.Value;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool scheduleValidation(long CampaignId = 0)
        {
            try
            {
                var SP_Name = Constants.ConversationSession.SP_WhatsAppFreeMessageScheduleValidationAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
              
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value)
                   , new System.Data.SqlClient.SqlParameter("@CampaignId",CampaignId)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.Bit;
                OutputParameter.ParameterName = "@Result";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),AppSettingsModel.ConnectionStrings);
                bool result = (bool)OutputParameter.Value;
                return result;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private long scheduleMessage(WhatsAppScheduledCampaign scheduledCampaign)
        {
            try
            {
                //var dateNow = DateTime.Now.AddHours(AppSettingsModel.AddHour);
                //DateTime dateTime = scheduledCampaign.SendTime.ToInfoSeedDateTime();

                var SP_Name = Constants.ConversationSession.SP_WhatsAppScheduledFreeMessageAdd;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CampaignId",scheduledCampaign.CampaignId),
                    new System.Data.SqlClient.SqlParameter("@SendDateTime",scheduledCampaign.SendTime.ToInfoSeedDateTime()),
                    new System.Data.SqlClient.SqlParameter("@IsRecurrence",scheduledCampaign.IsRecurrence),
                    new System.Data.SqlClient.SqlParameter("@IsActive",true),
                    new System.Data.SqlClient.SqlParameter("@TenantId",AbpSession.TenantId.Value),
                    new System.Data.SqlClient.SqlParameter("@CreatedDate", DateTime.UtcNow),
                    new System.Data.SqlClient.SqlParameter("@CreatedByUserId",AbpSession.UserId.Value)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@Id";
                OutputParameter.Direction = ParameterDirection.Output;

                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(),
                AppSettingsModel.ConnectionStrings);
                return (long)OutputParameter.Value;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private async Task<long> sendMessageToOpenConversationAsync(long messageId,int tenantId)
        {
            try
            {
               // long successCount = 0;
                int pageNumber = 0;
                int pageSize = 10;
                ConversationSessionEntity contacts = new ConversationSessionEntity();
                WhatsAppFreeMessageModel message = new WhatsAppFreeMessageModel();
                contacts = getOpenConversationSession(tenantId,pageNumber,pageSize);
                message = getFreeMessageById(messageId);
                message.UserId = AbpSession.UserId.Value.ToString();

                SetSendMessageInQueue(message);
                return 1;
                //foreach (var item in contacts.conversationSessionModel)
                //{
                //    bool result = await PostMessageD360(message, item.PhoneNumber);
                //    if (result)
                //    {
                //        successCount++;
                //    }
                //}
                
                //return successCount;
                
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async void UpdateCustomerChat(string phoneNumber, string msg, string type, string mediaUrl)
        {
            string userId = (int)AbpSession.TenantId.Value + "_" + phoneNumber;
            //   var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection);

            CustomerChat customerChat = new CustomerChat()
            {
                messageId = Guid.NewGuid().ToString(),
                TenantId = (int)AbpSession.TenantId.Value,
                userId = userId,
                text = msg,
                type = type,
                CreateDate = DateTime.Now,
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = mediaUrl,
                UnreadMessagesCount = 0,
                agentName = "admin",
                agentId = AbpSession.UserId.Value.ToString(),
            };


            var itemsCollection2 = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection,_IDocumentClient);
            await itemsCollection2.CreateItemAsync(customerChat);
        }
        private ConversationSessionEntity getOpenConversationSession(int tenantId,int pageNumber =0 , int pageSize = 10)
        {

            try
            {
                ConversationSessionEntity SessionEntity = new ConversationSessionEntity();
                List<ConversationSessionModel> conversations = new List<ConversationSessionModel>();
                var SP_Name = Constants.ConversationSession.SP_ConversationOpenSessionGet;
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> { 
                    new System.Data.SqlClient.SqlParameter("@TenantId", tenantId),
                    new System.Data.SqlClient.SqlParameter("@PageNumber", pageNumber),
                    new System.Data.SqlClient.SqlParameter("@PageSize", pageSize)
                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter();
                OutputParameter.SqlDbType = SqlDbType.BigInt;
                OutputParameter.ParameterName = "@TotalCount";
                OutputParameter.Direction = ParameterDirection.Output;
                sqlParameters.Add(OutputParameter);

                conversations = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapConversationSession, AppSettingsModel.ConnectionStrings).ToList();
                SessionEntity.conversationSessionModel = conversations;
                SessionEntity.TotalCount = Convert.ToInt32(OutputParameter.Value);


                return SessionEntity;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private async Task<WhatsAppHeaderUrl> getInfoSeedUrlFile([FromForm] UploadFileModel model)
        {
            try
            {
                WhatsAppHeaderUrl whatsAppHeaderUrl = new WhatsAppHeaderUrl();
                var url = "";
                if (model.FormFile != null)
                {
                    if (model.FormFile.Length > 0)
                    {
                        var formFile = model.FormFile;
                        long ContentLength = formFile.Length;
                        byte[] fileData = null;
                        using (var ms = new MemoryStream())
                        {
                            formFile.CopyTo(ms);
                            fileData = ms.ToArray();
                        }

                        AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                        AttachmentContent attachmentContent = new AttachmentContent()
                        {
                            Content = fileData,
                            Extension = Path.GetExtension(formFile.FileName),
                            MimeType = formFile.ContentType,
                            fileName=formFile.FileName.Replace(Path.GetExtension(formFile.FileName), "")

                        };
                        url = await azureBlobProvider.Save(attachmentContent);
                        whatsAppHeaderUrl.InfoSeedUrl = url;
                    }
                }
                return whatsAppHeaderUrl;
            }
            catch (Exception)
            {

                throw;
            }
            
        }
        private void SetSendMessageInQueue(WhatsAppFreeMessageModel message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettingsCoreModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("campaign-free-conservation");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception )
            {
                var Error = JsonConvert.SerializeObject(message);
                //this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
            }


        }
        #endregion
    }
}

