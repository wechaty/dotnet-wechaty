using System;
using System.Threading;

namespace Wechaty.Module.Plugin
{
    public interface IPluginContext
    {
        public IPluginFactory PluginFactory { get; }
        public IServiceProvider ServiceProvider { get; }
        public CancellationToken CancellationToken { get; }
    }
}
