using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;
using System.Collections.Generic;
using Infoseed.MessagingPortal.Evaluations.Dtos;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using System;

namespace Infoseed.MessagingPortal.Evaluations.Exporting
{
  public  class EvaluationExcelExporter : NpoiExcelExporterBase, IEvaluationExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public EvaluationExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
               base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetEvaluationForViewDto> contacts)
        {


                return CreateExcelPackage(
                      "evaluat.xlsx",
                      excelPackage =>
                      {

                          var sheet = excelPackage.CreateSheet(L("Contacts"));

                          AddHeader(
                              sheet,
                              L("Order"),
                              L("Customer"),
                              L("PhoneNumber"),
                              L("Evaluation"),
                              L("Rate"),
                              L("Date"),
                              L("Time")

                              );

                          AddObjects(
                              sheet, 2, contacts,
                              _ => _.evaluation.OrderNumber,
                              _ => _.evaluation.ContactName,
                              _ => _.evaluation.PhoneNumber,
                              _ => _.evaluation.EvaluationsText,
                              _ => _.evaluation.EvaluationsReat,
                              _ => _.CreatDate,
                              _ => _.CreatTime
                              );

                      });

            

        }
    
    }
}