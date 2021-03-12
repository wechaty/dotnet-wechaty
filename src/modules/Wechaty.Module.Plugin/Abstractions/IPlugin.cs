using System.Threading.Tasks;

namespace Wechaty.Module.Plugin.Abstractions
{
    public interface IPlugin
    {
        Task StartAsync(IPluginContext context);

        Task StopAsync(IPluginContext context);
    }
}
