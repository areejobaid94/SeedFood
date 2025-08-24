
using Infoseed.MessagingPortal.MgSystem;
using Newtonsoft.Json;

namespace Infoseed.MessagingPortal.Logic
{
    public class TicketsAPI : APIBase, ITicketsAPI
    {
        public TicketsAPI(string baseUrl, string key) : base(baseUrl, key)
        {
            
        }

        public TicketMg Create(CreateTicket[] ticket)
        {           
                var result = HttpPost(BaseUrl + "crm-objects/v1/objects/tickets", ticket);
              return JsonConvert.DeserializeObject<TicketMg>(result);
        }

        public void CreateTickets(CreateTicket ticket)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateTicketsMg(UpdateTicketsMg ticket)
        {
            var result = HttpPost(BaseUrl + "crm/v3/associations/contact/ticket/batch/create", ticket);
        }



    }
}
