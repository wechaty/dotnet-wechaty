using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Wechaty.Module.FileBox;
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
            //Console.WriteLine(JsonConvert.SerializeObject(data));
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

        private static async void WechatyMessageEventListenerAsync(User.Message message)
        {
            Console.WriteLine(message.Text);
            var room = message.Room;
            if (room == null)
            {
                room = await bot.Room.Find(new RoomQueryFilter() { Id = "19182933822@chatroom" });
            }
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
                room = await bot.Room.Find(new RoomQueryFilter() { Id = "19182933822@chatroom" });
                await message.Forward(room);
                Thread.Sleep(1000);

            }
            if ((message.Room != null && message.Room.Id == "19182933822@chatroom") || message.Talker.Id == "wxid_lucnxixqb97522")
            {

                if (string.IsNullOrEmpty(room.Id))
                {
                    room = await bot.Room.Find(new RoomQueryFilter() { Id = "19182933822@chatroom" });
                }
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
                    _ = img.Artwork();
                    //var aa = await img.Artwork();
                    //var bb = await img.HD();
                    //var cc = await img.Thumbnail();
                    //Console.WriteLine(await aa.ToBase64());
                    await message.Forward(room);

                }
                if (message.Type == MessageType.Contact)
                {
                    var contactCard = message.ToContact();
                    await message.Forward(room);
                }


                switch (message.Type)
                {
                    case MessageType.Unknown:
                        break;
                    case MessageType.Attachment:
                        break;
                    case MessageType.Audio:
                    case MessageType.Contact:
                    case MessageType.Image:
                    case MessageType.Text:
                    case MessageType.MiniProgram:
                    case MessageType.Url:
                    case MessageType.Video:
                        await message.Forward(room);
                        break;
                    case MessageType.ChatHistory:
                        break;
                    case MessageType.Emoticon:
                        break;
                    case MessageType.Location:
                        break;
                    case MessageType.GroupNote:
                        break;
                    case MessageType.Transfer:
                        break;
                    case MessageType.RedEnvelope:
                        break;
                    case MessageType.Recalled:
                        break;
                    default:
                        break;
                }

                if (message.Text == "喜洋洋美羊羊视频")
                {
                    var fileBox = FileBox.FromUrl("http://clips.vorwaerts-gmbh.de/big_buck_bunny.mp4");
                    var msg_video = await message.Say(fileBox);

                    await msg_video.Forward(room);
                }
                if (message.Text == "gif")
                {
                    var fileBox = FileBox.FromUrl("https://n.sinaimg.cn/tech/transform/419/w140h279/20201209/7ee0-keyancx3618731.gif");
                    var msg_gif = await message.Say(fileBox);
                    await msg_gif.Forward(room);
                }
                if (message.Text == "UrlLink")
                {
                    var urlLink = new UrlLink(new UrlLinkPayload()
                    {
                        Url = "https://u.jd.com/tyPpe4K",
                        ThumbnailUrl = "https://car2.autoimg.cn/cardfs/product/g25/M0B/C7/57/240x180_0_q95_c42_autohomecar__ChsEmF8EOK-Aa1_hAAi6_ZwI4QE965.jpg",
                        Title = "小久吖 柠檬味无骨鸡爪 200g/盒",
                        Description = "13.9元  第二份7.9"
                    });
                    var newMessage = await message.Say(urlLink);
                    await newMessage.Forward(room);
                }
                if (message.Text == "天府小程序")
                {
                    var miniProgram = new MiniProgram(new MiniProgramPayload()
                    {
                        Appid = "wxb296433268a1c654",
                        Title = "小红书·标记我的生活",
                        PagePath = "pages/main/home/index.html",
                        Description = "身份管家",
                        ThumbUrl = "305f0201000458305602010002042b9237d502032f501d02040cea8a9602045fd195c104316175706170706d73675f613437386236396437633339336261355f313630373537303838303339395f31303734393430360204010400030201000400",
                        ThumbKey = "f1f9625fd17726cf628552772b45302e",
                        IconUrl = "http://wx.qlogo.cn/mmhead/Q3auHgzwzM4icS8fx2lsbuIibqp5qVunB1ZILsaJWfXcAnY3gBhAT0Mg/96",
                        ShareId = "1_wxb296433268a1c654_a7847b1708ebdc2c9207660d1d168de5_1607570881_0",
                        Username = "wxid_lucnxixqb97522"
                    });
                    var msg_mini = await message.Say(miniProgram);
                    await msg_mini.Forward(room);
                }
                // 发送名片
                if (message.Text == "天使的信息")
                {
                    var tianshi = await bot.Contact.Find(new ContactQueryFilter() { Name = "天使" });
                    var msg_card = await message.Say(tianshi);
                    await msg_card.Forward(room);
                }
                if (message.Text == "天使图片")
                {
                    var fileBox = FileBox.FromUrl("https://wechaty.github.io/wechaty/images/bot-qr-code.png");
                    var msg_filebox = await message.Say(fileBox);
                    await msg_filebox.Forward(room);
                }

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
