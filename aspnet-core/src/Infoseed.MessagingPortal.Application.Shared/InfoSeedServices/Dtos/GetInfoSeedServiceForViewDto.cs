namespace Infoseed.MessagingPortal.InfoSeedServices.Dtos
{
    public class GetInfoSeedServiceForViewDto
    {
        public InfoSeedServiceDto InfoSeedService { get; set; }

        public string ServiceTypeServicetypeName { get; set; }

        public string ServiceStatusServiceStatusName { get; set; }

        public string ServiceFrquencyServiceFrequencyName { get; set; }

    }
}