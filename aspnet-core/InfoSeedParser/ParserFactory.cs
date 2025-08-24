using InfoSeedParser.Interfaces;
using InfoSeedParser.Parsers;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedParser
{
    public class ParserFactory
    {
        public IMenuParser CreateParser(string type)
        {
            switch (type)
            {
                case nameof(MenuExcelParser):
                    return new MenuExcelParser();
            }
            throw new NotSupportedException();
        }

        public IItemParser CreateParserItem(string type)
        {
            switch (type)
            {
                case nameof(ItemExcelParser):
                    return new ItemExcelParser();
            }
            throw new NotSupportedException();
        }

        public IContactParser CreateParserContact(string type)
        {
            switch (type)
            {
                case nameof(ContactExcelParser):
                    return new ContactExcelParser();
            }
            throw new NotSupportedException();
        }
        public IContactNewParser CreateNewParserContact(string type)
        {
            switch (type)
            {
                case nameof(ContactExcelNewParser):
                    return new ContactExcelNewParser();
            }
            throw new NotSupportedException();
        }
        public IMembersGroupParser CreateParserMembers(string type)
        {
            switch (type)
            {
                case nameof(MembersGroupParser):
                    return new MembersGroupParser();
            }
            throw new NotSupportedException();
        }
    }
}
