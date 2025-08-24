using System.Collections.Generic;
using Infoseed.MessagingPortal.InfoSeedServices.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.InfoSeedServices.Exporting
{
    public interface IInfoSeedServicesExcelExporter
    {
        FileDto ExportToFile(List<GetInfoSeedServiceForViewDto> infoSeedServices);
    }
}