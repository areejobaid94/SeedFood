using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.CaptionBot
{
    public interface ICaptionBotAppService : IApplicationService
    {
        Task<PagedResultDto<GetCaptionForViewDto>> GetAll(GetAllCaptionInput input);
        Task CreateCaption(int tenantId, int localID, TenantTypeEunm tenantType);
        List<CaptionDto> GetCaption(int tenantId, int? lang = null);
    }
}
