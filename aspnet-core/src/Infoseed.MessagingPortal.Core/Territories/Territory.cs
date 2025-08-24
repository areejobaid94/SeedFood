using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.Territories
{
    [Table("Territories")]
    public class Territory : Entity
    {

        public virtual string UserName { get; set; }

        public virtual string EnglishName { get; set; }

        public virtual string ArabicName { get; set; }

        public virtual string FacebookUri { get; set; }

        public virtual string Phone { get; set; }

        public virtual string Email { get; set; }

        public virtual string Age { get; set; }

        public virtual DateTime CreationDate { get; set; }

    }
}