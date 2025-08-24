using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Booking.Dtos;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Generic.Dto;
using Infoseed.MessagingPortal.InfoSeedParser;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppMediaResult;

namespace Infoseed.MessagingPortal.WhatsApp
{
    public interface IWhatsAppMessageTemplateAppService
    {
        Task SyncTemplateAsync();
        Task<WhatsAppAnalyticsModel> GetWhatsAppAnalyticAsync(DateTime start, DateTime end);
        CampinToQueueDto ReadFromExcelNew([FromForm] UploadFileModel file, long campaignId, long templateId);
        long SendMessageTemplate(WhatsAppContactsDto contacts, long templateId, long campaignId, bool IsContact);
        SendCampinStatesModel SendTemplate(ContactsEntity contactsModel, WhatsAppContactsDto contacts, long templateId, long campaignId, bool IsContact);
        SendCampinStatesModel SendCampaignNew(CampinToQueueDto contactsEntity, string sendTime);
        ContactsEntity GetFilterContacts(WhatsAppContactsDto contacts);
        Task<CampinToQueueDto> GetNewFilterContacts(WhatsAppContactsDto contacts);
        int GetContactsCount();
        DailylimitCount GetDailylimitCount();
        ContactsEntity GetExternalContacts(long templateId, long? campaignId, int? pageNumber = 0, int? pageSize = 20, int? tenantId = null);
        CampinToQueueDto GetNewExternalContacts(long templateId, long? campaignId, int? pageNumber = 0, int? pageSize = 20, int? tenantId = null);
        GetAllDashboard GetStatistics(int? tenantId = null);
        Task<WhatsAppMessageTemplateModel> GetWhatsAppMessageTemplateAsync();
        List<MessageTemplateModel> GetLocalTemplates();
        WhatsAppEntity GetCampaignByTemplateId(long templateId);
        Task<WhatsAppEntity> GetWhatsAppTemplateForCampaign(int pageNumber = 0, int pageSize = 50, int? tenantId = null);
        MessageTemplateModel GetTemplateById(long templateId);
        MessageTemplateModel GetTemplateByWhatsAppId(string templateId);
        Task<WhatsAppTemplateResultModel> AddWhatsAppMessageTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null);
        Task<WhatsAppTemplateResultModel> UpdateTemplateAsync(MessageTemplateModel messageTemplateModel, int? tenantId = null);
        Task<WhatsAppHeaderUrl> GetWhatsAppMediaLink([FromForm] UploadFileModel file);
        Task<WhatsAppMediaID> GetWhatsAppMediaID([FromForm] UploadFileModel file);
        Task<string> GetInfoSeedUrlFile([FromForm] UploadFileModel file);
        //void UpdateWhatsAppMessageTemplate(MessageTemplateModel whatsAppTemplateModel);
        Task<WhatsAppTemplateResultModel> DeleteWhatsAppMessageTemplateAsync(string templateName);
        WhatsAppEntity GetWhatsAppCampaign(int pageNumber = 0, int pageSize = 50, int? tenantId = null, int type = 1);
        Task<CampaignStatisticsDto> GetDetailsWhatsAppCampaign(long campaignId);

        WhatsAppEntity GetWhatsAppCampaignHistory(long campaignIdd);
        void UpdateWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel);
        long AddWhatsAppCampaign(WhatsAppCampaignModel whatsAppCampaignModel);
        //Task<Dictionary<string, dynamic>> SendCampignFromGroup(CampinToQueueDto contactsEntity);
        CampinToQueueDto GroupGetByIdForCampign(long groupId);
        void DeleteWhatsAppCampaign(long campaignId);
        long AddScheduledCampaign(WhatsAppContactsDto contacts, string sendTime, long campaignId, long templateId, bool isExternalContact);
        //SendCampinStatesModel AddScheduledTemplate(WhatsAppContactsNew contacts, string sendTime, string templateName);
        void UpdateActivationScheduledCampaign(long campaignId);
        bool checkPhoneNumber(string phone);
        bool SendCampaignValidation();
        decimal GetContactRate(List<string> contacts);
        Task<BookingContact> SendBookingTemplatesAsync(BookingModel booking, CaptionDto template);
        long GetTemplateIdByName(string templateName);
        BotReservedWordsEntity GetBotReservedWords(int? pageNumber = 0, int? pageSize = 20, int? tenantId = null, string keyFilter = "");
        List<ActionsModel> GetAllActions();
        BotReservedWordsModel GetByIdBotReservedWords(long Id);

        ResultBotReservedWordsModel AddBotReservedWord(BotReservedWordsModel model);
        Task<Dictionary<string, dynamic>> KeyWordAdd(KeyWordModel model);
        Dictionary<string, dynamic> KeyWordUpdate(KeyWordModel model);
        KeyWordModel KeyWordGetById(long id);
        PagedResultDto<KeyWordModel> KeyWordGetByAll(int? pageNumber = 0, int? pageSize = 20);


        KeyWordModel KeyWordGetByKey(int tenantId,string key);


        bool KeyWordDelete(long id);
        void DeleteBotReservedWord(long Id);
        ResultBotReservedWordsModel UpdateBotReservedWord(BotReservedWordsModel model);
        long AddExternalContact(WhatsAppContactsDto contact);
        ApiResponse<long> TitleCompaignCheck(string title);



        Task<FileDto> BackUpCampaginForAll(int campaignId);

    }
}
