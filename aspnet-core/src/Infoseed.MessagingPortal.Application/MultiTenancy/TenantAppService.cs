using Abp;
using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using DocumentFormat.OpenXml.Wordprocessing;
using Framework.Data;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.Currencies;
using Infoseed.MessagingPortal.Currencies.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Editions.Dto;
using Infoseed.MessagingPortal.General;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.MultiTenancy.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.Tenants.Dashboard;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using Infoseed.MessagingPortal.Url;
using Infoseed.MessagingPortal.Wallet;
using Infoseed.MessagingPortal.Zoho;
using Infoseed.MessagingPortal.Zoho.Dto;
using InfoSeedAzureFunction.AppFunEntities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static Infoseed.MessagingPortal.DashboardUI.Dto.DashboardEnums;
using static Infoseed.MessagingPortal.Tenants.Dashboard.Dto.CategoryTypeEnum;

namespace Infoseed.MessagingPortal.MultiTenancy
{

    public class TenantAppService : MessagingPortalAppServiceBase, ITenantAppService
    {
        private readonly IExcelExporterAppService _excelExporterAppServicer;
        private  IZohoAppService _zohoAppService;
        public IAppUrlService AppUrlService { get; set; }
        public IEventBus EventBus { get; set; }
        private readonly ICurrenciesAppService _currenciesAppService;
        private readonly TenantDashboardAppService _tenantDashboardAppService;
        private readonly IWalletAppService _walletAppService;
        public TenantAppService()
        {
            AppUrlService = NullAppUrlService.Instance;
            EventBus = NullEventBus.Instance;
        }

        public TenantAppService(
            IExcelExporterAppService excelExporterAppService
            , IZohoAppService zohoAppService 
            , ICurrenciesAppService currenciesAppService
            , TenantDashboardAppService tenantDashboardAppService
            , IWalletAppService walletAppService
            )
        {
            _excelExporterAppServicer = excelExporterAppService;
            _zohoAppService= zohoAppService;
            _currenciesAppService = currenciesAppService;
            _tenantDashboardAppService = tenantDashboardAppService;
            _walletAppService = walletAppService;
        }
        public async Task<PagedResultDto<TenantListDto>> GetTenants(GetTenantsInput input)
        {
            var query = TenantManager.Tenants
                .Include(t => t.Edition)
                .WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Name.Contains(input.Filter) || t.TenancyName.Contains(input.Filter))
                .WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
                .WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value)
                .WhereIf(input.SubscriptionEndDateStart.HasValue, t => t.SubscriptionEndDateUtc >= input.SubscriptionEndDateStart.Value.ToUniversalTime())
                .WhereIf(input.SubscriptionEndDateEnd.HasValue, t => t.SubscriptionEndDateUtc <= input.SubscriptionEndDateEnd.Value.ToUniversalTime())
                .WhereIf(input.EditionIdSpecified, t => t.EditionId == input.EditionId);

            var tenantCount = await query.CountAsync();
            var tenants = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<TenantListDto>(
                tenantCount,
                ObjectMapper.Map<List<TenantListDto>>(tenants)
                );
        }
        public async Task<PagedResultDto<HostTenantListDto>> GetHostTenants(int? pageSize = 10, int? pageNumber = 0, string filter="")
        {
            List<HostTenantListDto> tenants = new List<HostTenantListDto>();
            var SP_Name = Constants.Tenant.SP_HostTenantsInfoGet;
            var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@PageSize",pageSize)
                ,new SqlParameter("@PageNumber",pageNumber)
                ,new SqlParameter("@Filter",filter)
            };
            var OutputTotalPending = new SqlParameter
            {
                SqlDbType = SqlDbType.Int,
                ParameterName = "@TenantCount",
                Direction = ParameterDirection.Output
            };
            sqlParameters.Add(OutputTotalPending);

            tenants = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapHostTenantsInfo, AppSettingsModel.ConnectionStrings).ToList();
            int tenantCount = (int)OutputTotalPending.Value;
            return new PagedResultDto<HostTenantListDto>(
                tenantCount,tenants); 
        }
        public async Task<PagedResultDto<TenantListDto>> GetAllTenants()
        {
            var query = TenantManager.Tenants
                .Include(t => t.Edition);

            var tenantCount = await query.CountAsync();
            var tenants = await query.ToListAsync();

            return new PagedResultDto<TenantListDto>(
                tenantCount,
                ObjectMapper.Map<List<TenantListDto>>(tenants)
                );
        }
        [UnitOfWork(IsDisabled = true)]
        public async Task CreateTenant(CreateTenantInput input)
        {
            input.IsPaidInvoice=true;
            input.IsCaution=false;
            input.CautionDays=2;
            input.WarningDays=2;

            if(!input.CatalogueLink.IsNullOrEmpty() && !input.CatalogueLink.Contains("spreadsheets"))
            {
                throw new UserFriendlyException("The catalogue link must be a google sheet link");
            }

            await TenantManager.CreateWithAdminUserAsync(
                input.TenancyName,
                input.Name,
                input.AdminPassword,
                input.AdminEmailAddress,
                input.ConnectionString,
                input.IsActive,
                input.EditionId,
                input.ShouldChangePasswordOnNextLogin,
                input.SendActivationEmail,
                input.SubscriptionEndDateUtc?.ToUniversalTime(),
                input.IsInTrialPeriod,
                AppUrlService.CreateEmailActivationUrlFormat(input.TenancyName),
                input.CustomerID,
                input.CustomerLegalName,
                input.CommercialName,
                input.LegalID,
                input.CustomerStatusID,
                input.PhoneNumber,
                input.Address,
                input.Email,
                input.ContactPersonName,
                input.ContactPersonPhoneNumber,
                input.ContactPersonEmail,
                input.SmoochAppID,
                input.SmoochSecretKey,
                input.SmoochAPIKeyID,
                input.DirectLineSecret,
                input.botId,
                input.IsBotActive,
                input.Image,
                input.ImagBg,
                input.TenantType,
                input.D360Key,
                input.CostPerOrder ,
                input.SubscriptionFees,
                input.DueDay,
                input.BotLocal,
                input.Website,
                input.IsBellOn,
                input.IsBellContinues,
                input.BellSrc,
                input.IsBooking,
                input.InsideNumber,
                input.OutsideNumber,
                input.AccessToken,
                input.WhatsAppAccountID,
                input.WhatsAppAppID,
                input.BotTemplateId,
                input.BIDailyLimit,
                input.CurrencyCode,
                input.TimeZone,
                input.IsPreOrder,
                input.IsPickup,
                input.IsDelivery,
                input.ZohoCustomerId,
                input.IsPaidInvoice,
                 input.bookingCapacity ,
                input.reminderBookingHour ,
                input.IsCaution,
                input.CautionDays,
                input.WarningDays,
                null,
                input.ConfirmRequestText,
                input.RejectRequestText,
                input.IsD360Dialog,
                input.CatalogueLink,
                input.BusinessId,
                input.CatalogueAccessToken,
                input.DeliveryType,
                input.CareemAccessToken
            );
        }
        public async Task<TenantEditDto> GetTenantForEdit(EntityDto input)
        {
            var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(input.Id));
            tenantEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(tenantEditDto.ConnectionString);

            var x=tenantEditDto;
            return tenantEditDto;
        }
        public async Task<TenantEditDto> GetTenantForEditPhone(int input)
        {
            var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(input));
            tenantEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(tenantEditDto.ConnectionString);
            return tenantEditDto;
        }
        [HttpPost("GetFileURLT")]
        public async Task<string> GetFileURLT([FromForm] GetImageURLModel model)
        {
            var url = "";
            if (model.FormFile != null)
            {
                if (model.FormFile.Length > 0)
                {
                    var formFile = model.FormFile;
                    long ContentLength = formFile.Length;
                    byte[] fileData = null;
                    using (var ms = new MemoryStream())
                    {
                        formFile.CopyTo(ms);
                        fileData = ms.ToArray();
                    }

                    AzureBlobProvider azureBlobProvider = new AzureBlobProvider();
                    AttachmentContent attachmentContent = new AttachmentContent()
                    {
                        Content = fileData,
                        Extension = Path.GetExtension(formFile.FileName),
                        MimeType = formFile.ContentType,

                    };

                    url = await azureBlobProvider.Save(attachmentContent);

                }



            }

            return url;
        }
        public void UpdateTenantFile(string fileUrl, int TenantId)
        {
            string connString = AppSettingsModel.ConnectionStrings;
            using (SqlConnection connection = new SqlConnection(connString))
                try
                {

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "UPDATE AbpTenants SET fileUrl = @fileUrl  Where Id = @Id";

                        command.Parameters.AddWithValue("@Id", TenantId);
                        command.Parameters.AddWithValue("@fileUrl", fileUrl);
                        connection.Open();
                        command.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch (Exception )
                {


                }

        }
        public async Task UpdateTenantSettings(TenantEditDto input)
        {
            var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(input.Id));
            tenantEditDto.CurrencyCode = input.CurrencyCode;
            tenantEditDto.TimeZoneId = input.TimeZoneId;
            tenantEditDto.IsDelivery = input.IsDelivery;
            tenantEditDto.IsPickup = input.IsPickup;
            tenantEditDto.IsPreOrder = input.IsPreOrder;
            tenantEditDto.ZohoCustomerId= input.ZohoCustomerId;

            tenantEditDto.CautionDays= input.CautionDays;
            tenantEditDto.WarningDays= input.WarningDays;

            await UpdateTenant(tenantEditDto);

        }

        public void UpdateTenantConversation(ConversationMeasurements measurements)
        {
            try
            {
                var SP_Name = Constants.Tenant.SP_TenantConversationMeasurementUpdate;
                var sqlParameters = new List<SqlParameter>
                {
                     new SqlParameter("@TenantId",measurements.TenantId)
                    ,new SqlParameter("@Year",DateTime.UtcNow.Year)
                    ,new SqlParameter("@Month",DateTime.UtcNow.Month)
                    ,new SqlParameter("@TotalUIConversation",measurements.TotalUIConversation)
                    ,new SqlParameter("@TotalUsageUIConversation",measurements.TotalUsageUIConversation)
                    ,new SqlParameter("@TotalBIConversation",measurements.TotalBIConversation)
                    ,new SqlParameter("@TotalUsageBIConversation",measurements.TotalUsageBIConversation)
                    ,new SqlParameter("@TotalMarketingBIConversation",measurements.TotalMarketingBIConversation)
                    ,new SqlParameter("@TotalUsageMarketingBIConversation",measurements.TotalUsageMarketingBIConversation)
                    ,new SqlParameter("@TotalUtilityBIConversation",measurements.TotalUtilityBIConversation)
                    ,new SqlParameter("@TotalUsageUtilityBIConversation",measurements.TotalUsageUtilityBIConversation)
                };
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ConversationMeasurements GetTenantConversation(int tenantId)
        {
            try
            {
                ConversationMeasurements conversationMeasurements = new ConversationMeasurements();
                var SP_Name = Constants.Tenant.SP_TenantConversationMeasurementGet;

                var sqlParameters = new List<SqlParameter>
                {
                     new SqlParameter("@TenantId",tenantId)
                    ,new SqlParameter("@Year",DateTime.UtcNow.Year)
                    ,new SqlParameter("@Month",DateTime.UtcNow.Month)
                };

                conversationMeasurements = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTenantConversationMeasurements, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                return conversationMeasurements;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task UpdateTenant(TenantEditDto input)
        {

            input.DailyLimit=input.BIDailyLimit;
            await TenantManager.CheckEditionAsync(input.EditionId, input.IsInTrialPeriod);

            input.ConnectionString = SimpleStringCipher.Instance.Encrypt(input.ConnectionString);
            var tenant = await TenantManager.GetByIdAsync(input.Id);

            if (tenant.EditionId != input.EditionId)
            {
                EventBus.Trigger(new TenantEditionChangedEventData
                {
                    TenantId = input.Id,
                    OldEditionId = tenant.EditionId,
                    NewEditionId = input.EditionId
                });
            }
            
            ObjectMapper.Map(input, tenant);
            tenant.SubscriptionEndDateUtc = tenant.SubscriptionEndDateUtc?.ToUniversalTime();
            tenant.Integration = input.Integration; 


            await TenantManager.UpdateAsync(tenant);



            if(!input.AccessToken.IsNullOrEmpty() && !tenant.WhatsAppAccountID.IsNullOrEmpty())
            {
                try
                {
                    if (input.IsActive)
                    {
                        await SubscribedAsync(input.AccessToken, tenant.WhatsAppAccountID);
                    }
                    else
                    {
                        await UnSubscribedAsync(input.AccessToken, tenant.WhatsAppAccountID);
                    }

                }
                catch
                {


                }



            }

        }

        public string GetTenantCatalogueLink (int tenantId)
        {
            try
            {
                var catalogueLink = GetCatalogueLink(tenantId);
                return catalogueLink;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<CatalogueDto> GetCatalogue(int tenantId)
        {
            //retrieve business id and access token based on tenant id
            var tenant = GetTenantById(tenantId);
            var businessId = tenant.BusinessId ;
            var accessToken = tenant.CatalogueAccessToken;
            if(businessId.IsNullOrEmpty() || accessToken.IsNullOrEmpty())
            {
                throw new UserFriendlyException("Business Id or Access Token is missing");

            }
            var getUrl = $"https://graph.facebook.com/v22.0/{businessId}/owned_product_catalogs?access_token={accessToken}";

            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(getUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Error calling Facebook API: {response.StatusCode}");
            }
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var result = JsonSerializer.Deserialize<CatalogueDto>(json, options);

            return result;
        }

        public async Task<List<ProductItem>> GetCatalogueItems(int tenantId)
        {
            var tenant = GetTenantById(tenantId);
            var accessToken = tenant.CatalogueAccessToken;
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new UserFriendlyException("Access Token is missing");
            }

            var catalogue = await GetCatalogue(tenantId);
            var catalogueId = catalogue.Data[0].Id;

            var fields = "id,name,retailer_id,description,price,currency,availability,image_url,url";
            var baseUrl = $"https://graph.facebook.com/v19.0/{catalogueId}/products?fields={fields}&access_token={accessToken}";

            var httpClient = new HttpClient();

            var allItems = new List<ProductItem>();
            var url = baseUrl;

            do
            {
                var response = await httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Error calling Facebook API: {response.StatusCode}");
                }

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                var result = JsonSerializer.Deserialize<CatalogueItemsDto>(json, options);

                if (result?.Data != null)
                {
                    allItems.AddRange(result.Data);
                }

                url = result?.Paging?.Next;

            } while (!string.IsNullOrEmpty(url));

            return allItems;
        }



        [HttpPost]
        public bool AddCatalogueEditLog([FromBody] CatalogueAuditLogDto model)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.AddCatalogueAuditLog", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TenantId", model.TenantId);
                        command.Parameters.AddWithValue("@UserId", model.UserId);
                        command.Parameters.AddWithValue("@UserName", model.UserName);

                        connection.Open();
                        command.ExecuteNonQuery();

                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task DeleteTenant(EntityDto input)
        {
            var tenant = await TenantManager.GetByIdAsync(input.Id);
            await TenantManager.DeleteAsync(tenant);
        }
        public async Task<GetTenantFeaturesEditOutput> GetTenantFeaturesForEdit(EntityDto input)
        {
            var features = FeatureManager.GetAll()
                .Where(f => f.Scope.HasFlag(FeatureScopes.Tenant));
            var featureValues = await TenantManager.GetFeatureValuesAsync(input.Id);

            return new GetTenantFeaturesEditOutput
            {
                Features = ObjectMapper.Map<List<FlatFeatureDto>>(features).OrderBy(f => f.DisplayName).ToList(),
                FeatureValues = featureValues.Select(fv => new NameValueDto(fv)).ToList()
            };
        }
        public async Task UpdateTenantFeatures(UpdateTenantFeaturesInput input)
        {
            await TenantManager.SetFeatureValuesAsync(input.Id, input.FeatureValues.Select(fv => new NameValue(fv.Name, fv.Value)).ToArray());
        }
        public async Task ResetTenantSpecificFeatures(EntityDto input)
        {
            await TenantManager.ResetAllFeaturesAsync(input.Id);
        }
        public async Task<string> GetTenantPhoneNumber(EntityDto input)
        {
            var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(input.Id));
            tenantEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(tenantEditDto.ConnectionString);
            return tenantEditDto.PhoneNumber;
        }
        public async Task UnlockTenantAdmin(EntityDto input)
        {

            using (CurrentUnitOfWork.SetTenantId(input.Id))
            {
                var tenantAdmin = await UserManager.GetAdminAsync();
                if (tenantAdmin != null)
                {
                    tenantAdmin.Unlock();
                }
            }
        }
        public FileDto ExportTenantsToExcel(int month)
        {
            return _excelExporterAppServicer.ExportTenantsToFile(GetTenantsInfo(month));
        }
 
        [HttpGet]
        public FileDto ExportTenantsToExcelHost()
        {
            return _excelExporterAppServicer.ExportTenantsFileHost(ExportToExcelHost());
        }
        public async Task<ZohoContactsModel> ZohoContactSync(int TenantId)
        {
            var model = _zohoAppService.GetContacts(TenantId);
            return model;

        }

        public static void CreateExportToExcelHostRecord(ExportToExcelHost model)
        {
            var sqlParameters = new List<SqlParameter>
                    {
                        new SqlParameter("@TenantName", (object)model.TenantName ?? DBNull.Value),
                        new SqlParameter("@Name", (object)model.Name ?? DBNull.Value),
                        new SqlParameter("@TenancyName", (object)model.TenancyName ?? DBNull.Value),
                        new SqlParameter("@IsActive", model.IsActive),
                        new SqlParameter("@Integration", (object)model.Integration ?? DBNull.Value),
                        new SqlParameter("@CreatedBy", (object)model.CreatedBy ?? DBNull.Value),
                        new SqlParameter("@CreationTime", model.CreationTime),
                        new SqlParameter("@InvoiceId", (object)model.InvoiceId ?? DBNull.Value),
                        new SqlParameter("@PhoneNumber", (object)model.PhoneNumber ?? DBNull.Value),
                        new SqlParameter("@DomainName", (object)model.DomainName ?? DBNull.Value),
                        new SqlParameter("@CustomerName", (object)model.CustomerName ?? DBNull.Value),
                        // Ticket Statistics
                        new SqlParameter("@TotalTickets", model.TotalTickets),
                        new SqlParameter("@TotalPending", model.TotalPending),
                        new SqlParameter("@TotalOpened", model.TotalOpened),
                        new SqlParameter("@TotalClosed", model.TotalClosed),
                        new SqlParameter("@TotalExpired", model.TotalExpired),
                        new SqlParameter("@AvgResolutionTime", model.AvgResolutionTime),
                        new SqlParameter("@LastClosedTicketDate", (object)model.LastClosedTicketDate ?? DBNull.Value),

                        // Last Month Ticket Statistics
                        new SqlParameter("@LastMonthTotalTickets", model.LastMonthTotalTickets),
                        new SqlParameter("@LastMonthTotalPending", model.LastMonthTotalPending),
                        new SqlParameter("@LastMonthTotalOpened", model.LastMonthTotalOpened),
                        new SqlParameter("@LastMonthTotalClosed", model.LastMonthTotalClosed),
                        new SqlParameter("@LastMonthTotalExpired", model.LastMonthTotalExpired),
                        new SqlParameter("@LastMonthLastClosedTicketDate", (object)model.LastMonthLastClosedTicketDate ?? DBNull.Value),

                        // Wallet
                        new SqlParameter("@WalletBalance", model.WalletBalance),

                        // Order Statistics
                        

                        new SqlParameter("@TotalOrders", model.TotalOrder),
                        new SqlParameter("@PendingOrders", model.TotalOrderPending),
                        new SqlParameter("@DoneOrders", model.TotalOrderCompleted),
                        new SqlParameter("@DeletedOrders", model.TotalOrderDeleted),
                        new SqlParameter("@CanceledOrders", model.TotalOrderCanceled),
                        new SqlParameter("@PreOrders", model.TotalOrderPreOrder),

                        // Last Month Order Statistics
                        new SqlParameter("@LastMonthTotalOrders", model.LastMonthTotalOrders),
                        new SqlParameter("@LastMonthPendingOrders", model.LastMonthPendingOrders),
                        new SqlParameter("@LastMonthDoneOrders", model.LastMonthDoneOrders),
                        new SqlParameter("@LastMonthDeletedOrders", model.LastMonthDeletedOrders),
                        new SqlParameter("@LastMonthCanceledOrders", model.LastMonthCanceledOrders),
                        new SqlParameter("@LastMonthPreOrders", model.LastMonthPreOrders)
                    };

            try

            {
                SqlDataHelper.ExecuteNoneQuery("sp_InsertExportToExcelHost", sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

      

            }
            catch (Exception ex)
            {
                // Log error here
                throw new Exception("Failed to create ExportToExcelHost record", ex);
            }
        }

 
        private List<ExportToExcelHost> ExportToExcelHost()
        {
            try
            {
                var SP_Name = Constants.Tenant.sp_GetExportToExcelHost_LastDay;
                return SqlDataHelper.ExecuteReader(
                    SP_Name,
                    null,
                    DataReaderMapper.GetExportToExcelHostLastDay,
                    AppSettingsModel.ConnectionStrings
                ).ToList();
            }
            catch (Exception ex)
            {
                
                throw; 
            }
        }



        private List<ExportToExcelHost> GetAllTenantStatistics(int tenantId, DateTime currentWeekStart, DateTime currentWeekEnd, DateTime lastMonthStart, DateTime lastMonthEnd)
        {
            var sqlParameters = new List<SqlParameter>
    {
        new SqlParameter("@TenantId", tenantId),
        new SqlParameter("@CurrentWeekStart", currentWeekStart),
        new SqlParameter("@CurrentWeekEnd", currentWeekEnd),
        new SqlParameter("@LastMonthStart", lastMonthStart),
        new SqlParameter("@LastMonthEnd", lastMonthEnd)
    };

            var result = new ExportToExcelHost();

            SqlDataHelper.ExecuteReader<ExportToExcelHost>(
                Constants.LiveChat.SP_GetAllTenantStatisticsToExportExcel,
                sqlParameters.ToArray(),
                (reader) =>
                {
                   
                    reader.NextResult();
                    if (reader.Read())
                    {
                        result.TotalTickets = SqlDataHelper.GetValue<long?>(reader, "TotalTickets") ?? 0;
                        result.TotalPending = SqlDataHelper.GetValue<long?>(reader, "TotalPending") ?? 0;
                        result.TotalOpened = SqlDataHelper.GetValue<long?>(reader, "TotalOpened") ?? 0;
                        result.TotalClosed = SqlDataHelper.GetValue<long?>(reader, "TotalClosed") ?? 0;
                        result.TotalExpired = SqlDataHelper.GetValue<long?>(reader, "TotalExpired") ?? 0;
                        result.LastClosedTicketDate = SqlDataHelper.GetValue<DateTime?>(reader, "LastClosedTicketDate");
                    }

                    // Last Month Ticket Stats
                    reader.NextResult();
                    if (reader.Read())
                    {
                        result.LastMonthTotalTickets = SqlDataHelper.GetValue<long?>(reader, "TotalTickets") ?? 0;
                        result.LastMonthTotalPending = SqlDataHelper.GetValue<long?>(reader, "TotalPending") ?? 0;
                        result.LastMonthTotalOpened = SqlDataHelper.GetValue<long?>(reader, "TotalOpened") ?? 0;
                        result.LastMonthTotalClosed = SqlDataHelper.GetValue<long?>(reader, "TotalClosed") ?? 0;
                        result.LastMonthTotalExpired = SqlDataHelper.GetValue<long?>(reader, "TotalExpired") ?? 0;
                        result.LastMonthLastClosedTicketDate = SqlDataHelper.GetValue<DateTime?>(reader, "LastClosedTicketDate");
                    }

                    // Current Week Order Stats
                    reader.NextResult();
                    if (reader.Read())
                    {
                        result.TotalOrder = SqlDataHelper.GetValue<long?>(reader, "TotalOrders") ?? 0;
                        result.TotalOrderPending = SqlDataHelper.GetValue<long?>(reader, "Pending") ?? 0;
                        result.TotalOrderCompleted = SqlDataHelper.GetValue<long?>(reader, "Done") ?? 0;
                        result.TotalOrderDeleted = SqlDataHelper.GetValue<long?>(reader, "Deleted") ?? 0;
                        result.TotalOrderCanceled = SqlDataHelper.GetValue<long?>(reader, "Canceled") ?? 0;
                        result.TotalOrderPreOrder = SqlDataHelper.GetValue<long?>(reader, "PreOrder") ?? 0;
                    }

                    // Last Month Order Stats
                    reader.NextResult();
                    if (reader.Read())
                    {
                        result.LastMonthTotalOrders = SqlDataHelper.GetValue<long?>(reader, "TotalOrders") ?? 0;
                        result.LastMonthPendingOrders = SqlDataHelper.GetValue<long?>(reader, "Pending") ?? 0;
                        result.LastMonthDoneOrders = SqlDataHelper.GetValue<long?>(reader, "Done") ?? 0;
                        result.LastMonthDeletedOrders = SqlDataHelper.GetValue<long?>(reader, "Deleted") ?? 0;
                        result.LastMonthCanceledOrders = SqlDataHelper.GetValue<long?>(reader, "Canceled") ?? 0;
                        result.LastMonthPreOrders = SqlDataHelper.GetValue<long?>(reader, "PreOrder") ?? 0;
                    }

                    // Wallet Stats
                    reader.NextResult();
                    if (reader.Read())
                    {
                        result.WalletBalance = SqlDataHelper.GetValue<decimal?>(reader, "TotalAmount") ?? 0;
                     }

                    return result;
                },
                AppSettingsModel.ConnectionStrings
            );

            return new List<ExportToExcelHost> { result };
        }
        [HttpGet("ExportToExcelHost2")]
        public List<ExportToExcelHost> ExportToExcelHost1()
        {
            try
            {
                List<HostTenantListDto> tenants = GetTenants();
                DateTime now = DateTime.Now;

                DateTime currentWeekEnd = now;
                DateTime currentWeekStart = currentWeekEnd.AddDays(-7);
                DateTime lastMonthEnd = now;
                DateTime lastMonthStart = currentWeekEnd.AddDays(-30);

                List<ExportToExcelHost> statisticsList = new List<ExportToExcelHost>();

                foreach (var tenant in tenants)
                {
                    var allStats = GetAllTenantStatistics(tenant.Id, currentWeekStart, currentWeekEnd, lastMonthStart, lastMonthEnd).FirstOrDefault(); ;
                    var exportModel = new ExportToExcelHost
                    {
                        TenantName = tenant.Name,
                        Name = tenant.Name,
                        TenancyName = tenant.TenancyName,
                        IsActive = tenant.IsActive,
                        CreatedBy = tenant.CreatedBy,
                        CreationTime = tenant.CreationTime,
                        InvoiceId = tenant.InvoiceId,
                        PhoneNumber = tenant.PhoneNumber,
                        DomainName = tenant.TenancyName,
                        CustomerName = tenant.Name,
                        Integration = tenant.Integration,

                        TotalTickets = allStats.TotalTickets,
                        TotalPending = allStats.TotalPending,
                        TotalOpened = allStats.TotalOpened,
                        TotalClosed = allStats.TotalClosed,
                        TotalExpired = allStats.TotalExpired,
                        LastClosedTicketDate = allStats.LastClosedTicketDate,

                        WalletBalance = allStats.WalletBalance,

                        LastMonthTotalTickets = allStats.LastMonthTotalTickets,
                        LastMonthTotalPending = allStats.LastMonthTotalPending,
                        LastMonthTotalOpened = allStats.LastMonthTotalOpened,
                        LastMonthTotalClosed = allStats.LastMonthTotalClosed,
                        LastMonthTotalExpired = allStats.LastMonthTotalExpired,
                        LastMonthLastClosedTicketDate = allStats.LastMonthLastClosedTicketDate,

                        TotalOrder = allStats.TotalOrder,
                        TotalOrderPending = allStats.TotalOrderPending,
                        TotalOrderCompleted = allStats.TotalOrderCompleted,
                        DoneOrders = allStats.TotalOrderCompleted,
                        TotalOrderDeleted = allStats.TotalOrderDeleted,
                        TotalOrderCanceled = allStats.TotalOrderCanceled,
                        TotalOrderPreOrder = allStats.TotalOrderPreOrder,

                        LastMonthTotalOrders = allStats.LastMonthTotalOrders,
                        LastMonthPendingOrders = allStats.LastMonthPendingOrders,
                        LastMonthDoneOrders = allStats.LastMonthDoneOrders,
                        LastMonthDeletedOrders = allStats.LastMonthDeletedOrders,
                        LastMonthCanceledOrders = allStats.LastMonthCanceledOrders,
                        LastMonthPreOrders = allStats.LastMonthPreOrders
                    };

                    CreateExportToExcelHostRecord(exportModel);
                    statisticsList.Add(exportModel);
                }

                _excelExporterAppServicer.ExportTenantsFileHost(statisticsList);
                return statisticsList;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while exporting statistics", ex);
            }
        }

        private WalletModel walletGetByTenantId(int TenantId)
        {
            try
            {
                WalletModel walletModel = new WalletModel();

                var SP_Name = Constants.Wallet.SP_WalletGet;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
            new System.Data.SqlClient.SqlParameter("@TenantId", TenantId)
        };

                walletModel = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapWallet, AppSettingsModel.ConnectionStrings).FirstOrDefault();

                if (walletModel != null)
                {
                    walletModel.TotalAmountSAR = (walletModel.TotalAmount > 0)
                        ? Math.Round(walletModel.TotalAmount * (decimal)3.75, 3)
                        : 0;
                }
                else
                {
                    // Handle the case where no wallet data is returned, e.g., return a new WalletModel
                    walletModel = new WalletModel();  // or return null based on your business logic
                }

                return walletModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static WalletModel MapWallet(IDataReader dataReader)
        {
            try
            {
                WalletModel model = new WalletModel();
                model.WalletId = SqlDataHelper.GetValue<long>(dataReader, "WalletId");
                model.TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId");
                model.TotalAmount = SqlDataHelper.GetValue<decimal>(dataReader, "TotalAmount");
                model.OnHold = SqlDataHelper.GetValue<decimal>(dataReader, "OnHold");
                model.DepositDate = SqlDataHelper.GetValue<DateTime>(dataReader, "DepositDate");

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private List<HostTenantListDto> GetTenants()
        {
            var SP_Name = Constants.Tenant.SP_HostTenantsInfoGet;

            var sqlParameters = new List<SqlParameter>
        {
            new SqlParameter("@PageSize", 1000),
            new SqlParameter("@PageNumber", 0),
            new SqlParameter("@Filter", "")
        };

            var outputTotalPending = new SqlParameter
            {
                SqlDbType = SqlDbType.Int,
                ParameterName = "@TenantCount",
                Direction = ParameterDirection.Output
            };
            sqlParameters.Add(outputTotalPending);

            return SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(),
                     DataReaderMapper.MapHostTenantsInfo,
                     AppSettingsModel.ConnectionStrings).ToList();
        }

        private TicketsStatisticsModel GetTicketStatistics(int tenantId, DateTime start, DateTime end)
        {
            var sqlParameters = new List<SqlParameter>
            {
                new SqlParameter("@TenantId", tenantId),
                new SqlParameter("@Start", start),
                new SqlParameter("@End", end)
            };

            return SqlDataHelper.ExecuteReader(Constants.LiveChat.SP_TicketsStatisticsGet,
                    sqlParameters.ToArray(),
                    DataReaderMapper.MapTicketsGetStatistics,
                    AppSettingsModel.ConnectionStrings).FirstOrDefault();
        }

        private OrderStatisticsModel GetOrderStatistics(int tenantId, DateTime start, DateTime end)
        {
            var sqlParameters = new List<SqlParameter>
    {
        new SqlParameter("@TenantId", tenantId),
        new SqlParameter("@StartDate", start),
        new SqlParameter("@EndDate", end)
    };

            return SqlDataHelper.ExecuteReader("GetOrderStatusSummaryByTenantAndDate",
                     sqlParameters.ToArray(),
                     DataReaderMapper.MapOrderGetStatistics,
                     AppSettingsModel.ConnectionStrings).FirstOrDefault();
        }










        private List<TenantsToExcelDto> GetTenantsInfo(int month)
        {
            try
            {
                List<TenantsToExcelDto> tenants = new List<TenantsToExcelDto>();
                var SP_Name = Constants.Tenant.SP_TenantsInfoGet;
                var sqlParameters = new List<SqlParameter> {
                      new SqlParameter("@Month",month)
                };

                tenants = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapTenantsInfo, AppSettingsModel.ConnectionStrings).ToList();
                return tenants;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #region Public
        public async Task<SettingsTenantHostModel> GetSettingsTenantHost(int Tenant)
        {
            try
            {
                return await getSettingsTenantHostAsync(Tenant);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public SettingsTenantHostModel UpdateSettingsTenantHost(SettingsTenantHostModel model)
        {
            try
            {
                return updateSettingsTenantHost(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        #endregion

        #region Private

        private string GetCatalogueLink (int tenantId)
        {
            try
            {
                var CatalogueLink = string.Empty;
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GetTenantCatalogueLink", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TenantId", tenantId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CatalogueLink = reader["CatalogueLink"] != DBNull.Value ? reader["CatalogueLink"].ToString() : null;
                            }
                        }
                    }
                }

                return CatalogueLink;
            }
            catch(Exception ex)
            {
                throw;
            }

        }

        private TenantModel GetTenantById (int tenantId)
        {
            try {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GetTenantById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Id", tenantId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new TenantModel
                                {
                                    BusinessId = reader["BusinessId"]?.ToString(),
                                    CatalogueAccessToken = reader["CatalogueAccessToken"]?.ToString()
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private async Task SubscribedAsync(string token,string PhoneNumberId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v20.0/"+PhoneNumberId+"/subscribed_apps");
            request.Headers.Add("Authorization", "Bearer "+token);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var x = await response.Content.ReadAsStringAsync();
        }
        private async Task UnSubscribedAsync(string token, string PhoneNumberId)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Delete, "https://graph.facebook.com/v20.0/"+PhoneNumberId+"/subscribed_apps?\\access_token="+token);
            request.Headers.Add("Authorization", "Bearer "+token);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var x = await response.Content.ReadAsStringAsync();
        }




        private async Task<SettingsTenantHostModel> getSettingsTenantHostAsync(int Tenant)
        {
            try
            {
                //var tenantEditDto = ObjectMapper.Map<TenantEditDto>(await TenantManager.GetByIdAsync(Tenant));
                //tenantEditDto.ConnectionString = SimpleStringCipher.Instance.Decrypt(tenantEditDto.ConnectionString);

                SettingsTenantHostModel settingsTenantHostModel = new SettingsTenantHostModel();
                
                var SP_Name = Constants.Tenant.SP_getSettingsTenantHost;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",Tenant)
                };

                var model = SqlDataHelper.ExecuteReader(
                    SP_Name, 
                    sqlParameters.ToArray(), 
                    DataReaderMapper.MapSettingsTenantHost, 
                    AppSettingsModel.ConnectionStrings
                ).FirstOrDefault();

                settingsTenantHostModel.TimeZone = model.TimeZoneId;
                settingsTenantHostModel.ZohoCustomerId = model.ZohoCustomerId;
                settingsTenantHostModel.CautionDays = model.CautionDays;
                settingsTenantHostModel.WarningDays = model.WarningDays;
                settingsTenantHostModel.IsPreOrder = model.IsPreOrder;
                settingsTenantHostModel.IsPickup = model.IsPickup;
                settingsTenantHostModel.IsDelivery = model.IsDelivery;
                settingsTenantHostModel.TenantId = Tenant;

                settingsTenantHostModel.ClientIpAddress = model.ClientIpAddress;
                if ((model.Currency == null || model.Currency == "") && (model.CurrencyCode != null && model.CurrencyCode != ""))
                {
                    var currencies = _currenciesAppService.GetCurrencyByISOName(model.CurrencyCode);
                    settingsTenantHostModel.Currency = currencies.CurrencyName;
                }
                else
                {
                    settingsTenantHostModel.Currency = model.Currency;
                }
                settingsTenantHostModel.currencyList = _currenciesAppService.GetAllCurrencies();

                var wallet = _tenantDashboardAppService.WalletGetByTenantId(Tenant);

                if (wallet == null)
                {
                    _walletAppService.CreateWallet(Tenant);
                    wallet = _tenantDashboardAppService.WalletGetByTenantId(Tenant);
                }
                settingsTenantHostModel.TotalCustomerWallet = wallet.TotalAmount;

                return settingsTenantHostModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private SettingsTenantHostModel updateSettingsTenantHost(SettingsTenantHostModel model)
        {
            try
            {
                WalletModel WalletModel = new WalletModel();


                WalletModel = _tenantDashboardAppService.WalletGetByTenantId(model.TenantId);

                var SP_Name = Constants.Tenant.SP_UpdateTenantSettings;
                if (model.TimeZone == "")
                {
                    model.TimeZone = null;
                }
                if(model.Currency == "")
                {
                    model.Currency = null;
                }
                if (model.ZohoCustomerId == "")
                {
                    model.ZohoCustomerId = null;
                }
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@TenantId",model.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@TimeZone",model.TimeZone)
                    ,new System.Data.SqlClient.SqlParameter("@Currency",model.Currency)
                    ,new System.Data.SqlClient.SqlParameter("@ZohoCustomerId",model.ZohoCustomerId)
                    ,new System.Data.SqlClient.SqlParameter("@CautionDays",model.CautionDays)
                    ,new System.Data.SqlClient.SqlParameter("@WarningDays",model.WarningDays)
                    ,new System.Data.SqlClient.SqlParameter("@IsPreOrder",model.IsPreOrder)
                    ,new System.Data.SqlClient.SqlParameter("@IsPickup",model.IsPickup)
                    ,new System.Data.SqlClient.SqlParameter("@IsDelivery",model.IsDelivery)
                    ,new System.Data.SqlClient.SqlParameter("@TotalCustomerWallet",model.TotalCustomerWallet)
                     ,new System.Data.SqlClient.SqlParameter("@ClientIpAddress",model.ClientIpAddress)

                };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.BigInt,
                    ParameterName = "@OutPutId",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value != DBNull.Value && OutputParameter.Value.ToString() != "" && OutputParameter.Value.ToString() != null)
                {
                    if (WalletModel != null && model.Currency != "" && model.Currency != null)
                    {
                       var x= AbpSession.UserId.Value;
                        //Editing an existing user
                        var user =  UserManager.GetUserByIdAsync(x).Result;


                        TransactionModel transactionModel = new TransactionModel();
                        transactionModel.DoneBy = user.FullName;
                        transactionModel.TotalTransaction = model.TotalCustomerWallet;
                        transactionModel.TransactionDate = DateTime.UtcNow;
                        transactionModel.CategoryType = Enum.GetName(typeof(TransactionType), TransactionType.Deposit);
                        transactionModel.TotalRemaining = model.TotalCustomerWallet;
                        transactionModel.WalletId = WalletModel.WalletId;
                        transactionModel.Country = model.Currency;
                        transactionModel.TenantId = model.TenantId;
                        transactionModel.invoiceId = " ";
                        transactionModel.invoiceUrl = " ";
                        transactionModel.IsPayed = true;
                        transactionModel.Note = "Paid by InfoSeed Team";
                        long id = _tenantDashboardAppService.AddTransaction(transactionModel);
                    }
                    return model;
                }
                else
                {
                    return new SettingsTenantHostModel();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}