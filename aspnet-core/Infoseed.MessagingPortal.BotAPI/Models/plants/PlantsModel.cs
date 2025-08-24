namespace Infoseed.MessagingPortal.BotAPI.Models.plants
{
    public class PlantsModel
    {
       
            public int id { get; set; }
            public object custom_id { get; set; }
            public Meta_Data meta_data { get; set; }
            public float uploaded_datetime { get; set; }
            public float finished_datetime { get; set; }
            public Image[] images { get; set; }
            public Suggestion[] suggestions { get; set; }
            public object[] modifiers { get; set; }
            public string secret { get; set; }
            public object fail_cause { get; set; }
            public bool countable { get; set; }
            public object feedback { get; set; }
            public float is_plant_probability { get; set; }
            public bool is_plant { get; set; }
        

        public class Meta_Data
        {
            public object latitude { get; set; }
            public object longitude { get; set; }
            public string date { get; set; }
            public string datetime { get; set; }
        }

        public class Image
        {
            public string file_name { get; set; }
            public string url { get; set; }
        }

        public class Suggestion
        {
            public int id { get; set; }
            public string plant_name { get; set; }
            public Plant_Details plant_details { get; set; }
            public float probability { get; set; }
            public bool confirmed { get; set; }
        }

        public class Plant_Details
        {
            public string language { get; set; }
            public string scientific_name { get; set; }
            public Structured_Name structured_name { get; set; }
        }

        public class Structured_Name
        {
            public string genus { get; set; }
            public string species { get; set; }
        }

    }
}
