using Abp.Application.Services;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.LiveChat.Dto;
using System;

namespace Infoseed.MessagingPortal.LiveChat
{
    public interface ILiveChatAppService : IApplicationService
    {
        CustomerLiveChatModel AddLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat , string Department1 = null, string Department2 = null, bool IsOpen = true,int DepartmentId=0, string UserIds = "");
        CustomerLiveChatModel AddNewLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat, string type, string Department1 = null, string Department2 = null, bool IsOpen = true, int DepartmentId = 0, string UserIds = "", CustomerLiveChatModel newmodel = null, DateTime? ContactCreationDate = null);
        CustomerLiveChatModel UpdateLiveChat(int? tenantId, string phoneNumber, string userId, string displayName, int liveChatStatus, bool isliveChat, int agentId, string agentName,int selectedLiveChatID=0, bool IsOpen = true);
        FileDto GetLiveChatToExcel();
        LiveChatEntity GetLiveChat(string phoneNumber, string filteredUserId, string name, string startDate, string endDate, int? pageNumber = 0, int? pageSize = 50);
        LiveChatEntity GetTicket(DateTime? startDate, DateTime? endDate, string phoneNumber = null, string filteredUserId = null, string name = null, string departemnt = null, string ticketType = null, int? statusId = 0, int? pageNumber = 0, int? pageSize = 50, string ticketId= null,string userId= null, string summary = null, DateTime? startDateC = null, DateTime? endDateC = null, int byteam = 0);
        CustomerLiveChatModel UpdateTicket(int agentId, string agentName, int TicketId, int liveChatStatus, bool IsOpen = true, string Summary = "");

        void UpdateConversationsCount(string userId);
        void UpdateIsOpenLiveChat(int tenantId, string phoneNumber, bool IsOpen,int ConversationsCount=0, int creationTimestamp = 0, int expirationTimestamp = 0);
        void AssignLiveChatToUser( long liveChatId, string usersIds,string userName="", string UserAssignName="", long UserAssign=0, string TeamsIds = "");

        void UpdateNote(int tenantId, string phoneNumber, bool IsNote);
    }
}
