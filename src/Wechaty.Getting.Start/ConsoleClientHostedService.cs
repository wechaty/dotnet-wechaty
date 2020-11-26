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
        private readonly IConfiguration Configuration;

        public ConsoleClientHostedService(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ILoggerFactory loggerFactory = new LoggerFactory();
            var logger = new Logger<WechatyPuppet>(loggerFactory);

            var PuppetOptions = new Schemas.PuppetOptions()
            {
                // eg http://192.168.2.200:8788
                Endpoint = Configuration["Wechaty_EndPoint"],
                Token = Configuration["Wechaty_Token"]
            };

            var grpcPuppet = new GrpcPuppet(PuppetOptions, logger, loggerFactory);

            var wechatyOptions = new WechatyOptions()
            {
                Name = Configuration["Wechaty_Name"],
                Puppet = grpcPuppet,
            };

            var bot = new Wechaty(wechatyOptions, loggerFactory);

            await bot.OnScan(WechatyScanEventListener)
                .OnMessage(WechatyMessageEventListener)
                .OnHeartbeat(WechatyHeartbeatEventListener)
                .Start();
        }

        private static void WechatyHeartbeatEventListener(object data)
        {
            Console.WriteLine(JsonConvert.SerializeObject(data));
        }

        private static void WechatyScanEventListener(Wechaty wechaty, string qrcode, ScanStatus status, string? data)
        {
            Console.WriteLine(qrcode);
            const string QrcodeServerUrl = "https://wechaty.github.io/qrcode/";
            if (status == ScanStatus.Waiting || status == ScanStatus.Timeout)
            {
                var qrcodeImageUrl = QrcodeServerUrl + qrcode;
                Console.WriteLine(qrcodeImageUrl);
            }
        }


        private static void WechatyMessageEventListener(User.Message message)
        {
            Console.WriteLine(message.Text);
        }

        /// <summary>
        /// Stop
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Process.GetCurrentProcess().Kill();
            return;
        }
    }
}
