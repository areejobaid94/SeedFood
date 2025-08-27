using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.WhatsApp.Dto;

namespace Infoseed.MessagingPortal.Contacts
{
    public interface IContactsAppService : IApplicationService
    {
        List<ContactsInterestedOfModel> GetContactsInterested(int tenantId, int contactId);
        ContactsCampaignEntity GetContactsCampaign(int tenantId, int pageNumber = 0, int pageSize = 50 , string phone = null, long? templateId = null, long? campaignId = null, bool? isSent = null, bool? isDelivered = null, bool? isRead = null, bool? isFailed = null, bool? isHanged = null);
        Task<ContactsCampaignEntity> ContactsCampaignGet(ContactsCampaignFilter contactsCampaignFilter);
        Task<FileDto> BackUpConversationForAll();
        Task<FileDto> BackUpConversation(ContactDto input);

        Task<ContactDto> BlockContact(int contactId, bool isBlock,string username="");
        Task CreateOrEdit(ContactDto input);
        Task Delete(EntityDto input);
        Task<bool> DeleteContactChat(EntityDto input);
        Task<Dictionary<string, dynamic>> ContactDelete(EntityDto input);
        Task<Dictionary<string, dynamic>> PhoneNumberUpdate(int contactId, string phoneNumber);
        Task<FileDto> GetContactsToExcel(int pageNumber = 0, int pageSize = 50, string name = null, string phoneNumber = null, int? selectedStatus = null);

        ContactDto GetContactbyId(int id);
        void UpdateContact(ContactDto contactDto);
        //void UpdateContactInfo(ContactDto contactDto);
        int CreateContact(ContactDto contact);
        ContactEntity GetContact(int pageNumber = 0, int pageSize = 50,string name = null , string phoneNumber = null, int? selectedStatus = null);



        // should be deleted -- 13-11-2022
        Task<PagedResultDto<GetContactForViewDto>> GetAll(GetAllContactsInput input);
        Task<GetContactForViewDto> GetContactForView(int id);
        Task<GetContactForEditOutput> GetContactForEdit(EntityDto input);
        Task<PagedResultDto<ContactChatStatuseLookupTableDto>> GetAllChatStatuseForLookupTable(GetAllForLookupTableInput input);
        Task<PagedResultDto<ContactContactStatuseLookupTableDto>> GetAllContactStatuseForLookupTable(GetAllForLookupTableInput input);
        bool CheckIfExistContactByPhoneNumber(string phoneNumber);
        Task<FileDto> ExportContactCampaignToExcel(long? templateId, long? campaignId, bool? isSent = null, bool? isDelivered = null, bool? isRead = null, bool? isFailed = null, bool? isHanged = null);
        List<WhatsAppContactsDto> GetOptOutContactByTenantId(int tenantId);
        long AddNewContact(CreateContactDto model);


    }
}