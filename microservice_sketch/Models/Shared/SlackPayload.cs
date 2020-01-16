using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace microservice_sketch.Models.Shared
{
    public class SlackPayload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; } = "KoocoApiCenter";

        [JsonProperty("text")]
        public string Text { get; set; }
    }
}
