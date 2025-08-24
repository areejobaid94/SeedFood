using Infoseed.MessagingPortal.Maintenance.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Maintenance
{
    public class MaintenancesHub : Hub
    {
        public async Task brodCastBotOrder(GetMaintenancesForViewDto data) => await Clients.All.SendAsync("MaintenancesBotOrder", data);
    }
}
