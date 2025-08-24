using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto;

namespace Infoseed.MessagingPortal.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}