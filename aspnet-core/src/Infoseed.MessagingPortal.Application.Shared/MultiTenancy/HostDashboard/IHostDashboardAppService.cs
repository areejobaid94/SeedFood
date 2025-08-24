using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto;
using static Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto.GetMeassagesInfoHostOutput;

namespace Infoseed.MessagingPortal.MultiTenancy.HostDashboard
{
    public interface IHostDashboardAppService : IApplicationService
    {
        Task<GetTenantsDataFilterOutput> GetTenantsDataFilter(GetIncomeStatisticsDataInput date);
        Task<GetMeassagesInfoHostOutput> GetMessagesInfoHostAsync(int tenantId, GetIncomeStatisticsDataInput date);
        Task<TopStatsData> GetTopStatsData(GetTopStatsInput input);

        Task<GetRecentTenantsOutput> GetRecentTenantsData();

        Task<GetExpiringTenantsOutput> GetSubscriptionExpiringTenantsData();

        Task<GetIncomeStatisticsDataOutput> GetIncomeStatistics(GetIncomeStatisticsDataInput input);

        Task<GetEditionTenantStatisticsOutput> GetEditionTenantStatistics(GetEditionTenantStatisticsInput input);

    }
}