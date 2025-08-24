using Framework.Integration.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Integration.Interfaces
{
    public interface ITicketsAPI
    {
        TicketMg Create(CreateTicket[] ticket);
        void CreateTickets(CreateTicket ticket);
        void UpdateTicketsMg(UpdateTicketsMg ticket);
    }
}
