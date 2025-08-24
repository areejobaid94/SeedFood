using Infoseed.MessagingPortal.CaptionBot.Dtos;
using Infoseed.MessagingPortal.Customers.Dtos;
using Infoseed.MessagingPortal.Web.Models.Sunshine;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class BotStepModel
    {
        public TenantModel  tenantModel { get; set; }
        public List<CaptionDto>  captionDtos { get; set; }
        public CustomerModel customerModel { get; set; }
        public List<Activity> Bot { get; set; }
        public CustomerBehaviourModel customerBehaviourModel { get; set; }

        public string CancelText { get; set; }
        public string DetailText { get; set; }
        public bool isDelete { get; set; }

        public bool isFall { get; set; }
    }
}
