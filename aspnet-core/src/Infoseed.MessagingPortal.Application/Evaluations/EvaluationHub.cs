using Infoseed.MessagingPortal.Evaluations.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Evaluations
{
    public class EvaluationHub : Hub
    {
        public async Task brodCastBotEvaluation(EvaluationModel data) => await Clients.All.SendAsync("brodCastBotEvaluation", data);
    }
}
