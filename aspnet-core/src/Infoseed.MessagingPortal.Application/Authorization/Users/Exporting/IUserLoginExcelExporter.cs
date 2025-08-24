using System.Collections.Generic;
using Infoseed.MessagingPortal.Authorization.Users.Dto;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Authorization.Users.Exporting
{
    public interface IUserLoginExcelExporter
    {
        FileDto ExportToFile(List<UserLoginAttemptDto> userListDtos);
    }
}