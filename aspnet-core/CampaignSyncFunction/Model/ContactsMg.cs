using System;
using System.Collections.Generic;
using System.Text;

namespace CampaignSyncFunction.Model
{
    public class ContactsMg
    {


        public Contact[] contacts { get; set; }
        public bool hasmore { get; set; }
        public int vidoffset { get; set; }


        public class Contact
        {
            public long addedAt { get; set; }
            public int vid { get; set; }
            public int canonicalvid { get; set; }
            public object[] mergedvids { get; set; }
            public int portalid { get; set; }
            public bool iscontact { get; set; }
            public Properties properties { get; set; }
            public object[] formsubmissions { get; set; }
            public IdentityProfiles[] identityprofiles { get; set; }
            public object[] mergeaudits { get; set; }
        }
        public class Properties
        {
            public Car__cloned__1 car__cloned__1 { get; set; }
            public Hs_Latest_Source_Data_2 hs_latest_source_data_2 { get; set; }
            public Hs_Email_Hard_Bounce_Reason_Enum hs_email_hard_bounce_reason_enum { get; set; }
            public Hs_Is_Unworked hs_is_unworked { get; set; }
            public Hs_Latest_Source_Data_1 hs_latest_source_data_1 { get; set; }
            public Associatedcompanyid associatedcompanyid { get; set; }
            public Firstname firstname { get; set; }
            public City city { get; set; }
            public Hs_Latest_Source hs_latest_source { get; set; }
            public Num_Unique_Conversion_Events num_unique_conversion_events { get; set; }
            public Hs_Analytics_Revenue hs_analytics_revenue { get; set; }
            public Hs_Pipeline hs_pipeline { get; set; }
            public Createdate createdate { get; set; }
            public Hs_Social_Num_Broadcast_Clicks hs_social_num_broadcast_clicks { get; set; }
            public Hs_Analytics_Num_Visits hs_analytics_num_visits { get; set; }
            public Hs_Sequences_Actively_Enrolled_Count hs_sequences_actively_enrolled_count { get; set; }
            public Hs_Social_Linkedin_Clicks hs_social_linkedin_clicks { get; set; }
            public Hs_Analytics_Source hs_analytics_source { get; set; }
            public Hs_Searchable_Calculated_Phone_Number hs_searchable_calculated_phone_number { get; set; }
            public Hs_Analytics_Num_Page_Views hs_analytics_num_page_views { get; set; }
            public Hs_Email_Domain hs_email_domain { get; set; }
            public Company company { get; set; }
            public State state { get; set; }
            public Email email { get; set; }
            public Hs_Latest_Source_Timestamp hs_latest_source_timestamp { get; set; }
            public Zip zip { get; set; }
            public Website website { get; set; }
            public Address address { get; set; }
            public Hs_Analytics_First_Timestamp hs_analytics_first_timestamp { get; set; }
            public Lastmodifieddate lastmodifieddate { get; set; }
            public Hs_Social_Google_Plus_Clicks hs_social_google_plus_clicks { get; set; }
            public Hs_Analytics_Average_Page_Views hs_analytics_average_page_views { get; set; }
            public Hs_All_Contact_Vids hs_all_contact_vids { get; set; }
            public Lastname lastname { get; set; }
            public Hs_Social_Facebook_Clicks hs_social_facebook_clicks { get; set; }
            public Hs_Is_Contact hs_is_contact { get; set; }
            public Phone phone { get; set; }
            public Num_Conversion_Events num_conversion_events { get; set; }
            public Hs_Object_Id hs_object_id { get; set; }
            public Hs_Analytics_Num_Event_Completions hs_analytics_num_event_completions { get; set; }
            public Hs_Analytics_Source_Data_2 hs_analytics_source_data_2 { get; set; }
            public Hs_Social_Twitter_Clicks hs_social_twitter_clicks { get; set; }
            public Hs_Analytics_Source_Data_1 hs_analytics_source_data_1 { get; set; }
            public Hs_Lifecyclestage_Lead_Date hs_lifecyclestage_lead_date { get; set; }
            public Lifecyclestage lifecyclestage { get; set; }
        }
        public class Car__cloned__1
        {
            public string value { get; set; }
        }
        public class Hs_Latest_Source_Data_2
        {
            public string value { get; set; }
        }

        public class Hs_Email_Hard_Bounce_Reason_Enum
        {
            public string value { get; set; }
        }

        public class Hs_Is_Unworked
        {
            public string value { get; set; }
        }

        public class Hs_Latest_Source_Data_1
        {
            public string value { get; set; }
        }

        public class Associatedcompanyid
        {
            public string value { get; set; }
        }

        public class Firstname
        {
            public string value { get; set; }
        }

        public class City
        {
            public string value { get; set; }
        }

        public class Hs_Latest_Source
        {
            public string value { get; set; }
        }

        public class Num_Unique_Conversion_Events
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_Revenue
        {
            public string value { get; set; }
        }

        public class Hs_Pipeline
        {
            public string value { get; set; }
        }

        public class Createdate
        {
            public string value { get; set; }
        }

        public class Hs_Social_Num_Broadcast_Clicks
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_Num_Visits
        {
            public string value { get; set; }
        }

        public class Hs_Sequences_Actively_Enrolled_Count
        {
            public string value { get; set; }
        }

        public class Hs_Social_Linkedin_Clicks
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_Source
        {
            public string value { get; set; }
        }

        public class Hs_Searchable_Calculated_Phone_Number
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_Num_Page_Views
        {
            public string value { get; set; }
        }

        public class Hs_Email_Domain
        {
            public string value { get; set; }
        }

        public class Company
        {
            public string value { get; set; }
        }

        public class State
        {
            public string value { get; set; }
        }

        public class Email
        {
            public string value { get; set; }
        }

        public class Hs_Latest_Source_Timestamp
        {
            public string value { get; set; }
        }

        public class Zip
        {
            public string value { get; set; }
        }

        public class Website
        {
            public string value { get; set; }
        }

        public class Address
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_First_Timestamp
        {
            public string value { get; set; }
        }

        public class Lastmodifieddate
        {
            public string value { get; set; }
        }

        public class Hs_Social_Google_Plus_Clicks
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_Average_Page_Views
        {
            public string value { get; set; }
        }

        public class Hs_All_Contact_Vids
        {
            public string value { get; set; }
        }

        public class Lastname
        {
            public string value { get; set; }
        }

        public class Hs_Social_Facebook_Clicks
        {
            public string value { get; set; }
        }

        public class Hs_Is_Contact
        {
            public string value { get; set; }
        }

        public class Phone
        {
            public string value { get; set; }
        }

        public class Num_Conversion_Events
        {
            public string value { get; set; }
        }

        public class Hs_Object_Id
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_Num_Event_Completions
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_Source_Data_2
        {
            public string value { get; set; }
        }

        public class Hs_Social_Twitter_Clicks
        {
            public string value { get; set; }
        }

        public class Hs_Analytics_Source_Data_1
        {
            public string value { get; set; }
        }

        public class Hs_Lifecyclestage_Lead_Date
        {
            public string value { get; set; }
        }

        public class Lifecyclestage
        {
            public string value { get; set; }
        }

        public class IdentityProfiles
        {
            public int vid { get; set; }
            public int savedattimestamp { get; set; }
            public int deletedchangedtimestamp { get; set; }
            public Identity[] identities { get; set; }
        }

        public class Identity
        {
            public string type { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public bool isprimary { get; set; }
        }








    }
}
