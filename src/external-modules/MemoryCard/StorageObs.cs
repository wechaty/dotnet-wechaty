using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OBS;
using OBS.Model;

namespace Wechaty
{
    public class StorageObs : StorageBackend
    {
        private readonly ObsClient _obs;
        private readonly StorageObsOptions _options;
        private static readonly object VoidResult = new object();

        public StorageObs(string name, StorageObsOptions options, ILogger<StorageObs> logger) : base(name, options, logger)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor()");
            }
            _options = options;
            _obs = new ObsClient(options.AccessKeyId, options.SecretAccessKey, options.Server);
        }

        public override string ToString()
        {
            var text = string.Join("", new string[] { nameof(StorageObs), "<", Name, ">" });
            return text;
        }

        public override Task Destroy()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"destroy()");
            }
            return DeleteObject();
        }

        public override Task<MemoryCardPayload> Load()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"load()");
            }
            return GetObject();
        }

        public override Task Save(MemoryCardPayload payload)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"save()");
            }
            return PutObject(payload);
        }

        private Task PutObject(MemoryCardPayload payload)
        {
            var source = new TaskCompletionSource<object>();
            _ = Task.Run(() =>
            {
                try
                {
                    using var stream = new MemoryStream();
                    using var writer = new StreamWriter(stream, Encoding.UTF8, 512, true);
                    var serializer = new JsonSerializer();
                    serializer.Serialize(writer, payload);
                    _ = stream.Seek(0, SeekOrigin.Begin);
                    var request = new PutObjectRequest
                    {
                        InputStream = stream,
                        BucketName = _options.Bucket,
                        ObjectKey = Name
                    };
                    var response = _obs.PutObject(request);
                    if (response.StatusCode >= HttpStatusCode.OK && response.StatusCode < HttpStatusCode.Ambiguous)
                    {
                        source.SetResult(VoidResult);
                    }
                    else
                    {
                        var exception = new Exception($"put object failed, response status is {response.StatusCode}");
                        exception.Data.Add("request", request);
                        exception.Data.Add("response", response);
                        source.SetException(exception);
                    }
                }
                catch (Exception exception)
                {
                    source.SetException(exception);
                }
            });
            return source.Task;
        }

        private Task<MemoryCardPayload> GetObject()
        {
            var source = new TaskCompletionSource<MemoryCardPayload>();
            _ = Task.Run(() =>
            {
                try
                {
                    var request = new GetObjectRequest
                    {
                        BucketName = _options.Bucket,
                        ObjectKey = Name
                    };
                    var response = _obs.GetObject(request);
                    if (response.StatusCode >= HttpStatusCode.OK && response.StatusCode < HttpStatusCode.Ambiguous)
                    {
                        var serializer = new JsonSerializer();
                        var result = serializer.Deserialize<MemoryCardPayload>(new JsonTextReader(new StreamReader(response.OutputStream)));
                        source.SetResult(result ?? new MemoryCardPayload());
                    }
                    else
                    {
                        var exception = new Exception($"get object failed, response status is {response.StatusCode}");
                        exception.Data.Add("request", request);
                        exception.Data.Add("response", response);
                        source.SetException(exception);
                    }
                }
                catch (Exception exception)
                {
                    source.SetException(exception);
                }
            });
            return source.Task;
        }

        private Task DeleteObject()
        {
            var source = new TaskCompletionSource<object>();
            _ = Task.Run(() =>
            {
                try
                {
                    var request = new DeleteObjectRequest
                    {
                        BucketName = _options.Bucket,
                        ObjectKey = Name
                    };
                    var response = _obs.DeleteObject(request);
                    if (response.StatusCode >= HttpStatusCode.OK && response.StatusCode < HttpStatusCode.Ambiguous)
                    {
                        source.SetResult(null);
                    }
                    else
                    {
                        var exception = new Exception($"delete object failed, response status is {response.StatusCode}");
                        exception.Data.Add("request", request);
                        exception.Data.Add("response", response);
                        source.SetException(exception);
                    }
                }
                catch (Exception exception)
                {
                    source.SetException(exception);
                }
            });
            return source.Task;
        }
    }
}
