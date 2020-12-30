using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Wechaty.User;

namespace Wechaty.Plugin
{
    public class FriendshipAccepterPlugin : IWechatPlugin
    {
        public string Name => "FriendshipAccepter Plugin";

        public string Description => "Accept friendship automatically, and say/do something for greeting.";

        public string Version => "V1.0.0";


        private FriendshipAccepterConfig config;

        public FriendshipAccepterPlugin()
        {

        }

        public FriendshipAccepterPlugin(FriendshipAccepterConfig _config)
        {
            config = _config;
        }

        public Task Install(Wechaty bot)
        {
            bot.OnFriendship(async (Friendship friendship) =>
            {
                var friendshipType = friendship.Type;
                switch (friendshipType)
                {
                    case Module.Puppet.Schemas.FriendshipType.Confirm:
                        var contact = friendship.Contact;
                        await contact.Say(config.Greeting);
                        break;
                    case Module.Puppet.Schemas.FriendshipType.Receive:
                        var hello = friendship.Hello;
                        if (hello.Contains(config.Greeting))
                        {
                            await friendship.Accept();
                        }
                        break;
                    case Module.Puppet.Schemas.FriendshipType.Unknown:
                        break;
                    case Module.Puppet.Schemas.FriendshipType.Verify:
                        break;
                    default:
                        break;
                }
            });

            return Task.CompletedTask;
        }
    }

    public class FriendshipAccepterConfig
    {
        public string Greeting { get; set; }
        public string KeyWord { get; set; }
    }
}
