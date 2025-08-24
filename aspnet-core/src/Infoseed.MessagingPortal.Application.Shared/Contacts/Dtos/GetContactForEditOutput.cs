using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Contacts.Dtos
{
    public class GetContactForEditOutput
    {
        public CreateOrEditContactDto Contact { get; set; }

        public string ChatStatuseChatStatusName { get; set; }

        public string ContactStatuseContactStatusName { get; set; }

    }
}