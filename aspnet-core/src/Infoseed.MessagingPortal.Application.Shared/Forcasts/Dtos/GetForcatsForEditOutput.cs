using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Forcasts.Dtos
{
    public class GetForcatsForEditOutput
    {
        public CreateOrEditForcatsDto Forcats { get; set; }

    }
}