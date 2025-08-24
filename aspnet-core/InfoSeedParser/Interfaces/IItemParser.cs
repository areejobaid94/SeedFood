using Infoseed.MessagingPortal.InfoSeedParser;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedParser.Interfaces
{
    public interface IItemParser
    {
        ParseMenuModel Parse(ParseConfig parserOptions);
    }
}
