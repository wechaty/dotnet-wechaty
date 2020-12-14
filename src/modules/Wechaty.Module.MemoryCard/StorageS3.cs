using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Wechaty.Module.MemoryCard
{
    public class StorageS3 : StorageBackend
    {
        private readonly StorageS3Options _options;
        private readonly AmazonS3Client _s3;

        public StorageS3(string name, StorageS3Options options, ILogger<StorageS3> logger) : base(name, options, logger)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"constructor()");
            }
            _options = options;
            _s3 = new AmazonS3Client(new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey), RegionEndpoint.GetBySystemName(_options.Region));
        }

        public override string ToString()
        {
            var text = string.Join("", new string[] { nameof(StorageS3), "<", Name, ">" });
            return text;
        }

        public override async Task Destroy()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"destroy()");
            }
            var request = new DeleteObjectRequest { BucketName = _options.Bucket, Key = Name };
            var response = await _s3.DeleteObjectAsync(request);
            if (response.HttpStatusCode < HttpStatusCode.OK || response.HttpStatusCode >= HttpStatusCode.Ambiguous)
            {
                Logger.LogError($"destory() failed. status code is {response.HttpStatusCode}");
                var exception = new Exception("destroy() failed");
                exception.Data.Add("request", request);
                exception.Data.Add("response", response);
                throw exception;
            }
        }

        public override async Task<MemoryCardPayload> Load()
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"load()");
            }
            var request = new GetObjectRequest
            {
                BucketName = _options.Bucket,
                Key = Name
            };
            var response = await _s3.GetObjectAsync(request);
            if (response.HttpStatusCode < HttpStatusCode.OK || response.HttpStatusCode >= HttpStatusCode.Ambiguous)
            {
                Logger.LogError($"load() failed. status code is {response.HttpStatusCode}");
                var exception = new Exception("load() failed");
                exception.Data.Add("request", request);
                exception.Data.Add("response", response);
                throw exception;
            }
            using var reader = new StreamReader(response.ResponseStream);
            var text = await reader.ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(text))
            {
                return new MemoryCardPayload();
            }
            return JsonConvert.DeserializeObject<MemoryCardPayload>(text);
        }

        public override async Task Save(MemoryCardPayload payload)
        {
            if (Logger.IsEnabled(LogLevel.Trace))
            {
                Logger.LogTrace($"save()");
            }
            var request = new PutObjectRequest
            {
                BucketName = _options.Bucket,
                Key = Name
            };
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, Encoding.UTF8, 512, true);
            var serializer = new JsonSerializer();
            serializer.Serialize(new JsonTextWriter(writer), payload);
            stream.Seek(0, SeekOrigin.Begin);
            request.InputStream = stream;
            var response = await _s3.PutObjectAsync(request);
            if (response.HttpStatusCode < HttpStatusCode.OK || response.HttpStatusCode >= HttpStatusCode.Ambiguous)
            {
                Logger.LogError($"save() failed. status code is {response.HttpStatusCode}");
                var exception = new Exception("save() failed");
                exception.Data.Add("request", request);
                exception.Data.Add("response", response);
                throw exception;
            }
        }
    }
}
