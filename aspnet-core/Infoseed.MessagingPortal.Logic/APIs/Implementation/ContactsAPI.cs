
using Infoseed.MessagingPortal.MgSystem;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Infoseed.MessagingPortal.Logic
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

        public ContactsMg GetContactsMg()
        {
            var result = HttpGet(BaseUrl + "contacts/v1/lists/all/contacts/all");

            var rez= JsonConvert.DeserializeObject<ContactsMg>(result);
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
