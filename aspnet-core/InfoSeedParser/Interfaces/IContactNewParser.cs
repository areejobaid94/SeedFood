using Infoseed.MessagingPortal.WhatsApp.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedParser.Interfaces
{
    public interface IContactNewParser
    {
        CampinToQueueDto ParseContactNew(ParseConfig parserOptions);
    }
}
