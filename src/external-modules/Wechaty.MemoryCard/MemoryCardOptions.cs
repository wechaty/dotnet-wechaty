
namespace Wechaty.Memorycard
{
    public class MemoryCardOptions
    {
        public string? Name { get; set; }
        public StorageBackendOptions? StorageOptions { get; set; }
        public MuliplexMemroyCardOptions? Muliplex { get; set; }
    }
}
