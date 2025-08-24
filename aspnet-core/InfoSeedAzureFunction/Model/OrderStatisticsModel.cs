using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.Model
{
    public class OrderStatisticsModel
    {
        public long TotalOrder { get; set; }
        public int PercentagePending { get; set; }
        public int PercentageCompleted { get; set; }
        public int PercentageDeleted { get; set; }
        public int PercentageCanceled { get; set; }
        public int PercentagePreOrder { get; set; }
        public long TotalOrderPending { get; set; }
        public long TotalOrderCompleted { get; set; }
        public long TotalOrderDeleted { get; set; }
        public long TotalOrderCanceled { get; set; }
        public long TotalOrderPreOrder { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal RewardPoints { get; set; }
        public decimal RedeemedPoints { get; set; }
        public long TotalTakeaway { get; set; }
        public long TotalDelivery { get; set; }
        public decimal DeliveryCost { get; set; }
    }
}
