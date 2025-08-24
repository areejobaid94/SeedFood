using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.ConservationMeasurements
{
    public class ConservationMeasurementsModel
    {
		public int Id { get; set; }
		public int? TenantId { get; set; }

		public int Year { get; set; }

		public int Month { get; set; }

		public int BusinessInitiatedCount { get; set; }
		public int UserInitiatedCount { get; set; }
		public int ReferralConversionCount { get; set; }
		public int TotalFreeConversation { get; set; }

		public DateTime LastUpdatedDate { get; set; }
		public DateTime CreationDate { get; set; }
	}
}
