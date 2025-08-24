using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MgSystem
{
    public class TicketMg
    {

    
            public int portalId { get; set; }
            public string objectType { get; set; }
            public int objectId { get; set; }
            public Properties properties { get; set; }
            public bool isDeleted { get; set; }
        

        public class Properties
        {
            public Hs_Ticket_Category hs_ticket_category { get; set; }
            public Hs_Ticket_Id hs_ticket_id { get; set; }
            public Hs_Ticket_Priority hs_ticket_priority { get; set; }
            public Hs_Lastmodifieddate hs_lastmodifieddate { get; set; }
            public Subject subject { get; set; }
            public Hs_Pipeline hs_pipeline { get; set; }
            public Hs_Object_Id hs_object_id { get; set; }
            public Createdate createdate { get; set; }
            public Hs_Pipeline_Stage hs_pipeline_stage { get; set; }
            public Created_By created_by { get; set; }
            public Content content { get; set; }
        }

        public class Hs_Ticket_Category
        {
            public Version[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Hs_Ticket_Id
        {
            public Version1[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version1
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
        }

        public class Hs_Ticket_Priority
        {
            public Version2[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version2
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Hs_Lastmodifieddate
        {
            public Version3[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version3
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Subject
        {
            public Version4[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version4
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Hs_Pipeline
        {
            public Version5[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version5
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Hs_Object_Id
        {
            public Version6[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version6
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Createdate
        {
            public Version7[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version7
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Hs_Pipeline_Stage
        {
            public Version8[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version8
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Created_By
        {
            public Version9[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version9
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

        public class Content
        {
            public Version10[] versions { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public string sourceId { get; set; }
            public object updatedByUserId { get; set; }
        }

        public class Version10
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string sourceId { get; set; }
            public string source { get; set; }
            public object[] sourceVid { get; set; }
            public string requestId { get; set; }
        }

    }
}
