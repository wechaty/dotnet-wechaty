using System;
using Wechaty.Schemas;

namespace Wechaty
{
    public class WechatyOptions
    {
        public MemoryCard? Memory { get; set; }
        public string? Name { get; set; }
        /// <summary>
        /// DEPRECATED: use name instead
        /// </summary>
        [Obsolete("DEPRECATED: use name instead")]
        public string? Profile { get; set; }
        public Puppet? Puppet { get; set; }
        public PuppetOptions? PuppetOptions { get; set; }

        //[Obsolete("io token not supported yet.", true)]
        public string? IoToken { get; set; }
    }
}
