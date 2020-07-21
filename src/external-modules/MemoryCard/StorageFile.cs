using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Wechaty
{
    public class StorageFile : StorageBackend
    {
        private readonly string _absFileName;

        private static readonly Regex Regex = new Regex(@"!/\.memory-card\.json$/", RegexOptions.Compiled);

        public StorageFile(string name, StorageFileOptions options, ILogger<StorageFile> logger) : base(name, options, logger)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor({name}, ...}})");
            }
            _absFileName = Path.IsPathRooted(name) ? name : Path.Combine(Environment.CurrentDirectory, name);
            if (Regex.IsMatch(_absFileName))
            {
                _absFileName += ".memory-card.json";
            }
        }

        public override string ToString()
        {
            var text = string.Join("", new string[] { nameof(StorageFile), "<", _absFileName, ">" });
            return text;
        }

        public override Task Destroy()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"destroy()");
            }
            if (File.Exists(_absFileName))
            {
                File.Delete(_absFileName);
            }
            return Task.CompletedTask;
        }

        public override async Task<MemoryCardPayload> Load()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"Load() from {_absFileName}");
            }
            if (!File.Exists(_absFileName))
            {
                if (Logger.IsEnabled(LogLevel.Trace))
                {
                    Logger.LogTrace($"Load() file {_absFileName} not exist, NOOP");
                }
                return new MemoryCardPayload();
            }
            var text = await File.ReadAllTextAsync(_absFileName);
            try
            {
                return JsonConvert.DeserializeObject<MemoryCardPayload>(text);
            }
            catch (Exception exception)
            {
                Logger.LogError(exception, "Load() failed.");
            }
            return new MemoryCardPayload();
        }

        public override async Task Save(MemoryCardPayload payload)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"save() to {_absFileName}");
            }
            var text = JsonConvert.SerializeObject(payload);
            await File.WriteAllTextAsync(_absFileName, text);
        }
    }
}
