using System;
using System.Collections.Generic;

namespace Infoseed.MessagingPortal.Facebook_Template.Dtos
{
    public class ButtonsTemplate
    {
        public List<CallToActionButton> CallToActionButtons { get; set; }
        public List<QuickReplyButton> QuickReplyButtons { get; set; }

    }

    public class CallToActionButton
    {
        public TypeOfAction ActionType { get; set; }
        public string ButtonText { get; set; }
        public URLType? UrlType { get; set; }
        public string WebsiteUrl { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string OfferCode { get; set; }
        public bool isUrlInvalid { get; set; } = false;

    }

    public class QuickReplyButton
    {
        public QuickReplyType ReplyType { get; set; }
        public string ButtonText { get; set; }
        public string FooterText { get; set; }
        public bool IsOptOutResponsibilityAcknowledged { get; set; }
        public bool YouDirectMeta { get; set; }

        
    }

    public enum TypeOfAction
    {
        VisitWebsite,
        CallPhoneNumber,
        CompleteFlow,
        CopyOfferCode,
    }

    public enum QuickReplyType
    {
        MarketingOptOut,
        Custom
    }

    public enum URLType
    {
        Static,
        Dynamic
    }
}
