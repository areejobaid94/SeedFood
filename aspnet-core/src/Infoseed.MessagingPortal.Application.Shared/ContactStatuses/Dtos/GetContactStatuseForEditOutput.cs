using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.ContactStatuses.Dtos
{
    public class GetContactStatuseForEditOutput
    {
        public CreateOrEditContactStatuseDto ContactStatuse { get; set; }

    }
}