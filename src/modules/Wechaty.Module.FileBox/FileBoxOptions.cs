
namespace Wechaty.Module.Filebox
{
    /// <summary>
    /// URI to the file
    /// See:
    ///  <see href="https://nodejs.org/api/fs.html#fs_url_object_support">fs_url_object_support</see>.
    ///  <see href="https://danielmiessler.com/study/url-uri/">url-uri</see>
    /// FileType: LOCAL, REMOTE, BUFFER, STREAM
    /// </summary>
    public abstract class FileBoxOptions
    {
        public abstract FileBoxType Type { get; }
        public string Name { get; set; }
    }
}
