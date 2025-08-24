using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.TenantServices.Dtos
{
   public class TenantServiceModalDto
    {
        /// <summary>
        /// Gets or sets the TenantId.
        /// </summary>
        public int? TenantId { get; set; }
        /// <summary>
        /// Gets or sets the ServiceId.
        /// </summary>
        public int ServiceId { get; set; }
        /// <summary>
        /// Gets or sets the ServiceName.
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// Gets or sets the Fees.
        /// </summary>
        public Decimal Fees { get; set; }
        /// <summary>
        /// Gets or sets the TenantServiceCreationTime.
        /// </summary>
        public DateTime? TenantServiceCreationTime { get; set; }
    /// <summary>
    /// Gets or sets the IsSelected.
    /// </summary>
    public bool IsSelected { get; set; }

        public int FirstNumberOfOrders { get; set; }
        public decimal FeesForFirstOrder { get; set; }

        public bool IsFeesPerTransaction { get; set; }
    }
}
