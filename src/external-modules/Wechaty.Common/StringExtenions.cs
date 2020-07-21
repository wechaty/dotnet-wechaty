using System;
using System.Diagnostics.CodeAnalysis;

namespace Wechaty
{
    public static class StringExtenions
    {
        private const int MAX_LEN = 7089;

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
