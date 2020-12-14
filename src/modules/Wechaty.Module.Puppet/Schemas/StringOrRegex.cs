using System.Text.RegularExpressions;

namespace Wechaty.Schemas
{
    /// <summary>
    /// 文本或正则表达式包装
    /// </summary>
    public class StringOrRegex
    {
        private StringOrRegex(string value)
        {
            Value = value;
            IsRegex = false;
        }

        private StringOrRegex(Regex regex)
        {
            Regex = regex;
            IsRegex = true;
        }

        /// <summary>
        /// 文本值
        /// </summary>
        public string? Value { get; }

        /// <summary>
        /// 正则表达式
        /// </summary>
        public Regex? Regex { get; }

        /// <summary>
        /// 当前是否为正则表达式
        /// </summary>
        public bool IsRegex { get; private set; }

        /// <summary>
        /// 将 正则表达 转化为一个 <see cref="StringOrRegex"/> 实例
        /// </summary>
        /// <param name="value">正则表达式</param>
        public static implicit operator StringOrRegex?(Regex? value) => value == null ? default : new StringOrRegex(value);

        /// <summary>
        /// 将 字符串 化为一个 <see cref="StringOrRegex"/> 实例
        /// </summary>
        /// <param name="value">字符串</param>
        public static implicit operator StringOrRegex?(string? value) => value == null ? default : new StringOrRegex(value);
    }
}
