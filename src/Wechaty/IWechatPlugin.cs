using System;
using System.Threading.Tasks;

namespace Wechaty
{
    public interface IWechatPlugin
    {
        /// <summary>
        /// plugin name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// plugin description
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// plugin verson
        /// </summary>
        public string Version { get; }

        Task Install(Wechaty bot);
    }
}
