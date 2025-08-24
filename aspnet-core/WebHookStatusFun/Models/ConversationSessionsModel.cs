using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebHookStatusFun
{
    public class ConversationSessionsModel
    {
        public int Id { get; set; }
        public int creation_timestamp { get; set; }
        public int expiration_timestamp { get; set; }


    }
}
