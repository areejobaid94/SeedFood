using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Evaluations.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Evaluations.Exporting
{
    public interface IEvaluationExcelExporter
    {
        FileDto ExportToFile(List<GetEvaluationForViewDto> evaluation);
    }
}
