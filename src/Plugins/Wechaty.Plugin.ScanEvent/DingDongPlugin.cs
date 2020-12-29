using System.Threading.Tasks;
using Wechaty.User;

namespace Wechaty.Plugin.ScanEvent
{
    public class DingDongPlugin : IWechatPlugin
    {
        public string Name => "DingDong Plugin";

        public string Description => "ding dong  plugin";

        public string Version => "v1.0.0";

        public Task Install(Wechaty bot)
        {
            _ = bot.OnMessage( (message) =>
                  {
                      if (message.Text == "ding")
                      {
                          message.Say("dong");
                      }
                      if (message.Text == "dong")
                      {
                          message.Say("ding");
                      }
                  });
            return Task.CompletedTask;
        }
    }
}
