using System.Collections.Generic;
using Infoseed.MessagingPortal.Receipts.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Receipts.Exporting
{
    public interface IReceiptsExcelExporter
    {
        FileDto ExportToFile(List<GetReceiptForViewDto> receipts);
    }
}