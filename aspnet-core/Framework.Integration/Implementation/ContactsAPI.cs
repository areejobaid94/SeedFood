using Framework.Integration.Interfaces;
using Framework.Integration.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Integration.Implementation
{
    public class ContactsAPI : APIBase, IContactsAPI
    {
        public ContactsAPI(string baseUrl, string key) : base(baseUrl, key)
        {

        }

        public void Create(CreateContactMg contact)
        {
            var result = HttpPost(BaseUrl + "contacts/v1/contact", contact);

        }
        public void UpdateContactsMg(CreateContactMg contact,string vid)
        {
            var result = HttpPost(BaseUrl + "contacts/v1/contact/vid/"+vid, contact);
        }

        public ContactsMg GetContactsMg(string phoneNumber)
        {
            var result = HttpGet(BaseUrl + "contacts/v1/search/query?q="+phoneNumber);

            var rez = JsonConvert.DeserializeObject<ContactsMg>(result);
            return rez;
        }
        public ContactsMg GetContactsMg()
        {
            var result = HttpGet(BaseUrl + "contacts/v1/lists/all/contacts/all");

            var rez = JsonConvert.DeserializeObject<ContactsMg>(result);
            return rez;
        }

        public int SearchContactsMg(string phoneNumber)
        {
            var result = HttpGet(BaseUrl + "contacts/v1/search/query?q="+phoneNumber);

            var rez = JsonConvert.DeserializeObject<SearchContactsMg>(result);
            return rez.contacts.FirstOrDefault().vid;
        }


    }
}
