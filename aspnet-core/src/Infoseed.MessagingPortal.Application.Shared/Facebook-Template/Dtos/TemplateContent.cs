using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Facebook_Template.Dtos
{
    public class TemplateContent
    {
        public HeaderContent HeaderContent { get; set; }
        public string HeaderText { get; set; }
        public string Body { get; set; }
        public string Footer { get; set; }
        public string mediaLink { get; set; }  
        //public string VideoUrl { get; set; }  
        //public string DocumentUrl { get; set; }

    }
    public enum HeaderContent
    {   
        none,
        Text,
        Image,
        Video,
        Document
    }

}
