using Abp.Application.Services;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.Zoho.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.Tenants.Dashboard.Dto.GetMeassagesInfoOutput;
using CreateInvoicesDashbordModel = Infoseed.MessagingPortal.Tenants.Dashboard.Dto.CreateInvoicesDashbordModel;

namespace Infoseed.MessagingPortal.Tenants.Dashboard
{
    public interface ITenantDashboardAppService : IApplicationService
    {
        Task<GetBarChartInfoOutput> GetBarChartInfo();
        Task<GetBarChartInfoOutput> GetBarChartInfoArea(int id);
        Task<GetAllUserOutput> GetUserData(Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto.GetIncomeStatisticsDataInput date);
        Task<GetAllDashboard> GetAllInfoAsync(Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto.GetIncomeStatisticsDataInput date);
        Task<GetMeassagesInfoOutput> GetMessagesInfoAsync(long agentId, string userName, MultiTenancy.HostDashboard.Dto.GetIncomeStatisticsDataInput date);
        GetMemberActivityOutput GetMemberActivity();

        GetDashboardDataOutput GetDashboardData(GetDashboardDataInput input);

        GetDailySalesOutput GetDailySales();

        GetProfitShareOutput GetProfitShare();

        GetSalesSummaryOutput GetSalesSummary(GetSalesSummaryInput input);

        GetTopStatsOutput GetTopStats();

        GetRegionalStatsOutput GetRegionalStats();

        GetGeneralStatsOutput GetGeneralStats();
        GetTest GetTestlStats();
        Task StatisticsWAUpdateSync();
       
        Task<DashbardModel> GetDashoardInfo(DateTime start, DateTime end , int TenantId);
        Task<string> WalletDeposit(InvoicesWalletModel InvoicesWalletModel);
        List<LastFourTransactionModel> TransactionGetLastFour(int TenantId);
        List<string> CountryGetAll(int TenantId);
        ConversationPriceModel GetConvarsationPrice(ConversationPriceModel model,int TenantId);

        //Orders Statistics (Orders)
        OrderStatisticsModel OrdersStatisticsGet(DateTime start, DateTime end ,int TenantId, long BranchId = 0);
        //Booking Statistics (Booking)
        BookingStatisticsModel BookingStatisticsGet(DateTime start, DateTime end, int TenantId, long UserId = 0);
        //Tickets Statistics (Tickets)
        TicketsStatisticsModel TicketsStatisticsGet(DateTime start, DateTime end, int TenantId);
        //Contact Statistics (Contact)
        ContactStatisticsModel ContactStatisticsGet(DateTime start, DateTime end, int TenantId);
        //Campaign Statistics (Campaign)
        Task<CampaignStatisticsModel> CampaignStatisticsGet(DateTime start, DateTime end, int TenantId, long CampaignId = 0);

        List<BranchsModel> BranchsGetAll(int TenantId);
        List<UsersDashModel> GetAllUser(int TenantId);
        List<CampaignDashModel> GetAllCampaign(int TenantId);
        
        List<BestSellingModel> GetBestSellingItems(DateTime start, DateTime end, int TenantId);
        void CreateDepocit(int TenantId, string invoiceId, decimal Total);

        UsageDetailsGenericModel GetUsageDetails(int TenantId, long? CampaignId = 0, string GroupBy = "", int? pageNumber = 0, int? pageSize = 10, DateTime? start = null, DateTime? end = null);
        UsageStatisticsModel GetUsageStatistics(int TenantId, long CampaignId);
        FileDto GetUsageDetailsToExcel(int TenantId, long? CampaignId = 0, string GroupBy = "", DateTime? start = null, DateTime? end = null);
    }
}
