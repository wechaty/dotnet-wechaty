using System.Collections.Generic;
using Newtonsoft.Json;

namespace Wechaty.Module.FileBox
{
    public class FileBoxJsonObjectUrl : FileBoxJsonObject
    {
        [JsonProperty("boxType")]
        public override FileBoxType BoxType => FileBoxType.Url;

        [JsonProperty("remoteUrl")]
        public string RemoteUrl { get; set; }

        [JsonProperty("headers")]
        public IDictionary<string,IEnumerable<string>>? Headers { get; set; }
    }
}
