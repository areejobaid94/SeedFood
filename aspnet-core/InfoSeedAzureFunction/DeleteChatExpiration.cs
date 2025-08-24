using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace InfoSeedAzureFunction
{
    public class DeleteChatExpiration
    {
        //[FunctionName("DeleteChatExpirationFunction")]
        //public static void Run([TimerTrigger("0 0 01 * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)//every day at 01:00:00 AM
        //{
        //     // log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

        //   // ChatExpirationTimeOut(log).Wait();
        //}

        public static async Task<string> ChatExpirationTimeOut(ILogger log)
        {
            string result = string.Empty;

            List<int> TenantModel = new List<int>();
            TenantModel.Add(1);
            TenantModel.Add(2);
            TenantModel.Add(3);
            TenantModel.Add(4);
            TenantModel.Add(5);
            TenantModel.Add(6);
            TenantModel.Add(7);
            TenantModel.Add(8);
            TenantModel.Add(9);
            TenantModel.Add(10);
            TenantModel.Add(11);
            TenantModel.Add(12);
            TenantModel.Add(13);


            foreach (var item in TenantModel)
            {
                _=DeleteConversation(item);
            }


           
            return result;

        }
        private static async Task<string> DeleteConversation(int tenantId)
        {


            string result = string.Empty;

            var defaultdate = new DateTime(1970, 1, 1); //default  data

            TimeSpan timeSpan = DateTime.Now.AddMonths(-6)-defaultdate;


            var conversationChat = new DocumentDBHelper<ConversationChatModel>(CollectionTypes.ItemsCollection);
            var queryString = "SELECT * FROM c WHERE c.ItemType= 1 and c.TenantId="+tenantId+" and c._ts <= "+ timeSpan.TotalSeconds.ToString();
            await conversationChat.DeleteChatItem(queryString);

            //var objConversation = conversationChat.GetItemListAsync(p => p.ItemType == ContainerItemTypes.ConversationItem && p._ts<=timeSpan.TotalSeconds && p.TenantId==tenantId).Result;

            //if (objConversation != null)
            //{ // delete contact caht 


            //    var queryString = "SELECT * FROM c WHERE c.ItemType= 1 and c.TenantId="+tenantId+" and c._ts <= "+ timeSpan.TotalSeconds.ToString();
            //    await conversationChat.DeleteChatItem(queryString);

            //}


            return result;
        }

    }
}
