using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Wechaty.Schemas;

namespace Wechaty.Getting.Start
{
    public class ConsoleClientHostedService : IHostedService
    {
        private IConfiguration configuration { get; set; }

        public async Task StartAsync(CancellationToken cancellationToken)
        {


            ILoggerFactory loggerFactory = new LoggerFactory();

            var logger = new Logger<WechatyPuppet>(loggerFactory);

            var PuppetOptions = new Schemas.PuppetOptions()
            {
                Endpoint = "hostie gateway",
                Token = "your token"
            };

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

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Process.GetCurrentProcess().Kill();

            throw new NotImplementedException();
        }
    }
}
