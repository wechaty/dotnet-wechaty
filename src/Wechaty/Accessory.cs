using System.Diagnostics.CodeAnalysis;
using EventEmitter;
using Microsoft.Extensions.Logging;

namespace Wechaty
{
    /// <summary>
    /// Accessory
    /// </summary>
    /// <typeparam name="TAccessory"></typeparam>
    public abstract class Accessory<TAccessory> : EventEmitter<TAccessory>
        where TAccessory : Accessory<TAccessory>
    {
        private readonly long _counter;

        /// <summary>
        /// logger
        /// </summary>
        protected ILogger<TAccessory> Logger { get; }

        private static long _staticCounter;

        /// <summary>
        /// init <see cref="Accessory{TAccessory}"/>
        /// </summary>
        /// <param name="wechaty"></param>
        /// <param name="logger"></param>
        /// <param name="name"></param>
        protected Accessory([DisallowNull] Wechaty wechaty,
                            [DisallowNull] ILogger<TAccessory> logger,
                            [AllowNull] string? name = default)
        {
            WechatyInstance = wechaty;
            Logger = logger;
            name ??= ToString() ?? "";
            _counter = _staticCounter++;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"#{_counter}<{name}> constructor({name})");
            }
        }

        /// <summary>
        /// instance of wechaty
        /// </summary>
        public Wechaty WechatyInstance { get; }

        /// <summary>
        /// puppet of wechaty
        /// </summary>
        public WechatyPuppet Puppet => WechatyInstance.Puppet;
    }
}
