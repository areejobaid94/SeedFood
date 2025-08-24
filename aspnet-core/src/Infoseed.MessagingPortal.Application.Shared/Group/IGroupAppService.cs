using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Infoseed.MessagingPortal.Group.Dto;
using Infoseed.MessagingPortal.Groups.Dto;
using Infoseed.MessagingPortal.InfoSeedParser;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Group
{
    public interface IGroupAppService : IApplicationService
    {
        GroupModel GroupGetAll(string searchTerm = "", int ? pageNumber = 0, int? pageSize = 10);
        GroupMembersDto GroupGetById(long groupId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10);
        PagedResultDto<MembersDto> MembersFilter(MembersFilter input);
        Task<GroupCreateDto> GroupCreateMembers(GroupMembersDto input, bool isExternal, bool markContactsAsUnsubscribed = false);
        Dictionary<string, dynamic> ValidGroupName(string groupName);
        GroupCreateDto MovingGroup(MoveMembersDto input);
        Task<ReadFromExcelDto> ReadFromExcel([FromForm] UploadFileModel file);

        Task<GroupCreateDto> GroupUpdate(GroupMembersDto input, bool isExternal, int statusId = 0);
        GroupModel GroupDelete(long groupId);
        Task<GroupLog> GroupLogGetAll(long groupId,string searchTerm = "", int? pageNumber = 0, int? pageSize = 10);
        GroupProgressDto GetGroupProgress(long groupId, int tenantId);



        //PagedResultDto<MembersDto> MembersGet(long groupId, string searchTerm = "", int? pageNumber = 0, int? pageSize = 10);
        //GroupCreateDto RemoveMembers(GroupMembersDto input);
        //GroupCreateDto GroupCreateOldMembers(GroupMembersDto input);
    }
}
