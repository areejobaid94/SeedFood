
using System;
using Abp.Application.Services.Dto;

namespace Infoseed.MessagingPortal.Genders.Dtos
{
    public class GenderDto : EntityDto<long>
    {
		public string Name { get; set; }



    }
}