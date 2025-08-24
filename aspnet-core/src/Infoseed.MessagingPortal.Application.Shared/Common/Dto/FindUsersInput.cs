using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Common.Dto
{
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? TenantId { get; set; }

        public bool ExcludeCurrentUser { get; set; }
    }
}