using Infoseed.MessagingPortal.Contacts.Dtos;
using Infoseed.MessagingPortal.Customers.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Customers
{
    public interface ICustomerBehaviourAppService
    {
        void CreateInterestedOf(CustomerInterestedOf interestedOf);
        void UpdateCustomerBehavior(CustomerBehaviourModel behaviourModel);
        void UpdateContactName(int id, string name);
        void UpdateContactkinship(int id, string name);
        void UpdateCustomerBehaviorByUser(CustomerBehaviourModel behaviourModel);
    }
}
