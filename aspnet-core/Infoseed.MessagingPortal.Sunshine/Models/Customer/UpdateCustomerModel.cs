using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Sunshine.Models
{
    public class UpdateCustomerModel
    {
        public string UserId { get; set; }

        public string DisplayName { get; set; }

        public string PhoneNumber { get; set; }

        public string Website { get; set; }

        public string EmailAddress { get; set; }

        public string Description { get; set; }
    }
}
