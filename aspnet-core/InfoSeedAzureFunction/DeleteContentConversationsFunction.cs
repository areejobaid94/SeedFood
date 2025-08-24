using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace InfoSeedAzureFunction
{
    public static class DeleteContentConversationsFunction
    {
        //[FunctionName("DeleteContentConversationsFunction")]
        //public static void Run([TimerTrigger("0 5 * * * *", RunOnStartup = false)] TimerInfo myTimer, ILogger log)
        //{
          
        //   //Sync().Wait();
        //}

        public static async Task Sync()
        {
            try
            {

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public static async Task DeleteContactChat(int TenantId,string UserId)
        {
            //var contact = await _contactRepository.FirstOrDefaultAsync((int)input);
            var itemsCollection = new DocumentDBHelper<CustomerModel>(CollectionTypes.ItemsCollection);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == UserId);
            var customer = customerResult.Result;


            // delete contact caht 
            var queryString = "SELECT * FROM c WHERE c.TenantId=" + TenantId.ToString() + " and c.userId='" + customer.userId + "'";
            await itemsCollection.DeleteChatItem(queryString);
            // delete teaminbox caht 
            var queryString2 = "SELECT * FROM c WHERE c.ItemType= 1 and c.userId='" + customer.userId + "'";
            await itemsCollection.DeleteChatItem(queryString2);


        }
    }
}
