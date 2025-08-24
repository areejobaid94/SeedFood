using Infoseed.MessagingPortal.Dto;
using Infoseed.MessagingPortal.Orders.Dtos;
using Infoseed.MessagingPortal.Tenants.Dashboard.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Exporting
{
    public interface IUsageDetailsExcelExport
    {
        FileDto ExportToFile(List<UsageDetailsModel> orders);
    }
}
