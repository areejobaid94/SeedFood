using Infoseed.MessagingPortal.InfoSeedServices.Dtos;

namespace Infoseed.MessagingPortal.TenantServices.Dtos
{
    public class GetTenantServiceForViewDto
    {
        public TenantServiceDto TenantService { get; set; }
        public string InfoSeedServiceServiceName { get; set; }

        public string ServiceName { get; set; }
        public string ServiceFees { get; set; }
        public string ServiceCreationDate { get; set; }
        public string ServiceStoppingDate { get; set; }

        public string Remarks { get; set; }

    }
}