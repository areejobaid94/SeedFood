using Infoseed.MessagingPortal.Sunshine.Interfaces;
using Infoseed.MessagingPortal.Sunshine.Models;
using Infoseed.MessagingPortal.Sunshine.Models.Sunshine;
using LiteDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Services
{
    public class LiteDBService 
    {

       
     
        public List<CustomerModel> GetCustomers(int pageNumber, int pageSize)
        {
            List<CustomerModel> lstCustomerModel = new List<CustomerModel>();

            try
            {


                using (var db = new LiteDatabase(@"CustomerDb.db"))
                {
                    var customer = db.GetCollection<CustomerModel>("Customers");
                    lstCustomerModel = customer.FindAll().ToList();
                    return lstCustomerModel;
                }
           


            }
            catch (Exception ex)
            {

                throw;
            }
          
            return lstCustomerModel;
        }
        public CustomerModel GetCustomer(string userId)
        {
            using (var db = new LiteDatabase(@"CustomerDb.db"))
            {
                var customer = db.GetCollection<CustomerModel>("Customers");
                
                var customerModel = customer.FindAll().ToList();
                var customerObj = customerModel.Where(x => x.userId == userId).FirstOrDefault();

                return customerObj;


            }
        }

        public List<CustomerChat> GetCustomersChat(string userId, int pageNumber=1, int pageSize=10)
        {

            List<CustomerChat> lstCustomerChat = new List<CustomerChat>();
            try
            {
                using (var db = new LiteDatabase(@$"CustomerChatDb{userId}.db"))
                {
                    var customerchat = db.GetCollection<CustomerChat>("customerchat");

                    lstCustomerChat = customerchat.FindAll().ToList();
                    
                    //  .Take(pageSize).Skip(pageNumber).OrderBy(x => x.CreateDate)
                }
                return lstCustomerChat;
            }
            catch (Exception ex)
            {

                throw;
            }
        
        }

        public Task UpdateCustomersChat(string userId, List<string> messageIds)
        {
            List<CustomerChat> lstCustomerChat = new List<CustomerChat>();
            using (var db = new LiteDatabase(@$"CustomerChatDb{userId}.db"))
            {
                var customerchat = db.GetCollection<CustomerChat>("customerchat");

                lstCustomerChat = customerchat.Find(x => messageIds.Contains(x.messageId)).ToList();
                foreach (var obj in lstCustomerChat)
                {
                    obj.status = (int)Messagestatus.Read;
                }
                customerchat.Update(lstCustomerChat);
            }
            return Task.CompletedTask;
        }

        public async Task<string> CheckIsNewCustomer(SunshineMsgReceivedModel model)
        {
            try
            {

           
            string result = string.Empty;
                
            using (var db = new LiteDatabase(@"CustomerDb.db"))
            {
                var customer = db.GetCollection<CustomerModel>("Customers");

                    var all = customer.FindAll().ToList();
                    //var objConversation = all.Where(o => o.userId == model.events[0].payload.message.author.userId);
                    var objConversation = customer.Find(x=>!string.IsNullOrEmpty(x.id)).Where(o => o.userId == model.events[0].payload.message.author.userId).ToList();
                        //FirstOrDefault();
    
                        
                



                if (objConversation == null ||  objConversation.Count == 0)
                {

                        // Create your new conversation instance
                        var CustomerModel = new CustomerModel()
                        {
                       //     Id = Guid.NewGuid().ToString(),
                        userId = model.events[0].payload.message.author.userId,
                        displayName = model.events[0].payload.message.author.displayName,
                        avatarUrl = model.events[0].payload.message.author.avatarUrl,
                        type = model.events[0].payload.message.author.type,
                        SunshineAppID=model.app.id,
                        SunshineConversationId = model.events[0].payload.conversation.id,
                        CreateDate = DateTime.Now,
                    };
                    customer.Insert(CustomerModel);

                }
               

                UpdateCustomerChat(model);
            }
            return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void UpdateCustomerChat(SunshineMsgReceivedModel model)
        {
            string result = string.Empty;
            using (var db = new LiteDatabase(@$"CustomerChatDb{model.events[0].payload.message.author.userId}.db"))
            {
                var customerchat = db.GetCollection<CustomerChat>("customerchat");

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
                    sender= MessageSenderType.Customer,
                    mediaUrl= model.events[0].payload.message.content.mediaUrl
                };
                customerchat.Insert(CustomerChat);



            }

        }
        public void UpdateCustomerChat(Infoseed.MessagingPortal.Sunshine.Models.Sunshine.Content model, string userId,string conversationID)
        {
            string result = string.Empty;
            try
            {

          
            using (var db = new LiteDatabase(@$"CustomerChatDb{userId}.db"))
 
            {

                var customerchat = db.GetCollection<CustomerChat>("customerchat");

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
                    sender= MessageSenderType.TeamInbox,
                    mediaUrl=model.mediaUrl,


                 
                };
                customerchat.Insert(CustomerChat);



            }

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public Task<List<CustomerModel>> GetCustomersAsync(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

     
    }
}
