
namespace Wechaty.Filebox
{
    public class FileBoxJsonObjectQRCode : FileBoxJsonObject
    {
        public override FileBoxType BoxType => FileBoxType.QRCode;
        public string QrCode { get; set; }
    }
}
