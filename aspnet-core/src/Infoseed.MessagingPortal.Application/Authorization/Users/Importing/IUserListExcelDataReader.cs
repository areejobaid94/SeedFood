using System.Collections.Generic;
using Infoseed.MessagingPortal.Authorization.Users.Importing.Dto;
using Abp.Dependency;

namespace Infoseed.MessagingPortal.Authorization.Users.Importing
{
    public interface IUserListExcelDataReader: ITransientDependency
    {
        List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes);
    }
}
