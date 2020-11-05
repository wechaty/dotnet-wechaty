using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace Wechaty.Getting.Start
{
    //class Program
    //{
    //    static async System.Threading.Tasks.Task Main(string[] args)
    //    {

    //        ILoggerFactory loggerFactory = new LoggerFactory();

    //        var PuppetOptions = new Schemas.PuppetOptions()
    //        {
    //            Endpoint = "hostie gateway",
    //            Token = "your token"
    //        };

    //        var logger = new Logger<WechatyPuppet>(loggerFactory);

    //        var grpcPuppet = new GrpcPuppet(PuppetOptions, logger, loggerFactory);

    //        var wechatyOptions = new WechatyOptions()
    //        {
    //            Name = "Demo",
    //            Puppet = grpcPuppet,
    //        };

    //        var bot = new Wechaty(wechatyOptions, loggerFactory);

    //        await bot.OnScan(WechatyScanEventListener)
    //            .OnMessage(WechatyMessageEventListener)
    //            .OnHeartbeat(WechatyHeartbeatEventListener)
    //            .Start();

    //        Console.ReadKey();
    //    }

    //    private static void WechatyHeartbeatEventListener(Wechaty wechaty, object data)
    //    {
    //        Console.WriteLine(JsonConvert.SerializeObject(data));
    //    }

    //    private static void WechatyScanEventListener(Wechaty wechaty, string qrcode, ScanStatus status, string? data)
    //    {
    //        //throw new NotImplementedException();

    //    }


    //    private static void WechatyMessageEventListener(Wechaty wechaty, User.Message message)
    //    {
    //        Console.WriteLine(message.Text);
    //    }


    //}

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
