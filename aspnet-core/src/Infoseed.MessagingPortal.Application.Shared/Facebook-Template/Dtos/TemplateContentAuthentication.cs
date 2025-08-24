using Castle.MicroKernel.Lifestyle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.Facebook_Template.Dtos
{
    public class TemplateContentAuthentication
    {
        public bool AddSecurityRecommendation { get; set; }
        public bool AddExpirationTimeForCode { get; set; }
        public int? ExpirationMinutes { get; set; }
        public bool IsValidExpirationTime()
        {
            return ExpirationMinutes.HasValue && ExpirationMinutes.Value >= 1 && ExpirationMinutes.Value <= 90;
        }
    }
}
