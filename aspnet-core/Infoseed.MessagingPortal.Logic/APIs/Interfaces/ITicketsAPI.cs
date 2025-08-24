
using Infoseed.MessagingPortal.MgSystem;

namespace Infoseed.MessagingPortal.Logic
{
    public interface ITicketsAPI
    {
        TicketMg Create(CreateTicket[] ticket);
        void CreateTickets(CreateTicket ticket);
        void UpdateTicketsMg(UpdateTicketsMg ticket);
    }
}
