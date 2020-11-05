using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Schemas;

namespace Wechaty.Getting.Start
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {

            ILoggerFactory loggerFactory = new LoggerFactory();

            var PuppetOptions = new Schemas.PuppetOptions()
            {
                Endpoint = "http://47.100.24.97:8788",
                Token = "puppet_padplus_f415f3ab3bead8a0"
            };

            var logger = new Logger<WechatyPuppet>(loggerFactory);

            var grpcPuppet = new GrpcPuppet(PuppetOptions, logger, loggerFactory);

            var wechatyOptions = new WechatyOptions()
            {
                Name = "Demo",
                Puppet = grpcPuppet,
            };

            var bot = new Wechaty(wechatyOptions, loggerFactory);

            await bot.OnScan(WechatyScanEventListener)
                .OnMessage(WechatyMessageEventListener)
                .OnHeartbeat(WechatyHeartbeatEventListener)
                .Start();

            Console.ReadKey();
        }

        private static void WechatyHeartbeatEventListener(Wechaty wechaty, object data)
        {
            Console.WriteLine(JsonConvert.SerializeObject(data));
        }

        private static void WechatyScanEventListener(Wechaty wechaty, string qrcode, ScanStatus status, string? data)
        {
            //throw new NotImplementedException();

        }


        private static void WechatyMessageEventListener(Wechaty wechaty, User.Message message)
        {
            Console.WriteLine(message.Text);
        }


    }
}
