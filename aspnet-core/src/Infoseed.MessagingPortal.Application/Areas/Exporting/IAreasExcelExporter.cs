using System.Collections.Generic;
using Infoseed.MessagingPortal.Areas.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Areas.Exporting
{
    public interface IAreasExcelExporter
    {
        FileDto ExportToFile(List<GetAreaForViewDto> areas);
    }
}