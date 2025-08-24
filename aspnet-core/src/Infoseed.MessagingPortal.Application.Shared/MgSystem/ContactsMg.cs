using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal.MgSystem
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
            public Lastmodifieddate lastmodifieddate { get; set; }
        }

        public class Lastmodifieddate
        {
            public string value { get; set; }
        }

        public class IdentityProfiles
        {
            public int vid { get; set; }
            public long savedattimestamp { get; set; }
            public int deletedchangedtimestamp { get; set; }
            public Identity[] identities { get; set; }
        }

        public class Identity
        {
            public string type { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
        }


    }
}
