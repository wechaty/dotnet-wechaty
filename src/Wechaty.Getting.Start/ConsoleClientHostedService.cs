using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wechaty.Plugin;
using Wechaty.User;

namespace Wechaty.Getting.Start
{
    public class ConsoleClientHostedService : IHostedService
    {
        private readonly IConfiguration _configuration;

        public ConsoleClientHostedService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureService(IServiceCollection services)
        {


        }


        private static Wechaty bot;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var PuppetOptions = new Module.Puppet.Schemas.PuppetOptions()
            {
                Token = _configuration["WECHATY_PUPPET_SERVICE_TOKEN"],
                Endpoint = _configuration["WECHATY_PUPPET_SERVICE_ENDPOINT"]
            };
            bot = new Wechaty(PuppetOptions);


            // Automatic plug-in registration
            //var serviceCollection = new ServiceCollection()
            //    .AddSingleton<IWechatPlugin, ScanPlugin>()
            //    .AddSingleton<IWechatPlugin, DingDongPlugin>();
            //var plugins = serviceCollection.BuildServiceProvider().GetServices<IWechatPlugin>().ToArray();


            // Manual plug-in registration
            var qrCodeTerminalPlugin = new QRCodeTerminalPlugin();
            var dingDongPlugin = new DingDongPlugin();
            bot.Use(qrCodeTerminalPlugin)
               .Use(dingDongPlugin);



            await bot
              //.OnScan(WechatyScanEventListener)
              //.OnLogin(async (ContactSelf user) =>
              //{
              //    //Console.WriteLine($"{user.Name}在{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}上线了！");
              //})
              .OnMessage(WechatyMessageEventListenerAsync)
              .OnHeartbeat(WechatyHeartbeatEventListener)
              .OnRoomInvite(WechatyRoomInviteEventListener)
              .OnRoomJoin(WechatyRoomJoinEventListener)
              .OnRoomLeave(WechatyRoomLeaveEventListener)
              .OnRoomTopic(WechatyRoomTopicEventListener)
              .Start();
        }

        public static void WechatyLoginEventListener(ContactSelf user)
        {
            //Console.WriteLine($"{user.Name}在{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}上线了！");
        }

        private static void WechatyHeartbeatEventListener(object data)
        {
            //Console.WriteLine(JsonConvert.SerializeObject(data));
        }

        //private static void WechatyScanEventListener(string qrcode, ScanStatus status, string? data)
        //{
        //    Console.WriteLine(qrcode);
        //    const string QrcodeServerUrl = "https://wechaty.github.io/qrcode/";
        //    if (status == ScanStatus.Waiting || status == ScanStatus.Timeout)
        //    {
        //        var qrcodeImageUrl = QrcodeServerUrl + qrcode;
        //        Console.WriteLine(qrcodeImageUrl);
        //    }
        //    else if (status == ScanStatus.Scanned)
        //    {
        //        Console.WriteLine(data);
        //    }
        //}

        private static async void WechatyMessageEventListenerAsync(Message message)
        {

        }

        #region Room
        private static void WechatyRoomInviteEventListener(RoomInvitation roomInvitation)
        {
            // 1102977037

        }

        private static void WechatyRoomJoinEventListener(Room room, IReadOnlyList<Contact> inviteeList, Contact inviter, DateTime? date)
        {
            Console.WriteLine($"{inviter.Name} invites {string.Join(",", inviteeList.Select(x => x.Name))} into {room.GetTopic().Result} room !");
        }

        private static void WechatyRoomLeaveEventListener(Room room, IReadOnlyList<Contact> leaverList, Contact remover, DateTime? date)
        {

        }

        private static void WechatyRoomTopicEventListener(Room room, string newTopic, string oldTopic, Contact changer, DateTime? date)
        {
            Console.WriteLine($"{changer.Name} update room topic as {newTopic}");
        }
        #endregion

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
