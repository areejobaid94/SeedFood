using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.MenuDetails.Dtos
{
    public class MenuDetailItemLookupTableDto
    {
		public long Id { get; set; }

		public string DisplayName { get; set; }
        public string DisplayNameEnglish { get; set; }
    }
}