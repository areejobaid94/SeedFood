using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MultiTenancy.Dto
{
    public class DataExportToExcelHost
    {
        public long TotalTickets { get; set; }
        public long TotalPending { get; set; }
        public long TotalOpened { get; set; }
        public long TotalClosed { get; set; }
        public long TotalExpired { get; set; }
        public int PercentagePending { get; set; }
        public int PercentageOpened { get; set; }
        public int PercentageClosed { get; set; }
        public int PercentageExpired { get; set; }
        public decimal AvgResolutionTime { get; set; }

        public string TenantName { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string TenancyName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationTime { get; set; }
        public string InvoiceId { get; set; }

        public string PhoneNumber { get; set; }

        public string DomainName { get; set; }
        public string CustomerName { get; set; }

    }
}
