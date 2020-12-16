using System;
using System.Diagnostics.CodeAnalysis;

namespace Wechaty.Module.Common
{
    /// <summary>
    /// extensions for <see cref="string"/>
    /// </summary>
    public static class StringExtenions
    {
        private const int MAX_LEN = 7089;

        /// <summary>
        /// make sure then length of qr code less than <see cref="MAX_LEN"/>(7089)
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GuardQrCodeValue([DisallowNull] this string value)
        {
            if (value.Length > MAX_LEN)
            {
                throw new InvalidOperationException("QR Code Value is larger then the max len. Did you return the image base64 text by mistake? See: https://github.com/wechaty/wechaty/issues/1889");
            }
            return value;
        }
    }
}
