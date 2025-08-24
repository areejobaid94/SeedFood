using Infoseed.MessagingPortal.Logic;
using Infoseed.MessagingPortal.MgSystem;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using static Infoseed.MessagingPortal.MgSystem.CreateContactMg;
using static Infoseed.MessagingPortal.MgSystem.CreateTicket;

namespace Infoseed.MessagingPortal.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MGApiController : MessagingPortalControllerBase
    {
        public IContactsAPI _contactsAPI;
        public ITicketsAPI _ticketsAPI;
        public MGApiController()
        {
            _contactsAPI=new ContactsAPI("https://api.hubapi.com/", "pat-na1-0772c044-8365-4ce5-95be-529519d19f41");
            _ticketsAPI=new TicketsAPI("https://api.hubapi.com/", "pat-na1-0772c044-8365-4ce5-95be-529519d19f41");
        }

        [Route("GetContactsMg")]
        [HttpGet]
        public void GetContactsMg()
        {
            var x = _contactsAPI.GetContactsMg();

           
        }
        [Route("SearchContactsMg")]
        [HttpGet]
        public int SearchContactsMg(string phoneNumber)
        {
            var x = _contactsAPI.SearchContactsMg(phoneNumber);

            return x;
        }

        [Route("CreateContactsMg")]
        [HttpPost]
        public void CreateContactsMg(CreateContactFromInfoSeed model)
        {
            CreateContactMg createContactMg = CreateContactMgFun(model);

            _contactsAPI.Create(createContactMg);


        }

    

        [Route("CreateTicketsMg")]
        [HttpPost]
        public TicketMg CreateTicketsMg(SendTicketMg model)
        {
            List<CreateTicket> properties = CreateFun(model);

            var ticket = _ticketsAPI.Create(properties.ToArray());

            var contactId = _contactsAPI.SearchContactsMg(model.phoneNumber);

            UpdateFun(ticket, contactId);

            return ticket;

        }



        [Route("UpdateTicketsMg")]
        [HttpPost]
        public void UpdateTicketsMg(string ticketId, string contactID)
        {

            UpdateTicketsMg updateTicketsMg = new UpdateTicketsMg();
            List<UpdateTicketsMg.Input> input = new List<UpdateTicketsMg.Input>();
            UpdateTicketsMg.Input inp = new UpdateTicketsMg.Input {
            
               from=new UpdateTicketsMg.From { id="251" },
                to=new UpdateTicketsMg.To { id="830955707" },
                 type="contact_to_ticket"

            };


            input.Add(inp);

            updateTicketsMg.inputs=input.ToArray();

           

            _ticketsAPI.UpdateTicketsMg(updateTicketsMg);


        }




        private static List<CreateTicket> CreateFun(SendTicketMg model)
        {
            CreateTicket subject = new CreateTicket { name="subject", value=model.subject };
            CreateTicket content = new CreateTicket { name="content", value=model.content };



            CreateTicket hs_pipeline = new CreateTicket { name="hs_pipeline", value="0" };
            CreateTicket hs_pipeline_stage = new CreateTicket { name="hs_pipeline_stage", value="2" };
            CreateTicket hs_ticket_priority = new CreateTicket { name="hs_ticket_priority", value="HIGH" };
            CreateTicket hs_ticket_category = new CreateTicket { name="hs_ticket_category", value="GENERAL_INQUIRY" };


            List<CreateTicket> properties = new List<CreateTicket>();

            properties.Add(subject);
            properties.Add(content);
            properties.Add(hs_pipeline);
            properties.Add(hs_pipeline_stage);
            properties.Add(hs_ticket_priority);
            properties.Add(hs_ticket_category);
            return properties;
        }

        private void UpdateFun(TicketMg ticket, int contactId)
        {
            UpdateTicketsMg updateTicketsMg = new UpdateTicketsMg();
            List<UpdateTicketsMg.Input> input = new List<UpdateTicketsMg.Input>();
            UpdateTicketsMg.Input inp = new UpdateTicketsMg.Input
            {

                from=new UpdateTicketsMg.From { id=contactId.ToString() },
                to=new UpdateTicketsMg.To { id=ticket.objectId.ToString() },
                type="contact_to_ticket"

            };
            input.Add(inp);
            updateTicketsMg.inputs=input.ToArray();
            _ticketsAPI.UpdateTicketsMg(updateTicketsMg);
        }

        private static CreateContactMg CreateContactMgFun(CreateContactFromInfoSeed model)
        {
            model.company="infoseed";
            model.email=model.company+"-"+model.phone+"@infoseed.com";

            CreateContactMg createContactMg = new CreateContactMg();
            Property1 property1 = new Property1 { property="email", value=model.email };
            Property1 firstname = new Property1 { property="firstname", value=model.firstname };
            Property1 lastname = new Property1 { property="lastname", value=model.lastname };
            //  Property1 website = new Property1 { property="website", value="http://hubspot.com" };
            Property1 company = new Property1 { property="company", value=model.company };
            Property1 phone = new Property1 { property="phone", value=model.phone };
            // Property1 address = new Property1 { property="address", value="25 First Street" };
            //  Property1 city = new Property1 { property="city", value="Cambridge" };
            //  Property1 state = new Property1 { property="state", value="MA" };
            // Property1 zip = new Property1 { property="zip", value="02139" };
            List<Property1> properties = new List<Property1>();

            properties.Add(property1);
            properties.Add(firstname);
            properties.Add(lastname);
            //  properties.Add(website);
            properties.Add(company);
            properties.Add(phone);
            // properties.Add(address);
            //  properties.Add(city);
            //  properties.Add(state);
            //  properties.Add(zip);

            createContactMg.properties=properties.ToArray();
            return createContactMg;
        }
    }
}
