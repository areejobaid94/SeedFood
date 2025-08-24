using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Authorization.Users.Exporting;
using System;
using Framework.Data;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using System.Data;

namespace Infoseed.MessagingPortal.Authorization.Users
{
    [AbpAuthorize]
    public class UserLoginAppService : MessagingPortalAppServiceBase, IUserLoginAppService
    {
        private readonly IRepository<UserLoginAttempt, long> _userLoginAttemptRepository;
        private readonly IUserLoginExcelExporter _userListExcelExporter;
        public UserLoginAppService(IRepository<UserLoginAttempt, long> userLoginAttemptRepository, IUserLoginExcelExporter userListExcelExporter)
        {
            _userListExcelExporter = userListExcelExporter;
            _userLoginAttemptRepository = userLoginAttemptRepository;
        }

        [DisableAuditing]
        public async Task<ListResultDto<UserLoginAttemptDto>> GetRecentUserLoginAttempts()
        {
            var userId = AbpSession.GetUserId();

            var loginAttempts = await _userLoginAttemptRepository.GetAll()
                .Where(la => la.UserId == userId)
                .OrderByDescending(la => la.CreationTime)
                .Take(10)
                .ToListAsync();

            return new ListResultDto<UserLoginAttemptDto>(ObjectMapper.Map<List<UserLoginAttemptDto>>(loginAttempts));
        }


        public async Task<FileDto> GetUsersLoginToExcel()
        {
            var loginAttempts = getUserLoginExcel();


            return _userListExcelExporter.ExportToFile(loginAttempts);
        }

        private List<UserLoginAttemptDto> getUserLoginExcel()
        {
            try
            {
                List<UserLoginAttemptDto> lstUserLogin = new List<UserLoginAttemptDto>();
                var SP_Name = "UserLoginExcelGet";

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>
                {
                   new System.Data.SqlClient.SqlParameter("@tenantId",AbpSession.TenantId.Value)

                };

                lstUserLogin = SqlDataHelper.ExecuteReader(SP_Name, sqlParameters.ToArray(), MapGetUserLogin, AppSettingsModel.ConnectionStrings).ToList();


                return lstUserLogin;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static UserLoginAttemptDto MapGetUserLogin(IDataReader dataReader)
        {
            try
            {
                UserLoginAttemptDto model = new UserLoginAttemptDto();

                model.UserNameOrEmail = SqlDataHelper.GetValue<string>(dataReader, "UserNameOrEmailAddress") ;
                model.ClientIpAddress = SqlDataHelper.GetValue<string>(dataReader, "ClientIpAddress");
                model.BrowserInfo = SqlDataHelper.GetValue<string>(dataReader, "BrowserInfo");
                model.CreationTime = SqlDataHelper.GetValue<DateTime>(dataReader, "CreationTime");


                model.formattedDate= model.CreationTime.AddHours(3).ToString("yyyy-MM-dd");

                model.formattedTime=model.CreationTime.AddHours(3).ToString("hh:mm tt");


                if (string.IsNullOrEmpty(model.UserNameOrEmail))
                {

                    model.UserNameOrEmail="1";
                }



                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}