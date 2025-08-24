using Infoseed.MessagingPortal.InfoSeedParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedParser.Interfaces
{
   public  interface IMenuParser
    {
        ParseMenuModel Parse(ParseConfig parserOptions);
    }
}
