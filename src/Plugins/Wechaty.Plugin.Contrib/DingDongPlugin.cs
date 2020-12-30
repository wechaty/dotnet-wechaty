using System.Threading.Tasks;
using Wechaty.Module.Puppet.Schemas;
using Wechaty.User;

namespace Wechaty.Plugin
{
    public class DingDongPlugin : IWechatPlugin
    {
        public string Name => "DingDong Plugin";

        public string Description => "Reply dong if bot receives a ding message.";

        public string Version => "V1.0.0";

        private DingDongConfig _config=new DingDongConfig();

        public DingDongPlugin()
        {

        }

        public DingDongPlugin(DingDongConfig config)
        {
            _config = config;
        }

        public Task Install(Wechaty bot)
        {
            bot.OnMessage(async (Message message) =>
            {
                if (message.Type == MessageType.Text)
                {
                    if (_config.Ding == message.Text)
                    {
                        await message.Say(_config.Dong);
                    }
                }
            });
            return Task.CompletedTask;
        }

    }

    

    public class DingDongConfig
    {
        public string Ding { get; set; } = "ding";
        public string Dong { get; set; } = "dong";
    }
}
