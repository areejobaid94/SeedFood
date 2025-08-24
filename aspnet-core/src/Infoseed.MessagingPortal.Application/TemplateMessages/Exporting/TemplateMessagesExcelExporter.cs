using System.Collections.Generic;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using Infoseed.MessagingPortal.DataExporting.Excel.NPOI;
using Infoseed.MessagingPortal.TemplateMessages.Dtos;
using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Storage;

namespace Infoseed.MessagingPortal.TemplateMessages.Exporting
{
    public class TemplateMessagesExcelExporter : NpoiExcelExporterBase, ITemplateMessagesExcelExporter
    {

        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public TemplateMessagesExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager) :
    base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<GetTemplateMessageForViewDto> templateMessages)
        {
            return CreateExcelPackage(
                "TemplateMessages.xlsx",
                excelPackage =>
                {

                    var sheet = excelPackage.CreateSheet(L("TemplateMessages"));

                    AddHeader(
                        sheet,
                        L("TemplateMessageName"),
                        L("MessageCreationDate"),
                        (L("TemplateMessagePurpose")) + L("Purpose")
                        );

                    AddObjects(
                        sheet, 2, templateMessages,
                        _ => _.TemplateMessage.TemplateMessageName,
                        _ => _timeZoneConverter.Convert(_.TemplateMessage.MessageCreationDate, _abpSession.TenantId, _abpSession.GetUserId()),
                        _ => _.TemplateMessagePurposePurpose
                        );

                    for (var i = 1; i <= templateMessages.Count; i++)
                    {
                        SetCellDataFormat(sheet.GetRow(i).Cells[2], "yyyy-mm-dd");
                    }
                    sheet.AutoSizeColumn(2);
                });
        }
    }
}