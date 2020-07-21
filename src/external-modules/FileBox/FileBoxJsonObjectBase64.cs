
namespace Wechaty
{
    public class FileBoxJsonObjectBase64 : FileBoxJsonObject
    {
        public override FileBoxType BoxType => FileBoxType.Base64;
        public string Base64 { get; set; }
    }
}
