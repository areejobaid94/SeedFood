using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.MenuItemStatuses
{
	[Table("MenuItemStatuses")]
    public class MenuItemStatus : Entity<long> 
    {

		[Required]
		[StringLength(MenuItemStatusConsts.MaxNameLength, MinimumLength = MenuItemStatusConsts.MinNameLength)]
		public virtual string Name { get; set; }
		

    }
}