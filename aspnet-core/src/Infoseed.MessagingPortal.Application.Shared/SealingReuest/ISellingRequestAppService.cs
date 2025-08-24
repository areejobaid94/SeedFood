using Abp.Application.Services;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.SealingReuest
{
    
    public interface ISellingRequestAppService : IApplicationService
    {
        SellingRequestDto AddSellingRequest(SellingRequestDto sellingRequestDto);
        void UpdateSellingRequest(SellingRequestDto sellingRequestDto);
        void DeleteSellingRequest(long sellingRequestId);
        SellingRequestEntity GetSellingRequest(int? tenantId = null, int pageNumber = 0, int pageSize = 50);
        SellingRequestDto GetSellingRequestById(long sellingRequestId,int? tenantId);
        void DoneSellingRequest(long sellingRequestId);
        Task<string> TicketUpdateStatus(long ticketId, int statusId, string summary, int type = 1);
        long AddSginUpRequest(SellingRequestDto sellingRequestDto);
    }
}
