using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Wechaty
{
    public abstract class StorageBackend
    {
        protected StorageBackend(string name, StorageBackendOptions options, ILogger<StorageBackend> logger)
        {
            Name = name;
            Options = options;
            Logger = logger;
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor({name},{{ type:{options.Type} }})");
            }
        }

        protected string Name { get; set; }
        protected StorageBackendOptions Options { get; set; }
        protected ILogger<StorageBackend> Logger { get; }
        public abstract Task Save(MemoryCardPayload payload);
        public abstract Task<MemoryCardPayload> Load();
        public abstract Task Destroy();

        public static StorageBackend GetStorage(string? name,
                                                [DisallowNull] StorageBackendOptions options,
                                                [DisallowNull] ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<StorageBackend>();
            if (logger.IsEnabled(LogLevel.Trace))
            {
                logger.LogTrace($"name: {name}, options: {JsonConvert.SerializeObject(options)}");
            }
            if (string.IsNullOrEmpty(name))
            {
                if (options.Type != StorageBackendType.Nop)
                {
                    throw new ArgumentException("storage have to be `nop` with a un-named storage");
                }
                name = "nop";
            }
            switch (options.Type)
            {
                case StorageBackendType.File:
                    return new StorageFile(name, (StorageFileOptions)options, loggerFactory.CreateLogger<StorageFile>());
                case StorageBackendType.Nop:
                    return new StorageNop(name, (StorageNopOptions)options, loggerFactory.CreateLogger<StorageNop>());
                case StorageBackendType.S3:
                    return new StorageS3(name, (StorageS3Options)options, loggerFactory.CreateLogger<StorageS3>());
                case StorageBackendType.Obs:
                    return new StorageObs(name, (StorageObsOptions)options, loggerFactory.CreateLogger<StorageObs>());
                default:
                    throw new InvalidOperationException($"backend unknown: {options.Type}");
            }
        }
    }
}
