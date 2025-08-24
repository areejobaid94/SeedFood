using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Items.Dtos
{
    public class GetImageURLModel
    {
        public IFormFile FormFile { get; set; }
    }
}