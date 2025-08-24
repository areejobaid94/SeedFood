//using System.Collections.Generic;
//using Abp.Runtime.Session;
//using Abp.Timing.Timezone;
//using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
//using Infoseed.MessagingPortal.Territories.Dtos;
//using Infoseed.MessagingPortal.Dto;
//using Infoseed.MessagingPortal.Storage;

//namespace Infoseed.MessagingPortal.Territories.Exporting
//{
//    public class TerritoriesExcelExporter : NpoiExcelExporterBase, ITerritoriesExcelExporter
//    {

//        private readonly ITimeZoneConverter _timeZoneConverter;
//        private readonly IAbpSession _abpSession;

//        public TerritoriesExcelExporter(
//            ITimeZoneConverter timeZoneConverter,
//            IAbpSession abpSession,
//            ITempFileCacheManager tempFileCacheManager) :
//    base(tempFileCacheManager)
//        {
//            _timeZoneConverter = timeZoneConverter;
//            _abpSession = abpSession;
//        }

//        public FileDto ExportToFile(List<GetTerritoryForViewDto> territories)
//        {
//            return CreateExcelPackage(
//                "Territories.xlsx",
//                excelPackage =>
//                {

//                    var sheet = excelPackage.CreateSheet(L("Territories"));

//                    AddHeader(
//                        sheet,
//                        L("UserName"),
//                        L("EnglishName"),
//                        L("ArabicName"),
//                        L("FacebookUri"),
//                        L("Phone"),
//                        L("Email"),
//                        L("Age"),
//                        L("CreationDate")
//                        );

//                    AddObjects(
//                        sheet, 2, territories,
//                        _ => _.Territory.UserName,
//                        _ => _.Territory.EnglishName,
//                        _ => _.Territory.ArabicName,
//                        _ => _.Territory.FacebookUri,
//                        _ => _.Territory.Phone,
//                        _ => _.Territory.Email,
//                        _ => _.Territory.Age,
//                        _ => _timeZoneConverter.Convert(_.Territory.CreationDate, _abpSession.TenantId, _abpSession.GetUserId())
//                        );

//                    for (var i = 1; i <= territories.Count; i++)
//                    {
//                        SetCellDataFormat(sheet.GetRow(i).Cells[8], "yyyy-mm-dd");
//                    }
//                    sheet.AutoSizeColumn(8);
//                });
//        }
//    }
//}