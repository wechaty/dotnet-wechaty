using System;
using System.Threading.Tasks;

namespace Wechaty
{
    public interface IWechatPlugin
    {
        public string Name { get; }
        public string Description { get; }
        public string Version { get; }
        Task Install(Wechaty bot);

    }
}
