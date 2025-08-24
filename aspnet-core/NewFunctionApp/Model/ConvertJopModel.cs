using System;
using System.Collections.Generic;
using System.Text;

namespace NewFunctionApp
{
    public class ConvertJopModel
    {

       
        public Data data { get; set; }
        

        public class Data
        {
            public string id { get; set; }
            public string tag { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
            public object started_at { get; set; }
            public object ended_at { get; set; }
            public Task[] tasks { get; set; }
            public Links links { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
        }

        public class Task
        {
            public string id { get; set; }
            public string name { get; set; }
            public string job_id { get; set; }
            public string status { get; set; }
            public object credits { get; set; }
            public object code { get; set; }
            public object message { get; set; }
            public int percent { get; set; }
            public string operation { get; set; }
            public object result { get; set; }
            public DateTime created_at { get; set; }
            public object started_at { get; set; }
            public object ended_at { get; set; }
            public object retry_of_task_id { get; set; }
            public object copy_of_task_id { get; set; }
            public int user_id { get; set; }
            public int priority { get; set; }
            public object host_name { get; set; }
            public object storage { get; set; }
            public string[] depends_on_task_ids { get; set; }
            public Links1 links { get; set; }
            public object engine { get; set; }
            public object engine_version { get; set; }
        }

        public class Links1
        {
            public string self { get; set; }
        }

    }
}
