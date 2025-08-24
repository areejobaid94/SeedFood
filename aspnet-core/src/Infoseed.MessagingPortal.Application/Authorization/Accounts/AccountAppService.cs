using System;
using System.Threading.Tasks;
using System.Web;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Infoseed.MessagingPortal.Authorization.Accounts.Dto;
using Infoseed.MessagingPortal.Authorization.Impersonation;
using Infoseed.MessagingPortal.Authorization.Users;
using Infoseed.MessagingPortal.Configuration;
using Infoseed.MessagingPortal.Debugging;
using Infoseed.MessagingPortal.MultiTenancy;
using Infoseed.MessagingPortal.Security.Recaptcha;
using Infoseed.MessagingPortal.Url;
using Infoseed.MessagingPortal.Authorization.Delegation;
using Abp.Domain.Repositories;
using RestSharp;
using System.Net.Http;
using System.Text.Json;
using Nancy.Responses;
using Newtonsoft.Json;
using Infoseed.MessagingPortal.Currencies;

namespace Infoseed.MessagingPortal.Authorization.Accounts
{
    public class AccountAppService : MessagingPortalAppServiceBase, IAccountAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        public IRecaptchaValidator RecaptchaValidator { get; set; }

        private readonly IUserEmailer _userEmailer;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IUserLinkManager _userLinkManager;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IWebUrlService _webUrlService;
        private readonly IUserDelegationManager _userDelegationManager;

        public AccountAppService(
            IUserEmailer userEmailer,
            UserRegistrationManager userRegistrationManager,
            IImpersonationManager impersonationManager,
            IUserLinkManager userLinkManager,
            IPasswordHasher<User> passwordHasher,
            IWebUrlService webUrlService, 
            IUserDelegationManager userDelegationManager)
        {
            _userEmailer = userEmailer;
            _userRegistrationManager = userRegistrationManager;
            _impersonationManager = impersonationManager;
            _userLinkManager = userLinkManager;
            _passwordHasher = passwordHasher;
            _webUrlService = webUrlService;

            AppUrlService = NullAppUrlService.Instance;
            RecaptchaValidator = NullRecaptchaValidator.Instance;
            _userDelegationManager = userDelegationManager;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id, _webUrlService.GetServerRootAddress(input.TenancyName));
        }

        public Task<int?> ResolveTenantId(ResolveTenantIdInput input)
        {
            if (string.IsNullOrEmpty(input.c))
            {
                return Task.FromResult(AbpSession.TenantId);
            }

            var parameters = SimpleStringCipher.Instance.Decrypt(input.c);
            var query = HttpUtility.ParseQueryString(parameters);

            if (query["tenantId"] == null)
            {
                return Task.FromResult<int?>(null);
            }

            var tenantId = Convert.ToInt32(query["tenantId"]) as int?;
            return Task.FromResult(tenantId);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            if (UseCaptchaOnRegistration())
            {
                await RecaptchaValidator.ValidateAsync(input.CaptchaResponse);
            }

            var user = await _userRegistrationManager.RegisterAsync(
                input.Name,
                input.Surname,
                input.EmailAddress,
                input.UserName,
                input.Password,
                false,
                AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId)
            );

            var isEmailConfirmationRequiredForLogin = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }

        public async Task SendPasswordResetCode(SendPasswordResetCodeInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewPasswordResetCode();
            await _userEmailer.SendPasswordResetLinkAsync(
                user,
                AppUrlService.CreatePasswordResetUrlFormat(AbpSession.TenantId)
                );
        }

        public async Task<ResetPasswordOutput> ResetPassword(ResetPasswordInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user == null || user.PasswordResetCode.IsNullOrEmpty() || user.PasswordResetCode != input.ResetCode)
            {
                throw new UserFriendlyException(L("InvalidPasswordResetCode"), L("InvalidPasswordResetCode_Detail"));
            }

            await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
            CheckErrors(await UserManager.ChangePasswordAsync(user, input.Password));
            user.PasswordResetCode = null;
            user.IsEmailConfirmed = true;
            user.ShouldChangePasswordOnNextLogin = false;

            await UserManager.UpdateAsync(user);

            return new ResetPasswordOutput
            {
                CanLogin = user.IsActive,
                UserName = user.UserName
            };
        }

        public async Task SendEmailActivationLink(SendEmailActivationLinkInput input)
        {
            var user = await GetUserByChecking(input.EmailAddress);
            user.SetNewEmailConfirmationCode();
            await _userEmailer.SendEmailActivationLinkAsync(
                user,
                AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId)
            );
        }

        public async Task ActivateEmail(ActivateEmailInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.UserId);
            if (user != null && user.IsEmailConfirmed)
            {
                return;
            }

            if (user == null || user.EmailConfirmationCode.IsNullOrEmpty() || user.EmailConfirmationCode != input.ConfirmationCode)
            {
                throw new UserFriendlyException(L("InvalidEmailConfirmationCode"), L("InvalidEmailConfirmationCode_Detail"));
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationCode = null;

            await UserManager.UpdateAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Impersonation)]
        public virtual async Task<ImpersonateOutput> Impersonate(ImpersonateInput input)
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationToken(input.UserId, input.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TenantId)
            };
        }

        public virtual async Task<ImpersonateOutput> DelegatedImpersonate(DelegatedImpersonateInput input)
        {
            var userDelegation = await _userDelegationManager.GetAsync(input.UserDelegationId);
            if (userDelegation.TargetUserId != AbpSession.GetUserId())
            {
                throw new UserFriendlyException("User delegation error.");
            }

            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetImpersonationToken(userDelegation.SourceUserId, userDelegation.TenantId),
                TenancyName = await GetTenancyNameOrNullAsync(userDelegation.TenantId)
            };
        }

        public virtual async Task<ImpersonateOutput> BackToImpersonator()
        {
            return new ImpersonateOutput
            {
                ImpersonationToken = await _impersonationManager.GetBackToImpersonatorToken(),
                TenancyName = await GetTenancyNameOrNullAsync(AbpSession.ImpersonatorTenantId)
            };
        }

        public virtual async Task<SwitchToLinkedAccountOutput> SwitchToLinkedAccount(SwitchToLinkedAccountInput input)
        {
            if (!await _userLinkManager.AreUsersLinked(AbpSession.ToUserIdentifier(), input.ToUserIdentifier()))
            {
                throw new Exception(L("This account is not linked to your account"));
            }

            return new SwitchToLinkedAccountOutput
            {
                SwitchAccountToken = await _userLinkManager.GetAccountSwitchToken(input.TargetUserId, input.TargetTenantId),
                TenancyName = await GetTenancyNameOrNullAsync(input.TargetTenantId)
            };
        }

        private bool UseCaptchaOnRegistration()
        {
            return SettingManager.GetSettingValue<bool>(AppSettings.UserManagement.UseCaptchaOnRegistration);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await TenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        private async Task<string> GetTenancyNameOrNullAsync(int? tenantId)
        {
            return tenantId.HasValue ? (await GetActiveTenantAsync(tenantId.Value)).TenancyName : null;
        }

        private async Task<User> GetUserByChecking(string inputEmailAddress)
        {
            var user = await UserManager.FindByEmailAsync(inputEmailAddress);
            if (user == null)
            {
                throw new UserFriendlyException(L("InvalidEmailAddress"));
            }

            return user;
        }

        public async Task<TenantDataFacebook> FaceBookAccessToken(LoginFacebookModel model)
        {
            try
            {
                model.waba_id=model.waba_id.Trim();
                model.phone_number_id=model.phone_number_id.Trim();
                model.code=model.code.ToString();
                TenantDataFacebook tenantDataFacebook = new TenantDataFacebook();


                tenantDataFacebook.phoneNumberId=model.phone_number_id;
                tenantDataFacebook.whatsAppAccountId=model.waba_id;
               
                //access_token
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v20.0/oauth/access_token");

                // Add Authorization header
                request.Headers.Add("Authorization", "Bearer " + model.code);
                request.Headers.Add("Cookie", "ps_l=1; ps_n=1; sb=pekaZDOqpOtc-qAgQ3WxnYlO");

                // Prepare JSON content
                var content = new StringContent(
                    "{\r\n" +
                    "    \"client_id\": \"885586280068397\",\r\n" +
                    "    \"client_secret\": \"6bb8de39780aefe14f7a49a4acbf0c98\",\r\n" +
                    "    \"code\": \"" + model.code + "\",\r\n" +
                    "    \"grant_type\": \"authorization_code\"\r\n" +
                    "}",
                    System.Text.Encoding.UTF8,
                    "application/json");

                request.Content = content;

                // Send the request and get the response
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // Deserialize the response content to facebooktokenModel
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var facebookToken = JsonConvert.DeserializeObject<facebooktokenModel>(jsonResponse);
                // facebooktokenModel facebookToken = new facebooktokenModel();
                tenantDataFacebook.accessToken=facebookToken.access_token;

                //info
                var client2 = new HttpClient();
                var request2 = new HttpRequestMessage(HttpMethod.Get, "https://graph.facebook.com/v20.0/"+model.phone_number_id+"?\\fields=id,name,currency,owner_business_info&\\access_token="+facebookToken.access_token);
                request2.Headers.Add("Authorization", "Bearer " + facebookToken.access_token);
                request2.Headers.Add("Cookie", "ps_l=1; ps_n=1; sb=pekaZDOqpOtc-qAgQ3WxnYlO");
                var response2 = await client.SendAsync(request2);
                response.EnsureSuccessStatusCode();
                // Deserialize the response content to facebooktokenModel
                var jsonResponse2 = await response2.Content.ReadAsStringAsync();
                var facebookToken2 = JsonConvert.DeserializeObject<LoginFaceBookInfo>(jsonResponse2);
                string formattedPhoneNumber = facebookToken2.display_phone_number.Replace("+", "").Replace("-", "").Replace(" ", "");


                tenantDataFacebook.phoneNumber=formattedPhoneNumber;
                tenantDataFacebook.name=facebookToken2.verified_name;

                //register
                var client3 = new HttpClient();
                var request3 = new HttpRequestMessage(HttpMethod.Post, "https://graph.facebook.com/v20.0/"+model.phone_number_id+"/register");
                request3.Headers.Add("Authorization", "Bearer " + facebookToken.access_token);
                request3.Headers.Add("Cookie", "ps_l=1; ps_n=1; sb=pekaZDOqpOtc-qAgQ3WxnYlO");
                var content3 = new StringContent("{\r\n  \"messaging_product\": \"whatsapp\",\r\n  \"pin\": \"123456\"\r\n}", null, "application/json");
                request3.Content = content3;
                var response3 = await client.SendAsync(request3);
                response.EnsureSuccessStatusCode();
                //Console.WriteLine(await response3.Content.ReadAsStringAsync());


                return tenantDataFacebook;




            }
            catch (Exception ex) { 
            
                 return null;
            }


        }
    }
}