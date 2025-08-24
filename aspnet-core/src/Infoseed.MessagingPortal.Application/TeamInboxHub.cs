using Infoseed.MessagingPortal.Contacts;
using Infoseed.MessagingPortal.Orders.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal
{
    public class TeamInboxHub : Hub
    {

        public async Task broadcastTeamInboxMessage(CustomerChat2 data) => await Clients.All.SendAsync("broadcastTeamInboxMessage", data);
        public async Task brodCastBotOrder(CreateOrderModel data) => await Clients.All.SendAsync("brodCastBotOrder", data);
    }


	public class CustomerChat2
	{
		public DateTime lastNotificationsData { get; set; }
		public string notificationsText { get; set; }
		public string notificationID { get; set; }
		public int UnreadMessagesCount { get; set; }

		public int? TenantId { get; set; }
		public string messageId { get; set; }
		public string userId { get; set; }
		public string SunshineConversationId { get; set; }
		public DateTime CreateDate { get; set; }
		public string type { get; set; }
		public string text { get; set; }
		public int status { get; set; }
		public string fileName { get; set; }
		public Tenants.Contacts.MessageSenderType sender { get; set; }
		public string mediaUrl { get; set; }
		public string agentName { get; set; }
		public string agentId { get; set; }
		//public InfoSeedContainerItemTypes ItemType { get; } = InfoSeedContainerItemTypes.ConversationItem;
	}
}
