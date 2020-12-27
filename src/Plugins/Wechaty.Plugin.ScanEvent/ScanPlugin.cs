using System;
using System.Threading.Tasks;
using Wechaty;
using Wechaty.Module.Puppet.Schemas;
using Wechaty.User;

namespace Wechaty.Plugin.ScanEvent
{
    public class ScanPlugin : IWechatPlugin
    {
        public Task Execute(Wechaty bot)
        {
            bot.OnScan((string qrcode, ScanStatus status, string? data) =>
            {

            })
              .OnLogin((ContactSelf user) =>
              {
                  Console.WriteLine($"{user.Name}在{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}上线了！小仙仙在哪啊");
              })
              .OnMessage((Message) =>
              {
                  Console.WriteLine(Message.Text);
                  Console.WriteLine("小仙仙何在????");
              });

            return null;
        }
    }
}
