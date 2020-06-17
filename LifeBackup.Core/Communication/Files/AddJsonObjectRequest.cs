using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FileBackup.Core.Communication.Files
{
    public class AddJsonObjectRequest
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("timesent")]
        public DateTime TimeSent { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
 