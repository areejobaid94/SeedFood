using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MultiTenancy.Dto
{
    public class ExportToExcelHost
    {
        // Tenant Information
        public string TenantName { get; set; }
        public int TenanId { get; set; }
        public string Name { get; set; }
        public string TenancyName { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public bool? Integration { get; set; }
        public DateTime? CreationTime { get; set; }
        public string InvoiceId { get; set; }
        public string PhoneNumber { get; set; }
        public string DomainName { get; set; }
        public string CustomerName { get; set; }

        // Wallet
        public decimal? WalletBalance { get; set; }

        // Current Week Ticket Statistics
        public long TotalTickets { get; set; }
        public long TotalPending { get; set; }
        public long TotalOpened { get; set; }
        public long TotalClosed { get; set; }
        public long TotalExpired { get; set; }
        //public DateTime? LastClosedTicketDate { get; set; }
        public decimal? AvgResolutionTime { get; set; }
        public DateTime? LastClosedTicketDate { get; set; }

        // Last Month Ticket Statistics
        public long? LastMonthTotalTickets { get; set; }
        public long? LastMonthTotalPending { get; set; }
        public long? LastMonthTotalOpened { get; set; }
        public long? LastMonthTotalClosed { get; set; }
        public long? LastMonthTotalExpired { get; set; }
        public DateTime? LastMonthLastClosedTicketDate { get; set; }

        // Current Week Order Statistics
        public long? TotalOrder { get; set; }
        public long? TotalOrderPending { get; set; }
        public long? TotalOrderCompleted { get; set; }
        public long? DoneOrders { get; set; } // If this is the same as TotalOrderCompleted, consider removing
        public long? TotalOrderDeleted { get; set; }
        public long? TotalOrderCanceled { get; set; }
        public long? TotalOrderPreOrder { get; set; }

        // Last Month Order Statistics
        public long? LastMonthTotalOrders { get; set; }
        public long? LastMonthPendingOrders { get; set; }
        public long? LastMonthDoneOrders { get; set; }
        public long? LastMonthDeletedOrders { get; set; }
        public long? LastMonthCanceledOrders { get; set; }
        public long? LastMonthPreOrders { get; set; }
    }
}
