using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Configuration.Tenants.Dto;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Authorization.Users
{
    public interface IUserAppService : IApplicationService
    {
        Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input);

        Task<FileDto> GetUsersToExcel(GetUsersToExcelInput input);

        Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input);

        Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input);

        Task ResetUserSpecificPermissions(EntityDto<long> input);

        Task UpdateUserPermissions(UpdateUserPermissionsInput input);

        Task CreateOrUpdateUser(CreateOrUpdateUserInput input);

        Task DeleteUser(EntityDto<long> input);

        Task UnlockUser(EntityDto<long> input);

         void UpdateUserToken(UserTokenModel input);
        List<UserTokenModel> GetUserToken(int? tenantId,string userIds = null);

        Task<List<UserListDto>> GetUsersBot(int? tenantId, string userIds = null);
        Task<List<UserListDto>> GetBookingUsers(int tenantId ,string userIds = null);

        WorkModel GetUserSetting(long UserID);
         void SaveSetting(long UserId, WorkModel workModel);

        bool CheckIpAddress(int? id, int UserId);


        UserTicketsModel UserTicketsOpenUpdate(long userId, bool IsOpen);
        void UserTicketsUpdate(long userId, int MaximumTickets);
        UserTicketsModel UserTicketsGet(long userId);
    }
}