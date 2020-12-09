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
            var room = message.Room;


            if (message.Text == "天王盖地虎" || message.Text == "小鸡啄米")
            {
                _ = message.Say("宝塔镇河妖");
            }
            //var taker = message.Talker;
            //if (taker?.Name == "大狗" || taker?.Name == "Darren")
            //{
            //    Task.Run(async () =>
            //    {
            //        //await taker.SetAlias("大狗");
            //        //var findAll = await bot.Contact.FindAll();

            //        //var room = await bot.Room.Find(new RoomQueryFilter() { Id = "19182933822@chatroom" });
            //        //await message.Forward(room);
            //    });
            //}

            //if (true)
            //{
            //    Task.Run(async () =>
            //    {
            //        var rooms = await bot.Room.FindAll();
            //    });
            //}

            if (message.Room != null && room.Id == "17557292862@chatroom")
            {
                Task.Run(async () =>
                {
                    var room = await bot.Room.Find(new RoomQueryFilter() { Id = "19182933822@chatroom" });
                    await message.Forward(room);

                });
            }

            if ((message.Room != null && message.Room.Id == "19182933822@chatroom") || message.Talker.Id == "wxid_lucnxixqb97522")
            {
                Task.Run(async () =>
                {
                    var room = message.Room;
                    //var avt = await room.Avatar;

                    //var members = await room.MemberAll();
                    //var firstOne = await room.Alias(members[0]);
                    //var roomOwner = room.Owner;
                    //await room.Delete(members[0]);
                    //await room.Say("集合啦", members.Where(x => x.Name != "MacWeChat").Take(2).ToArray());
                    //var rooms = await bot.Room.FindAll();

                    if (message.Type == MessageType.Emoticon || message.Type == MessageType.Image)
                    {
                        var img = message.ToImage();
                        var aa = await img.Artwork();
                        var bb = await img.HD();
                        var cc = await img.Thumbnail();
                        Console.WriteLine(await aa.ToBase64());
                    }
                    if (message.Type == MessageType.Contact)
                    {
                        var contactCard = message.ToContact();
                    }

                    // 发送名片
                    if (message.Text == "天使的信息")
                    {
                        var tianshi = await bot.Contact.Find(new ContactQueryFilter() { Name = "天使" });
                        if (tianshi != null)
                        {
                            await message.Say(tianshi);
                        }
                    }
                    if (message.Text == "天使图片")
                    {
                        var fileBox = FileBox.FromUrl("https://wechaty.github.io/wechaty/images/bot-qr-code.png");
                        await message.Say(fileBox);
                    }



                    var urlLink = new UrlLink(new UrlLinkPayload()
                    {
                        Url = "https://u.jd.com/tyPpe4K",
                        ThumbnailUrl = "https://car2.autoimg.cn/cardfs/product/g25/M0B/C7/57/240x180_0_q95_c42_autohomecar__ChsEmF8EOK-Aa1_hAAi6_ZwI4QE965.jpg",
                        Title = "小久吖 柠檬味无骨鸡爪 200g/盒",
                        Description = "13.9元  第二份7.9"
                    });

                    var newMessage = await message.Say(urlLink);
                });
            }
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
