using Infoseed.MessagingPortal.Sunshine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Interfaces
{
    public interface IDBService
    {
       

        Task UpdateCustomersChat(string userId, List<string> messageIds);
        Task<List<CustomerModel>> GetCustomersAsync(int pageNumber , int pageSize, string searchTerm = default(string));
        Task<List<CustomerChat>> GetCustomersChat(string userId, int pageNumber , int pageSize);
        Task<CustomerChat> GetCustomersLastMessage(string userId);
        Task<string> CheckIsNewCustomer(SunshineMsgReceivedModel model);
        Task UpdateCustomerChat(SunshineMsgReceivedModel model);
        void UpdateCustomerChat(Infoseed.MessagingPortal.Sunshine.Models.Sunshine.Content model, string userId, string conversationID);
        Task<CustomerModel> GetCustomer(string userId);
     //   Task<CustomerModel> UpdateCustomer(CustomerModel customerModel);

         Task<string> LockedAndUnlockedByAgent(string userId, string agentName, bool isLocked);
         Task<string> UpdateCustomerInfo(UpdateCustomerModel customerModel);
        Dictionary<string, int> GetChatStatus();
     
    }
}
