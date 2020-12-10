using Newtonsoft.Json;

namespace Wechaty.Filebox
{
    public class FileBoxJsonObjectQRCode : FileBoxJsonObject
    {

        [JsonProperty("boxType")]
        public override FileBoxType BoxType => FileBoxType.QRCode;

        [JsonProperty("qrCode")]
        public string QrCode { get; set; }
    }
}
