using Infoseed.MessagingPortal.Sunshine.Models;
using Microsoft.AspNetCore.SignalR;
using System;

using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.SignalR
{
    public class TeamInboxHub : Hub
    {

        public async Task broadcastTeamInboxMessage(CustomerChat data) => await Clients.All.SendAsync("broadcastTeamInboxMessage", data);

    }
}
