using Abp.Application.Services;
using Infoseed.MessagingPortal.DashboardUI.Dto;
using System;

namespace Infoseed.MessagingPortal.DashboardUI
{
    public interface IDashboardUIAppService : IApplicationService
    {
        DashboardNumbersEntity GetDashboardNumbers(int tenantId, string start, string end);
        long CreateDashboardNumber(DashboardNumbers numbers);
    }
}
