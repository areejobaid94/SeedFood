using System;
using System.Collections.Generic;
using System.Text;

namespace NewFunctionApp
{
    public class GetUrlConvert
    {

        
            public Data data { get; set; }
        

        public class Data
        {
            public string id { get; set; }
            public string name { get; set; }
            public string job_id { get; set; }
            public string status { get; set; }
            public int credits { get; set; }
            public object code { get; set; }
            public object message { get; set; }
            public int percent { get; set; }
            public string operation { get; set; }
            public Payload payload { get; set; }
            public Result result { get; set; }
            public DateTime created_at { get; set; }
            public DateTime started_at { get; set; }
            public DateTime ended_at { get; set; }
            public object retry_of_task_id { get; set; }
            public object copy_of_task_id { get; set; }
            public int user_id { get; set; }
            public int priority { get; set; }
            public string host_name { get; set; }
            public string storage { get; set; }
            public string[] depends_on_task_ids { get; set; }
            public Links links { get; set; }
        }

        public class Payload
        {
            public string input { get; set; }
            public string operation { get; set; }
            public bool inline_additional { get; set; }
            public bool archive_multiple_files { get; set; }
        }

        public class Result
        {
            public File[] files { get; set; }
        }

        public class File
        {
            public string filename { get; set; }
            public int size { get; set; }
            public string url { get; set; }
            public string inline_url { get; set; }
        }

        public class Links
        {
            public string self { get; set; }
        }

    }
}
