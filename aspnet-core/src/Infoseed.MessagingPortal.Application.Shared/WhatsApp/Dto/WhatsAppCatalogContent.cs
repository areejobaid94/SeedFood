using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class WhatsAppCatalogContent : WhatsAppContent
    {
        public string catalogId { get; set; }
        public string headerText { get; set; }
        public string footerText { get; set; }
        public string sectionTitle { get; set; }
    }

    public class CatalogMessageRequest
    {
        public string messaging_product { get; set; } = "whatsapp";
        public string recipient_type { get; set; } = "individual";
        public string to { get; set; }
        public string type { get; set; } = "interactive";
        public CatalogInteractive interactive { get; set; }
    }

    public class CatalogInteractive
    {
        public string type { get; set; } = "catalog_message";
        public CatalogBody body { get; set; }
        public CatalogAction action { get; set; }
        public CatalogHeader header { get; set; }
        public CatalogFooter footer { get; set; }
    }

    public class CatalogBody
    {
        public string text { get; set; }
    }

    public class CatalogAction
    {
        public string name { get; set; } = "catalog_message";
        public CatalogParameters parameters { get; set; }
    }

    public class CatalogParameters
    {
        public string catalog_id { get; set; }
        public string section_title { get; set; }
    }

    public class CatalogHeader
    {
        public string type { get; set; } = "text";
        public string text { get; set; }
    }
    public class CatalogFooter
    {
        public string text { get; set; }
    }
}
