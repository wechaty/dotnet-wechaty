using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Wechaty;
using Wechaty.Module.Puppet.Schemas;
using Wechaty.User;

namespace Wechaty.Plugin.ScanEvent
{
    public class ScanPlugin : IWechatPlugin
    {
        public string Name => "QRCodeTerminal";

        public string Description => "wechaty scan QRCodeTerminal";

        public string Version => "V1.0.0";

        public Task Install(Wechaty bot)
        {
            bot.OnScan((string qrcode, ScanStatus status, string? data) =>
                 {
                     const string QrcodeServerUrl = "https://wechaty.github.io/qrcode/";
                     if (status == ScanStatus.Waiting || status == ScanStatus.Timeout)
                     {
                         var qrcodeImageUrl = QrcodeServerUrl + qrcode;
                         Console.WriteLine(qrcodeImageUrl);
                     }
                     else if (status == ScanStatus.Scanned)
                     {
                         Console.WriteLine(data);
                     }
                 })
               .OnLogin((ContactSelf user) =>
               {
                   Console.WriteLine($"{user.Name}在{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}上线了！");
               });

            return Task.CompletedTask;
        }

    }
}
