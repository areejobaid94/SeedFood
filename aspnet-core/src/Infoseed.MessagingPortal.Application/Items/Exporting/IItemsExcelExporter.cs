using System.Collections.Generic;
using Infoseed.MessagingPortal.Items.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Items.Exporting
{
    public interface IItemsExcelExporter
    {
        FileDto ExportToFile(List<GetItemForViewDto> items);
    }
}