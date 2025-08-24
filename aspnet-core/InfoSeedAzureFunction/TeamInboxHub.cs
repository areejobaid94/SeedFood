using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.AspNetCore.SignalR;
using System;

using System.Threading.Tasks;

namespace InfoSeedAzureFunction

{
    public class TeamInboxHub : Hub
    {

        public async Task broadcastTeamInboxMessage(CustomerChat data) => await Clients.All.SendAsync("broadcastTeamInboxMessage", data);

    }
}
