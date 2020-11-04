using System;

namespace Wechaty
{
    /// <summary>
    /// extensions for <see cref="DateTime"/>
    /// </summary>
    public static class DatetimeExtensions
    {
        /// <summary>
        /// Unix时间戳(秒)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetUnixEpoch(this DateTime dateTime) => (dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000000;

        /// <summary>
        /// Unix时间戳(毫秒)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long GetUnixEpochMillisecond(this DateTime dateTime) => (dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000;

        /// <summary>
        /// Unix timstamp to DateTime, support second or millisecond
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime TimestampToDateTime(this long timestamp)
        {
            // 1e11:
            //   in milliseconds:  Sat Mar 03 1973 09:46:39 UTC
            //   in seconds:       Wed Nov 16 5138 9:46:40 UTC
            if (timestamp < 1e11)
            {
                timestamp *= 1000;
            }
            return new DateTime((timestamp * 10000) + 621355968000000000, DateTimeKind.Utc);
        }
    }
}
