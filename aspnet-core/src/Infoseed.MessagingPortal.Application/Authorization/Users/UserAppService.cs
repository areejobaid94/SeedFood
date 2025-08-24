using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.Authorization.Permissions;
using Infoseed.MessagingPortal.Authorization.Permissions.Dto;
using Infoseed.MessagingPortal.Authorization.Roles;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Authorization.Users.Exporting;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Notifications;
using Infoseed.MessagingPortal.Url;
using Infoseed.MessagingPortal.Organizations.Dto;
using System.Data.SqlClient;
using Infoseed.MessagingPortal.Areas;
using System.Data;
using Framework.Data;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Newtonsoft.Json;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.SocketIOClient;
using System.Net.Http;
using System.Security.Claims;
using Abp.Runtime.Caching;

namespace Infoseed.MessagingPortal.Authorization.Users
{
   // [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UserAppService : MessagingPortalAppServiceBase, IUserAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        private readonly RoleManager _roleManager;
        private readonly ICacheManager _cacheManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUserListExcelExporter _userListExcelExporter;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IUserPolicy _userPolicy;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRoleManagementConfig _roleManagementConfig;
        private readonly UserManager _userManager;
       // private readonly IJwtSecurityStampHandler _securityStampHandler;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IRepository<OrganizationUnitRole, long> _organizationUnitRoleRepository;

        public UserAppService(
            ICacheManager cacheManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            IUserListExcelExporter userListExcelExporter,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository,
            IRepository<UserRole, long> userRoleRepository,
            IRepository<Role> roleRepository,
            IUserPolicy userPolicy,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRoleManagementConfig roleManagementConfig,
            UserManager userManager,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IRepository<OrganizationUnitRole, long> organizationUnitRoleRepository)
        {
            _cacheManager=cacheManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userListExcelExporter = userListExcelExporter;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _userRoleRepository = userRoleRepository;
            _userPolicy = userPolicy;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;
            _organizationUnitRepository = organizationUnitRepository;
            _roleManagementConfig = roleManagementConfig;
            _userManager = userManager;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _organizationUnitRoleRepository = organizationUnitRoleRepository;
            _roleRepository = roleRepository;

            AppUrlService = NullAppUrlService.Instance;
        }
        public UserAppService()
        {

        }


        public UserTicketsModel UserTicketsOpenUpdate(long userId, bool IsOpen)
        {
            try
            {

                UserTicketsModel userTicketsModel = new UserTicketsModel();
                var SP_Name = "[dbo].[UserTicketsOpenUpdate]";

                var sqlParameters = new List<SqlParameter> {
                      new SqlParameter("@UserId",userId),
                     new SqlParameter("@IsOpen",IsOpen)
                 };

                var OutputParameter = new System.Data.SqlClient.SqlParameter
                {
                    SqlDbType = SqlDbType.Bit,
                    ParameterName = "@Result",
                    Direction = ParameterDirection.Output
                };
                sqlParameters.Add(OutputParameter);


                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);


                var rez=(bool)OutputParameter.Value;


              var userTicketsModel1 =  UserTicketsGet(userId);

                userTicketsModel1.IsOpen=rez;

                return userTicketsModel1;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UserTicketsUpdate(long userId,int MaximumTickets)
        {
            try
            {
                var SP_Name = "[dbo].[UserTicketsUpdate]";

                var sqlParameters = new List<SqlParameter> {
                      new SqlParameter("@UserId",userId),
                     new SqlParameter("@MaximumTickets",MaximumTickets)
                 };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public UserTicketsModel UserTicketsGet(long userId)
        {
            try
            {
                var SP_Name = "[dbo].[UserTicketsGet]";

                var sqlParameters = new List<SqlParameter> {
                      new SqlParameter("@UserId",userId)
                 };

                var entity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapUserTickets, AppSettingsModel.ConnectionStrings).FirstOrDefault();
                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }







        public bool CheckIpAddress(int? id, int UserId)
        {
            if (id.Value==null)
            {
                return true;
            }
                var IP = GetTenantID(id.Value);

            if(!string.IsNullOrEmpty(IP))
            {
                var userIP = GetUserIpAddresse(id.Value, UserId);
                if (userIP==IP)
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


                return true;
            }

           
        }


        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {

            input.MaxResultCount=500;
            var query = GetUsersFilteredQuery(input);

            var userCount = await query.CountAsync();

            var users = await query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
                );
        }
        public async Task<List<UserListDto>> GetBookingUsers(int tenantId, string userIds = null)
        {

            return GetFuncationUserBot(tenantId, userIds);
        }

        public async Task<FileDto> GetUsersToExcel(GetUsersToExcelInput input)
        {
            var query = GetUsersFilteredQuery(input);

            var users = await query
                .OrderBy(input.Sorting)
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return _userListExcelExporter.ExportToFile(userListDtos);
        }

       // [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            //Getting all available roles
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName,

                })
                .ToArrayAsync();

            var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

            var output = new GetUserForEditOutput
            {
                AreaId= GetuserArea(input.Id),
                Roles = userRoleDtos,
                AllOrganizationUnits = ObjectMapper.Map<List<OrganizationUnitDto>>(allOrganizationUnits),
                MemberedOrganizationUnits = new List<string>()
            };

            if (input.Id==0)
            {
                //Creating a new user
                output.User = new UserEditDto
                {
                    
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled),
                    IsLockoutEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled),
                     IsEmailConfirmed= await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin),
                };
                output.AreaId = -1;
                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = ObjectMapper.Map<UserEditDto>(user);
                output.ProfilePictureId = user.ProfilePictureId;

                var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
                output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();

                var allRolesOfUsersOrganizationUnits = GetAllRoleNamesOfUsersOrganizationUnits(input.Id.Value);

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                    userRoleDto.InheritedFromOrganizationUnit = allRolesOfUsersOrganizationUnits.Contains(userRoleDto.RoleName);
                }
            }

            try
            {
                if (output.Roles!=null)
                {
                    var isfound = output.Roles.Where(x => x.IsAssigned && x.RoleName=="Admin").FirstOrDefault();


                    if (isfound!=null)
                    {
                        output.IsAdmin=true;
                    }
                    else
                    {
                        output.IsAdmin=false;

                    }
                }
            }
            catch
            {

            }
          
           

            return output;
        }

        private List<string> GetAllRoleNamesOfUsersOrganizationUnits(long userId)
        {
            return (from userOu in _userOrganizationUnitRepository.GetAll()
                    join roleOu in _organizationUnitRoleRepository.GetAll() on userOu.OrganizationUnitId equals roleOu
                        .OrganizationUnitId
                    join userOuRoles in _roleRepository.GetAll() on roleOu.RoleId equals userOuRoles.Id
                    where userOu.UserId == userId
                    select userOuRoles.Name).ToList();
        }

      //  [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

      //  [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

      //  [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }

        public async Task CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {

            if (input.User.Id.HasValue)
            {
                await UpdateUserAsync(input);


            }
            else
            {
                await CreateUserAsync(input);
            }


        }

        public int GetuserArea(long? UserID)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpUsers] where Id=" + UserID;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Area> branches = new List<Area>();



            int Id = -1;


            try
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {

                    Id = Convert.ToInt32(dataSet.Tables[0].Rows[i]["AreaId"]);

                }

            }
            catch
            {


            }

            conn.Close();
            da.Dispose();

            return Id;
        }

    //    [AbpAuthorize(AppPermissions.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("YouCanNotDeleteOwnAccount"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));


            user.SecurityStamp="123123";
            await _userManager.UpdateSecurityStampAsync(user);


            await _cacheManager.GetCache(AppConsts.SecurityStampKey)
                .SetAsync(GenerateCacheKey(AbpSession.TenantId, input.Id), "123123");

            AccountLoginModel accountLoginModel = new AccountLoginModel()
            {
                IsLogOut=true,
                TenantId=AbpSession.TenantId.Value,
                UserId=input.Id


            };

            SocketIOManager.SendAccount(accountLoginModel, AbpSession.TenantId.Value);

        }

        // [AbpAuthorize(AppPermissions.Pages_Administration_Users_Unlock)]
        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

   //     [AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
        protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            var user = await UserManager.FindByIdAsync(input.User.Id.Value.ToString());



            user.IsEmailConfirmed = input.User.IsEmailConfirmed;



                input.User.AreaId = input.AreaId;
                user.AreaId = input.AreaId;

            input.AreaIds = !string.IsNullOrEmpty(input.AreaIds) ? input.AreaIds : null;
            input.User.AreaIds = input.AreaIds;
            user.AreaIds = input.AreaIds;
            //Update user properties
            ObjectMapper.Map(input.User, user); //Passwords is not mapped (see mapping configuration)

            CheckErrors(await UserManager.UpdateAsync(user));

            if (input.SetRandomPassword)
            {
                var randomPassword = await _userManager.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
            }

            //Update roles
            CheckErrors(await UserManager.SetRolesAsync(user, input.AssignedRoleNames));

            //update organization units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }

            //user.SecurityStamp="123123";
            //await _userManager.UpdateSecurityStampAsync(user);


            //await _cacheManager.GetCache(AppConsts.SecurityStampKey)
            //    .SetAsync(GenerateCacheKey(AbpSession.TenantId, input.User.Id.Value), "123123");



            if (AbpSession.TenantId!=null)
            {

                AccountLoginModel accountLoginModel = new AccountLoginModel()
                {
                    IsLogOut=true,
                    TenantId=AbpSession.TenantId.Value,
                    UserId=input.User.Id.Value


                };

                SocketIOManager.SendAccount(accountLoginModel, AbpSession.TenantId.Value);


            }

           


        }
        private string GenerateCacheKey(int? tenantId, long userId) => $"{tenantId}.{userId}";
        //    [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        protected virtual async Task CreateUserAsync(CreateOrUpdateUserInput input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(AbpSession.GetTenantId());
            }

            var user = ObjectMapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)
            user.TenantId = AbpSession.TenantId;

            //Set password
            if (input.SetRandomPassword)
            {
                var randomPassword = await _userManager.CreateRandomPassword();
                user.Password = _passwordHasher.HashPassword(user, randomPassword);
                input.User.Password = randomPassword;
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
                }

                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);

            //Organization Units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());



            List<string> GrantedPermissionNames = new List<string>();

            GrantedPermissionNames.Add("Pages");
            GrantedPermissionNames.Add("Pages.TeamInbox");
            GrantedPermissionNames.Add("Pages.Contacts");
            GrantedPermissionNames.Add("Pages");
            GrantedPermissionNames.Add("Pages.Administration.TemplateMessages");
            GrantedPermissionNames.Add("Pages.Administration.TemplateMessages.Edit");
            GrantedPermissionNames.Add("Pages.Administration");
            GrantedPermissionNames.Add("Pages");
            GrantedPermissionNames.Add("Pages.Administration.TemplateMessages.Create");
            GrantedPermissionNames.Add("Pages.Administration.TemplateMessages");
            GrantedPermissionNames.Add("Pages.Administration");
            GrantedPermissionNames.Add("Pages");
            GrantedPermissionNames.Add("Pages.Administration.TemplateMessages.Delete");
            GrantedPermissionNames.Add("Pages.Administration.TemplateMessages");
            GrantedPermissionNames.Add("Pages.Administration");
            GrantedPermissionNames.Add("Pages");
            GrantedPermissionNames.Add("Pages.Administration.Users");
            GrantedPermissionNames.Add("Pages.Administration");
            GrantedPermissionNames.Add("Pages");
            GrantedPermissionNames.Add("Pages.Contacts.Create");
            GrantedPermissionNames.Add("Pages.Contacts.Edit");
            GrantedPermissionNames.Add("Pages.Contacts.Delete");
            GrantedPermissionNames.Add("Pages.LiveChat");
            GrantedPermissionNames.Add("Pages_LiveChat_Chat");
            GrantedPermissionNames.Add("Pages_Ticket_openAndClose");
            GrantedPermissionNames.Add("Pages_Ticket_reopen");
            GrantedPermissionNames.Add("Pages_Ticket_AssignToUser");


        //var user = await UserManager.GetUserByIdAsync(input.Id);
        var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(GrantedPermissionNames);
            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);

            //Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }
        }

        private async Task FillRoleNames(IReadOnlyCollection<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */
            var userIds = userListDtos.Select(u => u.Id);

            var userRoles = await _userRoleRepository.GetAll()
                .Where(userRole => userIds.Contains(userRole.UserId))
                .Select(userRole => userRole).ToListAsync();

            var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();

            foreach (var user in userListDtos)
            {
                var rolesOfUser = userRoles.Where(userRole => userRole.UserId == user.Id).ToList();
                user.Roles = ObjectMapper.Map<List<UserListRoleDto>>(rolesOfUser);
            }

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role != null)
                {
                    roleNames[roleId] = role.DisplayName;
                }
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    if (roleNames.ContainsKey(userListRoleDto.RoleId))
                    {
                        userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                    }
                }

                userListDto.Roles = userListDto.Roles.Where(r => r.RoleName != null).OrderBy(r => r.RoleName).ToList();
            }
        }

        private IQueryable<User> GetUsersFilteredQuery(IGetUsersInput input)
        {
            var query = UserManager.Users
                .WhereIf(input.Role.HasValue, u => u.Roles.Any(r => r.RoleId == input.Role.Value))
                .WhereIf(input.OnlyLockedUsers, u => u.LockoutEndDateUtc.HasValue && u.LockoutEndDateUtc.Value > DateTime.UtcNow)
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter)
                );

            if (input.Permissions != null && input.Permissions.Any(p => !p.IsNullOrWhiteSpace()))
            {
                var staticRoleNames = _roleManagementConfig.StaticRoles.Where(
                    r => r.GrantAllPermissionsByDefault &&
                         r.Side == AbpSession.MultiTenancySide
                ).Select(r => r.RoleName).ToList();

                input.Permissions = input.Permissions.Where(p => !string.IsNullOrEmpty(p)).ToList();

                query = from user in query
                        join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                        from ur in urJoined.DefaultIfEmpty()
                        join urr in _roleRepository.GetAll() on ur.RoleId equals urr.Id into urrJoined
                        from urr in urrJoined.DefaultIfEmpty()
                        join up in _userPermissionRepository.GetAll()
                            .Where(userPermission => input.Permissions.Contains(userPermission.Name)) on user.Id equals up.UserId into upJoined
                        from up in upJoined.DefaultIfEmpty()
                        join rp in _rolePermissionRepository.GetAll()
                            .Where(rolePermission => input.Permissions.Contains(rolePermission.Name)) on
                            new { RoleId = ur == null ? 0 : ur.RoleId } equals new { rp.RoleId } into rpJoined
                        from rp in rpJoined.DefaultIfEmpty()
                        where (up != null && up.IsGranted) ||
                              (up == null && rp != null && rp.IsGranted) ||
                              (up == null && rp == null && staticRoleNames.Contains(urr.Name))
                        select user;
            }

            return query;
        }

        public async Task<PagedResultDto<UserListDto>> GetAllUsersExceptLoggedIn()
        {
             var query = UserManager.Users.Where(p=>p.Id != AbpSession.GetUserId());

            var userCount = await query.CountAsync();

            var users = await query
                .ToListAsync();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
                );
        }

        public void  UpdateUserToken(UserTokenModel input)
        {
            UpdateFuncation(input);

        }

        private static void UpdateFuncation(UserTokenModel input)
        {

            try
            {
                string spName = "[dbo].[UserTokenAddUpdate]";
                var sqlParameters = new List<SqlParameter> {
                            new SqlParameter("@UserId",input.UserId),
                            new SqlParameter("@DeviceId",input.DeviceId),
                            new SqlParameter("@Token",input.Token),
                            new SqlParameter("@TenantId",input.TenantId),
                     };

                SqlDataHelper.ExecuteNoneQuery(
                   spName,
                    sqlParameters.ToArray(),
                   AppSettingsModel.ConnectionStrings);

            }
            catch(Exception e)
            {

            }
 
        }

        public List<UserTokenModel> GetUserToken(int? tenantId, string userIds = null)
        {
            IList<UserTokenModel> result = GetFuncation(tenantId,userIds);

            return result.ToList();
        }

        public async Task<List<UserListDto>> GetUsersBot(int? tenantId, string userIds = null)
        {
          
            var users = GetFuncationUserBot(tenantId, userIds );
            return users;
        }


        public void SaveSetting(long UserId, WorkModel workModel)
        {
            try
            {
                workModel.StartDateFri = checkValidValue(workModel.StartDateFri);
                workModel.StartDateSat = checkValidValue(workModel.StartDateSat);
                workModel.StartDateSun = checkValidValue(workModel.StartDateSun);
                workModel.StartDateMon = checkValidValue(workModel.StartDateMon);
                workModel.StartDateTues = checkValidValue(workModel.StartDateTues);
                workModel.StartDateWed = checkValidValue(workModel.StartDateWed);
                workModel.StartDateThurs = checkValidValue(workModel.StartDateThurs);

                workModel.EndDateFri = checkValidValue(workModel.EndDateFri);
                workModel.EndDateSat = checkValidValue(workModel.EndDateSat);
                workModel.EndDateSun = checkValidValue(workModel.EndDateSun);
                workModel.EndDateMon = checkValidValue(workModel.EndDateMon);
                workModel.EndDateTues = checkValidValue(workModel.EndDateTues);
                workModel.EndDateWed = checkValidValue(workModel.EndDateWed);
                workModel.EndDateThurs = checkValidValue(workModel.EndDateThurs);


                workModel.StartDateFriSP = checkValidValue(workModel.StartDateFriSP);
                workModel.StartDateSatSP = checkValidValue(workModel.StartDateSatSP);
                workModel.StartDateSunSP = checkValidValue(workModel.StartDateSunSP);
                workModel.StartDateMonSP = checkValidValue(workModel.StartDateMonSP);
                workModel.StartDateTuesSP = checkValidValue(workModel.StartDateTuesSP);
                workModel.StartDateWedSP = checkValidValue(workModel.StartDateWedSP);
                workModel.StartDateThursSP = checkValidValue(workModel.StartDateThursSP);

                workModel.EndDateFriSP = checkValidValue(workModel.EndDateFriSP);
                workModel.EndDateSatSP = checkValidValue(workModel.EndDateSatSP);
                workModel.EndDateSunSP = checkValidValue(workModel.EndDateSunSP);
                workModel.EndDateMonSP = checkValidValue(workModel.EndDateMonSP);
                workModel.EndDateTuesSP = checkValidValue(workModel.EndDateTuesSP);
                workModel.EndDateWedSP = checkValidValue(workModel.EndDateWedSP);
                workModel.EndDateThursSP = checkValidValue(workModel.EndDateThursSP);

                var BookingCapacity = 1;
                try
                {
                    BookingCapacity=int.Parse(workModel.WorkTextAR);
                }
                catch
                {

                }


                UserSettingUpdate(UserId, JsonConvert.SerializeObject(workModel, Formatting.Indented), BookingCapacity);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public WorkModel GetUserSetting(long UserID)
        {
            return getUserSetting(UserID);
        }


        private static IList<UserTokenModel> GetFuncation(int? tenantId, string userIds = null)
        {

            string spName = "[dbo].[UserTokenGet]";
            var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@TenantId",tenantId),
                new SqlParameter("@UserIds",userIds)
            };


            IList<UserTokenModel> result = SqlDataHelper.ExecuteReader(spName,sqlParameters.ToArray(),MapUserToken,AppSettingsModel.ConnectionStrings);
            return result;
        }

        private static UserTokenModel MapUserToken(IDataReader dataReader)
        {
            UserTokenModel model = new UserTokenModel
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                DeviceId = SqlDataHelper.GetValue<string>(dataReader, "DeviceId"),
                TenantId = SqlDataHelper.GetValue<int>(dataReader, "TenantId"),
                Token = SqlDataHelper.GetValue<string>(dataReader, "Token"),
                UserId = SqlDataHelper.GetValue<string>(dataReader, "UserId")


            };
            return model;
        }

        private static List<UserListDto> GetFuncationUserBot(int? tenantId, string userIds = null)
        {

            string spName = "[dbo].[GetUser]";
            var sqlParameters = new List<SqlParameter> {
                new SqlParameter("@TenantId",tenantId),
                new SqlParameter("@UserIds",userIds)
            };


            IList<UserListDto> result = SqlDataHelper.ExecuteReader(spName, sqlParameters.ToArray(), MapUserBot, AppSettingsModel.ConnectionStrings);
            return result.ToList();
        }

        private static UserListDto MapUserBot(IDataReader dataReader)
        {
            UserListDto model = new UserListDto
            {
                Id = SqlDataHelper.GetValue<long>(dataReader, "Id"),
                Name = SqlDataHelper.GetValue<string>(dataReader, "Name"),
                Surname = SqlDataHelper.GetValue<int>(dataReader, "Surname")


            };
            return model;
        }

        private string getValidValue(dynamic value)
        {
            string result = null;
            try
            {
                result = value.ToString();
                return result;

            }
            catch (Exception)
            {
                // return null if get unexpected value 

                return result;
            }


        }

        private string checkValidValue(dynamic value)
        {
            string result = null;
            try
            {

                DateTime dateTime = DateTime.Parse(value.ToString());
                dateTime = dateTime.AddHours(AppSettingsModel.AddHour);
                result = dateTime.ToString("HH:mm");
                return result;

            }
            catch (Exception)
            {
                // return null if get unexpected value 
                return result;
            }
        }







        private void UserSettingUpdate(long UserId, string UserSettingJson, int BookingCapacity)
        {
            try
            {
                var SP_Name = "[dbo].[UserSettingUpdate]";

                var sqlParameters = new List<SqlParameter> {
             new SqlParameter("@SettingJson",UserSettingJson)
            ,new SqlParameter("@UserId",UserId)
            ,new SqlParameter("@BookingCapacity",BookingCapacity)
            };

                SqlDataHelper.ExecuteNoneQuery(SP_Name, sqlParameters.ToArray(), AppSettingsModel.ConnectionStrings);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private WorkModel getUserSetting(long UserID)
        {
            try
            {
                WorkModel entity = new WorkModel();
                var SP_Name = "UserSettingGet";

                var sqlParameters = new List<SqlParameter> {
                    new SqlParameter("@UserID",UserID)
                };

                entity = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), DataReaderMapper.MapBranchSetting, AppSettingsModel.ConnectionStrings).FirstOrDefault();


                if (entity != null)
                {
                    entity.StartDateFri = getValidValue(entity.StartDateFri);
                    entity.StartDateSat = getValidValue(entity.StartDateSat);
                    entity.StartDateSun = getValidValue(entity.StartDateSun);
                    entity.StartDateMon = getValidValue(entity.StartDateMon);
                    entity.StartDateTues = getValidValue(entity.StartDateTues);
                    entity.StartDateWed = getValidValue(entity.StartDateWed);
                    entity.StartDateThurs = getValidValue(entity.StartDateThurs);



                    entity.EndDateFri = getValidValue(entity.EndDateFri);
                    entity.EndDateSat = getValidValue(entity.EndDateSat);
                    entity.EndDateSun = getValidValue(entity.EndDateSun);
                    entity.EndDateMon = getValidValue(entity.EndDateMon);
                    entity.EndDateTues = getValidValue(entity.EndDateTues);
                    entity.EndDateWed = getValidValue(entity.EndDateWed);
                    entity.EndDateThurs = getValidValue(entity.EndDateThurs);



                    entity.StartDateFriSP = getValidValue(entity.StartDateFriSP);
                    entity.StartDateSatSP = getValidValue(entity.StartDateSatSP);
                    entity.StartDateSunSP = getValidValue(entity.StartDateSunSP);
                    entity.StartDateMonSP = getValidValue(entity.StartDateMonSP);
                    entity.StartDateTuesSP = getValidValue(entity.StartDateTuesSP);
                    entity.StartDateWedSP = getValidValue(entity.StartDateWedSP);
                    entity.StartDateThursSP = getValidValue(entity.StartDateThursSP);



                    entity.EndDateFriSP = getValidValue(entity.EndDateFriSP);
                    entity.EndDateSatSP = getValidValue(entity.EndDateSatSP);
                    entity.EndDateSunSP = getValidValue(entity.EndDateSunSP);
                    entity.EndDateMonSP = getValidValue(entity.EndDateMonSP);
                    entity.EndDateTuesSP = getValidValue(entity.EndDateTuesSP);
                    entity.EndDateWedSP = getValidValue(entity.EndDateWedSP);
                    entity.EndDateThursSP = getValidValue(entity.EndDateThursSP);
                }

                return entity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetTenantID(int id)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from [dbo].[AbpTenants] where Id=" + id;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Area> branches = new List<Area>();



            string IP = "";


            try
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        IP = dataSet.Tables[0].Rows[i]["ClientIpAddress"].ToString();

                    }
                    catch
                    {
                        IP = null;

                    }

                }

            }
            catch
            {


            }

            conn.Close();
            da.Dispose();

            return IP;
        }
        private string GetUserIpAddresse(int id, int UserId)
        {

            string connString = AppSettingsModel.ConnectionStrings;
            string query = "select * from AbpUserLoginAttempts where TenantId="+id+" and UserId="+UserId+" ORDER BY Id DESC" ;

            SqlConnection conn = new SqlConnection(connString);
            SqlCommand cmd = new SqlCommand(query, conn);
            conn.Open();

            // create the DataSet 
            DataSet dataSet = new DataSet();

            // create data adapter
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            // this will query your database and return the result to your datatable
            da.Fill(dataSet);

            List<Area> branches = new List<Area>();



            string IP = "";


            try
            {
                for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        IP = dataSet.Tables[0].Rows[i]["ClientIpAddress"].ToString();

                    }
                    catch
                    {
                        IP = null;

                    }
                    break;
                }

            }
            catch
            {


            }

            conn.Close();
            da.Dispose();

            return IP;
        }
    }
}
