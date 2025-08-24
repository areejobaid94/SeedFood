using System.Collections.Generic;
using Infoseed.MessagingPortal.AccountBillings.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.AccountBillings.Exporting
{
    public interface IAccountBillingsExcelExporter
    {
        FileDto ExportToFile(List<GetAccountBillingForViewDto> accountBillings);
    }
}