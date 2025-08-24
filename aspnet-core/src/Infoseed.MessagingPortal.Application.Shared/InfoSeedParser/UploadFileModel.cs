using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.InfoSeedParser
{
   public  class UploadFileModel
    {
        public IFormFile FormFile { get; set; }

        public List<IFormFile> FormFileList { get; set; }
    }
}
