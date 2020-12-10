
namespace Wechaty.FileBox
{
    public enum FileBoxType
    {
        Unknown = 0,
        /// <summary>
        /// Serializable by toJSON()
        /// </summary>
        Base64 = 1,
        Url = 2,
        QRCode = 3,
        /// <summary>
        /// Not serializable by toJSON()
        /// Need to convert to FileBoxType.Base64 before call toJSON()
        /// </summary>
        Buffer = 4,
        File = 5,
        Stream = 6,
    }
}
