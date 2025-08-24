using System;
using Abp.Application.Services.Dto;
using System.ComponentModel.DataAnnotations;

namespace Infoseed.MessagingPortal.Customers.Dtos
{
    public class GetCustomerForEditOutput
    {
		public CreateOrEditCustomerDto Customer { get; set; }

		public string GenderName { get; set;}

		public string CityName { get; set;}


    }
}