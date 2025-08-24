using Abp.Domain.Repositories;
using Abp.Notifications;
using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.FacebookDTO.DTO;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Sunshine
{
    public interface IDBService
    {

        Task<TenantModel> GetTenantByKey2( string key);
        Task<TenantModel> GetTenantByKey(string phoneNumber,string key);

        Task<TenantModel> GetTenantByFacebookId(string phoneNumber, string key);
        Task<TenantModel> GetTenantByInstagramId(string phoneNumber, string key);


        Task<string> BlockCustomer(string contactId, int agentId, string agentName, bool isbloack);
     
        Task<CustomerModel> OpenByAgent(string contactId, int agentId, string agentName, bool isLocked, string text);

        Task<CustomerModel> UpdateLiveChat(CustomerModel customerModel);

        Task<List<CustomerModel>> GetCustomersAsync(int? tenantId, int pageNumber , int pageSize, string searchTerm = default(string));
        Task<List<CustomerModel>> CustomersGetAllAsync(int? tenantId, int pageNumber, int pageSize, string searchTerm = default(string),int searchId = 0 ,int chatFilterID = 0, int agentId = 0, string userId = "");
        Task<List<CustomerChat>> GetCustomersChat(string userId, int pageNumber , int pageSize);

        Task<List<CustomerChat>> GetNoteChat(string userId, int pageNumber, int pageSize);
        Task<CustomerChat> GetCustomersLastMessage(string userId);
        Task<CustomerChat> GetLastMessage(string userId);




        CustomerModel CheckCustomerD360(WebHookD360Model model,int? TenantId, string D360Key);
        CustomerModel CheckIsNewCustomerWithBotD360(WebHookD360Model model, string botID, int? TenantId, string D360Key);

  
        CustomerChat UpdateCustomerChatD360(WebHookD360Model model, int? TenantId, int count);
        CustomerChat UpdateCustomerChat(int?tenantId,Content model, string userId, string conversationID);

        CustomerChat UpdateCustomerChatD360(int? tenantId, Content model, string userId, string conversationID);
        CustomerChat UpdateCustomerChatWhatsAppAPI(WhatsAppContent model);
        CustomerChat UpdateCustomerChatFacebookAPI(FacebookContent model);

        Task<CustomerModel> GetCustomer(string userId);
        Task<CustomerModel> GetCustomerByContactId(string contactId);
        Task<CustomerModel> GetCustomerWithTenantId(string userId, int? tenantId);
        Task<string> LockedAndUnlockedByAgent(string contactId, int agentId, string agentName, bool isLocked, UserNotification userNotification);
        Task<CustomerModel> AssignTo(string contactId, int agentId, string agentName, bool isLocked, UserNotification userNotification);
        Task<string> UpdateCustomerInfo(UpdateCustomerModel customerModel);
        Task<int> NickNameUpdateAsync(int tenantId, int contactID, string nickName);
        List<ContactsTeamInboxs> ContactsTeamInbox(int tenantId, int pageNumber, int pageSize, string searchTerm = null);
        Task<CustomerModel> UpdateCustomerLocation(ContactDto user);

        Task<CustomerModel> UpdateComplaint(string contactId, int agentId, bool IsComplaint,string username="");
        Task<CustomerModel> UpdateComplaintBot(string contactId, int agentId, bool IsComplaint);

        Task<CustomerModel> UpdateLiveChat(string contactId, int agentId, bool IsLiveChat, string Department1 = null, string Department2 = null);

        Task<CustomerModel> UpdateCustomerStatus(string userId, string lockedByAgentName ,bool IsOpen, string text);
      
        Task<CustomerModel> UpdateIsSupport(string userId, bool IsOpen, string text, bool IsSupport);
        Dictionary<string, int> GetChatStatus();
        Task<TenantModel> GetTenantInfoById(int key);
        CustomerChat UpdateCustomerChat(CustomerModel customer,string userId, string text, string type, int TenantId, int count, string mediaUrl, string agentName, string agentId, MessageSenderType messageSenderType= MessageSenderType.Customer, string massageID = "", Content model = null, Referral referral=null);
        CustomerModel PrePareCustomerChat(CustomerModel customer, string userId, string text, string type, int TenantId, int count, string mediaUrl, string agentName, string agentId, MessageSenderType messageSenderType = MessageSenderType.Customer, string massageID = "");
        CustomerModel CreateNewCustomer(string from, string name, string type, string botID, int TenantId, string D360Key);        

        ContactDto GetCustomerfromDB(string from, string name, string type, string botID, int TenantId, string D360Key);

        CustomerChat UpdateCustomerStatus(CustomerModel customer);

        Task<TenantModel> GetTenantInfoByPhoneNumber(string phoneNumber);

        CustomerChat UpdateCustomerChatStatusNew(string messageId, int TenantId);
        bool IsValidContact(string phoneNumber);

        Task<TenantModel> GetTenantByMerchantId(string MerchantId);




        CustomerModel CreateNewCustomerFacebook(string psid, string name, string type, string botID, int TenantId, string fbPageToken,string avatarUrl = "",string Gender="");
        CustomerModel CreateNewCustomerInstagram(string from, string name, string type, string botID, int TenantId, string D360Key, InstagramUserInfoModel  instagramUserInfoModel );




        Task<CustomerModel> UpdateNoteByAgent(string contactId, int agentId, string agentName, bool IsNote);

        Task<CustomerModel> UpdateNoteCounter(string contactId, int agentId, string agentName, int NumberNote);
    }
}
