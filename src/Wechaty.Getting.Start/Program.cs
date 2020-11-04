using System;
using Microsoft.Extensions.Logging;
using Wechaty.Schemas;

namespace Wechaty.Getting.Start
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            ILoggerFactory loggerFactory = new LoggerFactory();

            var wechatyOptions = new WechatyOptions()
            {
                Name = "Demo",
                PuppetOptions = new Schemas.PuppetOptions()
                {
                    Endpoint = "your wechaty hosite gatway",
                    Token = "your wechaty token"
                }
            };

            var bot = new Wechaty(wechatyOptions, loggerFactory);

            await bot.OnScan(WechatyScanEventListener)
                .Start();
        }


        private static void WechatyScanEventListener(Wechaty wechaty, string qrcode, ScanStatus status, string? data)
        {
            throw new NotImplementedException();
        }



    }
}
