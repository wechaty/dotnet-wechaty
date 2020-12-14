
namespace Wechaty.Module.MemoryCard
{
    public abstract class StorageBackendOptions
    {
        public abstract StorageBackendType Type { get; }
    }
    public class StorageNopOptions : StorageBackendOptions
    {
        public override StorageBackendType Type => StorageBackendType.Nop;
    }
    public class StorageFileOptions : StorageBackendOptions
    {
        public override StorageBackendType Type => StorageBackendType.File;
    }
    public class StorageS3Options : StorageBackendOptions
    {
        public override StorageBackendType Type => StorageBackendType.S3;
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string Region { get; set; }
        public string Bucket { get; set; }
    }
    public class StorageObsOptions : StorageBackendOptions
    {
        public override StorageBackendType Type => StorageBackendType.Obs;
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string Server { get; set; }
        public string Bucket { get; set; }
    }
}
