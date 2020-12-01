using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Wechaty.Getting.Start
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
#if DEBUG
                            .MinimumLevel.Debug()
#else
                            .MinimumLevel.Information()
#endif
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                  .Enrich.FromLogContext()
                  .WriteTo.File("Logs/logs.txt")
                  .WriteTo.Console()
                  .CreateLogger();

            try
            {
                await CreateHostBuilder(args).RunConsoleAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly!");
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureServices((hostContext, services) =>
               {
                   services.AddHostedService<ConsoleClientHostedService>();
               });
    }
}
