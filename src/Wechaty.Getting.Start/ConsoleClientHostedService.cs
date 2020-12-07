using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Wechaty.Schemas;
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


        private static Wechaty bot;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var PuppetOptions = new Schemas.PuppetOptions()
            {
                Token = _configuration["WECHATY_PUPPET_HOSTIE_TOKEN"],
                PuppetService = _configuration["WECHATY_PUPPET"]
            };
            bot = new Wechaty(PuppetOptions);

            await bot.OnScan(WechatyScanEventListener)
              .OnLogin(WechatyLoginEventListener)
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
            Console.WriteLine($"{user.Name}在{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}上线了！");
        }

        private static void WechatyHeartbeatEventListener(object data)
        {
            Console.WriteLine(JsonConvert.SerializeObject(data));
        }

        private static void WechatyScanEventListener(string qrcode, ScanStatus status, string? data)
        {
            Console.WriteLine(qrcode);
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
        }

        private static void WechatyMessageEventListenerAsync(User.Message message)
        {
            Console.WriteLine(message.Text);
            if (message.Text == "天王盖地虎" || message.Text == "小鸡啄米")
            {
                _ = message.Say("宝塔镇河妖");
            }
        }

        #region Room
        private static void WechatyRoomInviteEventListener(RoomInvitation roomInvitation)
        {
            // 1102977037

        }

        private static void WechatyRoomJoinEventListener(Room room, IReadOnlyList<Contact> inviteeList, Contact inviter, DateTime? date)
        {
            Console.WriteLine($"{inviter.Name} invites {string.Join(",", inviteeList.Select(x => x.Name))} into { _=room.GetTopic()} room !");
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
