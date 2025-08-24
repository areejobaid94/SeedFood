using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Infoseed.MessagingPortal.SalesUserCreate
{
    [Table("SalesUserCreate")]
    public class SalesUserCreate : Entity, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public virtual int UserId { get; set; }
        public virtual string UserName { get; set; }

        public virtual int TotalCreate { get; set; }

        public virtual bool IsActiveButton { get; set; }

        public virtual bool IsActiveSubmitButton { get; set; }
    }
}