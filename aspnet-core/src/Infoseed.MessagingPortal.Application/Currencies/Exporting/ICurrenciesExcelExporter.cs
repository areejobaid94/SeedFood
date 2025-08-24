using System.Collections.Generic;
using Infoseed.MessagingPortal.Currencies.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Currencies.Exporting
{
    public interface ICurrenciesExcelExporter
    {
        FileDto ExportToFile(List<GetCurrencyForViewDto> currencies);
    }
}