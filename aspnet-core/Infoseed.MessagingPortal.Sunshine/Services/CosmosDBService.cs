using Framework.Data;
using Infoseed.MessagingPortal.Sunshine.Interfaces;
using Infoseed.MessagingPortal.Sunshine.Models;
using Infoseed.MessagingPortal.Sunshine.Models.Sunshine;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Services
{
    public class CosmosDBService : IDBService
    {
        private readonly IDocumentClient _IDocumentClient;

        public CosmosDBService(IDocumentClient iDocumentClient)
        {
                _IDocumentClient = iDocumentClient;

        }


        public async Task<string> CheckIsNewCustomer(SunshineMsgReceivedModel model)
        {
            string result = string.Empty;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection,_IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a =>a.ItemType== InfoSeedContainerItemTypes.CustomerItem&& model.events[0].payload.message.author.userId!=null && a.userId == model.events[0].payload.message.author.userId);
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer == null)
                {
                    var CustomerModel = new CustomerModel()
                    {
                        //Id = Guid.NewGuid().ToString(),
                        userId = model.events[0].payload.message.author.userId,
                        displayName = model.events[0].payload.message.author.displayName,
                        avatarUrl = model.events[0].payload.message.author.avatarUrl,
                        type = model.events[0].payload.message.author.type,
                        SunshineAppID = model.app.id,
                        SunshineConversationId = model.events[0].payload.conversation.id,
                        CreateDate = DateTime.Now,
                        IsLockedByAgent = false,
                         IsConversationExpired=false,
                        CustomerChatStatusID =(int) CustomerChatStatus.Active,
                        CustomerStatusID= (int)CustomerStatus.Active,
                        LastMessageData= DateTime.Now,
                        IsNew = true
                    };
                    await itemsCollection.CreateItemAsync(CustomerModel);
                }
                else
                {
                    customer.LastMessageData = DateTime.Now;
                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                  
                }
               
                await UpdateCustomerChat(model);

            }
         
            return result;

        }

        public async Task<CustomerModel> GetCustomer(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var customer = await itemsCollection.GetItemAsync(a =>a.ItemType==InfoSeedContainerItemTypes.CustomerItem && a.userId==userId);
            return customer;
        }

        public async Task<List<CustomerModel>> GetCustomersAsync(int pageNumber, int pageSize, string searchTerm = default(string))
        {


            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customers = await itemsCollection.GetItemsAsync(
                a => a.ItemType== InfoSeedContainerItemTypes.CustomerItem
                &&
               ((!string.IsNullOrEmpty(searchTerm) ? a.displayName.ToLower().Contains(searchTerm) :a.displayName != null)
              ||(!string.IsNullOrEmpty(searchTerm) ? a.phoneNumber.ToLower().Contains(searchTerm) :(a.phoneNumber != null || a.phoneNumber == null)))
            ,null,pageSize,pageNumber);
            
            return customers.Item1.ToList();
        }

        public async Task<List<CustomerChat>> GetCustomersChat(string userId, int pageNumber, int pageSize)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var chatConversation = await itemsCollection.GetItemsAsync(a=> a.ItemType==InfoSeedContainerItemTypes.ConversationItem && a.userId==userId,null,pageSize,pageNumber);

            return chatConversation.Item1.ToList();
        }

        public async Task UpdateCustomerChat(SunshineMsgReceivedModel model)
        {
            try
            {
                var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
               

                // Create your new conversation instance
                var CustomerChat = new CustomerChat()
                {
                    messageId = model.events[0].payload.message.id,
                    userId = model.events[0].payload.message.author.userId,
                    SunshineConversationId = model.events[0].payload.conversation.id,
                    text = model.events[0].payload.message.content.text,
                    type = model.events[0].payload.message.content.type,
                    CreateDate = DateTime.Now,
                    status = (int)Messagestatus.New,
                    sender = MessageSenderType.Customer,
                    mediaUrl = model.events[0].payload.message.content.mediaUrl
                };
                await itemsCollection.CreateItemAsync(CustomerChat);
                

            }
            catch (Exception ex)
            {

                throw;
            }
         
        }

        public async void UpdateCustomerChat(Content model, string userId, string conversationID)
        {


            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
            

            // Create your new conversation instance
            var CustomerChat = new CustomerChat()
            {
                messageId = Guid.NewGuid().ToString(),
                userId = userId,
                SunshineConversationId = conversationID,
                text = model.text,
                type = model.type,
                CreateDate = DateTime.Now,
                status = (int)Messagestatus.New,
                sender = MessageSenderType.TeamInbox,
                mediaUrl = model.mediaUrl,
                agentName = model.agentName,
                agentId = model.agentId
            };
            await itemsCollection.CreateItemAsync(CustomerChat);

            
        }

        public Task UpdateCustomersChat(string userId, List<string> messageIds)
        {
            throw new NotImplementedException();
        }

        public async Task<string> LockedAndUnlockedByAgent(string userId, string agentName,bool isLocked)
        {
            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);
 
             string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    customer.IsLockedByAgent = isLocked;
                    customer.LockedByAgentName = agentName;
                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    result = "Done";
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return result;



        }

        public Dictionary<string, int> GetChatStatus()
        {
          
      
      
            Dictionary<string, int> dictionary = new Dictionary<string,int>();
            foreach (int enumValue in  Enum.GetValues(typeof(CustomerChatStatus)))
            {
                dictionary.Add(Enum.GetName(typeof(CustomerChatStatus), enumValue), enumValue);
            }
            return dictionary;
        }

        public async Task<string> UpdateCustomerInfo(UpdateCustomerModel customerModel)
        {

            var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == customerModel.UserId);

            string result = string.Empty;
            if (customerResult.IsCompletedSuccessfully)
            {
                var customer = customerResult.Result;
                if (customer != null)
                {
                    customer.displayName = customerModel.DisplayName;
                    customer.phoneNumber = customerModel.PhoneNumber;
                    customer.EmailAddress = customerModel.EmailAddress;
                    customer.Website = customerModel.Website;
                    customer.Description = customerModel.Description;

                    await itemsCollection.UpdateItemAsync(customer._self, customer);
                    result = "done";
                }
            }
            else
            {
                result = "CustomerNotFound";
            }
            return result;


         
        }


        public async Task<CustomerChat> GetCustomersLastMessage(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;
            var itemsCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);

            var message = await itemsCollection.GetItemOrderDescAsync(
                a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == userId,
                a => a.CreateDate);
            return message;
        }
        //public Task<CustomerModel> UpdateCustomer(CustomerModel customerModel)
        //{
        //    //var itemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection);
        //    //var customerResult = itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.userId == userId);

        //    string result = string.Empty;
        //    //if (customerResult.IsCompletedSuccessfully)
        //    //{
        //    //    var customer = customerResult.Result;
        //    //    if (customer != null)
        //    //    {
        //    //        customer.IsLockedByAgent = false;
        //    //        customer.LockedByAgentName = agentName;
        //    //        await itemsCollection.UpdateItemAsync(customer._self, customer);
        //    //        result = "Done";
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    result = "CustomerNotFound";
        //    //}
        //    return customerModel;
        //}
    }
}
