using System.Collections.Generic;
using Infoseed.MessagingPortal.Authorization.Users.Importing.Dto;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Authorization.Users.Importing
{
    public interface IInvalidUserExporter
    {
        FileDto ExportToFile(List<ImportUserDto> userListDtos);
    }
}
