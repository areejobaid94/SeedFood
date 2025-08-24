using System.Collections.Generic;
using Infoseed.MessagingPortal.Cities.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Cities.Exporting
{
    public interface ICitiesExcelExporter
    {
        FileDto ExportToFile(List<GetCityForViewDto> cities);
    }
}