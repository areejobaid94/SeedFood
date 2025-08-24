using System.Collections.Generic;
using Infoseed.MessagingPortal.Menus.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Menus.Exporting
{
    public interface IMenusExcelExporter
    {
        FileDto ExportToFile(List<GetMenuForViewDto> menus);
    }
}