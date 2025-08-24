using Framework.Data;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Sunshine
{
    public class SessionTimeOutSync
    {
        private readonly IDocumentClient _IDocumentClient;

        private IHubContext<TeamInboxHub> _hub;
        IDBService _dbService;
        public SessionTimeOutSync(
             IDBService dbService,
            IHubContext<TeamInboxHub> hub,
            IDocumentClient  iDocumentClient

            )
        {
            _dbService = dbService;
            _hub = hub;
            _IDocumentClient = iDocumentClient;

        }

        public async Task<string> SessionTimeOut()
        {
            string result = string.Empty;

            var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemListAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot).Result;

            if (objConversation != null)
            {
                foreach (var item in objConversation)
                {

                    var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    var Customer = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == item.userId).Result;
                    if(Customer!=null)
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
                        catch
                        {


                        }


                    }

                }

            }


            return result;
        }

        private async Task<string> DeleteConversation(string userId)
        {


            string result = string.Empty;

            var conversationChat = new DocumentCosmoseDB<ConversationChatModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var objConversation = conversationChat.GetItemAsync(p => p.ItemType == InfoSeedContainerItemTypes.ConversationBot && p.userId == userId).Result;

            if (objConversation != null)
            { // delete contact caht 

                var queryString = "SELECT * FROM c WHERE c.ItemType= 3 and  c.userId= '" + userId + "'";
                await conversationChat.DeleteChatItem(queryString);

            }


            return result;
        }
    }
}
