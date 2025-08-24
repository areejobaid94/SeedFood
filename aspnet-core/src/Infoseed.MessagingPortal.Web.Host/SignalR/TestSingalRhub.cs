using Infoseed.MessagingPortal.Orders.Exporting;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.SignalR
{
 
    public class TestSingalRhub : Hub
    {
        public async Task broadcastTestSingalRt(TestSingalRModel data) => await Clients.All.SendAsync("broadcastTestSingalR", data);

    }
}
