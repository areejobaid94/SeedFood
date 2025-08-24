using Infoseed.MessagingPortal.InfoSeedParser;
using Infoseed.MessagingPortal.WhatsApp.Dto;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.WhatsApp.Dto.WhatsAppMediaResult;

namespace Infoseed.MessagingPortal.WhatsApp
{
    public interface IWhatsAppConversationSessionAppService
    {
        ConversationSessionEntity GetOpenConversationSession(int tenantId);
        WhatsAppEntity GetFreeMessage( int pageNumber = 0, int pageSize = 50);
        WhatsAppScheduledCampaign GetScheduledCampaignByCampaignId(long messageId);
        WhatsAppFreeMessageModel GetFreeMessageById(long messageId);
        long AddFreeMessage(WhatsAppFreeMessageModel message);
        bool DeleteFreeMessage(long messageId);
        Task<long> SendFreeMessageToOpenConversation(long messageId, int TenantId);
        long ScheduleMessage(WhatsAppScheduledCampaign scheduledCampaign);
        Task<WhatsAppHeaderUrl> GetInfoSeedUrlFile([FromForm] UploadFileModel file);
        void UpdateActivationCampaign(long messageId);
        bool ScheduleValidation(long CampaignId = 0);
    }
}
