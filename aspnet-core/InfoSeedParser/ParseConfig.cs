using InfoSeedParser.ConfigrationFile;
using InfoSeedParser.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedParser
{
    public class ParseConfig
    {
        public string FilePath { get; set; }
        public string Parser { get; set; }
        public ConfigrationExcelFile config { get; set; }
        public ItmeConfigrationExcelFile config2 { get; set; }
        public ContactConfigurationExcelFile ContactConfig { get; set; }
        public MembersConfigrationExcelFile MembersConfig { get; set; }
        public byte[] FileData { get; set; }
        public string FileName { get; set; }
    }
}
