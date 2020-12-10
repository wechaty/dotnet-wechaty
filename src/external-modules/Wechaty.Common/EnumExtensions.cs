using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Wechaty.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// get <typeparamref name="TAttribute"/> attributes declared on <paramref name="this"/>
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetAttributes<TEnum, TAttribute>(this TEnum @this)
            where TEnum : struct, Enum
            where TAttribute : Attribute => TypeCache<TEnum>.GetAttributes<TAttribute>(@this);

        /// <summary>
        /// get <typeparamref name="TAttribute"/> attribute declared on <paramref name="this"/>
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="this"></param>
        /// <param name="defaultAttribute"></param>
        /// <returns></returns>
        public static TAttribute? GetAttribute<TEnum, TAttribute>(this TEnum @this, TAttribute? defaultAttribute = default)
            where TEnum : struct, Enum
            where TAttribute : Attribute => TypeCache<TEnum>.GetAttributes<TAttribute>(@this)
            .FirstOrDefault() ?? defaultAttribute;

        /// <summary>
        /// get <see cref="EnumMemberAttribute"/> attribute declared on <paramref name="this"/>
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="this"></param>
        /// <param name="defaultEnumMemberAttribute"></param>
        /// <returns></returns>
        public static EnumMemberAttribute? GetEnumMemberAttribute<TEnum>(this TEnum @this, EnumMemberAttribute? defaultEnumMemberAttribute = default)
            where TEnum : struct, Enum => @this.GetAttribute(defaultEnumMemberAttribute);

        /// <summary>
        /// get <see cref="EnumMemberAttribute.Value"/> declared on <paramref name="this"/>
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="this"></param>
        /// <param name="defaultEnumMember"></param>
        /// <returns></returns>
        public static string? GetEnumMember<TEnum>(this TEnum @this, string? defaultEnumMember = default)
            where TEnum : struct, Enum => @this.GetEnumMemberAttribute()?.Value ?? defaultEnumMember;

        private static class TypeCache<TEnum>
            where TEnum : struct, Enum
        {
            private static readonly Type TypeOfT = typeof(TEnum);
            private static readonly Dictionary<TEnum, IEnumerable<Attribute>> ItemsAttributes;

            static TypeCache()
            {
                var fields = TypeOfT.GetFields(BindingFlags.Public | BindingFlags.Static);
                ItemsAttributes = fields
                    .ToDictionary(f => (TEnum)f.GetValue(null), f => f.GetCustomAttributes().OfType<Attribute>());

            }

            public static IEnumerable<TAttribute> GetAttributes<TAttribute>(TEnum @this)
                where TAttribute : Attribute
            {
                if (ItemsAttributes.TryGetValue(@this, out var attributes))
                {
                    return attributes.OfType<TAttribute>();
                }
                return Enumerable.Empty<TAttribute>();
            }
        }
    }
}
