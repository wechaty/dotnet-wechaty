using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Wechaty.Memorycard
{
    public class MemoryCard : IAsyncMap<string, object>
    {
        public const string NAMESPACE_MULTIPLEX_SEPRATOR = "\r";
        public const string NAMESPACE_KEY_SEPRATOR = "\n";

        public static readonly Regex NAMESPACE_MULTIPLEX_SEPRATOR_REGEX = new Regex(NAMESPACE_MULTIPLEX_SEPRATOR, RegexOptions.Compiled);
        public static readonly Regex NAMESPACE_KEY_SEPRATOR_REGEX = new Regex(NAMESPACE_KEY_SEPRATOR, RegexOptions.Compiled);
        public string? Name { get; set; }

        protected MemoryCard? Parent { get; set; }
        protected MemoryCardPayload? Payload { get; set; }
        protected StorageBackend? Storage { get; set; }
        protected List<string> MultiplexNameList { get; set; }

        private readonly MemoryCardOptions? _options;
        private readonly ILogger<MemoryCard> _logger;
        private readonly ILoggerFactory _loggerFactory;
        private readonly Dictionary<string, object> _container;

        public MemoryCard([AllowNull] string? name,
                          [DisallowNull] ILogger<MemoryCard> logger,
                          [DisallowNull] ILoggerFactory loggerFactory) :
            this(new MemoryCardOptions { Name = name }, logger, loggerFactory)
        { }

        public MemoryCard([AllowNull] MemoryCardOptions? options,
                          [DisallowNull] ILogger<MemoryCard> logger,
                          [DisallowNull] ILoggerFactory loggerFactory)
        {
            _options = options;
            _logger = logger;
            _loggerFactory = loggerFactory;
            _container = new Dictionary<string, object>();
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"constructor({JsonConvert.SerializeObject(options)}, ...}})");
            }
            MultiplexNameList = new List<string>();
            if (_options != null)
            {
                Name = _options.Name;
                if (_options.Muliplex != null)
                {
                    Parent = _options.Muliplex.Parent;
                    Payload = _options.Muliplex.Parent.Payload;
                    MultiplexNameList.AddRange(Parent.MultiplexNameList);
                    MultiplexNameList.Add(_options.Muliplex.Name);
                }
                else
                {
                    Storage = GetStorage();
                }
            }
            else
            {
                Storage = GetStorage();
            }
        }

        private StorageBackend? GetStorage()
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"getStorage() for storage type: {_options?.StorageOptions?.Type.ToString() ?? "N/A"}");
            }
            if (_options != null && _options.StorageOptions != null)
            {
                return StorageBackend.GetStorage(Name, _options.StorageOptions, _loggerFactory);
            }
            return null;
        }

        protected static TMemoryCard Multiplex<TMemoryCard>([DisallowNull] Func<MemoryCardOptions, ILogger<TMemoryCard>, ILoggerFactory, TMemoryCard> func,
                                                            [DisallowNull] TMemoryCard memory,
                                                            [DisallowNull] string name)
            where TMemoryCard : MemoryCard
        {
            if (memory._logger.IsEnabled(LogLevel.Trace))
            {
                memory._logger.LogTrace($"static multiplex({memory}, {name})");
            }
            var options = new MemoryCardOptions
            {
                Name = memory._options?.Name,
                StorageOptions = memory._options?.StorageOptions,
                Muliplex = new MuliplexMemroyCardOptions
                {
                    Name = name,
                    Parent = memory
                }
            };
            return func(options, memory._loggerFactory.CreateLogger<TMemoryCard>(), memory._loggerFactory);
        }

        public override string ToString()
        {
            var names = "";
            if (MultiplexNameList.Count > 0)
            {
                names = string.Join("", MultiplexNameList
                  .Select(name => $".multiplex({name})"));
            }

            var name = _options?.Name ?? "";

            return $"MemoryCard <{name}>{names}";
        }

        public async Task Load()
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"load() from storage: {Storage?.ToString() ?? "N/A"}");
            }
            if (IsMultiplex)
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"load() should not be called on a multiplex MemoryCard. NOOP");
                }
                return;
            }
            if (Payload != null)
            {
                throw new InvalidOperationException("memory had already loaded before.");
            }

            if (Storage != null)
            {
                Payload = await Storage.Load();
            }
            else
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"load() no storage");
                }
                Payload = new MemoryCardPayload();
            }
        }

        public async Task Save()
        {
            if (IsMultiplex)
            {
                if (Parent == null)
                {
                    throw new InvalidOperationException("multiplex memory no parent");
                }
                await Parent.Save();
                return;
            }

            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"<{Name ?? ""}>{MultiplexPath} save() to {Storage?.ToString() ?? "N/A"}");
            }

            if (Payload == null)
            {
                throw new InvalidOperationException("no payload, please call load() first.");
            }

            if (Storage == null)
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    _logger.LogTrace($"save() no storage, NOOP");
                }
                return;
            }

            await Storage.Save(Payload);
        }

        /// <summary>
        /// Multiplexing related functions START
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool IsMultiplexKey(string key)
        {
            if (NAMESPACE_MULTIPLEX_SEPRATOR_REGEX.IsMatch(key)
                && NAMESPACE_KEY_SEPRATOR_REGEX.IsMatch(key)
            )
            {
                var @namespace = MultiplexNamespace();
                return key.StartsWith(@namespace);
            }
            return false;
        }

        protected string MultiplexNamespace()
        {
            if (!IsMultiplex)
            {
                throw new InvalidOperationException("not a multiplex memory");
            }

            var @namespace = NAMESPACE_MULTIPLEX_SEPRATOR
                      + string.Join(NAMESPACE_MULTIPLEX_SEPRATOR, MultiplexNameList);
            return @namespace;
        }

        protected string ResolveKey(string name)
        {
            if (IsMultiplex)
            {
                var @namespace = MultiplexNamespace();
                return string.Join(NAMESPACE_KEY_SEPRATOR, @namespace, name);
            }
            else
            {
                return name;
            }
        }

        protected bool IsMultiplex => MultiplexNameList.Count > 0;

        protected string MultiplexPath => string.Join("/", MultiplexNameList);

        public Task<int> Size => Task.FromResult(_container.Count);

        public IAsyncEnumerator<KeyValuePair<string, object>> Entries => _container.AsAsyncEnumerable().GetAsyncEnumerator();

        public IAsyncEnumerator<string> Keys => _container.Keys.AsAsyncEnumerable().GetAsyncEnumerator();

        public IAsyncEnumerator<object> Values => _container.Values.AsAsyncEnumerable().GetAsyncEnumerator();

        public virtual MemoryCard Multiplex(string name)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"multiplex({name})");
            }
            return Multiplex((o, l, f) => new MemoryCard(o, l, f), this, name);
        }

        public async Task Destroy()
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace($"destroy() storage: {Storage?.ToString() ?? "N/A"}");
            }

            if (IsMultiplex)
            {
                throw new InvalidOperationException("can not destroy on a multiplexed memory");
            }

            await Clear();

            if (Storage != null)
            {
                await Storage.Destroy();
                Storage = null;
            }

            // to prevent to use a destroied card
            Payload = null;
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        public async Task<Optional<object>> Get(string key)
        {
            if (_container.TryGetValue(key, out var value))
            {
                return value;
            }
            return default;
        }

        public async Task Set(string key, object value) => _container[key] = value;

        public async Task<bool> Has(string key) => _container.ContainsKey(key);

        public async Task Delete(string key) => _ = _container.Remove(key);

        public async Task Clear() => _container.Clear();
#pragma warning restore CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
    }
}
