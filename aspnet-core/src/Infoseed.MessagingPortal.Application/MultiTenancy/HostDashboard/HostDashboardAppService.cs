using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Timing;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.MultiTenancy.HostDashboard.Dto;
using Infoseed.MessagingPortal.MultiTenancy.Payments;
using Framework.Data;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Abp.Collections.Extensions;
using Infoseed.MessagingPortal.Contacts;
using Microsoft.Azure.Documents;

namespace Infoseed.MessagingPortal.MultiTenancy.HostDashboard
{
    [DisableAuditing]
    [AbpAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
    public class HostDashboardAppService : MessagingPortalAppServiceBase, IHostDashboardAppService
    {
        private const int SubscriptionEndAlertDayCount = 30;
        private const int MaxExpiringTenantsShownCount = 10;
        private const int MaxRecentTenantsShownCount = 10;
        private const int RecentTenantsDayCount = 7;

        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IIncomeStatisticsService _incomeStatisticsService;
        private readonly IRepository<Contact> _contactRepository;
        private readonly IDocumentClient _IDocumentClient;

        public HostDashboardAppService(
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            IRepository<Tenant> tenantRepository,
            IIncomeStatisticsService incomeStatisticsService,
            IRepository<Contact> contactRepository
            ,IDocumentClient iDocumentClient)

        {
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _tenantRepository = tenantRepository;
            _incomeStatisticsService = incomeStatisticsService;
            _contactRepository = contactRepository;
            _IDocumentClient = iDocumentClient;

        }

        public async Task<TopStatsData> GetTopStatsData(GetTopStatsInput input)
        {
            return new TopStatsData
            {
                DashboardPlaceholder1 = 125,
                DashboardPlaceholder2 = 830,
                NewTenantsCount = await GetTenantsCountByDate(input.StartDate, input.EndDate),
                NewSubscriptionAmount = GetNewSubscriptionAmount(input.StartDate, input.EndDate)
            };
        }

        public async Task<GetRecentTenantsOutput> GetRecentTenantsData()
        {
            var tenantCreationStartDate = Clock.Now.ToUniversalTime().AddDays(-RecentTenantsDayCount);

            var recentTenants = await GetRecentTenantsData(tenantCreationStartDate, MaxRecentTenantsShownCount);

            return new GetRecentTenantsOutput()
            {
                RecentTenants = recentTenants,
                TenantCreationStartDate = tenantCreationStartDate,
                RecentTenantsDayCount = RecentTenantsDayCount,
                MaxRecentTenantsShownCount = MaxRecentTenantsShownCount
            };
        }
        public async Task<GetTenantsDataFilterOutput> GetTenantsDataFilter(GetIncomeStatisticsDataInput date)
        {

            List<RecentTenant> recentTenants = new List<RecentTenant>();



            //get all tenant
            var querytenant = TenantManager.Tenants.Include(t => t.CreatorUser);           
            var tenantlist = querytenant.ToList();


            foreach(var item in tenantlist)
            {

                var Result = GetMessagesInfoHostAsync(item.Id, date).Result;
                recentTenants.Add(new RecentTenant
                {
                    Id = item.Id,
                    CreationTime = item.CreationTime,
                    ExpiryDate = item.CreationTime.AddYears(1),
                    Name = item.Name,
                    SendMessagesCount = Result.SendMessagesCount,
                    ReceivedMessagesCount = Result.ReceivedMessagesCount,
                    TotalOfConversation = Result.TotalOfConvsersations,
                    TotalNumberOfCustomers = Result.TotalNumberOfCustomers

                }) ;

            }

            //var tenantlistMap = (tenantlist.ToList())
            //    .Select(t => ObjectMapper.Map<RecentTenant>(t))
            //    .ToList();




            return new GetTenantsDataFilterOutput()
            {              
                RecentTenants = recentTenants,
                RecentTenantsDayCount = RecentTenantsDayCount,
                MaxRecentTenantsShownCount = MaxRecentTenantsShownCount
            };
        }

        public async Task<GetExpiringTenantsOutput> GetSubscriptionExpiringTenantsData()
        {
            var subscriptionEndDateEndUtc = Clock.Now.ToUniversalTime().AddDays(SubscriptionEndAlertDayCount);
            var subscriptionEndDateStartUtc = Clock.Now.ToUniversalTime();

            var expiringTenants = await GetExpiringTenantsData(subscriptionEndDateStartUtc, subscriptionEndDateEndUtc,
                MaxExpiringTenantsShownCount);

            return new GetExpiringTenantsOutput()
            {
                ExpiringTenants = expiringTenants,
                MaxExpiringTenantsShownCount = MaxExpiringTenantsShownCount,
                SubscriptionEndAlertDayCount = SubscriptionEndAlertDayCount,
                SubscriptionEndDateStart = subscriptionEndDateStartUtc,
                SubscriptionEndDateEnd = subscriptionEndDateEndUtc
            };
        }

        public async Task<GetIncomeStatisticsDataOutput> GetIncomeStatistics(GetIncomeStatisticsDataInput input)
        {
            return new GetIncomeStatisticsDataOutput(
                await _incomeStatisticsService.GetIncomeStatisticsData(
                    input.StartDate, 
                    input.EndDate,
                    input.IncomeStatisticsDateInterval)
            );
        }

        public async Task<GetEditionTenantStatisticsOutput> GetEditionTenantStatistics(GetEditionTenantStatisticsInput input)
        {
            return new GetEditionTenantStatisticsOutput(
                await GetEditionTenantStatisticsData(input.StartDate, input.EndDate)
            );
        }

        private async Task<List<TenantEdition>> GetEditionTenantStatisticsData(DateTime startDate, DateTime endDate)
        {
            return (await _tenantRepository.GetAll()
                .Where(t => t.EditionId.HasValue &&
                            t.IsActive &&
                            t.CreationTime >= startDate &&
                            t.CreationTime <= endDate)
                .Select(t => new { t.EditionId, t.Edition.DisplayName })
                .ToListAsync()
                )
                .GroupBy(t => t.EditionId)
                .Select(t => new TenantEdition
                {
                    Label = t.First().DisplayName,
                    Value = t.Count()
                })
                .OrderBy(t => t.Label)
                .ToList();
        }

        private decimal GetNewSubscriptionAmount(DateTime startDate, DateTime endDate)
        {
            return  _subscriptionPaymentRepository.GetAll()
                  .Where(s => s.CreationTime >= startDate &&
                              s.CreationTime <= endDate &&
                              s.Status == SubscriptionPaymentStatus.Paid)
                  .Select(x => x.Amount).AsEnumerable()
                  .Sum();
        }

        private async Task<int> GetTenantsCountByDate(DateTime startDate, DateTime endDate)
        {
            return await _tenantRepository.GetAll()
                .Where(t => t.CreationTime >= startDate && t.CreationTime <= endDate)
                .CountAsync();
        }

        private async Task<List<ExpiringTenant>> GetExpiringTenantsData(DateTime subscriptionEndDateStartUtc, DateTime subscriptionEndDateEndUtc, int? maxExpiringTenantsShownCount = null)
        {
            var query = _tenantRepository.GetAll()
                .Where(t =>
                    t.SubscriptionEndDateUtc.HasValue &&
                    t.SubscriptionEndDateUtc.Value >= subscriptionEndDateStartUtc &&
                    t.SubscriptionEndDateUtc.Value <= subscriptionEndDateEndUtc)
                .Select(t => new
                {
                    t.Name,
                    t.SubscriptionEndDateUtc
                });

            if (maxExpiringTenantsShownCount.HasValue)
            {
                query = query.Take(maxExpiringTenantsShownCount.Value);
            }

            return (await query.ToListAsync())
                .Select(t => new ExpiringTenant
                {
                    TenantName = t.Name,
                    RemainingDayCount = Convert.ToInt32(t.SubscriptionEndDateUtc.Value.Subtract(subscriptionEndDateStartUtc).TotalDays)
                })
                .OrderBy(t => t.RemainingDayCount)
                .ThenBy(t => t.TenantName)
                .ToList();
        }

        private async Task<List<RecentTenant>> GetRecentTenantsData(DateTime creationDateStart, int? maxRecentTenantsShownCount = null)
        {
            ////get all tenant
            //var querytenant = TenantManager.Tenants.Include(t => t.CreatorUser);
            //var tenantCount = await querytenant.CountAsync();
            //var tenantlist = querytenant.ToList().Where(t => t.CreationTime >= creationDateStart)
            //    .OrderByDescending(t => t.CreationTime);

            var query = _tenantRepository.GetAll()
                .Where(t => t.CreationTime >= creationDateStart)
                .OrderByDescending(t => t.CreationTime);
            var filteredContacts = _contactRepository.GetAll();

            if (maxRecentTenantsShownCount.HasValue)
            {
                query = (IOrderedQueryable<Tenant>)query.Take(maxRecentTenantsShownCount.Value);
            }

            return (await query.ToListAsync())
                .Select(t => ObjectMapper.Map<RecentTenant>(t))
                .ToList();
        }

  

        public async Task<GetMeassagesInfoHostOutput> GetMessagesInfoHostAsync(int tenantId, GetIncomeStatisticsDataInput date)
        {

            var today = DateTime.Now;
            var dateselecte = today;
            dateselecte = SwitchFilter(date, ref today);

            // get all customers with this user for this  tenant
            var CustomeritemsCollection = new DocumentCosmoseDB<CustomerModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            var customers = await CustomeritemsCollection.GetItemsAsync(a => a.ItemType == InfoSeedContainerItemTypes.CustomerItem && a.TenantId == tenantId, null, int.MaxValue, 1);


            //Total Number Of Customers
            var TotalNumberOfCustomers = customers.Item1.Count();


            int TotalOfConvsersations = 0;
            int senderCustomer = 0;
            int senderTeamInbox = 0;

            foreach (var item in customers.Item1)
            {
                //get all CustomerChat with this user for this  tenant
                var CustomerChatCollection = new DocumentCosmoseDB<CustomerChat>(CollectionTypes.ItemsCollection, _IDocumentClient);
                var chatConversation = await CustomerChatCollection.GetItemsAsync(a => a.ItemType == InfoSeedContainerItemTypes.ConversationItem && a.userId == item.userId, null, int.MaxValue, 1);

                if (chatConversation.Item1 != null && chatConversation.Item1.Count() > 0)
                {

                    var lastmessCustomer = chatConversation.Item1.Where(x => x.sender == MessageSenderType.Customer).LastOrDefault();
                    if (lastmessCustomer != null)
                    {
                        if (lastmessCustomer.CreateDate <= today && lastmessCustomer.CreateDate >= dateselecte)
                        {
                            TotalOfConvsersations++;
                        }
                    }

                    senderCustomer = senderCustomer + chatConversation.Item1.Where(x => x.sender == MessageSenderType.Customer && x.CreateDate <= today && x.CreateDate >= dateselecte).Count();
                    senderTeamInbox = senderTeamInbox + chatConversation.Item1.Where(x => x.sender == MessageSenderType.TeamInbox && x.CreateDate <= today && x.CreateDate >= dateselecte).Count();

                }

            }


            return new GetMeassagesInfoHostOutput
            {
                TotalNumberOfCustomers = TotalNumberOfCustomers,
                TotalOfConvsersations = TotalOfConvsersations,
                ReceivedMessagesCount = senderCustomer,
                SendMessagesCount = senderTeamInbox
            };

        }

        private static DateTime SwitchFilter(GetIncomeStatisticsDataInput date, ref DateTime today)
        {
            DateTime dateselecte;
            if (date == null)
            {
                dateselecte = today.AddYears(-10);
                return dateselecte;
            }
            switch (date.IncomeStatisticsDateInterval.ToString())
            {
                case "None":
                    today = date.EndDate;
                    dateselecte = date.StartDate;
                    break;
                case "Daily":
                    today = DateTime.Today.AddDays(1);
                    dateselecte = DateTime.Today;
                    break;
                case "Weekly":
                    dateselecte = today.AddDays(-7);
                    break;
                case "Monthly":
                    dateselecte = today.AddMonths(-1);
                    break;
                case "Yearly":
                    dateselecte = today.AddYears(-1);
                    break;
                default:
                    dateselecte = today;
                    break;
            }

            return dateselecte;
        }
    }
}