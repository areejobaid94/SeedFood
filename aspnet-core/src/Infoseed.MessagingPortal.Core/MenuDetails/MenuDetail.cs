using Infoseed.MessagingPortal.Items;
using Infoseed.MessagingPortal.Menus;
using Infoseed.MessagingPortal.MenuItemStatuses;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Entities;

namespace Infoseed.MessagingPortal.MenuDetails
{
	[Table("MenuDetails")]
    public class MenuDetail : Entity<long> , IMayHaveTenant
    {
			public int? TenantId { get; set; }
			

		public virtual string Description { get; set; }
		
		public virtual bool IsStandAlone { get; set; }
		
		public virtual decimal? Price { get; set; }
		

		//public virtual long ItemId { get; set; }
		
  //      [ForeignKey("ItemId")]
		//public Item ItemFk { get; set; }
		
		//public virtual long MenuId { get; set; }
		
  //      [ForeignKey("MenuId")]
		//public Menu MenuFk { get; set; }
		
		//public virtual long? MenuItemStatusId { get; set; }
		
  //      [ForeignKey("MenuItemStatusId")]
		//public MenuItemStatus MenuItemStatusFk { get; set; }
		
    }
}