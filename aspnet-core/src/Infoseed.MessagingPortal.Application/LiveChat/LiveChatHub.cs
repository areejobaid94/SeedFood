
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.Tenants.Contacts;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.LiveChat
{
    public class LiveChatHub : Hub
    {
        public async Task brodCastBotLiveChat(CustomerModel data) => await Clients.All.SendAsync("brodCastBotLiveChat", data);

    }
}
