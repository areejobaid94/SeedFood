using Abp.Domain.Repositories;
using Framework.Data;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.OrderOffer.Dtos;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.MultiTenancy
{
    public class TenantSyncService
    {
        private readonly TenantAppService tenantAppService;
        private readonly IDocumentClient _IDocumentClient;

        public TenantSyncService(IDocumentClient iDocumentClient
)
        {
            tenantAppService = new TenantAppService();
            _IDocumentClient = iDocumentClient; 
        }

        public async Task Sync(IConfigurationRoot _appConfiguration)
        {
            var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
            {
            };


            IList<TenantModel> result =
                   SqlDataHelper.ExecuteReader(
                       "dbo.TenantsGet",
                       sqlParameters.ToArray(),
                       MapTenantModel,
                       _appConfiguration.GetConnectionString("Default"));


            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            foreach (var item in result)
            {
                var tenant = itemsCollection.GetItemAsync(p => p.TenantId == item.TenantId && p.ItemType == InfoSeedContainerItemTypes.Tenant).Result;
                if (tenant == null)
                {
                    await itemsCollection.CreateItemAsync(new TenantModel()
                    {
                        ItemType = InfoSeedContainerItemTypes.Tenant,
                        SmoochAppID = item.SmoochAppID,
                        SmoochSecretKey = item.SmoochSecretKey,
                        SmoochAPIKeyID = item.SmoochAPIKeyID,
                        TenantId = item.TenantId,

                        DirectLineSecret = item.DirectLineSecret,
                        botId = item.botId,
                        IsBotActive = item.IsBotActive,
                        MassageIfBotNotActive = item.MassageIfBotNotActive,
                        IsCancelOrder = item.IsCancelOrder,
                        CancelTime = item.CancelTime,

                        BookingCapacity = item.BookingCapacity,
                        ReminderBookingHour = item.ReminderBookingHour,

                        ConfirmRequestText= item.ConfirmRequestText,
                        RejectRequestText= item.RejectRequestText,
                        IsReplyAfterHumanHandOver= item.IsReplyAfterHumanHandOver,

                        MenuReminderMessage = item.MenuReminderMessage,
                        IsActiveMenuReminder = item.IsActiveMenuReminder,
                        TimeReminder = item.TimeReminder,
                        //StartDate = item.StartDate,
                        //EndDate = item.EndDate,

                        //WorkText = item.WorkText,
                        IsWorkActive = item.IsWorkActive,
                        IsBellContinues = item.IsBellContinues,
                        IsBellOn = item.IsBellOn,

                        IsMenuLinkFirst = item.IsMenuLinkFirst,
                        IsInquiry = item.IsInquiry,
                        IsPickup = item.IsPickup,
                        IsDelivery = item.IsDelivery,
                        IsPreOrder = item.IsPreOrder,
                        IsBotLanguageEn = item.IsBotLanguageEn,
                        IsBotLanguageAr = item.IsBotLanguageAr,
                        IsSelectPaymentMethod = item.IsSelectPaymentMethod,

                        workModel = new WorkModel
                        {
                            EndDateFri = item.workModel.EndDateFri,
                            EndDateMon = item.workModel.EndDateMon,
                            EndDateSat = item.workModel.EndDateSat,
                            EndDateSun = item.workModel.EndDateSun,
                            EndDateThurs = item.workModel.EndDateThurs,
                            EndDateTues = item.workModel.EndDateTues,
                            EndDateWed = item.workModel.EndDateWed,

                            IsWorkActiveFri = item.workModel.IsWorkActiveFri,
                            IsWorkActiveMon = item.workModel.IsWorkActiveMon,
                            IsWorkActiveSat = item.workModel.IsWorkActiveSat,
                            IsWorkActiveSun = item.workModel.IsWorkActiveSun,
                            IsWorkActiveThurs = item.workModel.IsWorkActiveThurs,
                            IsWorkActiveTues = item.workModel.IsWorkActiveTues,
                            IsWorkActiveWed = item.workModel.IsWorkActiveWed,

                            StartDateFri = item.workModel.StartDateFri,
                            StartDateMon = item.workModel.StartDateMon,
                            StartDateSat = item.workModel.StartDateSat,
                            StartDateSun = item.workModel.StartDateSun,
                            StartDateThurs = item.workModel.StartDateThurs,
                            StartDateTues = item.workModel.StartDateTues,
                            StartDateWed = item.workModel.StartDateWed,

                            WorkTextFri = item.workModel.WorkTextFri,
                            WorkTextMon = item.workModel.WorkTextMon,
                            WorkTextSat = item.workModel.WorkTextSat,
                            WorkTextSun = item.workModel.WorkTextSun,
                            WorkTextThurs = item.workModel.WorkTextThurs,
                            WorkTextTues = item.workModel.WorkTextTues,
                            WorkTextWed = item.workModel.WorkTextWed

                        },

                        IsEvaluation = item.IsEvaluation,
                        EvaluationText = item.EvaluationText,
                        EvaluationTime = item.EvaluationTime,
                         PhoneNumber = item.PhoneNumber,

                        D360Key = item.D360Key,
                        IsD360Dialog=item.IsD360Dialog,

                        isOrderOffer = item.isOrderOffer,

                        AccessToken = item.AccessToken,
                        WhatsAppAccountID = item.WhatsAppAccountID,
                        WhatsAppAppID = item.WhatsAppAppID,
                        BotTemplateId = item.BotTemplateId,
                        BIDailyLimit = item.BIDailyLimit,
                        DeliveryCostTypeId = item.DeliveryCostTypeId,
                        IsBundleActive = item.IsBundleActive,
                        IsLiveChatWorkActive = item.IsLiveChatWorkActive,
                        ZohoCustomerId = item.ZohoCustomerId,
                        IsCaution=item.IsCaution,

                        CautionDays = item.CautionDays,
                        WarningDays = item.WarningDays,
                        IsPaidInvoice= item.IsPaidInvoice,
                        MerchantID = item.MerchantID,


                        FacebookPageId = item.FacebookPageId,
                        FacebookAccessToken = item.FacebookAccessToken,
                        InstagramId = item.InstagramId,
                        InstagramAccessToken = item.InstagramAccessToken,
                        id = item.id,
                        _rid = item._rid,
                        _self = item._self,
                        _etag = item._etag,
                        _attachments = item._attachments,
                        _ts = item._ts
                    });
                }
                else
                {
                    // await itemsCollection.UpdateTenantItemAsync(tenant);
                    //Update
                }
            }
        }
        public async Task Sync(string connectionString)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json"/*, optional: true, reloadOnChange: true*/)
                     //.AddEnvironmentVariables()
                     //.AddCommandLine(args)
                     .Build();
            var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
            {
            };


            IList<TenantModel> result =
                   SqlDataHelper.ExecuteReader(
                       "dbo.TenantsGet",
                       sqlParameters.ToArray(),
                       MapTenantModel,
                      connectionString);


            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            foreach (var item in result)
            {
                var tenant = itemsCollection.GetItemAsync(p => p.TenantId == item.TenantId && p.ItemType == InfoSeedContainerItemTypes.Tenant).Result;
                if (tenant == null)
                {
                    await itemsCollection.CreateItemAsync(new TenantModel()
                    {
                        ItemType = InfoSeedContainerItemTypes.Tenant,
                        SmoochAppID = item.SmoochAppID,
                        SmoochSecretKey = item.SmoochSecretKey,
                        SmoochAPIKeyID = item.SmoochAPIKeyID,
                        TenantId = item.TenantId,

                        DirectLineSecret = item.DirectLineSecret,
                        botId = item.botId,
                        IsBotActive = item.IsBotActive,
                        MassageIfBotNotActive = item.MassageIfBotNotActive,
                        IsCancelOrder = item.IsCancelOrder,
                        CancelTime = item.CancelTime,
                        
                        BookingCapacity = item.BookingCapacity,
                        ReminderBookingHour = item.ReminderBookingHour,
                        UnAvailableBookingDates = null,

                        RejectRequestText= item.RejectRequestText,
                        ConfirmRequestText= item.ConfirmRequestText,
                        IsReplyAfterHumanHandOver= item.IsReplyAfterHumanHandOver,

                        MenuReminderMessage = item.MenuReminderMessage,
                        IsActiveMenuReminder = item.IsActiveMenuReminder,
                        TimeReminder = item.TimeReminder,

                        //StartDate = item.StartDate,
                        //EndDate = item.EndDate,

                        //WorkText = item.WorkText,

                        IsMenuLinkFirst = item.IsMenuLinkFirst,
                        IsBotLanguageAr = item.IsBotLanguageAr,
                        IsBotLanguageEn = item.IsBotLanguageEn,
                        IsInquiry   = item.IsInquiry,
                        IsPreOrder = item.IsPreOrder,
                        IsPickup = item.IsPickup,
                        IsDelivery = item.IsDelivery,
                         IsSelectPaymentMethod = item.IsSelectPaymentMethod,
                        IsWorkActive = item.IsWorkActive,
                        IsBellContinues= item.IsBellContinues,
                        IsBellOn = item.IsBellOn,
                         workModel =new WorkModel
                         {
                              EndDateFri=item.workModel.EndDateFri,
                              EndDateMon = item.workModel.EndDateMon,
                              EndDateSat = item.workModel.EndDateSat,
                              EndDateSun = item.workModel.EndDateSun,
                              EndDateThurs = item.workModel.EndDateThurs,
                              EndDateTues = item.workModel.EndDateTues,
                              EndDateWed = item.workModel.EndDateWed,

                              IsWorkActiveFri = item.workModel.IsWorkActiveFri,
                              IsWorkActiveMon = item.workModel.IsWorkActiveMon,
                              IsWorkActiveSat = item.workModel.IsWorkActiveSat,
                              IsWorkActiveSun = item.workModel.IsWorkActiveSun,
                              IsWorkActiveThurs = item.workModel.IsWorkActiveThurs,
                              IsWorkActiveTues = item.workModel.IsWorkActiveTues,
                              IsWorkActiveWed = item.workModel.IsWorkActiveWed,

                              StartDateFri = item.workModel.StartDateFri,
                              StartDateMon = item.workModel.StartDateMon,
                              StartDateSat = item.workModel.StartDateSat,
                              StartDateSun = item.workModel.StartDateSun,
                              StartDateThurs = item.workModel.StartDateThurs,
                              StartDateTues = item.workModel.StartDateTues,
                              StartDateWed = item.workModel.StartDateWed,

                              WorkTextFri = item.workModel.WorkTextFri,
                              WorkTextMon = item.workModel.WorkTextMon,
                              WorkTextSat = item.workModel.WorkTextSat,
                              WorkTextSun = item.workModel.WorkTextSun,
                              WorkTextThurs = item.workModel.WorkTextThurs,
                              WorkTextTues = item.workModel.WorkTextTues,
                              WorkTextWed = item.workModel.WorkTextWed

                         },

                        IsEvaluation = item.IsEvaluation,
                        EvaluationText = item.EvaluationText,
                        EvaluationTime = item.EvaluationTime,

                        IsLoyalityPoint = item.IsLoyalityPoint,
                        Points = item.Points,

                        D360Key = item.D360Key,

                        IsD360Dialog = item.IsD360Dialog,
                        isOrderOffer = item.isOrderOffer,
                        PhoneNumber = item.PhoneNumber,
                         Image=item.Image,
                          ImageBg=item.ImageBg,

                        AccessToken = item.AccessToken,
                        WhatsAppAccountID = item.WhatsAppAccountID,
                        WhatsAppAppID = item.WhatsAppAppID,
                        BotTemplateId = item.BotTemplateId,
                        BIDailyLimit = item.BIDailyLimit,
                        DeliveryCostTypeId = item.DeliveryCostTypeId,
                        IsBundleActive = item.IsBundleActive,
                        IsLiveChatWorkActive = item.IsLiveChatWorkActive,
                        ZohoCustomerId = item.ZohoCustomerId,
                        IsCaution = item.IsCaution,


                       CautionDays = item.CautionDays,
                       WarningDays = item.WarningDays,


                        MerchantID = item.MerchantID,


                        FacebookPageId = item.FacebookPageId,
                        FacebookAccessToken = item.FacebookAccessToken,
                        InstagramId = item.InstagramId,
                        InstagramAccessToken = item.InstagramAccessToken,

                    });
                }
                else
                {
                    tenant.ItemType = InfoSeedContainerItemTypes.Tenant;
                  //  tenant.SmoochAppID = item.SmoochAppID;
                  //  tenant.SmoochSecretKey = item.SmoochSecretKey;
                  //  tenant.SmoochAPIKeyID = item.SmoochAPIKeyID;
                    tenant.TenantId = item.TenantId;
                    tenant.DirectLineSecret = item.DirectLineSecret;
                    tenant.botId = item.botId;

                    tenant.PhoneNumber = item.PhoneNumber;
                    tenant.D360Key = item.D360Key;
                    tenant.IsD360Dialog = item.IsD360Dialog;

                    tenant.Image= item.Image;
                    tenant.ImageBg = item.ImageBg;
                    tenant.ZohoCustomerId = item.ZohoCustomerId;
                    tenant.IsCaution = item.IsCaution;

                    tenant.CautionDays = item.CautionDays;
                    tenant.WarningDays = item.WarningDays;
                    tenant.WhatsAppAccountID = item.WhatsAppAccountID;
                    // tenant.isOrderOffer = item.isOrderOffer;
                    tenant.MerchantID = item.MerchantID;


                    tenant.FacebookPageId = item.FacebookPageId;
                    tenant.FacebookAccessToken = item.FacebookAccessToken;
                    tenant.InstagramId = item.InstagramId;
                    tenant.InstagramAccessToken = item.InstagramAccessToken;

                    await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                    //Update
                }
            }
        }
        private static TenantModel MapTenantModel(IDataReader dataReader)
        {
            TenantModel catalogue = new TenantModel
            {
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "Id"),
                SmoochAppID = SqlDataHelper.GetValue<string>(dataReader, "SmoochAppID"),
                SmoochSecretKey = SqlDataHelper.GetValue<string>(dataReader, "SmoochSecretKey"),
                SmoochAPIKeyID = SqlDataHelper.GetValue<string>(dataReader, "SmoochAPIKeyID"),

                DirectLineSecret = SqlDataHelper.GetValue<string>(dataReader, "DirectLineSecret"),
                botId = SqlDataHelper.GetValue<string>(dataReader, "botId"),
                IsBotActive = SqlDataHelper.GetValue<bool>(dataReader, "IsBotActive"),

                MassageIfBotNotActive = SqlDataHelper.GetValue<string>(dataReader, "MassageIfBotNotActive"),

                IsCancelOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsCancelOrder"),
                CancelTime = SqlDataHelper.GetValue<int>(dataReader, "CancelTime"),
                BookingCapacity = SqlDataHelper.GetValue<int>(dataReader, "BookingCapacity"),
                ReminderBookingHour = SqlDataHelper.GetValue<int>(dataReader, "ReminderBookingHour"),
                UnAvailableBookingDates = SqlDataHelper.GetValue<string>(dataReader, "UnAvailableBookingDates"),
                ConfirmRequestText = SqlDataHelper.GetValue<string>(dataReader, "ConfirmRequestText"),
                RejectRequestText = SqlDataHelper.GetValue<string>(dataReader, "RejectRequestText"),
                IsReplyAfterHumanHandOver = SqlDataHelper.GetValue<bool>(dataReader, "IsReplyAfterHumanHandOver"),

                MenuReminderMessage = SqlDataHelper.GetValue<string>(dataReader, "MenuReminderMessage"),
                IsActiveMenuReminder = SqlDataHelper.GetValue<bool>(dataReader, "IsActiveMenuReminder"),
                TimeReminder = SqlDataHelper.GetValue<int>(dataReader, "TimeReminder"),

                IsInquiry = SqlDataHelper.GetValue<bool>(dataReader, "IsInquiry"),
                IsDelivery = SqlDataHelper.GetValue<bool>(dataReader, "IsDelivery"),
                IsPickup = SqlDataHelper.GetValue<bool>(dataReader, "IsPickup"),
                IsPreOrder = SqlDataHelper.GetValue<bool>(dataReader, "IsPreOrder"),
                IsBotLanguageEn = SqlDataHelper.GetValue<bool>(dataReader, "IsBotLanguageEn"),
                IsBotLanguageAr = SqlDataHelper.GetValue<bool>(dataReader, "IsBotLanguageAr"),
                IsMenuLinkFirst = SqlDataHelper.GetValue<bool>(dataReader, "IsMenuLinkFirst"),
                IsSelectPaymentMethod = SqlDataHelper.GetValue<bool>(dataReader, "IsSelectPaymentMethod"),

                //StartDate = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDate"),
                //EndDate = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDate"),
                //WorkText = SqlDataHelper.GetValue<string>(dataReader, "WorkText"),
                IsWorkActive = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActive"),
                IsBellContinues = SqlDataHelper.GetValue<bool>(dataReader, "IsBellContinues"),
                IsBellOn = SqlDataHelper.GetValue<bool>(dataReader, "IsBellOn"),
                workModel = new WorkModel
                 {
                     IsWorkActiveSun= SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveSun"),
                     IsWorkActiveFri = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveFri"),
                     IsWorkActiveMon = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveMon"),
                     IsWorkActiveSat = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveSat"),
                     IsWorkActiveThurs = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveThurs"),
                     IsWorkActiveTues = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveTues"),
                     IsWorkActiveWed = SqlDataHelper.GetValue<bool>(dataReader, "IsWorkActiveWed"),

                     WorkTextMon = SqlDataHelper.GetValue<string>(dataReader, "WorkTextMon"),
                     WorkTextSat = SqlDataHelper.GetValue<string>(dataReader, "WorkTextSat"),
                     WorkTextSun = SqlDataHelper.GetValue<string>(dataReader, "WorkTextSun"),
                     WorkTextThurs = SqlDataHelper.GetValue<string>(dataReader, "WorkTextThurs"),
                     WorkTextTues = SqlDataHelper.GetValue<string>(dataReader, "WorkTextTues"),
                     WorkTextWed = SqlDataHelper.GetValue<string>(dataReader, "WorkTextWed"),
                     WorkTextFri = SqlDataHelper.GetValue<string>(dataReader, "WorkTextFri"),

                     StartDateSat = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateSat"),
                     StartDateFri = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateFri"),
                     StartDateMon = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateMon"),
                     StartDateSun = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateSun"),
                     StartDateThurs = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateThurs"),
                     StartDateTues = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateTues"),
                     StartDateWed = SqlDataHelper.GetValue<DateTime>(dataReader, "StartDateWed"),


                     EndDateFri = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateFri"),
                     EndDateMon = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateMon"),
                     EndDateSat = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateSat"),
                     EndDateSun = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateSun"),
                     EndDateThurs = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateThurs"),
                     EndDateTues = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateTues"),
                     EndDateWed = SqlDataHelper.GetValue<DateTime>(dataReader, "EndDateWed"),

                 },

                IsEvaluation = SqlDataHelper.GetValue<bool>(dataReader, "IsEvaluation"),
                EvaluationText = SqlDataHelper.GetValue<string>(dataReader, "EvaluationText"),
                EvaluationTime = SqlDataHelper.GetValue<int>(dataReader, "EvaluationTime"),
                D360Key = SqlDataHelper.GetValue<string>(dataReader, "D360Key"),
                IsD360Dialog= SqlDataHelper.GetValue<bool>(dataReader, "IsD360Dialog"),
                isOrderOffer = SqlDataHelper.GetValue<bool>(dataReader, "isOrderOffer"),

                IsLoyalityPoint = SqlDataHelper.GetValue<bool>(dataReader, "IsLoyalityPoint"),
                Points = SqlDataHelper.GetValue<int>(dataReader, "Points "),

                  PhoneNumber = SqlDataHelper.GetValue<string>(dataReader, "PhoneNumber"),
                   Image= SqlDataHelper.GetValue<string>(dataReader, "Image"),
                    ImageBg= SqlDataHelper.GetValue<string>(dataReader, "ImageBg"),
                AccessToken = SqlDataHelper.GetValue<string>(dataReader, "AccessToken"),
                WhatsAppAccountID = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppAccountID"),
                WhatsAppAppID = SqlDataHelper.GetValue<string>(dataReader, "WhatsAppAppID"),
                BotTemplateId = SqlDataHelper.GetValue<int>(dataReader, "BotTemplateId"),
                BIDailyLimit = SqlDataHelper.GetValue<int>(dataReader, "BIDailyLimit"),

                DeliveryCostTypeId = SqlDataHelper.GetValue<int>(dataReader, "DeliveryCostTypeId"),
                 IsBundleActive = SqlDataHelper.GetValue<bool>(dataReader, "IsBundleActive"),
                 IsLiveChatWorkActive = SqlDataHelper.GetValue<bool>(dataReader, "IsLiveChatWorkActive"),
                ZohoCustomerId = SqlDataHelper.GetValue<string>(dataReader, "ZohoCustomerId"),
                IsCaution = SqlDataHelper.GetValue<bool>(dataReader, "IsCaution"),

                CautionDays = SqlDataHelper.GetValue<int>(dataReader, "CautionDays"),
                WarningDays = SqlDataHelper.GetValue<int>(dataReader, "WarningDays"),

                MerchantID = SqlDataHelper.GetValue<string>(dataReader, "MerchantID"),

                FacebookPageId = SqlDataHelper.GetValue<string>(dataReader, "FacebookPageId"),
                FacebookAccessToken = SqlDataHelper.GetValue<string>(dataReader, "FacebookAccessToken"),
                InstagramId = SqlDataHelper.GetValue<string>(dataReader, "InstagramId"),
                InstagramAccessToken = SqlDataHelper.GetValue<string>(dataReader, "InstagramAccessToken"),

                id = SqlDataHelper.GetValue<string>(dataReader, "id"),
                _rid = SqlDataHelper.GetValue<string>(dataReader, "_rid"),
                _self = SqlDataHelper.GetValue<string>(dataReader, "_self"),
                _etag = SqlDataHelper.GetValue<string>(dataReader, "_etag"),
                _attachments = SqlDataHelper.GetValue<string>(dataReader, "_attachments"),
                _ts = SqlDataHelper.GetValue<int>(dataReader, "_ts"),


            };
            return catalogue;
        }

    }

    public class TenantModel
    {
        public int DeliveryCostTypeId { get; set; }
        public int? TenantId { get; set; }
        public InfoSeedContainerItemTypes ItemType { get; set; }
        public string SmoochAppID { get; set; }
        public string SmoochSecretKey { get; set; }
        public string SmoochAPIKeyID { get; set; }

        public string DirectLineSecret { get; set; }
        public string botId { get; set; }
        public bool IsBotActive { get; set; }
        public string MassageIfBotNotActive { get; set; }

        public bool IsCancelOrder { get; set; }

        public bool IsTaxOrder { get; set; }
        public decimal TaxValue { get; set; }


        public int CancelTime { get; set; }



        public bool IsWorkActive { get; set; }
        public bool IsBellOn { get; set; }
        public bool IsBellContinues { get; set; }

        public WorkModel workModel { get; set; }


        public string D360Key { get; set; }
        public bool IsD360Dialog{ get; set; }

        public bool IsEvaluation { get; set; }

        public string EvaluationText { get; set; }
        public int EvaluationTime { get; set; }


        public bool isOrderOffer { get; set; }

        public bool IsLoyalityPoint { get; set; }
        public int Points { get; set; }

        public string PhoneNumber { get; set; }

        public string Image { get; set; }

        public string ImageBg { get; set; }
        public string ZohoCustomerId { get; set; }

        public string id { get; set; }
        public string _rid { get; set; }
        public string _self { get; set; }
        public string _etag { get; set; }
        public string _attachments { get; set; }
        public int _ts { get; set; }
        public bool IsBundleActive { get; set; }
        public bool IsLiveChatWorkActive { get; set; }

        public WorkModel LiveChatWorkingHours { get; set; }

        public string AccessToken { get; set; }

        public string WhatsAppAccountID { get; set; }
        public string WhatsAppAppID { get; set; }
        public int? BotTemplateId { get; set; }
        public int BIDailyLimit { get; set; }
        public string CurrencyCode { get; set; }
        public string TimeZoneId { get; set; }
        public bool IsPreOrder { get; set; }
        public bool IsPickup { get; set; }
        public bool IsDelivery { get; set; }
        public int ReminderBookingHour { get; set; }
        public int BookingCapacity { get; set; }

        public string ConfirmRequestText { get; set; }
        public string RejectRequestText { get; set; }

        public bool IsReplyAfterHumanHandOver { get; set; } = true;


        //Audai   the defulat of bool is false why ??
        public bool IsCaution { get; set; } = false;
        public bool IsPaidInvoice { get; set; }

        public int CautionDays { get; set; }
        public int WarningDays { get; set; }

        public string UnAvailableBookingDates { get; set; }
        public string MenuReminderMessage { get; set; }
        public bool IsActiveMenuReminder { get; set; }
        public int TimeReminder { get; set; }

        public bool IsBotLanguageAr { get; set; }
        public bool IsBotLanguageEn { get; set; }
        public bool IsInquiry { get; set; }
        public bool IsMenuLinkFirst { get; set; }
        public bool IsSelectPaymentMethod { get; set; }
        public string MerchantID { get; set; }

        public string FacebookPageId { get; set; }
        public string FacebookAccessToken { get; set; }
        public string InstagramId { get; set; }
        public string InstagramAccessToken { get; set; }
        public string CatalogueLink { get; set; }
        public string BusinessId { get; set; }
        public string CatalogueAccessToken { get; set; }

    }
    public class WorkModel 
    {

        public bool IsWorkActiveSun { get; set; }
        public bool HasSPSun { get; set; }
        public string WorkTextSun { get; set; }
        public DateTime? StartDateSun { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateSun { get; set; } = DateTime.UtcNow;
        public DateTime? StartDateSunSP { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateSunSP { get; set; } = DateTime.UtcNow;


        public bool IsWorkActiveMon { get; set; }
        public bool HasSPMon { get; set; }
        public string WorkTextMon { get; set; }
        public DateTime? StartDateMon { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateMon { get; set; } = DateTime.UtcNow;
        public DateTime? StartDateMonSP { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateMonSP { get; set; } = DateTime.UtcNow;


        public bool IsWorkActiveTues { get; set; }
        public bool HasSPTues { get; set; }
        public string WorkTextTues { get; set; }
        public DateTime? StartDateTues { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateTues { get; set; } = DateTime.UtcNow;
        public DateTime? StartDateTuesSP { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateTuesSP { get; set; } = DateTime.UtcNow;



        public bool IsWorkActiveWed { get; set; }
        public bool HasSPWed { get; set; }
        public string WorkTextWed { get; set; }
        public DateTime? StartDateWed { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateWed { get; set; } = DateTime.UtcNow;
        public DateTime? StartDateWedSP { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateWedSP { get; set; } = DateTime.UtcNow;


        public bool IsWorkActiveThurs { get; set; }
        public bool HasSPThurs { get; set; }
        public string WorkTextThurs { get; set; }
        public DateTime? StartDateThurs { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateThurs { get; set; } = DateTime.UtcNow;
        public DateTime? StartDateThursSP { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateThursSP { get; set; } = DateTime.UtcNow;


        public bool IsWorkActiveFri { get; set; }
        public bool HasSPFri { get; set; }
        public string WorkTextFri { get; set; }
        public DateTime? StartDateFri { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateFri { get; set; } = DateTime.UtcNow;
        public DateTime? StartDateFriSP { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateFriSP { get; set; } = DateTime.UtcNow;



        public bool IsWorkActiveSat { get; set; }
        public bool HasSPSat { get; set; }
        public string WorkTextSat { get; set; }
        public DateTime? StartDateSat { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateSat { get; set; } = DateTime.UtcNow;
        public DateTime? StartDateSatSP { get; set; } = DateTime.UtcNow;
        public DateTime? EndDateSatSP { get; set; } = DateTime.UtcNow;


    }
}
