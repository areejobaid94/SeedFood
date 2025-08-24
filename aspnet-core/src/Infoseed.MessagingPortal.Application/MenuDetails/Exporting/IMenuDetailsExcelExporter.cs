using System.Collections.Generic;
using Infoseed.MessagingPortal.MenuDetails.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.MenuDetails.Exporting
{
    public interface IMenuDetailsExcelExporter
    {
        FileDto ExportToFile(List<GetMenuDetailForViewDto> menuDetails);
    }
}