using Abp.Application.Services;
using Infoseed.MessagingPortal.Teams.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Teams
{
    public interface ITeamsAppService : IApplicationService
    {
        TeamsModel TeamsGetAll(string searchTerm = "", int? pageNumber = 0, int? pageSize = 10);
        TeamsMembersDto TeamsGetById(long teamsId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10);
        Task<TeamsCreateDto> TeamsCreateMembers(TeamsMembersDto input);
        Dictionary<string, dynamic> ValidTeamsName(string teamsName , bool isCreate = true);
        Task<TeamsCreateDto> TeamsUpdate(TeamsMembersDto input, bool isExternal, int statusId = 0);
        TeamsModel TeamsDelete(long teamsId);
        Task<TeamsLog> TeamsLogGetAll(long teamsId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10);

    }
}
