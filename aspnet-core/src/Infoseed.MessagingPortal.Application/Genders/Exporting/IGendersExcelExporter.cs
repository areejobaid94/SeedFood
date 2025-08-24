using System.Collections.Generic;
using Infoseed.MessagingPortal.Genders.Dtos;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Genders.Exporting
{
    public interface IGendersExcelExporter
    {
        FileDto ExportToFile(List<GetGenderForViewDto> genders);
    }
}