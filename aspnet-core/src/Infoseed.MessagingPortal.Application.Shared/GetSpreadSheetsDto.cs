using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infoseed.MessagingPortal
{
    public class GetSpreadSheetsDto
    {
        public string ErrorMsg { get; set; }
        public List<GoogleDriveFile> Files { get; set; }
    }

    public class GoogleDriveFile
    {
        public string Kind { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
    }

    public class DriveApiResponse
    {
        [JsonProperty("files")]
        public List<GoogleDriveFile> Files { get; set; }

        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("incompleteSearch")]
        public bool IncompleteSearch { get; set; }
    }

}
