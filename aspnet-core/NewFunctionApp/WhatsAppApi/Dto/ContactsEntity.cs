using System;
using System.Collections.Generic;
using System.Text;

namespace NewFunctionApp
{
    public class ContactsEntity
    {
        public List<WhatsAppContactsDto> contacts { get; set; }
        public int TotalCount { get; set; }
        public int TotalOptOut { get; set; }


    }
}
