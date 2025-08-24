using System;

namespace BotService.Models.API
{
    public class MenuContcatKeyModel
    {
        public long Id { get; set; }
        public string KeyMenu { get; set; }
        public string Value { get; set; }

        public DateTime CreationTime { get; set; }
        public int ContactID { get; set; }
    }
}
