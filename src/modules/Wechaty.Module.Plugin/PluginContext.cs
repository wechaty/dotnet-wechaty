using System;
using System.Threading;

namespace Wechaty.Module.Plugin
{
    internal class PluginContext : IPluginContext
    {

        public PluginContext(IPluginFactory pluginFactory, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            PluginFactory = pluginFactory;
            ServiceProvider = serviceProvider;
            CancellationToken = cancellationToken;
        }

        public IPluginFactory PluginFactory { get; }

        public IServiceProvider ServiceProvider { get; }

        public CancellationToken CancellationToken { get; }
    }
}
