using System;
using Wechaty.Schemas;

namespace Wechaty
{
    public class WechatyPuppetOptions
    {
        public MemoryCard? Memory { get; set; }
        public string? Name { get; set; }
        /// <summary>
        /// DEPRECATED: use name instead
        /// </summary>
        [Obsolete("DEPRECATED: use name instead")]
        public string? Profile { get; set; }
        public WechatyPuppet? Puppet { get; set; }
        public PuppetOptions? PuppetOptions { get; set; }
    }
}
