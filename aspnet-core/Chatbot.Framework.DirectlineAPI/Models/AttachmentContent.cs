﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatbot.Framework.DirectlineAPI.Models
{
    public class AttachmentContent
    {
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("actions")]
        public ChatbotAction[] actions { get; set; }
    }
}
