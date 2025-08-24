
using Infoseed.MessagingPortal.MgSystem;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Logic
{
    public interface IContactsAPI
    {
        ContactsMg GetContactsMg();
        int SearchContactsMg(string phoneNumber);
        void Create(CreateContactMg contact);
    }
}
