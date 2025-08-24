using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Json;
using Abp.Net.Mail;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Zero.Configuration;
using Abp.Zero.Ldap.Configuration;
using Azure.Storage.Blobs.Models;
using Framework.Data;
using Google.Apis.Auth.OAuth2;
using Infoseed.MessagingPortal.Authentication;
using Infoseed.MessagingPortal.Authorization;
using Infoseed.MessagingPortal.CaptionBot;
using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Configuration.Dto;
using Infoseed.MessagingPortal.Configuration.Host.Dto;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Loyalty;
using Infoseed.MessagingPortal.Loyalty.Dto;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.OrderOffer.Dtos;
using Infoseed.MessagingPortal.Security;
using Infoseed.MessagingPortal.Storage;
using Infoseed.MessagingPortal.TenantInformation;
using Infoseed.MessagingPortal.Timing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Configuration.Tenants
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Tenant_Settings)]
    public class TenantSettingsAppService : SettingsAppServiceBase, ITenantSettingsAppService
    {
        public IExternalLoginOptionsCacheManager ExternalLoginOptionsCacheManager { get; set; }

        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly ITimeZoneService _timeZoneService;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IAbpZeroLdapModuleConfig _ldapModuleConfig;
        private readonly IRepository<Caption, long> _captionRepository;
        private readonly IRepository<OrderOffers.OrderOffer, long> _orderOfferRepository;
        private readonly ITenantInformation  _tenantInformation;
        private readonly ITenantAppService _tenantAppService;
        private readonly IDocumentClient _IDocumentClient;
        private readonly ILoyaltyAppService  _loyaltyAppService;
        private readonly ICacheManager _cacheManager;

        public TenantSettingsAppService(
            IAbpZeroLdapModuleConfig ldapModuleConfig,
            IMultiTenancyConfig multiTenancyConfig,
            ITimeZoneService timeZoneService,
            IEmailSender emailSender,
            ITenantInformation tenantInformation,
            IBinaryObjectManager binaryObjectManager,
            IAppConfigurationAccessor configurationAccessor,
            IRepository<Caption, long> captionRepository,
            IRepository<OrderOffers.OrderOffer, long> orderOfferRepository,
            ITenantAppService tenantAppService,
            IDocumentClient iDocumentClient,
            ILoyaltyAppService loyaltyAppService,
            ICacheManager cacheManager
            ) : base(emailSender, configurationAccessor)
        {
            ExternalLoginOptionsCacheManager = NullExternalLoginOptionsCacheManager.Instance;

            _multiTenancyConfig = multiTenancyConfig;
            _ldapModuleConfig = ldapModuleConfig;
            _timeZoneService = timeZoneService;
            _binaryObjectManager = binaryObjectManager;
            _captionRepository = captionRepository;
            _tenantInformation = tenantInformation;
            _orderOfferRepository = orderOfferRepository;
            _tenantAppService=tenantAppService;
            _IDocumentClient =iDocumentClient;
            _loyaltyAppService=loyaltyAppService;
            _cacheManager = cacheManager;

        }

        #region Get Settings

        #region GoogleSheetIntegration
        //[HttpGet("OAuthUrl")] 
        public string GetGoogleAuthUrl(int tenantId)
        {
            string clientId = AppSettingsModel.googleSheetClientId;
            string redirectUri = AppSettingsModel.googleSheetRedirectUri;

            var url = GoogleAuthConsts.AuthorizationUrl;

            var queryParams = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["scope"] = "https://www.googleapis.com/auth/drive.file https://www.googleapis.com/auth/spreadsheets https://www.googleapis.com/auth/userinfo.email",
                ["access_type"] = "offline",
                ["prompt"] = "consent",
                ["state"] = tenantId.ToString()
            };

            var authUrl = QueryHelpers.AddQueryString(url, queryParams);

            return authUrl;
        }

        //[Route("GetGoogleAccessToken")]
        //[HttpGet]
        public async Task<(string accessToken, string refreshToken)> GetGoogleAccessTokenAsync(string code, int tenantId)
        {
            string clientId = AppSettingsModel.googleSheetClientId;
            string redirectUri = AppSettingsModel.googleSheetRedirectUri;
            string clientSecret = AppSettingsModel.googleSheetClientSecret;
            string tokenUrl = "https://oauth2.googleapis.com/token";

            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "grant_type", "authorization_code" },
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "redirect_uri", redirectUri },
                    { "code", code }
                };

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync(tokenUrl, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error fetching access token: {responseString}");
                }

                dynamic jsonResponse = JsonConvert.DeserializeObject(responseString);

                string accessToken = jsonResponse.access_token;
                string refreshToken = jsonResponse.refresh_token;

                string email = null;
                using (var userInfoClient = new HttpClient())
                {
                    userInfoClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var userInfoResponse = await userInfoClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                    var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();

                    if (userInfoResponse.IsSuccessStatusCode)
                    {
                        dynamic userInfo = JsonConvert.DeserializeObject(userInfoJson);
                        email = userInfo.email;
                    }
                }

                GoogleSheetConfigAdd(accessToken,refreshToken, true, tenantId, email);

                return (accessToken, refreshToken);
            }
        }

        [AllowAnonymous]
        [Route("api/google/callback")]
        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                return new ContentResult
                {
                    Content = "<script>window.close();</script>",
                    ContentType = "text/html"
                };
            }

            try
            {
                int tenantId = int.Parse(state);
                var (accessToken, refreshToken) = await GetGoogleAccessTokenAsync(code, tenantId);

                if (string.IsNullOrEmpty(accessToken))
                {
                    return new ContentResult
                    {
                        Content = "<script>alert('Failed to retrieve token.'); window.close();</script>",
                        ContentType = "text/html"
                    };
                }

                string html = $@"<!DOCTYPE html>
                    <html>
                    <head><title>OAuth Complete</title></head>
                    <body>
                    <script>
                    if (window.opener) {{
                         window.opener.postMessage('oauth-success', '*');
                    }}
                    window.close();
                    </script>
                    <p>You may close this window.</p>
                    </body>
                    </html>";


                return new ContentResult
                {
                    Content = html,
                    ContentType = "text/html"
                };
            }
            catch (Exception ex)
            {
                return new ContentResult
                {
                    Content = $"<script>alert('OAuth error: {ex.Message}'); window.close();</script>",
                    ContentType = "text/html"
                };
            }
        }


        [HttpGet]
        public async Task<string> RevokeGoogleAccess(int tenantId)
        {
            var googleSheetConfig = GoogleSheetConfigGet(tenantId);
            var accessToken = googleSheetConfig.AccessToken;

            using var client = new HttpClient();

            var requestUri = "https://oauth2.googleapis.com/revoke";

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "token", accessToken }
                });

            var response = await client.PostAsync(requestUri, content);

            if (response.IsSuccessStatusCode)
            {
                GoogleSheetConfigRemove(tenantId);
                return new string("Access token successfully revoked.");
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await RefreshAccessTokenAsync(tenantId);
                googleSheetConfig = GoogleSheetConfigGet(tenantId);
                accessToken = googleSheetConfig.AccessToken;

                content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "token", accessToken }
                });

                response = await client.PostAsync(requestUri, content);
                if (response.IsSuccessStatusCode)
                {
                    GoogleSheetConfigRemove(tenantId);
                    return new string("Access token successfully revoked.");
                }
            }

            var error = await response.Content.ReadAsStringAsync();
            return new string($"Failed to revoke token: {error}");
        }

        [HttpGet]
        public GoogleSheetConfigDto GoogleSheetConfigGet(int? tenantId)
        {
            try
            {
                var result = new GoogleSheetConfigDto();
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = new SqlCommand("dbo.GoogleSheetConfigGet", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@TenantId", tenantId);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                result.TenantId = reader.GetInt32(reader.GetOrdinal("TenantId"));
                                result.AccessToken = reader["AccessToken"] as string;
                                result.RefreshToken = reader["RefreshToken"] as string;
                                result.IsConnected = reader["IsConnected"] != DBNull.Value ? (bool)reader["IsConnected"] : (bool?)null;
                                result.GoogleEmail = reader["GoogleEmail"] as string;

                            }
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        private async Task RefreshAccessTokenAsync(int tenantId)
        {
            string clientId = AppSettingsModel.googleSheetClientId;
            string clientSecret = AppSettingsModel.googleSheetClientSecret;
            string tokenUrl = "https://oauth2.googleapis.com/token";

            var config = GoogleSheetConfigGet(tenantId);
            var refreshToken = config.RefreshToken;

            using (var httpClient = new HttpClient())
            {
                var parameters = new Dictionary<string, string>
                    {
                        { "client_id", clientId },
                        { "client_secret", clientSecret },
                        { "refresh_token", refreshToken },
                        { "grant_type", "refresh_token" }
                    };

                var content = new FormUrlEncodedContent(parameters);
                var response = await httpClient.PostAsync(tokenUrl, content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to refresh token: {responseBody}");
                }

                var json = JsonDocument.Parse(responseBody);
                var accessToken = json.RootElement.GetProperty("access_token").GetString();
                var newRefreshToken = json.RootElement.TryGetProperty("refresh_token", out var rtElement)
                    ? rtElement.GetString()
                    : refreshToken;

                // Only update refresh token if it's actually new
                if (!string.IsNullOrEmpty(newRefreshToken) && newRefreshToken != refreshToken)
                {
                    GoogleSheetConfigUpdate(accessToken, newRefreshToken, true, tenantId);
                }
                else
                {
                    GoogleSheetConfigUpdate(accessToken, refreshToken, true, tenantId);
                }

            }
        }

        private void GoogleSheetConfigUpdate(string accessToken, string refreshToken, bool? isConnected, int tenantId)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.GoogleSheetConfigUpdate";

                        command.Parameters.AddWithValue("@TenantId", (object)tenantId ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AccessToken", accessToken);
                        command.Parameters.AddWithValue("@RefreshToken", refreshToken);
                        command.Parameters.AddWithValue("@IsConnected", isConnected);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion


        public async Task<TenantSettingsEditDto> GetAllSettings()
       {
            var settings = new TenantSettingsEditDto
            {
                //UserManagement = await GetUserManagementSettingsAsync(),
                //Security = await GetSecuritySettingsAsync(),
                //Billing = await GetBillingSettingsAsync(),
                //OtherSettings = await GetOtherSettingsAsync(),
                //Email = await GetEmailSettingsAsync(),
                //ExternalLoginProviderSettings = await GetExternalLoginProviderSettings(),
                TenantInformation= await GetTenantInformation()
            };

            if (!_multiTenancyConfig.IsEnabled || Clock.SupportsMultipleTimezone)
            {
                settings.General = await GetGeneralSettingsAsync();
            }

            if (_ldapModuleConfig.IsEnabled)
            {
                settings.Ldap = await GetLdapSettingsAsync();
            }
            else
            {
                settings.Ldap = new LdapSettingsEditDto { IsModuleEnabled = false };
            }

            return settings;
        }
        public async Task<TenantSettingsEditDto> GetGeneralSettingsMobile(EunmFlagSetting flag)
        {

            TenantInformationDto tenantInformationDto = new TenantInformationDto();
            switch (flag)
            {
                case EunmFlagSetting.GeneralSettings:
                {
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());

                    tenantInformationDto = new TenantInformationDto
                    {
                        CancelTime = tenant.CancelTime,
                        IsCancelOrder = tenant.IsCancelOrder,
                        IsTaxOrder = tenant.IsTaxOrder,
                        TaxValue = tenant.TaxValue,
                        IsBotActive = tenant.IsBotActive,
                        MassageIfBotNotActive = tenant.MassageIfBotNotActive,
                        IsEvaluation =tenant.IsEvaluation,
                        EvaluationText=tenant.EvaluationText,
                        EvaluationTime=tenant.EvaluationTime,
                        DeliveryCostTypeId=tenant.DeliveryCostTypeId,
                        ReminderBookingHour = tenant.ReminderBookingHour,
                        RejectRequestText=tenant.RejectRequestText,
                        ConfirmRequestText=tenant.ConfirmRequestText,
                        IsReplyAfterHumanHandOver=tenant.IsReplyAfterHumanHandOver,
                        BookingCapacity = tenant.BookingCapacity,
                        UnAvailableBookingDates = tenant.UnAvailableBookingDates,
                        MenuReminderMessage = tenant.MenuReminderMessage,
                        IsActiveMenuReminder = tenant.IsActiveMenuReminder,
                        TimeReminder =  tenant.TimeReminder,
                        IsBotLanguageAr = tenant.IsBotLanguageAr,
                        IsBotLanguageEn = tenant.IsBotLanguageEn,
                        IsInquiry = tenant.IsInquiry,
                        IsDelivery = tenant.IsDelivery,
                        IsPickup = tenant.IsPickup,
                        IsPreOrder = tenant.IsPreOrder,
                        IsMenuLinkFirst = tenant.IsMenuLinkFirst,
                        IsSelectPaymentMethod = tenant.IsSelectPaymentMethod
                    };
                        
                    break;
                }

                case EunmFlagSetting.WorkingHour:
                {
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());
                    Dto.WorkModel workModel = new Dto.WorkModel();
                    workModel = PrepareGetWorkModel(tenant.workModel);

                    tenantInformationDto = new TenantInformationDto
                    {
                        IsWorkActive = tenant.IsWorkActive,
                        IsBellContinues = tenant.IsBellContinues,
                        IsBellOn = tenant.IsBellOn,
                        workModel = workModel,
                    };
                    break;
                }

                case EunmFlagSetting.LiveChatWorkingHour:
                {
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());


                    Dto.WorkModel liveChatWorkModel = new Dto.WorkModel();
                    liveChatWorkModel = PrepareGetWorkModel(tenant.LiveChatWorkingHours); 

                    tenantInformationDto = new TenantInformationDto
                    {
                        IsLiveChatWorkActive = tenant.IsLiveChatWorkActive,
                        LiveChatWorkingHours = liveChatWorkModel,
                        ConfirmRequestText = tenant.ConfirmRequestText,
                        RejectRequestText = tenant.RejectRequestText,

                        IsReplyAfterHumanHandOver = tenant.IsReplyAfterHumanHandOver,
                    };
                    break;
                }

                case EunmFlagSetting.BotCaption:
                {
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());


                    List<CaptionDto> captionDtos = new List<CaptionDto>();

                    var listCaption = _captionRepository.GetAll();

                    foreach (var item in listCaption)
                    {


                        captionDtos.Add(new CaptionDto
                        {
                            Text= item.Text,
                            Id= item.Id,
                            TenantId= item.TenantId,
                            LanguageBotId= item.LanguageBotId,
                            TextResourceId= item.TextResourceId,
                                HeaderText=item.HeaderText

                        });
                    }
                    tenantInformationDto = new TenantInformationDto
                    {
                        Captions=captionDtos

                    };

                    break;
                }

                case EunmFlagSetting.Condations:
                {
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());

                    var listorderOffer = _orderOfferRepository.GetAll();
                       
                    List<OrderOfferDto> orderOfferDtos = new List<OrderOfferDto>();
                    foreach (var item in listorderOffer)
                    {


                        orderOfferDtos.Add(new OrderOfferDto
                        {
                            Area = item.Area,
                            Cities = item.Cities,
                            FeesEnd = item.FeesEnd,
                            FeesStart = item.FeesStart,
                            isAvailable = item.isAvailable,
                            isPersentageDiscount=item.isPersentageDiscount,
                            NewFees = item.NewFees,

                            OrderOfferDateEnd = item.OrderOfferDateEnd,
                            OrderOfferDateStart = item.OrderOfferDateStart,


                            OrderOfferEndS = item.OrderOfferEnd.ToString("HH:mm"),
                            OrderOfferStartS = item.OrderOfferStart.ToString("HH:mm"),
                            Id = item.Id,
                            BranchesIds = item.BranchesIds,
                            BranchesName = item.BranchesName,
                            isBranchDiscount=item.isBranchDiscount,

                        });
                    }
                    tenantInformationDto = new TenantInformationDto
                    {
                        OrderOffers=orderOfferDtos,
                        isOrderOffer=tenant.isOrderOffer

                    };
                    break;
                }

                case EunmFlagSetting.ActiveCondations:
                {
                    break;
                }

                case EunmFlagSetting.Ringtone:
                {
                    var x = await _tenantAppService.GetTenantForEditPhone((int)AbpSession.GetTenantId());
                    tenantInformationDto = new TenantInformationDto
                    {
                        IsBellOn = x.IsBellOn,
                        IsBellContinues = x.IsBellContinues

                    };

                    break;
                }
                case EunmFlagSetting.Booking:
                {
                    var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                    TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());


                    tenantInformationDto = new TenantInformationDto
                    {
                        BookingCapacity = tenant.BookingCapacity,
                        ReminderBookingHour = tenant.ReminderBookingHour,
                        UnAvailableBookingDates = tenant.UnAvailableBookingDates,
                    };
                    break;
                }
                case EunmFlagSetting.Loyality:
                    {
                      //  var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
                       // TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());

                        var loyaltyModel = _loyaltyAppService.GetAll();
                        tenantInformationDto = new TenantInformationDto
                        {
                            loyaltyModel = loyaltyModel,
                             IsLoyalityPoint =loyaltyModel.IsLoyalityPoint
                        };

                       
                        break;
                    }
            }

      
            var settings = new TenantSettingsEditDto
            {
                TenantInformation= tenantInformationDto
            };

            if (!_multiTenancyConfig.IsEnabled || Clock.SupportsMultipleTimezone)
            {
                settings.General = await GetGeneralSettingsAsync();
            }

            if (_ldapModuleConfig.IsEnabled)
            {
                settings.Ldap = await GetLdapSettingsAsync();
            }
            else
            {
                settings.Ldap = new LdapSettingsEditDto { IsModuleEnabled = false };
            }

            return settings;
        }
        private async Task<LdapSettingsEditDto> GetLdapSettingsAsync()
        {
            return new LdapSettingsEditDto
            {
                IsModuleEnabled = true,
                IsEnabled = await SettingManager.GetSettingValueForTenantAsync<bool>(LdapSettingNames.IsEnabled, AbpSession.GetTenantId()),
                Domain = await SettingManager.GetSettingValueForTenantAsync(LdapSettingNames.Domain, AbpSession.GetTenantId()),
                UserName = await SettingManager.GetSettingValueForTenantAsync(LdapSettingNames.UserName, AbpSession.GetTenantId()),
                Password = await SettingManager.GetSettingValueForTenantAsync(LdapSettingNames.Password, AbpSession.GetTenantId()),
            };
        }
        private async Task<TenantInformationDto> GetTenantInformation()
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);
            //try
            //{
            //    TenantModel tenant333 = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId);
            //}
            //catch (Exception ex)
            //{


            //}
            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.TenantId);

            if (tenant.LiveChatWorkingHours == null)
                tenant.LiveChatWorkingHours = new MultiTenancy.WorkModel();
            
            if (tenant.workModel == null)
                tenant.workModel = new MultiTenancy.WorkModel();

            List<CaptionDto> captionDtos = new List<CaptionDto>();

            var listCaption = _captionRepository.GetAll();

            foreach (var item in listCaption)
            {
                captionDtos.Add(new CaptionDto
                {
                    Text= item.Text,
                    Id= item.Id,
                    TenantId= item.TenantId,
                    LanguageBotId= item.LanguageBotId,
                    TextResourceId= item.TextResourceId,
                    HeaderText=item.HeaderText
                });
            }

            List<OrderOfferDto> orderOfferDtos = new List<OrderOfferDto>();
            var listorderOffer = _orderOfferRepository.GetAll();
            foreach (var item in listorderOffer)
            {
                orderOfferDtos.Add(new OrderOfferDto
                {
                    Area = item.Area,
                    Cities = item.Cities,
                    FeesEnd = item.FeesEnd,
                    FeesStart = item.FeesStart,
                    isAvailable = item.isAvailable,
                    isPersentageDiscount=item.isPersentageDiscount,
                    NewFees = item.NewFees,
                    //SelectetDate = new OrderOfferDate(item.OrderOfferDateStart, item.OrderOfferDateEnd),

                    OrderOfferDateEnd = item.OrderOfferDateEnd,
                    OrderOfferDateStart = item.OrderOfferDateStart,


                    OrderOfferEndS = item.OrderOfferEnd.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    OrderOfferStartS = item.OrderOfferStart.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    Id = item.Id,
                    BranchesIds = item.BranchesIds,
                    BranchesName = item.BranchesName,
                    isBranchDiscount=item.isBranchDiscount,

                });
            }

            var loyaltyModel = _loyaltyAppService.GetAll();
            TenantInformationDto result = new TenantInformationDto();



            Dto.WorkModel workModel = new Dto.WorkModel();

            workModel = PrepareGetWorkModel(tenant.workModel);
            
            
            result =  new TenantInformationDto
            {
                loyaltyModel = loyaltyModel,
                CancelTime = tenant.CancelTime,
                IsCancelOrder = tenant.IsCancelOrder,
                TaxValue = tenant.TaxValue,
                IsTaxOrder = tenant.IsTaxOrder,
    
                IsBotActive = tenant.IsBotActive,
                MassageIfBotNotActive = tenant.MassageIfBotNotActive,
                IsWorkActive = tenant.IsWorkActive,
                IsBellContinues = tenant.IsBellContinues,
                IsBellOn = tenant.IsBellOn,
                IsBundleActive = tenant.IsBundleActive,
                DeliveryCostTypeId = tenant.DeliveryCostTypeId,
                TenantId = tenant.TenantId.Value,
                //WorkText = tenant.WorkText,
                //StartDate = tenant.StartDate.ToString("HH:mm"),
                //EndDate = tenant.EndDate.ToString("HH:mm"),

                workModel = workModel,
                IsLiveChatWorkActive = tenant.IsLiveChatWorkActive,

                LiveChatWorkingHours = new Dto.WorkModel
                {
                    EndDateFri = tenant.LiveChatWorkingHours.EndDateFri.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    EndDateMon = tenant.LiveChatWorkingHours.EndDateMon.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    EndDateSat = tenant.LiveChatWorkingHours.EndDateSat.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    EndDateSun = tenant.LiveChatWorkingHours.EndDateSun.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    EndDateThurs = tenant.LiveChatWorkingHours.EndDateThurs.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    EndDateTues = tenant.LiveChatWorkingHours.EndDateTues.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    EndDateWed = tenant.LiveChatWorkingHours.EndDateWed.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),

                    IsWorkActiveFri = tenant.LiveChatWorkingHours.IsWorkActiveFri,
                    IsWorkActiveMon = tenant.LiveChatWorkingHours.IsWorkActiveMon,
                    IsWorkActiveSat = tenant.LiveChatWorkingHours.IsWorkActiveSat,
                    IsWorkActiveSun = tenant.LiveChatWorkingHours.IsWorkActiveSun,
                    IsWorkActiveThurs = tenant.LiveChatWorkingHours.IsWorkActiveThurs,
                    IsWorkActiveTues = tenant.LiveChatWorkingHours.IsWorkActiveTues,
                    IsWorkActiveWed = tenant.LiveChatWorkingHours.IsWorkActiveWed,

                    StartDateFri = tenant.LiveChatWorkingHours.StartDateFri.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    StartDateMon = tenant.LiveChatWorkingHours.StartDateMon.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    StartDateSat = tenant.LiveChatWorkingHours.StartDateSat.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    StartDateSun = tenant.LiveChatWorkingHours.StartDateSun.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    StartDateThurs = tenant.LiveChatWorkingHours.StartDateThurs.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    StartDateTues = tenant.LiveChatWorkingHours.StartDateTues.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),
                    StartDateWed = tenant.LiveChatWorkingHours.StartDateWed.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm"),

                    WorkTextFri = tenant.LiveChatWorkingHours.WorkTextFri,
                    WorkTextMon = tenant.LiveChatWorkingHours.WorkTextMon,
                    WorkTextSat = tenant.LiveChatWorkingHours.WorkTextSat,
                    WorkTextSun = tenant.LiveChatWorkingHours.WorkTextSun,
                    WorkTextThurs = tenant.LiveChatWorkingHours.WorkTextThurs,
                    WorkTextTues = tenant.LiveChatWorkingHours.WorkTextTues,
                    WorkTextWed = tenant.LiveChatWorkingHours.WorkTextWed

                },

                IsEvaluation =tenant.IsEvaluation,
                EvaluationText=tenant.EvaluationText,
                EvaluationTime=tenant.EvaluationTime,

                IsLoyalityPoint =loyaltyModel.IsLoyalityPoint,
                Points = tenant.Points,
                isOrderOffer =tenant.isOrderOffer,
                Captions= captionDtos,
                OrderOffers= orderOfferDtos,

                ReminderBookingHour = tenant.ReminderBookingHour,
                IsReplyAfterHumanHandOver = tenant.IsReplyAfterHumanHandOver,
                BookingCapacity = tenant.BookingCapacity,
                ConfirmRequestText= tenant.ConfirmRequestText,
                RejectRequestText= tenant.RejectRequestText,
                UnAvailableBookingDates = tenant.UnAvailableBookingDates,
                MenuReminderMessage =tenant.MenuReminderMessage,
                IsActiveMenuReminder = tenant.IsActiveMenuReminder,
                TimeReminder = tenant.TimeReminder,
                IsMenuLinkFirst = tenant.IsMenuLinkFirst,
                IsInquiry = tenant.IsInquiry,
                IsPreOrder = tenant.IsPreOrder,
                IsPickup = tenant.IsPickup,
                IsDelivery = tenant.IsDelivery,
                IsBotLanguageEn = tenant.IsBotLanguageEn,
                IsBotLanguageAr = tenant.IsBotLanguageAr,
                IsSelectPaymentMethod = tenant.IsSelectPaymentMethod,
            };
            return result;
        }
        private async Task<TenantEmailSettingsEditDto> GetEmailSettingsAsync()
        {
            var useHostDefaultEmailSettings = await SettingManager.GetSettingValueForTenantAsync<bool>(AppSettings.Email.UseHostDefaultEmailSettings, AbpSession.GetTenantId());

            if (useHostDefaultEmailSettings)
            {
                return new TenantEmailSettingsEditDto
                {
                    UseHostDefaultEmailSettings = true
                };
            }

            var smtpPassword = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Password, AbpSession.GetTenantId());

            return new TenantEmailSettingsEditDto
            {
                UseHostDefaultEmailSettings = false,
                DefaultFromAddress = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.DefaultFromAddress, AbpSession.GetTenantId()),
                DefaultFromDisplayName = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.DefaultFromDisplayName, AbpSession.GetTenantId()),
                SmtpHost = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Host, AbpSession.GetTenantId()),
                SmtpPort = await SettingManager.GetSettingValueForTenantAsync<int>(EmailSettingNames.Smtp.Port, AbpSession.GetTenantId()),
                SmtpUserName = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.UserName, AbpSession.GetTenantId()),
                SmtpPassword = SimpleStringCipher.Instance.Decrypt(smtpPassword),
                SmtpDomain = await SettingManager.GetSettingValueForTenantAsync(EmailSettingNames.Smtp.Domain, AbpSession.GetTenantId()),
                SmtpEnableSsl = await SettingManager.GetSettingValueForTenantAsync<bool>(EmailSettingNames.Smtp.EnableSsl, AbpSession.GetTenantId()),
                SmtpUseDefaultCredentials = await SettingManager.GetSettingValueForTenantAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials, AbpSession.GetTenantId())
            };
        }
        private async Task<GeneralSettingsEditDto> GetGeneralSettingsAsync()
        {
            var settings = new GeneralSettingsEditDto();

            if (Clock.SupportsMultipleTimezone)
            {
                var timezone = await SettingManager.GetSettingValueForTenantAsync(TimingSettingNames.TimeZone, AbpSession.GetTenantId());

                settings.Timezone = timezone;
                settings.TimezoneForComparison = timezone;
            }

            var defaultTimeZoneId = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Tenant, AbpSession.TenantId);

            if (settings.Timezone == defaultTimeZoneId)
            {
                settings.Timezone = string.Empty;
            }

            return settings;
        }
        private async Task<TenantUserManagementSettingsEditDto> GetUserManagementSettingsAsync()
        {
            return new TenantUserManagementSettingsEditDto
            {
                AllowSelfRegistration = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.AllowSelfRegistration),
                IsNewRegisteredUserActiveByDefault = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault),
                IsEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin),
                UseCaptchaOnRegistration = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration),
                UseCaptchaOnLogin = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.UseCaptchaOnLogin),
                IsCookieConsentEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsCookieConsentEnabled),
                IsQuickThemeSelectEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsQuickThemeSelectEnabled),
                AllowUsingGravatarProfilePicture = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.AllowUsingGravatarProfilePicture),
                SessionTimeOutSettings = new SessionTimeOutSettingsEditDto()
                {
                    IsEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.SessionTimeOut.IsEnabled),
                    TimeOutSecond = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.SessionTimeOut.TimeOutSecond),
                    ShowTimeOutNotificationSecond = await SettingManager.GetSettingValueAsync<int>(AppSettings.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond),
                    ShowLockScreenWhenTimedOut = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut)
                }
            };
        }
        private async Task<SecuritySettingsEditDto> GetSecuritySettingsAsync()
        {
            var passwordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit),
                RequireLowercase = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await SettingManager.GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength)
            };

            var defaultPasswordComplexitySetting = new PasswordComplexitySetting
            {
                RequireDigit = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit),
                RequireLowercase = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase),
                RequireNonAlphanumeric = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric),
                RequireUppercase = await SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase),
                RequiredLength = await SettingManager.GetSettingValueForApplicationAsync<int>(AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength)
            };

            return new SecuritySettingsEditDto
            {
                UseDefaultPasswordComplexitySettings = passwordComplexitySetting.Equals(defaultPasswordComplexitySetting),
                PasswordComplexity = passwordComplexitySetting,
                DefaultPasswordComplexity = defaultPasswordComplexitySetting,
                UserLockOut = await GetUserLockOutSettingsAsync(),
                TwoFactorLogin = await GetTwoFactorLoginSettingsAsync(),
                AllowOneConcurrentLoginPerUser = await GetOneConcurrentLoginPerUserSetting()
            };
        }
        private async Task<TenantBillingSettingsEditDto> GetBillingSettingsAsync()
        {
            return new TenantBillingSettingsEditDto()
            {
                LegalName = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingLegalName),
                Address = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingAddress),
                TaxVatNo = await SettingManager.GetSettingValueAsync(AppSettings.TenantManagement.BillingTaxVatNo)
            };
        }
        private async Task<TenantOtherSettingsEditDto> GetOtherSettingsAsync()
        {
            return new TenantOtherSettingsEditDto()
            {
                IsQuickThemeSelectEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.IsQuickThemeSelectEnabled)
            };
        }
        private async Task<UserLockOutSettingsEditDto> GetUserLockOutSettingsAsync()
        {
            return new UserLockOutSettingsEditDto
            {
                IsEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled),
                MaxFailedAccessAttemptsBeforeLockout = await SettingManager.GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout),
                DefaultAccountLockoutSeconds = await SettingManager.GetSettingValueAsync<int>(AbpZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds)
            };
        }
        private Task<bool> IsTwoFactorLoginEnabledForApplicationAsync()
        {
            return SettingManager.GetSettingValueForApplicationAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled);
        }
        private async Task<TwoFactorLoginSettingsEditDto> GetTwoFactorLoginSettingsAsync()
        {
            var settings = new TwoFactorLoginSettingsEditDto
            {
                IsEnabledForApplication = await IsTwoFactorLoginEnabledForApplicationAsync()
            };

            if (_multiTenancyConfig.IsEnabled && !settings.IsEnabledForApplication)
            {
                return settings;
            }

            settings.IsEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled);
            settings.IsRememberBrowserEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled);

            if (!_multiTenancyConfig.IsEnabled)
            {
                settings.IsEmailProviderEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEmailProviderEnabled);
                settings.IsSmsProviderEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsSmsProviderEnabled);
                settings.IsGoogleAuthenticatorEnabled = await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.TwoFactorLogin.IsGoogleAuthenticatorEnabled);
            }

            return settings;
        }
        private async Task<bool> GetOneConcurrentLoginPerUserSetting()
        {
            return await SettingManager.GetSettingValueAsync<bool>(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser);
        }
        private async Task<ExternalLoginProviderSettingsEditDto> GetExternalLoginProviderSettings()
        {
            var facebookSettings = await SettingManager.GetSettingValueForTenantAsync(AppSettings.ExternalLoginProvider.Tenant.Facebook, AbpSession.GetTenantId());
            var googleSettings = await SettingManager.GetSettingValueForTenantAsync(AppSettings.ExternalLoginProvider.Tenant.Google, AbpSession.GetTenantId());
            var twitterSettings = await SettingManager.GetSettingValueForTenantAsync(AppSettings.ExternalLoginProvider.Tenant.Twitter, AbpSession.GetTenantId());
            var microsoftSettings = await SettingManager.GetSettingValueForTenantAsync(AppSettings.ExternalLoginProvider.Tenant.Microsoft, AbpSession.GetTenantId());
            
            var openIdConnectSettings = await SettingManager.GetSettingValueForTenantAsync(AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect, AbpSession.GetTenantId());
            var openIdConnectMappedClaims = await SettingManager.GetSettingValueAsync(AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims);
            
            var wsFederationSettings = await SettingManager.GetSettingValueForTenantAsync(AppSettings.ExternalLoginProvider.Tenant.WsFederation, AbpSession.GetTenantId());
            var wsFederationMappedClaims = await SettingManager.GetSettingValueAsync(AppSettings.ExternalLoginProvider.WsFederationMappedClaims);

            return new ExternalLoginProviderSettingsEditDto
            {
                Facebook = facebookSettings.IsNullOrWhiteSpace()
                    ? new FacebookExternalLoginProviderSettings()
                    : facebookSettings.FromJsonString<FacebookExternalLoginProviderSettings>(),
                Google = googleSettings.IsNullOrWhiteSpace()
                    ? new GoogleExternalLoginProviderSettings()
                    : googleSettings.FromJsonString<GoogleExternalLoginProviderSettings>(),
                Twitter = twitterSettings.IsNullOrWhiteSpace()
                    ? new TwitterExternalLoginProviderSettings()
                    : twitterSettings.FromJsonString<TwitterExternalLoginProviderSettings>(),
                Microsoft = microsoftSettings.IsNullOrWhiteSpace()
                    ? new MicrosoftExternalLoginProviderSettings()
                    : microsoftSettings.FromJsonString<MicrosoftExternalLoginProviderSettings>(),
                
                OpenIdConnect = openIdConnectSettings.IsNullOrWhiteSpace()
                    ? new OpenIdConnectExternalLoginProviderSettings()
                    : openIdConnectSettings.FromJsonString<OpenIdConnectExternalLoginProviderSettings>(),
                OpenIdConnectClaimsMapping = openIdConnectMappedClaims.IsNullOrWhiteSpace()
                    ? new List<JsonClaimMapDto>() 
                    : openIdConnectMappedClaims.FromJsonString<List<JsonClaimMapDto>>(),
                
                WsFederation = wsFederationSettings.IsNullOrWhiteSpace()
                    ? new WsFederationExternalLoginProviderSettings()
                    : wsFederationSettings.FromJsonString<WsFederationExternalLoginProviderSettings>(),
                WsFederationClaimsMapping = wsFederationMappedClaims.IsNullOrWhiteSpace()
                    ? new List<JsonClaimMapDto>() 
                    : wsFederationMappedClaims.FromJsonString<List<JsonClaimMapDto>>()
            };
        }



        #endregion






        #region Update Settings

        [HttpPost]
        public async Task UpdateGeneralSettingsMobile(TenantInformationDto input)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());
         
            try {
                switch ((EunmFlagSetting)input.flag)
                {

                    case EunmFlagSetting.GeneralSettings:
                        {

                            tenant.CancelTime = input.CancelTime;
                            tenant.IsCancelOrder = input.IsCancelOrder;
                            tenant.TaxValue = tenant.TaxValue;
                            tenant.IsTaxOrder = tenant.IsTaxOrder;
                            tenant.IsBotActive = input.IsBotActive;
                            tenant.MassageIfBotNotActive = input.MassageIfBotNotActive;
                            tenant.IsEvaluation = input.IsEvaluation;
                            tenant.EvaluationText = input.EvaluationText;
                            tenant.EvaluationTime = input.EvaluationTime;
                            tenant.DeliveryCostTypeId = input.DeliveryCostTypeId;
                            tenant.ReminderBookingHour = input.ReminderBookingHour;
                            tenant.ConfirmRequestText= input.ConfirmRequestText;
                            tenant.RejectRequestText= input.RejectRequestText;
                            tenant.IsReplyAfterHumanHandOver= input.IsReplyAfterHumanHandOver;
                            tenant.UnAvailableBookingDates = input.UnAvailableBookingDates;
                            tenant.BookingCapacity= input.BookingCapacity;
                            tenant.MenuReminderMessage = input.MenuReminderMessage;
                            tenant.IsActiveMenuReminder = input.IsActiveMenuReminder;
                            tenant.TimeReminder = input.TimeReminder;
                            tenant.IsBotLanguageAr= input.IsBotLanguageAr;
                            tenant.IsBotLanguageEn = input.IsBotLanguageEn;
                            tenant.IsInquiry = input.IsInquiry;
                            tenant.IsDelivery = input.IsDelivery;
                            tenant.IsPickup = input.IsPickup;
                            tenant.IsPreOrder = input.IsPreOrder;
                            tenant.IsMenuLinkFirst = input.IsMenuLinkFirst;
                            tenant.IsSelectPaymentMethod = input.IsSelectPaymentMethod;
                            updateTenantDB(tenant);

                            await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                            break;
                        }
                    case EunmFlagSetting.WorkingHour:
                        {

                            tenant.IsWorkActive = input.IsWorkActive;
                            tenant.IsBellContinues = input.IsBellContinues;
                            tenant.IsBellOn = input.IsBellOn;
                            tenant.workModel = new MultiTenancy.WorkModel();
                            tenant.workModel = PrepareUpdateWorkModelMobile(input.workModel);

                            await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                            break;
                        }
                    case EunmFlagSetting.LiveChatWorkingHour:
                        {
                            tenant.IsLiveChatWorkActive = input.IsLiveChatWorkActive;
                            tenant.LiveChatWorkingHours = new MultiTenancy.WorkModel();
                            tenant.LiveChatWorkingHours = PrepareUpdateWorkModelMobile(input.LiveChatWorkingHours);
                            tenant.ConfirmRequestText = input.ConfirmRequestText;
                            tenant.RejectRequestText = input.RejectRequestText;
                            tenant.IsReplyAfterHumanHandOver = input.IsReplyAfterHumanHandOver;
                            tenant.IsReplyAfterHumanHandOver=input.IsReplyAfterHumanHandOver;
                            updateRequestMessageDB(tenant);
                            await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                            break;
                        }
                    case EunmFlagSetting.BotCaption:
                        {
                            _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + input.OneCaption.TenantId.ToString());
                            Caption caption = new Caption
                            {
                                Text = input.OneCaption.Text.Replace(" \n", "\r\n"),
                                Id = input.OneCaption.Id,
                                TenantId = input.OneCaption.TenantId,
                                LanguageBotId = input.OneCaption.LanguageBotId,
                                TextResourceId = input.OneCaption.TextResourceId,
                                HeaderText = input.OneCaption.HeaderText
                            };
                            await _captionRepository.UpdateAsync(caption);

                            break;
                        }
                    case EunmFlagSetting.Condations:
                        {
                            OrderOffers.OrderOffer orderOffer = new OrderOffers.OrderOffer
                            {
                                Id = input.OneOrderOffern.Id,
                                FeesEnd = input.OneOrderOffern.FeesEnd,
                                FeesStart = input.OneOrderOffern.FeesStart,
                                Area = input.OneOrderOffern.Area,
                                Cities = input.OneOrderOffern.Cities,
                                NewFees = input.OneOrderOffern.NewFees,
                                isAvailable = input.OneOrderOffern.isAvailable,
                                OrderOfferDateEnd = input.OneOrderOffern.OrderOfferDateEnd,
                                OrderOfferDateStart = input.OneOrderOffern.OrderOfferDateStart,
                                isPersentageDiscount = input.OneOrderOffern.isPersentageDiscount,
                                OrderOfferEnd = input.OneOrderOffern.OrderOfferEnd.AddHours(AppSettingsModel.DivHour),
                                OrderOfferStart = input.OneOrderOffern.OrderOfferStart.AddHours(AppSettingsModel.DivHour),
                                TenantId = input.TenantId,
                                BranchesIds = input.OneOrderOffern.BranchesIds,
                                BranchesName = input.OneOrderOffern.BranchesName,
                                isBranchDiscount = input.OneOrderOffern.isBranchDiscount,
                            };

                            var x= _orderOfferRepository.InsertOrUpdate(orderOffer);

                            tenant.isOrderOffer = input.isOrderOffer;
                            await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                            break;
                        }
                    case EunmFlagSetting.ActiveCondations:
                        {
                            tenant.isOrderOffer = input.isOrderOffer;
                            await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                            break;
                        }
                    case EunmFlagSetting.Ringtone:
                        {
                            EntityDto inputd = new EntityDto { Id = AbpSession.GetTenantId() };
                            var x = await _tenantAppService.GetTenantForEditPhone((int)AbpSession.GetTenantId());
                            x.IsBellOn = input.IsBellOn;
                            x.IsBellContinues = input.IsBellContinues;
                            updateTenantDB(tenant);
                            await _tenantAppService.UpdateTenant(x);
                            break;
                        }
                    case EunmFlagSetting.Booking:
                        {
                            tenant.BookingCapacity = input.BookingCapacity;
                            tenant.ReminderBookingHour = input.ReminderBookingHour;
                            tenant.UnAvailableBookingDates = input.UnAvailableBookingDates;
                            updateTenantDB(tenant);
                            await itemsCollection.UpdateItemAsync(tenant._self, tenant);
                            break;
                        }
                }
                _cacheManager.GetCache("CacheTenant").Remove(tenant.D360Key.ToString());
                await SendToRestaurantsBot(AbpSession.GetTenantId());
            }
            catch (Exception ex) 
            {
            
            
            }
           
        }
        public async Task UpdateAllSettings(TenantSettingsEditDto input)
        {


             await UpdateTenantInformationAsync(input.TenantInformation);
            //Time Zone
            if (Clock.SupportsMultipleTimezone)
            {
                if (input.General.Timezone.IsNullOrEmpty())
                {
                    var defaultValue = await _timeZoneService.GetDefaultTimezoneAsync(SettingScopes.Tenant, AbpSession.TenantId);
                    await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), TimingSettingNames.TimeZone, defaultValue);
                }
                else
                {
                    await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), TimingSettingNames.TimeZone, input.General.Timezone);
                }
            }

            if (!_multiTenancyConfig.IsEnabled)
            {
                await UpdateOtherSettingsAsync(input.OtherSettings);

                input.ValidateHostSettings();
            }

            if (_ldapModuleConfig.IsEnabled)
            {
                await UpdateLdapSettingsAsync(input.Ldap);
            }
        }
        public async Task UpdateCaptionAsync(List<CaptionDto> captions)
        {
            _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + AbpSession.TenantId.Value.ToString());
            foreach (var item in captions)
            {
                Caption caption = new Caption
                {
                    Id = item.Id,
                    LanguageBotId = item.LanguageBotId,
                    TenantId = item.TenantId,
                    Text = item.Text.Replace(" \n", "\r\n"),
                    TextResourceId = item.TextResourceId,
                    HeaderText = item.HeaderText


                };

                await _captionRepository.UpdateAsync(caption);
            }
        }
        public bool UpdateCaptionById(CaptionDto captions)
        {
            return updateCaptionById(captions);
        }
        private bool updateCaptionById(CaptionDto captions)
        {
            try
            {
                _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + AbpSession.TenantId.Value.ToString());
                var SP_Name = Constants.Caption.SP_CaptionUpdateById;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                     new System.Data.SqlClient.SqlParameter("@CaptionId",captions.Id)
                    ,new System.Data.SqlClient.SqlParameter("@TenantId",captions.TenantId)
                    ,new System.Data.SqlClient.SqlParameter("@Text",captions.Text)
                };
                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Int,
                    ParameterName = "@CaptionIdOut",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);
                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);

                if (OutputParameter.Value.ToString() != "")
                {
                    int OutId = (int)OutputParameter.Value;
                    if (OutId == captions.Id)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public CaptionDto GetCaptionById(int captionId)
        {
            return getcaptionById(captionId);
        }  
        private CaptionDto getcaptionById(int captionId) 
        {
            try
            {
                _cacheManager.GetCache("CacheTenant_CaptionStps").Remove("Step_" + AbpSession.TenantId.Value.ToString());
                
                CaptionDto captionDto = new CaptionDto();
                int TenantId = AbpSession.TenantId.Value;

                var SP_Name = Constants.Caption.SP_CaptionGetById;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@CaptionId",captionId),
                    new System.Data.SqlClient.SqlParameter("@TenantId",TenantId),
                };
                captionDto = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.ConvertCaptionDto, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                return captionDto;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task UpdateOrderOffersAsync(TenantInformationDto input)
        {

            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());
            tenant.isOrderOffer = input.isOrderOffer;
            await itemsCollection.UpdateItemAsync(tenant._self, tenant);
            if (input.isOrderOffer)
            {

                foreach (var item in input.OrderOffers)
                {
                    OrderOffers.OrderOffer orderOffer = new OrderOffers.OrderOffer
                    {
                        Id = item.Id,
                        FeesEnd = item.FeesEnd,
                        FeesStart = item.FeesStart,
                        Area = item.Area,
                        Cities = item.Cities,
                        NewFees = item.NewFees,
                        isAvailable = item.isAvailable,
                        OrderOfferDateEnd = item.OrderOfferDateEnd,
                        OrderOfferDateStart = item.OrderOfferDateStart,
                        isPersentageDiscount = item.isPersentageDiscount,
                        OrderOfferEnd = item.OrderOfferEnd,//.AddHours(AppSettingsModel.AddHour),
                        OrderOfferStart = item.OrderOfferStart,//.AddHours(AppSettingsModel.AddHour),
                        TenantId = AbpSession.GetTenantId(),
                        BranchesIds = item.BranchesIds,
                        BranchesName = item.BranchesName,
                        isBranchDiscount = item.isBranchDiscount,


                    };

                    await _orderOfferRepository.UpdateAsync(orderOffer);
                }
            }
        }



        private async Task UpdateTenantInformationAsync(TenantInformationDto input)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());


            tenant.IsBotActive = input.IsBotActive;
            tenant.MassageIfBotNotActive = input.MassageIfBotNotActive;
            tenant.IsCancelOrder = input.IsCancelOrder;
            tenant.IsTaxOrder = input.IsTaxOrder;


            try
            {
                tenant.CancelTime = input.CancelTime;
                tenant.TaxValue = input.TaxValue;

            }
            catch (Exception )
            {

            }
           
            tenant.IsWorkActive = input.IsWorkActive;
            tenant.IsBellOn = input.IsBellOn;
            tenant.IsBellContinues = input.IsBellContinues;

            tenant.workModel = PrepareUpdateWorkModel(input.workModel);

            tenant.IsLiveChatWorkActive = input.IsLiveChatWorkActive;
            tenant.LiveChatWorkingHours = new MultiTenancy.WorkModel
            {
                EndDateFri = input.LiveChatWorkingHours.EndDateFri,//.AddHours(AppSettingsModel.AddHour),
                EndDateMon = input.LiveChatWorkingHours.EndDateMon,//.AddHours(AppSettingsModel.AddHour),
                EndDateSat = input.LiveChatWorkingHours.EndDateSat,//.AddHours(AppSettingsModel.AddHour),
                EndDateSun = input.LiveChatWorkingHours.EndDateSun,//.AddHours(AppSettingsModel.AddHour),
                EndDateThurs = input.LiveChatWorkingHours.EndDateThurs,//.AddHours(AppSettingsModel.AddHour),
                EndDateTues = input.LiveChatWorkingHours.EndDateTues,//.AddHours(AppSettingsModel.AddHour),
                EndDateWed = input.LiveChatWorkingHours.EndDateWed,//.AddHours(AppSettingsModel.AddHour),

                IsWorkActiveFri = input.LiveChatWorkingHours.IsWorkActiveFri,
                IsWorkActiveMon = input.LiveChatWorkingHours.IsWorkActiveMon,
                IsWorkActiveSat = input.LiveChatWorkingHours.IsWorkActiveSat,
                IsWorkActiveSun = input.LiveChatWorkingHours.IsWorkActiveSun,
                IsWorkActiveThurs = input.LiveChatWorkingHours.IsWorkActiveThurs,
                IsWorkActiveTues = input.LiveChatWorkingHours.IsWorkActiveTues,
                IsWorkActiveWed = input.LiveChatWorkingHours.IsWorkActiveWed,

                StartDateFri = input.LiveChatWorkingHours.StartDateFri,//.AddHours(AppSettingsModel.AddHour),
                StartDateMon = input.LiveChatWorkingHours.StartDateMon,//.AddHours(AppSettingsModel.AddHour),
                StartDateSat = input.LiveChatWorkingHours.StartDateSat,//.AddHours(AppSettingsModel.AddHour),
                StartDateSun = input.LiveChatWorkingHours.StartDateSun,//.AddHours(AppSettingsModel.AddHour),
                StartDateThurs = input.LiveChatWorkingHours.StartDateThurs,//.AddHours(AppSettingsModel.AddHour),
                StartDateTues = input.LiveChatWorkingHours.StartDateTues,//.AddHours(AppSettingsModel.AddHour),
                StartDateWed = input.LiveChatWorkingHours.StartDateWed,//.AddHours(AppSettingsModel.AddHour),

                WorkTextFri = input.LiveChatWorkingHours.WorkTextFri,
                WorkTextMon = input.LiveChatWorkingHours.WorkTextMon,
                WorkTextSat = input.LiveChatWorkingHours.WorkTextSat,
                WorkTextSun = input.LiveChatWorkingHours.WorkTextSun,
                WorkTextThurs = input.LiveChatWorkingHours.WorkTextThurs,
                WorkTextTues = input.LiveChatWorkingHours.WorkTextTues,
                WorkTextWed = input.LiveChatWorkingHours.WorkTextWed

            };


            tenant.IsEvaluation = input.IsEvaluation;
            tenant.EvaluationText = input.EvaluationText;
            tenant.EvaluationTime = input.EvaluationTime;

            tenant.IsLoyalityPoint = input.IsLoyalityPoint;
            tenant.Points = input.Points;

            tenant.isOrderOffer = input.isOrderOffer;
            tenant.DeliveryCostTypeId = input.DeliveryCostTypeId;

            tenant.BookingCapacity = input.BookingCapacity;
            tenant.ReminderBookingHour= input.ReminderBookingHour;
            tenant.UnAvailableBookingDates = input.UnAvailableBookingDates;
            
            tenant.RejectRequestText = input.RejectRequestText;
            tenant.ConfirmRequestText= input.ConfirmRequestText;
            tenant.IsReplyAfterHumanHandOver = input.IsReplyAfterHumanHandOver;

            tenant.MenuReminderMessage = input.MenuReminderMessage;
            tenant.IsActiveMenuReminder = input.IsActiveMenuReminder;
            tenant.TimeReminder = input.TimeReminder;

            tenant.IsBotLanguageAr = input.IsBotLanguageAr;
            tenant.IsBotLanguageEn = input.IsBotLanguageEn;
            tenant.IsInquiry = input.IsInquiry;
            tenant.IsPreOrder = input.IsPreOrder;
            tenant.IsPickup = input.IsPickup;
            tenant.IsDelivery = input.IsDelivery;
            tenant.IsMenuLinkFirst = input.IsMenuLinkFirst;
            tenant.IsSelectPaymentMethod = input.IsSelectPaymentMethod;

           
            updateTenantDB(tenant);
            await itemsCollection.UpdateItemAsync(tenant._self, tenant);



            foreach (var item in input.OrderOffers)
            {
                OrderOffers.OrderOffer orderOffer = new OrderOffers.OrderOffer
                {
                    Id = item.Id,
                    FeesEnd = item.FeesEnd,
                    FeesStart = item.FeesStart,
                    Area = item.Area,
                    Cities = item.Cities,
                    NewFees = item.NewFees,
                    isAvailable = item.isAvailable,
                    OrderOfferDateEnd = item.OrderOfferDateEnd,
                    OrderOfferDateStart = item.OrderOfferDateStart,
                    isPersentageDiscount = item.isPersentageDiscount,
                    OrderOfferEnd = item.OrderOfferEnd,//.AddHours(AppSettingsModel.AddHour),
                    OrderOfferStart = item.OrderOfferStart,//.AddHours(AppSettingsModel.AddHour),
                    TenantId = AbpSession.GetTenantId(),
                    BranchesIds = item.BranchesIds,
                    BranchesName = item.BranchesName,
                    isBranchDiscount = item.isBranchDiscount,


                };

                await _orderOfferRepository.UpdateAsync(orderOffer);
            }


            _cacheManager.GetCache("CacheTenant").Remove(tenant.D360Key.ToString());
            await SendToRestaurantsBot(AbpSession.GetTenantId());
        }
        private void updateTenantDB(TenantModel tenant)
        {
            try
            {
                var SP_Name = Constants.Tenant.SP_TenantUpdate;
                string tenantJson = JsonConvert.SerializeObject(tenant);
                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {

                    new System.Data.SqlClient.SqlParameter("@TenantId",tenant.TenantId),
                    new System.Data.SqlClient.SqlParameter("@TenantJSON",JsonConvert.SerializeObject(tenant)),
                };


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private void updateRequestMessageDB(TenantModel tenant)
        {
            try
            {
                var SP_Name = Constants.Tenant.SP_RequestMessagesUpdate;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter> {
                    new System.Data.SqlClient.SqlParameter("@tenantId",(int)tenant.TenantId),
                    new System.Data.SqlClient.SqlParameter("@confirmRequestText",tenant.ConfirmRequestText),
                    new System.Data.SqlClient.SqlParameter("@rejectRequestText",tenant.RejectRequestText),
                };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task DeleteOrderOffer(long id)
        {
            await _orderOfferRepository.DeleteAsync(id);
        }
        public async Task CreateOrEdit(CreateOrEditOrderOfferDto input)
        {
            if (input.Id == null)
            {
                await Create(input);
            }
            else
            {
                await Update(input);
            }
        }
        private async Task UpdateOtherSettingsAsync(TenantOtherSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.IsQuickThemeSelectEnabled,
                input.IsQuickThemeSelectEnabled.ToString().ToLowerInvariant()
            );
        }
        private async Task UpdateBillingSettingsAsync(TenantBillingSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AppSettings.TenantManagement.BillingLegalName, input.LegalName);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AppSettings.TenantManagement.BillingAddress, input.Address);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AppSettings.TenantManagement.BillingTaxVatNo, input.TaxVatNo);
        }
        private async Task UpdateLdapSettingsAsync(LdapSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), LdapSettingNames.IsEnabled, input.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), LdapSettingNames.Domain, input.Domain.IsNullOrWhiteSpace() ? null : input.Domain);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), LdapSettingNames.UserName, input.UserName.IsNullOrWhiteSpace() ? null : input.UserName);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), LdapSettingNames.Password, input.Password.IsNullOrWhiteSpace() ? null : input.Password);
        }
        private async Task UpdateEmailSettingsAsync(TenantEmailSettingsEditDto input)
        {
            if (_multiTenancyConfig.IsEnabled && !MessagingPortalConsts.AllowTenantsToChangeEmailSettings)
            {
                return;
            }

            var useHostDefaultEmailSettings = _multiTenancyConfig.IsEnabled && input.UseHostDefaultEmailSettings;

            if (useHostDefaultEmailSettings)
            {
                var smtpPassword = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Password);

                input = new TenantEmailSettingsEditDto
                {
                    UseHostDefaultEmailSettings = true,
                    DefaultFromAddress = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromAddress),
                    DefaultFromDisplayName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.DefaultFromDisplayName),
                    SmtpHost = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Host),
                    SmtpPort = await SettingManager.GetSettingValueForApplicationAsync<int>(EmailSettingNames.Smtp.Port),
                    SmtpUserName = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.UserName),
                    SmtpPassword = SimpleStringCipher.Instance.Decrypt(smtpPassword),
                    SmtpDomain = await SettingManager.GetSettingValueForApplicationAsync(EmailSettingNames.Smtp.Domain),
                    SmtpEnableSsl = await SettingManager.GetSettingValueForApplicationAsync<bool>(EmailSettingNames.Smtp.EnableSsl),
                    SmtpUseDefaultCredentials = await SettingManager.GetSettingValueForApplicationAsync<bool>(EmailSettingNames.Smtp.UseDefaultCredentials)
                };
            }

            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AppSettings.Email.UseHostDefaultEmailSettings, useHostDefaultEmailSettings.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.DefaultFromAddress, input.DefaultFromAddress);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.DefaultFromDisplayName, input.DefaultFromDisplayName);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.Smtp.Host, input.SmtpHost);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.Smtp.Port, input.SmtpPort.ToString(CultureInfo.InvariantCulture));
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.Smtp.UserName, input.SmtpUserName);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.Smtp.Password, SimpleStringCipher.Instance.Encrypt(input.SmtpPassword));
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.Smtp.Domain, input.SmtpDomain);
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.Smtp.EnableSsl, input.SmtpEnableSsl.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), EmailSettingNames.Smtp.UseDefaultCredentials, input.SmtpUseDefaultCredentials.ToString().ToLowerInvariant());
        }
        private async Task UpdateUserManagementSettingsAsync(TenantUserManagementSettingsEditDto settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.AllowSelfRegistration,
                settings.AllowSelfRegistration.ToString().ToLowerInvariant()
            );
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.IsNewRegisteredUserActiveByDefault,
                settings.IsNewRegisteredUserActiveByDefault.ToString().ToLowerInvariant()
            );
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                settings.IsEmailConfirmationRequiredForLogin.ToString().ToLowerInvariant()
            );
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.UseCaptchaOnRegistration,
                settings.UseCaptchaOnRegistration.ToString().ToLowerInvariant()
            );
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.UseCaptchaOnLogin,
                settings.UseCaptchaOnLogin.ToString().ToLowerInvariant()
            );
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.IsCookieConsentEnabled,
                settings.IsCookieConsentEnabled.ToString().ToLowerInvariant()
            );
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.AllowUsingGravatarProfilePicture,
                settings.AllowUsingGravatarProfilePicture.ToString().ToLowerInvariant()
            );
            
            await UpdateUserManagementSessionTimeOutSettingsAsync(settings.SessionTimeOutSettings);
        }
        private async Task UpdateUserManagementSessionTimeOutSettingsAsync(SessionTimeOutSettingsEditDto settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.SessionTimeOut.IsEnabled,
                settings.IsEnabled.ToString().ToLowerInvariant()
            );
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.SessionTimeOut.TimeOutSecond,
                settings.TimeOutSecond.ToString()
            );
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.SessionTimeOut.ShowTimeOutNotificationSecond,
                settings.ShowTimeOutNotificationSecond.ToString()
            );
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.UserManagement.SessionTimeOut.ShowLockScreenWhenTimedOut,
                settings.ShowLockScreenWhenTimedOut.ToString()
            );
        }
        private async Task UpdateSecuritySettingsAsync(SecuritySettingsEditDto settings)
        {
            if (settings.UseDefaultPasswordComplexitySettings)
            {
                await UpdatePasswordComplexitySettingsAsync(settings.DefaultPasswordComplexity);
            }
            else
            {
                await UpdatePasswordComplexitySettingsAsync(settings.PasswordComplexity);
            }

            await UpdateUserLockOutSettingsAsync(settings.UserLockOut);
            await UpdateTwoFactorLoginSettingsAsync(settings.TwoFactorLogin);
            await UpdateOneConcurrentLoginPerUserSettingAsync(settings.AllowOneConcurrentLoginPerUser);
        }
        private async Task UpdatePasswordComplexitySettingsAsync(PasswordComplexitySetting settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                settings.RequireDigit.ToString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                settings.RequireLowercase.ToString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                settings.RequireNonAlphanumeric.ToString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                settings.RequireUppercase.ToString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AbpZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                settings.RequiredLength.ToString()
            );
        }
        private async Task UpdateUserLockOutSettingsAsync(UserLockOutSettingsEditDto settings)
        {
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled, settings.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds, settings.DefaultAccountLockoutSeconds.ToString());
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout, settings.MaxFailedAccessAttemptsBeforeLockout.ToString());
        }
        private async Task UpdateTwoFactorLoginSettingsAsync(TwoFactorLoginSettingsEditDto settings)
        {
            if (_multiTenancyConfig.IsEnabled &&
                !await IsTwoFactorLoginEnabledForApplicationAsync()) //Two factor login can not be used by tenants if disabled by the host
            {
                return;
            }

            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled, settings.IsEnabled.ToString().ToLowerInvariant());
            await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled, settings.IsRememberBrowserEnabled.ToString().ToLowerInvariant());

            if (!_multiTenancyConfig.IsEnabled)
            {
                //These settings can only be changed by host, in a multitenant application.
                await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEmailProviderEnabled, settings.IsEmailProviderEnabled.ToString().ToLowerInvariant());
                await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsSmsProviderEnabled, settings.IsSmsProviderEnabled.ToString().ToLowerInvariant());
                await SettingManager.ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AppSettings.UserManagement.TwoFactorLogin.IsGoogleAuthenticatorEnabled, settings.IsGoogleAuthenticatorEnabled.ToString().ToLowerInvariant());
            }
        }
        private async Task UpdateOneConcurrentLoginPerUserSettingAsync(bool allowOneConcurrentLoginPerUser)
        {
            if (_multiTenancyConfig.IsEnabled)
            {
                return;
            }
            await SettingManager.ChangeSettingForApplicationAsync(AppSettings.UserManagement.AllowOneConcurrentLoginPerUser, allowOneConcurrentLoginPerUser.ToString());
        }
        private async Task UpdateExternalLoginSettingsAsync(ExternalLoginProviderSettingsEditDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.ExternalLoginProvider.Tenant.Facebook,
                input.Facebook == null || !input.Facebook.IsValid() ? "" : input.Facebook.ToJsonString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.ExternalLoginProvider.Tenant.Google,
                input.Google == null || !input.Google.IsValid() ? "" : input.Google.ToJsonString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.ExternalLoginProvider.Tenant.Twitter,
                input.Twitter == null || !input.Twitter.IsValid() ? "" : input.Twitter.ToJsonString()
            );

            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.ExternalLoginProvider.Tenant.Microsoft,
                input.Microsoft == null || !input.Microsoft.IsValid() ? "" : input.Microsoft.ToJsonString()
            );
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.ExternalLoginProvider.Tenant.OpenIdConnect,
                input.OpenIdConnect == null || !input.OpenIdConnect.IsValid() ? "" : input.OpenIdConnect.ToJsonString()
            );

            var openIdConnectMappedClaimsValue = "";
            if (input.OpenIdConnect == null || !input.OpenIdConnect.IsValid() || input.OpenIdConnectClaimsMapping.IsNullOrEmpty())
            {
                openIdConnectMappedClaimsValue = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims);//set default value
            }
            else
            {
                openIdConnectMappedClaimsValue = input.OpenIdConnectClaimsMapping.ToJsonString();
            }
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.ExternalLoginProvider.OpenIdConnectMappedClaims,
                openIdConnectMappedClaimsValue
            );
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.ExternalLoginProvider.Tenant.WsFederation,
                input.WsFederation == null || !input.WsFederation.IsValid() ? "" : input.WsFederation.ToJsonString()
            );

            var wsFederationMappedClaimsValue = "";
            if (input.WsFederation == null || !input.WsFederation.IsValid() || input.WsFederationClaimsMapping.IsNullOrEmpty())
            {
                wsFederationMappedClaimsValue = await SettingManager.GetSettingValueForApplicationAsync(AppSettings.ExternalLoginProvider.WsFederationMappedClaims);//set default value
            }
            else
            {
                wsFederationMappedClaimsValue = input.WsFederationClaimsMapping.ToJsonString();
            }
            
            await SettingManager.ChangeSettingForTenantAsync(
                AbpSession.GetTenantId(),
                AppSettings.ExternalLoginProvider.WsFederationMappedClaims,
                wsFederationMappedClaimsValue
            );
            
            ExternalLoginOptionsCacheManager.ClearCache();
        }
        protected virtual async Task Create(CreateOrEditOrderOfferDto input)
        {
            OrderOffers.OrderOffer orderOffer = new OrderOffers.OrderOffer
            {
                Id = (long)input.Id,
                FeesEnd = input.FeesEnd,
                FeesStart = input.FeesStart,
                Area = input.Area,
                Cities = input.Cities,
                NewFees = input.NewFees,
                isAvailable = input.isAvailable,
                OrderOfferEnd = input.OrderOfferEnd,
                OrderOfferStart = input.OrderOfferStart,
                TenantId = AbpSession.GetTenantId()

            };


            if (AbpSession.TenantId != null)
            {
                orderOffer.TenantId = (int?)AbpSession.TenantId;
            }


            await _orderOfferRepository.InsertAsync(orderOffer);
        }
        protected virtual async Task Update(CreateOrEditOrderOfferDto input)
        {
            var order = await _orderOfferRepository.FirstOrDefaultAsync((long)input.Id);


            OrderOffers.OrderOffer orderOffer = new OrderOffers.OrderOffer
            {
                Id = (long)input.Id,
                FeesEnd = input.FeesEnd,
                FeesStart = input.FeesStart,
                Area = input.Area,
                Cities = input.Cities,
                NewFees = input.NewFees,
                isAvailable = input.isAvailable,
                OrderOfferEndS = input.OrderOfferEnd.ToString("HH:mm"),
                OrderOfferStartS = input.OrderOfferStart.ToString("HH:mm"),
                TenantId = AbpSession.GetTenantId()

            };

            await _orderOfferRepository.UpdateAsync(orderOffer);
        }




        #endregion




        #region Others

        private async Task SendToRestaurantsBot(int id)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, AppSettingsModel.BotApi+"api/RestaurantsChatBot/DeleteCache?TenantId="+id);
                request.Headers.Add("accept", "text/plain");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
               

            }
            catch
            {

            }




        }
        public async Task ClearLogo()
        {
            var tenant = await GetCurrentTenantAsync();

            if (!tenant.HasLogo())
            {
                return;
            }

            var logoObject = await _binaryObjectManager.GetOrNullAsync(tenant.LogoId.Value);
            if (logoObject != null)
            {
                await _binaryObjectManager.DeleteAsync(tenant.LogoId.Value);
            }

            tenant.ClearLogo();
        }
        public async Task ClearCustomCss()
        {
            var tenant = await GetCurrentTenantAsync();

            if (!tenant.CustomCssId.HasValue)
            {
                return;
            }

            var cssObject = await _binaryObjectManager.GetOrNullAsync(tenant.CustomCssId.Value);
            if (cssObject != null)
            {
                await _binaryObjectManager.DeleteAsync(tenant.CustomCssId.Value);
            }

            tenant.CustomCssId = null;
        }
        public async Task UpdateLoyalty(LoyaltyModel input)
        {
            var itemsCollection = new DocumentCosmoseDB<TenantModel>(CollectionTypes.ItemsCollection, _IDocumentClient);

            TenantModel tenant = await itemsCollection.GetItemAsync(a => a.ItemType == InfoSeedContainerItemTypes.Tenant && a.TenantId == AbpSession.GetTenantId());

            tenant.IsLoyalityPoint = input.IsLoyalityPoint;
      
            await itemsCollection.UpdateItemAsync(tenant._self, tenant);

            var x=  _loyaltyAppService.CreateOrEdit(input);
          
        }
        private Dto.WorkModel PrepareGetWorkModel(MultiTenancy.WorkModel input)
        {
            Dto.WorkModel workModel = new Dto.WorkModel();

            #region Saturday
            workModel.WorkTextSat = input.WorkTextSat;
            workModel.IsWorkActiveSat = input.IsWorkActiveSat;
            workModel.StartDateSat = input.StartDateSat.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.EndDateSat = input.EndDateSat.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.HasSPSat = input.HasSPSat;
            if (input.StartDateSatSP != null)
            {
                workModel.StartDateSatSP = input.StartDateSatSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.StartDateSatSP = DateTime.MinValue;
            }
            if (input.EndDateSatSP != null)
            {
                workModel.EndDateSatSP = input.EndDateSatSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.EndDateSatSP = DateTime.MinValue;
            }
            #endregion

            #region Sunday
            workModel.IsWorkActiveSun = input.IsWorkActiveSun;
            workModel.StartDateSun = input.StartDateSun.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.EndDateSun = input.EndDateSun.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.WorkTextSun = input.WorkTextSun;
            workModel.HasSPSun = input.HasSPSun;

            if (input.StartDateSunSP != null)
            {
                workModel.StartDateSunSP = input.StartDateSunSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.StartDateSunSP = DateTime.MinValue;
            }
            if (input.EndDateSunSP != null)
            {
                workModel.EndDateSunSP = input.EndDateSunSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.EndDateSunSP = DateTime.MinValue;
            }
            #endregion

            #region Monday
            workModel.IsWorkActiveMon = input.IsWorkActiveMon;
            workModel.WorkTextMon = input.WorkTextMon;
            workModel.StartDateMon = input.StartDateMon.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.EndDateMon = input.EndDateMon.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.HasSPMon = input.HasSPMon;
            if (input.StartDateMonSP != null)
            {
                workModel.StartDateMonSP = input.StartDateMonSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.StartDateMonSP = DateTime.MinValue;
            }
            if (input.EndDateMonSP != null)
            {
                workModel.EndDateMonSP = input.EndDateMonSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.EndDateMonSP = DateTime.MinValue;
            }
            #endregion

            #region Tuesday
            workModel.IsWorkActiveTues = input.IsWorkActiveTues;
            workModel.WorkTextTues = input.WorkTextTues;
            workModel.StartDateTues = input.StartDateTues.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.EndDateTues = input.EndDateTues.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.HasSPTues = input.HasSPTues;
            if (input.StartDateTuesSP != null)
            {
                workModel.StartDateTuesSP = input.StartDateTuesSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.StartDateTuesSP = DateTime.MinValue;
            }
            if (input.EndDateTuesSP != null)
            {
                workModel.EndDateTuesSP = input.EndDateTuesSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.EndDateTuesSP = DateTime.MinValue;
            }
            #endregion

            #region Wednesday
            workModel.EndDateWed = input.EndDateWed.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.IsWorkActiveWed = input.IsWorkActiveWed;
            workModel.StartDateWed = input.StartDateWed.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.WorkTextWed = input.WorkTextWed;
            workModel.HasSPWed = input.HasSPWed;
            if (input.StartDateWedSP != null)
            {
                workModel.StartDateWedSP = input.StartDateWedSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.StartDateWedSP = DateTime.MinValue;
            }
            if (input.EndDateWedSP != null)
            {
                workModel.EndDateWedSP = input.EndDateWedSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.EndDateWedSP = DateTime.MinValue;
            }
            #endregion

            #region Thursday
            workModel.IsWorkActiveThurs = input.IsWorkActiveThurs;
            workModel.WorkTextThurs = input.WorkTextThurs;
            workModel.StartDateThurs = input.StartDateThurs.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.EndDateThurs = input.EndDateThurs.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.HasSPThurs = input.HasSPThurs;
            if (input.StartDateThursSP != null)
            {
                workModel.StartDateThursSP = input.StartDateThursSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.StartDateThursSP = DateTime.MinValue;
            }
            if (input.EndDateThursSP != null)
            {
                workModel.EndDateThursSP = input.EndDateThursSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.EndDateThursSP = DateTime.MinValue;
            }
            #endregion

            #region Firday
            workModel.IsWorkActiveFri = input.IsWorkActiveFri;
            workModel.WorkTextFri = input.WorkTextFri;
            workModel.StartDateFri = input.StartDateFri.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.EndDateFri = input.EndDateFri.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            workModel.HasSPFri = input.HasSPFri;
            if (input.StartDateFriSP != null)
            {
                workModel.StartDateFriSP = input.StartDateFriSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.StartDateFriSP = DateTime.MinValue.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            if (input.EndDateFriSP != null)
            {
                workModel.EndDateFriSP = input.EndDateFriSP.Value.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            else
            {
                workModel.EndDateFriSP = DateTime.MinValue.AddHours(AppSettingsModel.AddHour).ToString("HH:mm");
            }
            #endregion

            return workModel;
        }
        private MultiTenancy.WorkModel PrepareUpdateWorkModel(Dto.WorkModel input)
        {
            MultiTenancy.WorkModel workModel = new MultiTenancy.WorkModel();

            // Saturday
            #region  Saturday

            workModel.WorkTextSat = input.WorkTextSat;
            workModel.IsWorkActiveSat = input.IsWorkActiveSat;
            workModel.StartDateSat = input.StartDateSat;
            workModel.EndDateSat = input.EndDateSat;
            workModel.StartDateSat=workModel.StartDateSat.Value.ToUniversalTime();
            workModel.EndDateSat=workModel.EndDateSat.Value.ToUniversalTime();
            workModel.HasSPSat = input.HasSPSat;

            if (input.StartDateSatSP != null)
            {
                workModel.StartDateSatSP = input.StartDateSatSP;
                workModel.StartDateSatSP=workModel.StartDateSatSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.StartDateSatSP = DateTime.MinValue;
            }
            if (input.EndDateSatSP != null)
            {
                workModel.EndDateSatSP = input.EndDateSatSP;
                workModel.EndDateSatSP=workModel.EndDateSatSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.EndDateSatSP = DateTime.MinValue;
            }
            #endregion

            // Sunday
            #region Sunday

            workModel.IsWorkActiveSun = input.IsWorkActiveSun;
            workModel.WorkTextSun = input.WorkTextSun;
            workModel.StartDateSun = input.StartDateSun;
            workModel.EndDateSun = input.EndDateSun;
            workModel.StartDateSun=workModel.StartDateSun.Value.ToUniversalTime();
            workModel.EndDateSun=workModel.EndDateSun.Value.ToUniversalTime();
            workModel.HasSPSun = input.HasSPSun;

            if (input.StartDateSunSP != null)
            {
                workModel.StartDateSunSP = input.StartDateSunSP;
                workModel.StartDateSunSP=workModel.StartDateSunSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.StartDateSunSP = DateTime.MinValue;
            }
            if (input.EndDateSunSP != null)
            {
                workModel.EndDateSunSP = input.EndDateSunSP;
                workModel.EndDateSunSP=workModel.EndDateSunSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.EndDateSunSP = DateTime.MinValue;
            }
            #endregion

            // Monday
            #region Monday

            workModel.IsWorkActiveMon = input.IsWorkActiveMon;
            workModel.WorkTextMon = input.WorkTextMon;
            workModel.StartDateMon = input.StartDateMon;
            workModel.EndDateMon = input.EndDateMon;
            workModel.StartDateMon = workModel.StartDateMon.Value.ToUniversalTime();
            workModel.EndDateMon = workModel.EndDateMon.Value.ToUniversalTime();
            workModel.HasSPMon = input.HasSPMon;

            if (input.StartDateMonSP != null)
            {
                workModel.StartDateMonSP = input.StartDateMonSP;
                workModel.StartDateMonSP=workModel.StartDateMonSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.StartDateMonSP = DateTime.MinValue;
            }
            if (input.EndDateMonSP != null)
            {
                workModel.EndDateMonSP = input.EndDateMonSP;
                workModel.EndDateMonSP=workModel.EndDateMonSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.EndDateMonSP = DateTime.MinValue;
            }
            #endregion

            // Tuesday
            #region Tuesday

            workModel.IsWorkActiveTues = input.IsWorkActiveTues;
            workModel.WorkTextTues = input.WorkTextTues;
            workModel.StartDateTues = input.StartDateTues;
            workModel.EndDateTues = input.EndDateTues;
            workModel.StartDateTues = workModel.StartDateTues.Value.ToUniversalTime();
            workModel.EndDateTues = workModel.EndDateTues.Value.ToUniversalTime();
            workModel.HasSPTues = input.HasSPTues;

            if (input.StartDateTuesSP != null)
            {
                workModel.StartDateTuesSP = input.StartDateTuesSP;
                workModel.StartDateTuesSP = workModel.StartDateTuesSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.StartDateTuesSP = DateTime.MinValue;
            }
            if (input.EndDateTuesSP != null)
            {
                workModel.EndDateTuesSP = input.EndDateTuesSP;
                workModel.EndDateTuesSP = workModel.EndDateTuesSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.EndDateTuesSP = DateTime.MinValue;
            }
            #endregion

            // Wednesday
            #region Wednesday

            workModel.IsWorkActiveWed = input.IsWorkActiveWed;
            workModel.StartDateWed = input.StartDateWed;
            workModel.EndDateWed = input.EndDateWed;
            workModel.StartDateWed = workModel.StartDateWed.Value.ToUniversalTime();
            workModel.EndDateWed = workModel.EndDateWed.Value.ToUniversalTime();
            workModel.WorkTextWed = input.WorkTextWed;
            workModel.HasSPWed = input.HasSPWed;

            if (input.StartDateWedSP != null)
            {
                workModel.StartDateWedSP = input.StartDateWedSP;
                workModel.StartDateWedSP = workModel.StartDateWedSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.StartDateWedSP = DateTime.MinValue;
            }
            if (input.EndDateWedSP != null)
            {
                workModel.EndDateWedSP = input.EndDateWedSP;
                workModel.EndDateWedSP = workModel.EndDateWedSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.EndDateWedSP = DateTime.MinValue;
            }
            #endregion

            // Thursday
            #region Thursday

            workModel.IsWorkActiveThurs = input.IsWorkActiveThurs;
            workModel.WorkTextThurs = input.WorkTextThurs;
            workModel.StartDateThurs = input.StartDateThurs;
            workModel.EndDateThurs = input.EndDateThurs;
            workModel.StartDateThurs = workModel.StartDateThurs.Value.ToUniversalTime();
            workModel.EndDateThurs = workModel.EndDateThurs.Value.ToUniversalTime();
            workModel.HasSPThurs = input.HasSPThurs;

            if (input.StartDateThursSP != null)
            {
                workModel.StartDateThursSP = input.StartDateThursSP;
                workModel.StartDateThursSP = workModel.StartDateThursSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.StartDateThursSP = DateTime.MinValue;
            }
            if (input.EndDateThursSP != null)
            {
                workModel.EndDateThursSP = input.EndDateThursSP;
                workModel.EndDateThursSP = workModel.EndDateThursSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.EndDateThursSP = DateTime.MinValue;
            }
            #endregion

            // Firday
            #region Firday
            workModel.IsWorkActiveFri = input.IsWorkActiveFri;
            workModel.WorkTextFri = input.WorkTextFri;
            workModel.StartDateFri = input.StartDateFri;
            workModel.EndDateFri = input.EndDateFri;

            workModel.StartDateFri = workModel.StartDateFri.Value.ToUniversalTime();
            workModel.EndDateFri = workModel.EndDateFri.Value.ToUniversalTime();
            workModel.HasSPFri = input.HasSPFri;

            if (input.StartDateFriSP != null)
            {
                workModel.StartDateFriSP = input.StartDateFriSP;
                workModel.StartDateFriSP = workModel.StartDateFriSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.StartDateFriSP = DateTime.MinValue;
            }
            if (input.EndDateFriSP != null)
            {
                workModel.EndDateFriSP = input.EndDateFriSP;
                workModel.EndDateFriSP = workModel.EndDateFriSP.Value.ToUniversalTime();
            }
            else
            {
                workModel.EndDateFriSP = DateTime.MinValue;
            }
            #endregion


            return workModel;
        }
        private MultiTenancy.WorkModel PrepareUpdateWorkModelMobile(Dto.WorkModel input)
        {
            MultiTenancy.WorkModel workModel = new MultiTenancy.WorkModel();

            // Saturday
            #region  Saturday

            workModel.WorkTextSat = input.WorkTextSat;
            workModel.IsWorkActiveSat = input.IsWorkActiveSat;
            workModel.StartDateSat = input.StartDateSat;
            workModel.StartDateSat = workModel.StartDateSat.Value.AddHours(AppSettingsModel.DivHour);
            workModel.EndDateSat = input.EndDateSat;
            workModel.EndDateSat = workModel.EndDateSat.Value.AddHours(AppSettingsModel.DivHour);

            workModel.HasSPSat = input.HasSPSat;

            if (input.StartDateSatSP != null)
            {
                workModel.StartDateSatSP = input.StartDateSatSP;
                workModel.StartDateSatSP = workModel.StartDateSatSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.StartDateSatSP = DateTime.MinValue;
            }
            if (input.EndDateSatSP != null)
            {
                workModel.EndDateSatSP = input.EndDateSatSP;
                workModel.EndDateSatSP = workModel.EndDateSatSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.EndDateSatSP = DateTime.MinValue;
            }
            #endregion

            // Sunday
            #region Sunday

            workModel.IsWorkActiveSun = input.IsWorkActiveSun;
            workModel.StartDateSun = input.StartDateSun;
            workModel.StartDateSun = workModel.StartDateSun.Value.AddHours(AppSettingsModel.DivHour);

            workModel.EndDateSun = input.EndDateSun;
            workModel.EndDateSun = workModel.EndDateSun.Value.AddHours(AppSettingsModel.DivHour);

            workModel.WorkTextSun = input.WorkTextSun;
            workModel.HasSPSun = input.HasSPSun;

            if (input.StartDateSunSP != null)
            {
                workModel.StartDateSunSP = input.StartDateSunSP;
                workModel.StartDateSunSP = workModel.StartDateSunSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.StartDateSunSP = DateTime.MinValue;
            }
            if (input.EndDateSunSP != null)
            {
                workModel.EndDateSunSP = input.EndDateSunSP;
                workModel.EndDateSunSP = workModel.EndDateSunSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.EndDateSunSP = DateTime.MinValue;
            }
            #endregion

            // Monday
            #region Monday

            workModel.IsWorkActiveMon = input.IsWorkActiveMon;
            workModel.WorkTextMon = input.WorkTextMon;
            workModel.StartDateMon = input.StartDateMon;
            workModel.StartDateMon = workModel.StartDateMon.Value.AddHours(AppSettingsModel.DivHour);

            workModel.EndDateMon = input.EndDateMon;
            workModel.EndDateMon = workModel.EndDateMon.Value.AddHours(AppSettingsModel.DivHour);

            workModel.HasSPMon = input.HasSPMon;

            if (input.StartDateMonSP != null)
            {
                workModel.StartDateMonSP = input.StartDateMonSP;
                workModel.StartDateMonSP = workModel.StartDateMonSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.StartDateMonSP = DateTime.MinValue;
            }
            if (input.EndDateMonSP != null)
            {
                workModel.EndDateMonSP = input.EndDateMonSP;
                workModel.EndDateMonSP = workModel.EndDateMonSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.EndDateMonSP = DateTime.MinValue;
            }
            #endregion

            // Tuesday
            #region Tuesday

            workModel.IsWorkActiveTues = input.IsWorkActiveTues;
            workModel.WorkTextTues = input.WorkTextTues;
            workModel.StartDateTues = input.StartDateTues;
            workModel.StartDateTues = workModel.StartDateTues.Value.AddHours(AppSettingsModel.DivHour);

            workModel.EndDateTues = input.EndDateTues;
            workModel.EndDateTues = workModel.EndDateTues.Value.AddHours(AppSettingsModel.DivHour);

            workModel.HasSPTues = input.HasSPTues;

            if (input.StartDateTuesSP != null)
            {
                workModel.StartDateTuesSP = input.StartDateTuesSP;
                workModel.StartDateTuesSP = workModel.StartDateTuesSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.StartDateTuesSP = DateTime.MinValue;
            }
            if (input.EndDateTuesSP != null)
            {
                workModel.EndDateTuesSP = input.EndDateTuesSP;
                workModel.EndDateTuesSP = workModel.EndDateTuesSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.EndDateTuesSP = DateTime.MinValue;
            }
            #endregion

            // Wednesday
            #region Wednesday

            workModel.IsWorkActiveWed = input.IsWorkActiveWed;
            workModel.StartDateWed = input.StartDateWed;
            workModel.StartDateWed = workModel.StartDateWed.Value.AddHours(AppSettingsModel.DivHour);

            workModel.EndDateWed = input.EndDateWed;
            workModel.EndDateWed = workModel.EndDateWed.Value.AddHours(AppSettingsModel.DivHour);

            workModel.WorkTextWed = input.WorkTextWed;
            workModel.HasSPWed = input.HasSPWed;

            if (input.StartDateWedSP != null)
            {
                workModel.StartDateWedSP = input.StartDateWedSP;
                workModel.StartDateWedSP = workModel.StartDateWedSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.StartDateWedSP = DateTime.MinValue;
            }
            if (input.EndDateWedSP != null)
            {
                workModel.EndDateWedSP = input.EndDateWedSP;
                workModel.EndDateWedSP = workModel.EndDateWedSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.EndDateWedSP = DateTime.MinValue;
            }
            #endregion

            // Thursday
            #region Thursday

            workModel.IsWorkActiveThurs = input.IsWorkActiveThurs;
            workModel.WorkTextThurs = input.WorkTextThurs;
            workModel.StartDateThurs = input.StartDateThurs;
            workModel.StartDateThurs = workModel.StartDateThurs.Value.AddHours(AppSettingsModel.DivHour);

            workModel.EndDateThurs = input.EndDateThurs;
            workModel.EndDateThurs = workModel.EndDateThurs.Value.AddHours(AppSettingsModel.DivHour);

            workModel.HasSPThurs = input.HasSPThurs;

            if (input.StartDateThursSP != null)
            {
                workModel.StartDateThursSP = input.StartDateThursSP;
                workModel.StartDateThursSP = workModel.StartDateThursSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.StartDateThursSP = DateTime.MinValue;
            }
            if (input.EndDateThursSP != null)
            {
                workModel.EndDateThursSP = input.EndDateThursSP;
                workModel.EndDateThursSP = workModel.EndDateThursSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.EndDateThursSP = DateTime.MinValue;
            }
            #endregion

            // Firday
            #region Firday
            workModel.IsWorkActiveFri = input.IsWorkActiveFri;
            workModel.WorkTextFri = input.WorkTextFri;
            workModel.StartDateFri = input.StartDateFri;
            workModel.StartDateFri = workModel.StartDateFri.Value.AddHours(AppSettingsModel.DivHour);

            workModel.EndDateFri = input.EndDateFri;
            workModel.EndDateFri = workModel.EndDateFri.Value.AddHours(AppSettingsModel.DivHour);

            workModel.HasSPFri = input.HasSPFri;

            if (input.StartDateFriSP != null)
            {
                workModel.StartDateFriSP = input.StartDateFriSP;
                workModel.StartDateFriSP = workModel.StartDateFriSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.StartDateFriSP = DateTime.MinValue;
            }
            if (input.EndDateFriSP != null)
            {
                workModel.EndDateFriSP = input.EndDateFriSP;
                workModel.EndDateFriSP = workModel.EndDateFriSP.Value.AddHours(AppSettingsModel.DivHour);

            }
            else
            {
                workModel.EndDateFriSP = DateTime.MinValue;
            }
            #endregion


            return workModel;
        }

        #endregion

        #region Private For GoogleSheet
        private void GoogleSheetConfigAdd(string accessToken, string refreshToken, bool? isConnected, int TenantId, string googleEmail)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.GoogleSheetConfigAdd";

                        command.Parameters.AddWithValue("@TenantId", (object)TenantId ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AccessToken", accessToken);
                        command.Parameters.AddWithValue("@RefreshToken", refreshToken);
                        command.Parameters.AddWithValue("@IsConnected", isConnected);
                        command.Parameters.AddWithValue("@GoogleEmail", googleEmail);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex) {
                throw;
            }
        }

        private void GoogleSheetConfigUpdate(string accessToken, string refreshToken, bool? isConnected, int tenantId, string googleEmail)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;
                using (var connection = new SqlConnection(connectionString))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "dbo.GoogleSheetConfigUpdate";

                        command.Parameters.AddWithValue("@TenantId", (object)tenantId ?? DBNull.Value);
                        command.Parameters.AddWithValue("@AccessToken", accessToken);
                        command.Parameters.AddWithValue("@RefreshToken", refreshToken);
                        command.Parameters.AddWithValue("@IsConnected", isConnected);
                        command.Parameters.AddWithValue("@GoogleEmail", googleEmail);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        

        private void GoogleSheetConfigRemove(int? tenantId)
        {
            try
            {
                string connectionString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("dbo.GoogleSheetConfigRemove", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@TenantId", tenantId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        #endregion
    }
}
