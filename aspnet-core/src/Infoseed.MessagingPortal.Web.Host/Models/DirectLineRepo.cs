using Infoseed.MessagingPortal.Web.Models.DirectLineBot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models
{
    public class DirectLineRepo
    {
        Dictionary<string, DirectlineConnector> _repo = new Dictionary<string, DirectlineConnector>();

        private readonly string _secret;
        public DirectLineRepo(string clientSecret)
        {
            _secret = clientSecret;
        }

        public DirectlineConnector GetClientConversation(string id)
        {
            if (_repo.ContainsKey(id))
            {
                return _repo[id];
            }
            else
            {
                // Create New Clinet 
                var client = new Microsoft.Bot.Connector.DirectLine.DirectLineClient(_secret);
                var botConnector = new DirectlineConnector(client, "whatsapp");
                _repo.Add(id, botConnector);
                return botConnector;
            }
        }
    }
}
