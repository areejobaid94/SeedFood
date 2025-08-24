using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.OrderDetails.Dtos
{
    public class OrderDetailMenuLookupTableDto
    {
		public long Id { get; set; }

		public string DisplayName { get; set; }
        public string DisplayNameEnglish { get; set; }
    }
}