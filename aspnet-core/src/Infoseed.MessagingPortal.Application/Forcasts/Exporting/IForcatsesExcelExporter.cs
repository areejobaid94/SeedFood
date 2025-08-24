using System.Collections.Generic;
using Infoseed.MessagingPortal.Forcasts.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Forcasts.Exporting
{
    public interface IForcatsesExcelExporter
    {
        FileDto ExportToFile(List<GetForcatsForViewDto> forcatses);
    }
}