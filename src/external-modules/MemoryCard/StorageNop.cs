using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Wechaty
{
    public class StorageNop : StorageBackend
    {
        public StorageNop(string name, StorageBackendOptions options, ILogger<StorageNop> logger) : base(name, options, logger)
        {
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace($"constructor({name}, ...)");
            }
        }

        public override string ToString()
        {
            const string text = nameof(StorageNop) + "<nop>";
            return text;
        }

        public override Task Destroy()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"destroy()");
            }
            return Task.CompletedTask;
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public override async Task<MemoryCardPayload> Load()
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"Load()");
            }
            return new MemoryCardPayload();
        }

        public override Task Save(MemoryCardPayload payload)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"save()");
            }
            return Task.CompletedTask;
        }
    }
}
