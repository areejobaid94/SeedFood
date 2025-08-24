using Abp.Application.Services.Dto;
using System;

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class GetAllContactsInput : PagedAndSortedResultRequestDto
    {
        public string Filter { get; set; }

        public string DisplayNameFilter { get; set; }

        public string PhoneNumberFilter { get; set; }

        public int? IsLockedByAgentFilter { get; set; }

        public string LockedByAgentNameFilter { get; set; }

        public int? IsOpenFilter { get; set; }

        public string EmailAddressFilter { get; set; }

        public string UserIdFilter { get; set; }

        public string ChatStatuseChatStatusNameFilter { get; set; }

        public string ContactStatuseContactStatusNameFilter { get; set; }


        public int DeliveryOrder { get; set; }
        public int TakeAwayOrder { get; set; }
        public int TotalOrder { get; set; }
        public int loyalityPoint { get; set; }

    }
}