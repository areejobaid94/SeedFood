using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Menus.Dtos
{
    public class MenuReminderMessages
    {
        public long Id { get; set; }
        public DateTime CreationDate { get; set; }= DateTime.UtcNow;
        public bool IsActive { get; set; }
        public int ContactId { get; set; }
        public int TenantId { get; set; }
        public string PhoneNumber { get; set; }
    }
}
