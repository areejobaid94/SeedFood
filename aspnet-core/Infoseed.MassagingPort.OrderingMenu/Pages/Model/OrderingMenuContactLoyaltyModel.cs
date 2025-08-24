using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.ContactStatuses;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Infoseed.MassagingPort.OrderingMenu.Pages.Model
{
    public class OrderingMenuContactLoyaltyModel
	{
		//public int AgentId { get; set; }
		public int? TenantId { get; set; }
		public virtual string DisplayName { get; set; }	
		public virtual string PhoneNumber { get; set; }
		public virtual bool IsLockedByAgent { get; set; }
		public virtual bool IsBlock { get; set; }

		public virtual string LockedByAgentName { get; set; }

		public virtual string EmailAddress { get; set; }

		public virtual string Description { get; set; }

		public virtual string UserId { get; set; }

		public virtual int? ContactStatuseId { get; set; }

		public decimal OriginalLoyaltyPoints { get; set; }
		public string ContactDisplayName { get; set; }


	

    }



   
}
