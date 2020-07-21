using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Wechaty
{
    public abstract class Accessory<TAccessory> : EventEmitter<TAccessory>
        where TAccessory : Accessory<TAccessory>
    {
        private readonly long _counter;

        protected ILogger<TAccessory> Logger { get; }

        private static long _staticCounter;

        protected Accessory([DisallowNull] Wechaty wechaty,
                            [DisallowNull] Puppet puppet,
                            [DisallowNull] ILogger<TAccessory> logger,
                            [AllowNull] string? name = default)
        {
            WechatyInstance = wechaty;
            Puppet = puppet;
            Logger = logger;
            name ??= ToString() ?? "";
            _counter = _staticCounter++;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"#{_counter}<{name}> constructor({name})");
            }
        }

        public Wechaty WechatyInstance { get; }

        public Puppet Puppet { get; }
    }
}
