using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.WhatsApp.Dto
{
    public class InstagramModel
    {


        public string _object { get; set; }
        public Entry435[] entry { get; set; }


        public class Entry435
        {
            public long time { get; set; }
            public string id { get; set; }
            public Messagingerter[] messaging { get; set; }
        }

        public class Messagingerter
        {
            public Sender3453 sender { get; set; }
            public Recipient3453 recipient { get; set; }
            public long timestamp { get; set; }
            public Message3453 message { get; set; }
        }

        public class Sender3453
        {
            public string id { get; set; }
        }

        public class Recipient3453
        {
            public string id { get; set; }
        }

        public class Message3453
        {
            public string mid { get; set; }
            public string text { get; set; }



                public Attachment[] attachments { get; set; }
            

           

        }
        public class Attachment
        {
            public string type { get; set; }
            public Payload payload { get; set; }
        }

        public class Payload
        {
            public string url { get; set; }
        }
        //public string _object { get; set; }
        //public Entry12[] entry { get; set; }


        //public class Entry12
        //{
        //    public long time { get; set; }
        //    public string id { get; set; }
        //    public Standby12[] standby { get; set; }
        //}

        //public class Standby12
        //{
        //    public Sender23 sender { get; set; }
        //    public Recipient34 recipient { get; set; }
        //    public long timestamp { get; set; }
        //    public Message56 message { get; set; }





        //}
        //public class Attachment
        //{
        //    public string type { get; set; }
        //    public Payload payload { get; set; }
        //}

        //public class Payload
        //{
        //    public string url { get; set; }
        //}
        //public class Sender23
        //{
        //    public string id { get; set; }
        //}

        //public class Recipient34
        //{
        //    public string id { get; set; }
        //}

        //public class Message56
        //{
        //    public string mid { get; set; }
        //    public string text { get; set; }

        //    public Attachment[] attachments { get; set; }
        //}

    }






}
