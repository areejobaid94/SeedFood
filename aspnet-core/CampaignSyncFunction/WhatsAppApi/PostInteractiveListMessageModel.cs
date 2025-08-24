using System.Collections.Generic;

namespace CampaignSyncFunction.WhatsAppApi
{



    public class PostInteractiveListMessageModel
    {
        public string messaging_product { get; set; }
        public string recipient_type { get; set; }
        public string to { get; set; }
        public string type { get; set; }
       public InteractiveListModel interactive { get; set; }
    }

    public class InteractiveListModel
        {
            public string type { get; set; }
            public InteractiveHeaderListModel header { get; set; }
            public InteractiveBodyModel body { get; set; }
            public InteractiveFooterListModel footer { get; set; }
            public InteractiveActionListModel action { get; set; }
        }

        public class InteractiveHeaderListModel
        {
            public string type { get; set; }
            public string text { get; set; }
        }



        public class InteractiveFooterListModel
        {
            public string text { get; set; }
        }

        public class InteractiveActionListModel
        {
            public string button { get; set; }
            public List<InteractiveSectionListModel> sections { get; set; }
        }

        public class InteractiveSectionListModel
        {
            public string title { get; set; }
            public List<InteractiveRowListModel> rows { get; set; }
        }

        public class InteractiveRowListModel
        {
            public string id { get; set; }
            public string title { get; set; }
            public string description { get; set; }
        }
    





}
