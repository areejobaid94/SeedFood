using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace InfoSeedAzureFunction
{
    public static class SessionTimeoutFunction
    {



        //[FunctionName("SessionTimeoutFunction")]
        //public static void Run([TimerTrigger("0 5 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
        //    log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        //    SessionTimeOut(log).Wait();
        //}
        public static async Task<string> SessionTimeOut(ILogger log)
        {
            string result = string.Empty;

            var conversationChat = new DocumentDBHelper<ConversationChatModel>(CollectionTypes.ItemsCollection);
            var objConversation = conversationChat.GetItemListAsync(p => p.ItemType == ContainerItemTypes.ConversationBot).Result;

            if (objConversation != null)
            {
                int count = 0;
                foreach (var item in objConversation)
                {
                    try
                    {

                        var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
                        var Customer = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == item.userId).Result;



                        if (Customer != null)
                        {
                            try
                            {

                                if (Customer.customerChat != null)
                                {
                                    TimeSpan timeSpan = DateTime.Now - Customer.customerChat.CreateDate.Value;
                                    int totalMinutes = (int)Math.Ceiling(timeSpan.TotalMinutes);
                                    if (totalMinutes >= 520)
                                    {
                                        var DeleteBotChat = DeleteConversation(item.userId).Result;

                                    }

                                }

                            }
                            catch (Exception ex)
                            {

                                log.LogInformation(ex.Message);

                            }


                        }
                        count++;
                        //log.LogInformation($"Number of deleted conversation: {count}");
                    }
                    catch (Exception ex)
                    {

                        log.LogInformation(ex.Message);

                    }
                }

            }


            return result;
        }

        private static async Task<string> DeleteConversation(string userId)
        {


            string result = string.Empty;

            var conversationChat = new DocumentDBHelper<ConversationChatModel>(CollectionTypes.ItemsCollection);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == ContainerItemTypes.ConversationBot && p.userId == userId).Result;

            if (objConversation != null)
            { // delete contact caht 

                var queryString = "SELECT * FROM c WHERE c.ItemType= 3 and  c.userId= '" + userId + "'";
                await conversationChat.DeleteChatItem(queryString);

            }


            return result;
        }
    }
}
