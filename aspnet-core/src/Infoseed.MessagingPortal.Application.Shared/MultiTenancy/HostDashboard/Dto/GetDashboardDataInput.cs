using Abp.Runtime.Validation;

namespace Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto
{
    public class GetDashboardDataInput : DashboardInputBase, IShouldNormalize
    {
        public ChartDateInterval IncomeStatisticsDateInterval { get; set; }
        public DateMessages IncomeStatisticsHostDateInterval { get; set; }
        public void Normalize()
        {
            TrimTime();
        }

        private void TrimTime()
        {
            StartDate = StartDate.Date;
            StartDate = StartDate.Date;
        }
    }
}