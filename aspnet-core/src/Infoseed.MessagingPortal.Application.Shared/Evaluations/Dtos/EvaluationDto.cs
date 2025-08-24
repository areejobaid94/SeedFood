using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Evaluations.Dtos
{
    public class EvaluationDto : EntityDto<long>
    {
		public int? TenantId { get; set; }


		public  long OrderNumber { get; set; }


		public  string ContactName { get; set; }


		public  string EvaluationsText { get; set; }

		public string EvaluationsReat { get; set; }

		public  DateTime CreationTime { get; set; }


		public  int OrderId { get; set; }

		public virtual string PhoneNumber { get; set; }
	}
}
