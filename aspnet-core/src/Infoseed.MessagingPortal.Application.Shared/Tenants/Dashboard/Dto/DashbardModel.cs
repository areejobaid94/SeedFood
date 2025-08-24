using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Tenants.Dashboard.Dto
{
    public class DashbardModel
    {
        public BundleModel bundleModel { get; set; }
        public WalletModel walletModel { get; set; }
        public UserPerformanceOrderGenarecModel userPerformanceOrderModel { get; set; }
        public UserPerformanceTicketGenarecModel userPerformanceTicketModel { get; set; }
        public UserPerformanceBookingGenarecModel userPerformanceBookingModel { get; set; }
        //public OrderStatisticsModel orderStatisticsModel { get; set; }
        //public BookingStatisticsModel bookingStatisticsModel { get; set; }
        //public TicketsStatisticsModel ticketsStatisticsModel { get; set; }


    }
}
