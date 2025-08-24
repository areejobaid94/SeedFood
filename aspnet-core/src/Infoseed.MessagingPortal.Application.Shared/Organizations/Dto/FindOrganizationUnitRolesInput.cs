using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Organizations.Dto
{
    public class FindOrganizationUnitRolesInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}