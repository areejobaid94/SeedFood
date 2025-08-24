using System.Collections.Generic;
using Infoseed.MessagingPortal.ReceiptDetails.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.ReceiptDetails.Exporting
{
    public interface IReceiptDetailsExcelExporter
    {
        FileDto ExportToFile(List<GetReceiptDetailForViewDto> receiptDetails);
    }
}