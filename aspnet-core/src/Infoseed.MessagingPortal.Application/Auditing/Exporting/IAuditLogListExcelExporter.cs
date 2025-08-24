using System.Collections.Generic;
using Infoseed.MessagingPortal.Auditing.Dto;
using Infoseed.MessagingPortal.Dto;

namespace Infoseed.MessagingPortal.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);

        FileDto ExportToFile(List<EntityChangeListDto> entityChangeListDtos);
    }
}
