using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.ExtraOrderDetails.Dtos
{
    public class CreateOrEditExtraOrderDetailsDto
    {
		public int? TenantId { get; set; }
		public virtual long? Id { get; set; }
		public virtual string Name { get; set; }
		public virtual string NameEnglish { get; set; }
		public virtual int? Quantity { get; set; }

		public virtual decimal? UnitPrice { get; set; }

		public virtual decimal? Total { get; set; }

		public virtual long? OrderDetailId { get; set; }
	}
}
