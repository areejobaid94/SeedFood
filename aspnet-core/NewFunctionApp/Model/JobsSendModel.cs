using System;
using System.Collections.Generic;
using System.Text;

namespace NewFunctionApp
{
    public class JobsSendModel
    {

            public string tag { get; set; }
            public Tasks tasks { get; set; }
        

        public class Tasks
        {
            public Import1 import1 { get; set; }
            public ConvertMp3 convertmp3 { get; set; }
            public Export export { get; set; }
        }

        public class Import1
        {
            public string operation { get; set; }
            public string url { get; set; }
            public string filename { get; set; }
        }

        public class ConvertMp3
        {
            public string[] input { get; set; }
            public string operation { get; set; }
            public string engine { get; set; }
            public string input_format { get; set; }
            public string output_format { get; set; }
        }

        public class Export
        {
            public string input { get; set; }
            public string operation { get; set; }
            public bool inline_additional { get; set; }
            public bool archive_multiple_files { get; set; }
        }

    }
}
