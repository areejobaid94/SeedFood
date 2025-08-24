using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Genders.Dtos
{
    public class GetGenderForEditOutput
    {
		public CreateOrEditGenderDto Gender { get; set; }


    }
}