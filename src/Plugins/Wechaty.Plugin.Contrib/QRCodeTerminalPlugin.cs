using System;
using System.Threading.Tasks;
using QRCoder;
using Wechaty.Module.Puppet.Schemas;
using static QRCoder.PayloadGenerator;

namespace Wechaty.Plugin
{
    public class QRCodeTerminalPlugin : IWechatPlugin
    {
        public string Name => "QRCodeTerminal Plugin";

        public string Description => "Show QR Code for Scan in Terminal";

        public string Version => "V1.0.0";


        public Task Install(Wechaty bot)
        {
            bot.OnScan((string qrcode, ScanStatus status, string? data) =>
            {
                if (status == ScanStatus.Waiting || status == ScanStatus.Timeout)
                {
                    const string QrcodeServerUrl = "https://wechaty.github.io/qrcode/";
                    var qrcodeImageUrl = QrcodeServerUrl + qrcode;
                    Console.WriteLine(qrcodeImageUrl);

                    var generator = new Url(qrcode);
                    var payload = generator.ToString();

                    var qrGenerator = new QRCodeGenerator();
                    var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.M);

                    var qrCodeAsi = new AsciiQRCode(qrCodeData);
                    var qrCodeAsAsciiArt = qrCodeAsi.GetGraphic(1);
                    Console.WriteLine(qrCodeAsAsciiArt);
                }
            });



            return Task.CompletedTask;
        }
    }
}
