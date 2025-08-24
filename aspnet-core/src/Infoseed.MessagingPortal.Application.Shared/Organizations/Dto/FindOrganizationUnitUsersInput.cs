using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Organizations.Dto
{
    public class FindOrganizationUnitUsersInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}
