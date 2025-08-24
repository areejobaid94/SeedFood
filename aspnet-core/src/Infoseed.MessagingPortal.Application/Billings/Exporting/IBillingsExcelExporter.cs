using System.Collections.Generic;
using Infoseed.MessagingPortal.Billings.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Billings.Exporting
{
    public interface IBillingsExcelExporter
    {
        FileDto ExportToFile(List<GetBillingForViewDto> billings);
    }
}