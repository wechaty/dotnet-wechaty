
namespace Wechaty.FileBox
{
    public class FileBoxOptionsBuffer : FileBoxOptions
    {
        public override FileBoxType Type => FileBoxType.Buffer;
        public byte[] Buffer { get; set; }
    }
}
