using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.Evaluations
{
	[Table("Evaluations")]
	public class Evaluation : Entity<long>, IMayHaveTenant
	{
		public int? TenantId { get; set; }


		public virtual long OrderNumber { get; set; }


		public virtual string ContactName { get; set; }
		

		[Required]
		public virtual string EvaluationsText { get; set; }

		public virtual string EvaluationsReat { get; set; }


		public virtual DateTime CreationTime { get; set; }

		
		public virtual string PhoneNumber { get; set; }

		public virtual int OrderId { get; set; }
		public virtual int EvaluationRate { get; set; }


	}
}