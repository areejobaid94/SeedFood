using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotAPI.Models
{
    public class CancelOrderModel
    {
        public bool IsTrueOrder { get; set; }
        public string TextCancelOrder { get; set; }
        public bool CancelOrder { get; set; }

        public bool WrongOrder { get; set; }
    }
}
