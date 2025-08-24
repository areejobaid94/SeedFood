using Infoseed.MessagingPortal.TenantServices.Dtos;

namespace Infoseed.MessagingPortal.AccountBillings.Dtos
{
    public class GetAccountBillingForViewDto
    {
        public AccountBillingDto AccountBilling { get; set; }
        public TenantServiceDto TenantServiceDto { get; set; }

        public int  TotalOrder { get; set; }
        public bool IsFeesPerTransaction { get; set; }

        public string InfoSeedServiceServiceName { get; set; }

        public string ServiceTypeServicetypeName { get; set; }

        public string CurrencyCurrencyName { get; set; }

        public string BillingBillingID { get; set; }



    }
}