using Framework.Integration.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Integration.Interfaces
{
    public interface IContactsAPI
    {
        ContactsMg GetContactsMg();
        ContactsMg GetContactsMg(string phoneNumber);
        int SearchContactsMg(string phoneNumber);
        void Create(CreateContactMg contact);
        void UpdateContactsMg(CreateContactMg contact, string vid);
    }
}
