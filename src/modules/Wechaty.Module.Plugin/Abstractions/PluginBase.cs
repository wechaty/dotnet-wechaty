using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wechaty.Module.Plugin.Abstractions
{
    public abstract class PluginBase : IPlugin
    {
        public Task StartAsync(IPluginContext context) => Task.CompletedTask;
        public Task StopAsync(IPluginContext context) => Task.CompletedTask;
    }
}
