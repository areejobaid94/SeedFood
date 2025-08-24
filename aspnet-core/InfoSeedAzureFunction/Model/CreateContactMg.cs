using System;
using System.Collections.Generic;
using System.Text;

namespace InfoSeedAzureFunction.Model
{
    public class CreateContactMg
    {
        public CreateAppointmentMGModel createAppointmentMGModel { get; set; }
        public int ContactId { get; set; }

        public int? levelTwoId { get; set; }

        public string vid { get; set; }

        public Property1[] properties { get; set; }


        public class Property1
        {
            public string property { get; set; }
            public string value { get; set; }
        }

    }
}
