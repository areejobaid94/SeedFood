
using Infoseed.MessagingPortal.LiveChat.Dto;
using Infoseed.MessagingPortal.SealingReuest.Dto;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.SealingReuest
{
    public class SellingRequestHub : Hub
    {
        public async Task brodCastSellingRequest(SellingRequestDto data) => await Clients.All.SendAsync("brodCastSellingRequest", data);

    }
}
