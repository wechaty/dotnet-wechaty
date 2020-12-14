
namespace Wechaty.Module.FileBox
{
    public class FileBoxOptionsBase64 : FileBoxOptions
    {
        public override FileBoxType Type => FileBoxType.Base64;
        public string Base64 { get; set; }
    }
}
