using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wechaty
{
    public abstract class FileBoxJsonObject
    {
        [JsonProperty("boxType")]
        public abstract FileBoxType BoxType { get; }

        [JsonProperty("metadata")]
        public Dictionary<string, object> Metadata { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
