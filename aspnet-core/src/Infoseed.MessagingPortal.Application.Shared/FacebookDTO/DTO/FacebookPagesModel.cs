using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.FacebookDTO.DTO
{
    public class FacebookPagesModel
    {

            public Datum122[] data { get; set; }
            public Paging122 paging { get; set; }
        

        public class Paging122
        {
            public Cursors122 cursors { get; set; }
        }

        public class Cursors122
        {
            public string before { get; set; }
            public string after { get; set; }
        }

        public class Datum122
        {
            public string access_token { get; set; }
            public string category { get; set; }
            public Category_List122[] category_list { get; set; }
            public string name { get; set; }
            public string id { get; set; }
            public string[] tasks { get; set; }

            public bool isSubscribe { get; set; } = false;
        }

        public class Category_List122
        {
            public string id { get; set; }
            public string name { get; set; }
        }

    }
}
