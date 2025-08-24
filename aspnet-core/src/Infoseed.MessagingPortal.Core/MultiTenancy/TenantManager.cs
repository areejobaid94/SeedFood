using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Editions;
using Infoseed.MessagingPortal.MultiTenancy.Demo;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Infoseed.MessagingPortal.Notifications;
using System;
using System.Diagnostics;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.UI;
using Infoseed.MessagingPortal.MultiTenancy.Payments;
using Infoseed.MessagingPortal.TenantInformation;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.CaptionBot;
using WebJobEntities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System.Collections.Generic;
using Castle.Core.Internal;
using Abp.Runtime.Caching;
using Infoseed.MessagingPortal.Location;
using Infoseed.MessagingPortal.Wallet;

namespace Infoseed.MessagingPortal.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// </summary>
    public class TenantManager : AbpTenantManager<Tenant, User>
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IUserEmailer _userEmailer;
        private readonly TenantDemoDataBuilder _demoDataBuilder;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<SubscribableEdition> _subscribableEditionRepository;
        private readonly ITenantInformation _tenantInformation;
        private readonly ICaptionBotAppService _captionBot;
        private readonly ICacheManager _cacheManager;
        private readonly ILocationAppService _locationAppService;
        private readonly IWalletAppService _walletAppService;

        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            EditionManager editionManager,
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            TenantDemoDataBuilder demoDataBuilder,
            UserManager userManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IAbpZeroFeatureValueStore featureValueStore,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IPasswordHasher<User> passwordHasher,
            IRepository<SubscribableEdition> subscribableEditionRepository,
            ITenantInformation tenantInformation,
            ICaptionBotAppService captionBot,
            ICacheManager cacheManager,
            IWalletAppService walletAppService,
            ILocationAppService locationAppService
            ) : base(
                tenantRepository,
                tenantFeatureRepository,
                editionManager,
                featureValueStore
            )
        {
            AbpSession = NullAbpSession.Instance;

            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _demoDataBuilder = demoDataBuilder;
            _userManager = userManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _passwordHasher = passwordHasher;
            _subscribableEditionRepository = subscribableEditionRepository;
            _tenantInformation = tenantInformation;
            _captionBot = captionBot;
            _cacheManager = cacheManager;
            _locationAppService = locationAppService;
            _walletAppService = walletAppService;
        }

        public async Task<int> CreateWithAdminUserAsync(
            string tenancyName,
            string name,
            string adminPassword,
            string adminEmailAddress,
            string connectionString,
            bool isActive,
            int? editionId,
            bool shouldChangePasswordOnNextLogin,
            bool sendActivationEmail,
            DateTime? subscriptionEndDate,
            bool isInTrialPeriod,
            string emailActivationLink,
            int? customerID = null,
            string customerLegalName = default(string),
            string commercialName = default(string),
            string legalID = default(string),
            int? customerStatusID = null,
            string phoneNumber = default(string),
            string address = default(string),
            string email = default(string),
            string contactPersonName = default(string),
            string contactPersonPhoneNumber = default(string),
            string contactPersonEmail = default(string),
            string smoochAppID = default(string),
            string smoochSecretKey = default(string),
            string SmoochAPIKeyID = default(string),
            string DirectLineSecret = default(string),
            string botId = default(string),
            bool IsBotActive = false,
            string Image = default(string),
            string ImageBg = default(string),
            TenantTypeEunm TenantType = TenantTypeEunm.Restaurants,
            string D360Key = default(string),
            decimal CostPerOrder = 0,
            decimal SubscriptionFees = 0,
            int DueDay = 0,
            TenantBotLocalEunm BotLocal = TenantBotLocalEunm.Arabic,
            string Website = default(string),
            bool IsBellOn = true,
            bool IsBellContinues = false,
            string BellSrc = default(string),
            bool IsBooking = false,
            int InsideNumber = 0,
            int OutsideNumber = 0,
            string AccessToken = default(string),
            string WhatsAppAccountID = default(string),
            string WhatsAppAppID = default(string),
            int? BotTemplateId = null,
            int BIDailyLimit = 0,
            string CurrencyCode = null,
            string TimeZoneId = null,
            bool isPreOrder = false,
            bool isPickup = false,
            bool isDelivery = false,
            string ZohoCustomerId = default(string),
            bool IsPaidInvoice = true,
            int bookingCapacity =0,
            int reminderBookingHour =0,
            bool IsCaution  = false,
            int CautionDays=2,
            int WarningDays=2,
            string UnAvailableBookingDates = null,
            string ConfirmRequestText = null,
            string RejectRequestText = null,
            bool IsD360Dialog = false,
            string CatalogueLink = null,
            string BusinessId = null,
            string CatalogueAccessToken = null

            )
        {
            int newTenantId;
            long newAdminId;

            await CheckEditionAsync(editionId, isInTrialPeriod);

            if (isInTrialPeriod && !subscriptionEndDate.HasValue)
            {
                throw new UserFriendlyException(LocalizationManager.GetString(MessagingPortalConsts.LocalizationSourceName, "TrialWithoutEndDateErrorMessage"));
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //Create tenant
                var tenant = new Tenant(tenancyName, name)
                {
                    IsActive = isActive,
                    EditionId = editionId,
                    SubscriptionEndDateUtc = subscriptionEndDate?.ToUniversalTime(),
                    IsInTrialPeriod = isInTrialPeriod,
                    CustomerID = customerID.HasValue ? customerID.Value : 0,
                    CustomerLegalName = customerLegalName,
                    CommercialName = commercialName,
                    LegalID = legalID,
                    CustomerStatusID = customerStatusID.HasValue ? customerStatusID.Value : 0,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    Email = adminEmailAddress,
                    ContactPersonName = contactPersonName,
                    ContactPersonPhoneNumber = contactPersonPhoneNumber,
                    ContactPersonEmail = contactPersonEmail,
                    SmoochSecretKey = smoochSecretKey,
                    SmoochAppID = smoochAppID,
                    SmoochAPIKeyID = SmoochAPIKeyID,
                    ConnectionString = connectionString.IsNullOrWhiteSpace() ? null : SimpleStringCipher.Instance.Encrypt(connectionString),
                    DirectLineSecret = DirectLineSecret,
                    botId = botId,
                    IsBotActive = IsBotActive,
                    Image = Image,
                    ImageBg = ImageBg,
                    TenantType = TenantType,
                    D360Key = D360Key,
                    CostPerOrder = CostPerOrder,
                    SubscriptionFees = SubscriptionFees,
                    DueDay = DueDay,
                    BotLocal = BotLocal,
                    IsBellContinues = IsBellContinues,
                    BellSrc = BellSrc,
                    IsBellOn = IsBellOn,
                    IsBooking = IsBooking,
                    InsideNumber = InsideNumber,
                    OutsideNumber = OutsideNumber,
                    Website = Website,
                    AccessToken = AccessToken,
                    WhatsAppAccountID = WhatsAppAccountID,
                    WhatsAppAppID = WhatsAppAppID,
                    BotTemplateId = BotTemplateId,
                    BIDailyLimit = BIDailyLimit,
                    DailyLimit = BIDailyLimit,
                    CurrencyCode = CurrencyCode,
                    TimeZoneId = TimeZoneId,
                    IsDelivery = isDelivery,
                    IsPickup = isPickup,
                    IsPreOrder = isPreOrder,
                    ZohoCustomerId=ZohoCustomerId,
                    IsPaidInvoice=IsPaidInvoice,     
                    BookingCapacity = bookingCapacity,
                    ReminderBookingHour = reminderBookingHour,
                    IsCaution =   IsCaution,
                    CautionDays= CautionDays,
                    WarningDays= WarningDays,
                    UnAvailableBookingDates = UnAvailableBookingDates,
                    ConfirmRequestText  = ConfirmRequestText,
                    RejectRequestText= RejectRequestText,
                    IsD360Dialog=IsD360Dialog,
                    CatalogueLink = CatalogueLink,
                    BusinessId = BusinessId,
                    CatalogueAccessToken = CatalogueAccessToken
                };
                tenant.CreatorUserId=1;
                tenant.BellSrc="https://infoseedmediastorageprod.blob.core.windows.net/4408da24-aad7-4802-a1f1-46f5ad9e2074/notificacionlivechat.mp3";

                await CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get new tenant's id.

                //Create tenant database
                _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

                //We are working entities of new tenant, so changing tenant filter
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    //Create static roles for new tenant
                    CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    await _roleManager.GrantAllPermissionsAsync(adminRole);

                    //User role should be default
                    var userRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.User);
                    userRole.IsDefault = true;
                    CheckErrors(await _roleManager.UpdateAsync(userRole));

                    //Create TenantInformation with start and end work time 
                    await _tenantInformation.CreateTenantInformationAsync(tenant.Id, DateTime.Now, DateTime.Now.AddHours(8));

                    //Create Defulat Template 
                    //await _templateMessagesAppService.CreateTemplate();


                    //Create admin user for the tenant
                    var adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress);
                    SetConversationMeasurmentsInQueue(new TenantSyncMessage() { TenantId = tenant.Id });
                    adminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                    adminUser.IsActive = true;

                    if (adminPassword.IsNullOrEmpty())
                    {
                        adminPassword = await _userManager.CreateRandomPassword();
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
                        foreach (var validator in _userManager.PasswordValidators)
                        {
                            CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                        }

                    }

                    adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);

                    try
                    {
                        CheckErrors(await _userManager.CreateAsync(adminUser));

                    }
                    catch (Exception)
                    {

                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                    //Assign admin user to admin role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                    //Notifications
                    await _appNotifier.WelcomeToTheApplicationAsync(adminUser);

                    //Send activation email
                    if (sendActivationEmail)
                    {
                        adminUser.SetNewEmailConfirmationCode();
                        await _userEmailer.SendEmailActivationLinkAsync(adminUser, emailActivationLink, adminPassword);
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    await _demoDataBuilder.BuildForAsync(tenant);

                    newTenantId = tenant.Id;
                    newAdminId = adminUser.Id;

                    //Create Caption
                    await _captionBot.CreateCaption(tenant.Id, (int)BotLocal, tenant.TenantType);

                    //Create Default Location
                    _locationAppService.CreateDefaultLocation(tenant.Id);

                    //Create Wallet 
                    _walletAppService.CreateWallet(tenant.Id);

                }
                //create   Conservation Measurements




                List<ConversationMeasurementsTenantModel> lstconservation = new List<ConversationMeasurementsTenantModel>();



                for (int i = 1; i <= 12; i++)
                {
                    ConversationMeasurementsTenantModel conservationMeasurementsModel = new ConversationMeasurementsTenantModel();

                    conservationMeasurementsModel.TenantId = tenant.Id;
                    conservationMeasurementsModel.Year = DateTime.Now.Year;
                    conservationMeasurementsModel.Month = i;
                    conservationMeasurementsModel.BusinessInitiatedCount = 0;
                    conservationMeasurementsModel.UserInitiatedCount = 0;
                    conservationMeasurementsModel.ReferralConversionCount = 0;
                    conservationMeasurementsModel.TotalFreeConversation = 1000;
                    conservationMeasurementsModel.LastUpdatedDate = DateTime.Now;
                    conservationMeasurementsModel.CreationDate = DateTime.Now;

                    conservationMeasurementsModel.TotalFreeConversationWA = 1000;
                    conservationMeasurementsModel.TotalUsageFreeConversationWA = 0;
                    conservationMeasurementsModel.TotalUsageFreeUIWA = 0;
                    conservationMeasurementsModel.TotalUsageFreeBIWA = 0;
                    conservationMeasurementsModel.TotalUsagePaidConversationWA = 0;
                    conservationMeasurementsModel.TotalUsagePaidUIWA = 0;
                    conservationMeasurementsModel.TotalUsagePaidBIWA = 0;
                    conservationMeasurementsModel.TotalUsageFreeConversation = 0;
                    conservationMeasurementsModel.TotalUIConversation = 0;
                    conservationMeasurementsModel.TotalUsageUIConversation = 0;
                    conservationMeasurementsModel.TotalBIConversation = 0;
                    conservationMeasurementsModel.TotalUsageBIConversation = 0;


                    lstconservation.Add(conservationMeasurementsModel);

                }
              //  CreateTenantsConversations(lstconservation);
                await uow.CompleteAsync();
            }

            //Used a second UOW since UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(newTenantId))
                {
                    await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(new UserIdentifier(newTenantId, newAdminId));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }

            return newTenantId;
        }

        public async Task CheckEditionAsync(int? editionId, bool isInTrialPeriod)
        {
            if (!editionId.HasValue || !isInTrialPeriod)
            {
                return;
            }

            var edition = await _subscribableEditionRepository.GetAsync(editionId.Value);
            if (!edition.IsFree)
            {
                return;
            }

            var error = LocalizationManager.GetSource(MessagingPortalConsts.LocalizationSourceName).GetString("FreeEditionsCannotHaveTrialVersions");
            throw new UserFriendlyException(error);
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public decimal GetUpgradePrice(SubscribableEdition currentEdition, SubscribableEdition targetEdition, int totalRemainingHourCount, PaymentPeriodType paymentPeriodType)
        {
            int numberOfHoursPerDay = 24;

            var totalRemainingDayCount = totalRemainingHourCount / numberOfHoursPerDay;
            var unusedPeriodCount = totalRemainingDayCount / (int)paymentPeriodType;
            var unusedHoursCount = totalRemainingHourCount % ((int)paymentPeriodType * numberOfHoursPerDay);

            decimal currentEditionPriceForUnusedPeriod = 0;
            decimal targetEditionPriceForUnusedPeriod = 0;

            var currentEditionPrice = currentEdition.GetPaymentAmount(paymentPeriodType);
            var targetEditionPrice = targetEdition.GetPaymentAmount(paymentPeriodType);

            if (currentEditionPrice > 0)
            {
                currentEditionPriceForUnusedPeriod = currentEditionPrice * unusedPeriodCount;
                currentEditionPriceForUnusedPeriod += (currentEditionPrice / (int)paymentPeriodType) / numberOfHoursPerDay * unusedHoursCount;
            }

            if (targetEditionPrice > 0)
            {
                targetEditionPriceForUnusedPeriod = targetEditionPrice * unusedPeriodCount;
                targetEditionPriceForUnusedPeriod += (targetEditionPrice / (int)paymentPeriodType) / numberOfHoursPerDay * unusedHoursCount;
            }

            return targetEditionPriceForUnusedPeriod - currentEditionPriceForUnusedPeriod;
        }

        public async Task<Tenant> UpdateTenantAsync(int tenantId, bool isActive, bool? isInTrialPeriod, PaymentPeriodType? paymentPeriodType, int editionId, EditionPaymentType editionPaymentType)
        {
            var tenant = await FindByIdAsync(tenantId);

            tenant.IsActive = isActive;

            if (isInTrialPeriod.HasValue)
            {
                tenant.IsInTrialPeriod = isInTrialPeriod.Value;
            }

            tenant.EditionId = editionId;

            if (paymentPeriodType.HasValue)
            {
                tenant.UpdateSubscriptionDateForPayment(paymentPeriodType.Value, editionPaymentType);
            }

            return tenant;
        }

        public async Task<EndSubscriptionResult> EndSubscriptionAsync(Tenant tenant, SubscribableEdition edition, DateTime nowUtc)
        {
            if (tenant.EditionId == null || tenant.HasUnlimitedTimeSubscription())
            {
                throw new Exception($"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} tenant has unlimited time subscription!");
            }

            Debug.Assert(tenant.SubscriptionEndDateUtc != null, "tenant.SubscriptionEndDateUtc != null");

            var subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value;
            if (!tenant.IsInTrialPeriod)
            {
                subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value.AddDays(edition.WaitingDayAfterExpire ?? 0);
            }

            if (subscriptionEndDateUtc >= nowUtc)
            {
                throw new Exception($"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} since subscription has not expired yet!");
            }

            if (!tenant.IsInTrialPeriod && edition.ExpiringEditionId.HasValue)
            {
                tenant.EditionId = edition.ExpiringEditionId.Value;
                tenant.SubscriptionEndDateUtc = null;

                await UpdateAsync(tenant);

                return EndSubscriptionResult.AssignedToAnotherEdition;
            }

            tenant.IsActive = false;
            tenant.IsInTrialPeriod = false;

            await UpdateAsync(tenant);

            return EndSubscriptionResult.TenantSetInActive;
        }

        public override Task UpdateAsync(Tenant tenant)
        {
            if (tenant.IsInTrialPeriod && !tenant.SubscriptionEndDateUtc.HasValue)
            {
                throw new UserFriendlyException(LocalizationManager.GetString(MessagingPortalConsts.LocalizationSourceName, "TrialWithoutEndDateErrorMessage"));
            }
            _cacheManager.GetCache("CacheTenant").Remove(tenant.D360Key.ToString());

            var result = base.UpdateAsync(tenant);
            SetConversationMeasurmentsInQueue(new TenantSyncMessage() { TenantId = tenant.Id });
            return result;
        }

        private void SetConversationMeasurmentsInQueue(TenantSyncMessage message)
        {
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(AppSettingsCoreModel.AzureWebJobsStorage);
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue;
                queue = queueClient.GetQueueReference("tenant-sync");
                queue.CreateIfNotExistsAsync();

                CloudQueueMessage queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(
                           message
                        ));
                queue.AddMessageAsync(queueMessage);

            }
            catch (Exception)
            {
                var Error = JsonConvert.SerializeObject(message);
                //this.telemetry.TrackTrace(e.Message.ToString(), SeverityLevel.Error);
                //this.telemetry.TrackTrace(Error, SeverityLevel.Error);
            }


        }


      

    }
}
