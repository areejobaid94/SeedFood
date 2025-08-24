using Infoseed.MessagingPortal.BotAPI.Models;
//using Infoseed.MessagingPortal.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infoseed.MessagingPortal.BotAPI.Helper
{
    public class Helper
    {
        //Download Attachment File
        public bool DownloadAttachment(string _url, out byte[] _content, out string _contentType, out long? _KBSize)
        {
            _KBSize = 0;
            _content = null;
            _contentType = "";
            try
            {
                using (var http = new HttpClient())
                {
                    var file_content = http.GetAsync(_url).Result;
                    var ByteSize = file_content.Content.Headers.ContentLength;
                    _KBSize = ByteSize / 1024;
                    _content = file_content.Content.ReadAsByteArrayAsync().Result;
                    _contentType = file_content.Content.Headers.ContentType.MediaType;
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //get Attacment Types Allowed
        public string GetContentType(string name)
        {
            try
            {
                var types = AppsettingsModel.AttacmentTypesAllowedM;
                var ext = Path.GetExtension(name).ToLowerInvariant();
                return types[ext];
            }
            catch
            {
                return null;
            }
     
        }
    }
}
