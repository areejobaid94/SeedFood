using InfoSeedAzureFunction.WhatsAppApi;
using InfoSeedAzureFunction.WhatsAppApi.Dto;
using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoSeedAzureFunction
{
    public static class MenuReminderMessage
    {
        //[FunctionName("MenuReminderMessageFunction")]
        //public static void Run([TimerTrigger("0 */5 * * * *", RunOnStartup = false)] TimerInfo myTimer)
        //{
        //    List<MenuReminderMessages> messages = getMenuReminderMessage();
        //    if (messages.Any())
        //    {
        //        Send(messages).Wait();
        //    }
        //}

        public static async Task Send(List<MenuReminderMessages> messages)
        {
            try
            {
                var itemsCollection = new DocumentDBHelper<TenantModel>(CollectionTypes.ItemsCollection);
                var postBody = string.Empty;

                foreach (var message in messages)
                {
                    TimeSpan difference = DateTime.UtcNow - message.CreationDate;
                    int totalMinutes = (int)difference.TotalMinutes;
                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == ContainerItemTypes.Tenant && a.TenantId == message.TenantId);

                    if (tenant.TimeReminder <= totalMinutes)
                    {
                        WhatsAppAppService whatsAppAppService = new WhatsAppAppService();
                        SendToWhatsAppResultModel sendToWhatsAppResultModel = new SendToWhatsAppResultModel();

                        PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel
                        {
                            to = message.PhoneNumber,
                            type = "text",
                            text = new PostWhatsAppMessageModel.Text
                            {
                                body = tenant.MenuReminderMessage
                            }
                        };

                        postBody = whatsAppAppService.PrepareTextMessage(postWhatsAppMessageModel);

                        sendToWhatsAppResultModel = whatsAppAppService.SendMsgToWhatsApp(postBody, tenant.D360Key, tenant.AccessToken).Result;
                        if (sendToWhatsAppResultModel.response.IsSuccessStatusCode)
                        {
                            updateMenuReminderMessage(message.Id);
                            var userId = message.TenantId+"_"+message.PhoneNumber;
                            UpdateCustomerChatAsync(message.PhoneNumber, tenant.MenuReminderMessage, "text", "", message.TenantId, userId);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }


        private static List<MenuReminderMessages> getMenuReminderMessage()
        {
            try
            {
                var SP_Name = Constants.Menu.SP_MenuReminderMessagesGet;
                List<MenuReminderMessages> messages = new List<MenuReminderMessages>();
                var sqlParameters = new List<SqlParameter>();
                messages = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapMenuReminderMessage, Constants.ConnectionString).ToList();

                return messages;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private static void updateMenuReminderMessage(long id)
        {
            try
            {
                var SP_Name = Constants.Menu.SP_MenuReminderMessagesUpdate;
                List<MenuReminderMessages> messages = new List<MenuReminderMessages>();
                var sqlParameters = new List<SqlParameter>
                {
                    new SqlParameter("@Id", id) 
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), Constants.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static MenuReminderMessages MapMenuReminderMessage(IDataReader dataReader)
        {
            MenuReminderMessages booking = new MenuReminderMessages
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                IsActive = SqlDataHelper.GetValue<bool>(dataReader, "IsActive"),
                ContactId = SqlDataHelper.GetValue<int>(dataReader, "ContactId"),
                CreationDate = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationDate"),
                PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber"),
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
            };

            return booking;
        }

        private static string GetPostBody(string message, string phoneNumber)
        {
            string postBody = string.Empty;
            WhatsAppAppService whatsAppApiService = new WhatsAppAppService();

            PostWhatsAppMessageModel postWhatsAppMessageModel = new PostWhatsAppMessageModel
            {
                to = phoneNumber,
                type = "text",
                text = new PostWhatsAppMessageModel.Text
                {
                    body = message
                }
            };
            postBody = whatsAppApiService.PrepareTextMessage(postWhatsAppMessageModel);

            return postBody;
        }

        private static async Task UpdateCustomerChatAsync(string phoneNumber, string msg, string type, string mediaUrl, int TenantId, string UserId)
        {
            string userId = TenantId + "_" + phoneNumber;
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
    }
}
