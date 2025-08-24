using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Maintenance.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Maintenance
{
   public  interface IMaintenancesAppService : IApplicationService
    {
        PagedResultDto<GetMaintenancesForViewDto> GetAll(GetAllMaintenancesInput input);
        Task Lock(EntityDto<long> input, int agentId, string agentName, string stringTotall);
        Task UnLock(EntityDto<long> input, string stringTotall);
        Task DeleteOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName);
        Task DoneOrder(EntityDto<long> input, string stringTotall, int agentId, string agentName);
        Task CloseOrder(EntityDto<long> input, string stringTotall);
    }
}
