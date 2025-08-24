using Abp.AutoMapper;
using Infoseed.MessagingPortal.Organizations.Dto;

namespace Infoseed.MessagingPortal.Models.Users
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitModel : OrganizationUnitDto
    {
        public bool IsAssigned { get; set; }
    }
}