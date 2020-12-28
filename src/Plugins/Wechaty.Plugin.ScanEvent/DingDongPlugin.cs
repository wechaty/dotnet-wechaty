using System.Threading.Tasks;
using Wechaty.User;

namespace Wechaty.Plugin.ScanEvent
{
    public class DingDongPlugin : IWechatPlugin
    {
        public string Name => "DingDong Plugin";

        public string Description => "ding dong heartbeat plugin";

        public string Version => "v 1.0.0";

        public DingDongPlugin()
        {

        }

        private string messageText { get; set; }

        public DingDongPlugin(string _message)
        {
            messageText = _message;
        }

        public async Task Install(Wechaty bot)
        {
            var p = bot.OnMessage((Message message) =>
            {
                if (message.Text=="ding")
                {
                    message.Say(messageText);
                }
            });

        }
    }
}
