using Infoseed.MessagingPortal.TenantServices.Dtos;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Billings.Dtos
{
    public class GetBillingForViewDto
    {
        public List<TenantServiceModalDto> TenantService { get; set; }
        public BillingDto Billing { get; set; }

        public string CurrencyCurrencyName { get; set; }

        public string ServiceName { get; set; }

    }
}