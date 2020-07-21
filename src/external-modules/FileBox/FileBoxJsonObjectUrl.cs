using System.Collections.Generic;

namespace Wechaty
{
    public class FileBoxJsonObjectUrl : FileBoxJsonObject
    {
        public override FileBoxType BoxType => FileBoxType.Url;
        public string RemoteUrl { get; set; }
        public IDictionary<string,IEnumerable<string>>? Headers { get; set; }
    }
}
