using System;
using System.Collections.Generic;
using System.Text;

namespace NewFunctionApp
{
    public class MenuReminderMessages
    {
        public long Id { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsActive { get; set; }
        public int ContactId { get; set; }
        public int TenantId { get; set; }
        public string PhoneNumber { get; set; }    
    }
}
