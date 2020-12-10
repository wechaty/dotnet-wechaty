
namespace Wechaty.FileBox
{
    public class FileBoxOptionsFile : FileBoxOptions
    {
        public override FileBoxType Type => FileBoxType.File;
        public string Path { get; set; }
    }
}
