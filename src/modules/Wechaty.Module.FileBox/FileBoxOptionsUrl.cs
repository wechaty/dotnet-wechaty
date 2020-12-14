using System.Collections.Generic;

namespace Wechaty.Module.Filebox
{
    public class FileBoxOptionsUrl : FileBoxOptions
    {
        public override FileBoxType Type => FileBoxType.Url;
        public string Url { get; set; }
        public IDictionary<string, IEnumerable<string>>? Headers { get; set; }
    }
}
