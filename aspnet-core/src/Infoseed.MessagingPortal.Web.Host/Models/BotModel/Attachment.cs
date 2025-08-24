using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.Web.Models.BotModel
{
    public class Attachment
    {

        public string Filename { get; set; }
        public string FileType { get; set; }
        public byte[] Base64 { get; set; }
    }
}
