using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wechaty.Module.Puppet.Schemas
{
    internal static class FilterExtensions
    {
        public static Func<TElement, bool> Every<TFilter, TElement>(this TFilter filter)
            where TFilter : IFilter
        {
            var keys = filter.Keys;
            var expectedKeyValues = new Dictionary<string, StringOrRegex>();
            foreach (var key in keys)
            {
                var expected = filter[key];
                if (expected != null)
                {
                    expectedKeyValues.Add(key, expected);
                }
            }
            var properties = typeof(TElement).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p);
            //TODO: reflect faster
            return element =>
            {
                foreach (var item in expectedKeyValues)
                {
                    if (properties.TryGetValue(item.Key, out var property))
                    {
                        string actual;
                        if (property.PropertyType == typeof(string))
                        {
                            actual = property.GetValue(element) as string;
                        }
                        else
                        {
                            actual = property.GetValue(element)?.ToString();
                        }
                        if (item.Value.IsRegex)
                        {
                            if (!item.Value.Regex.IsMatch(actual))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (actual != item.Value.Value)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            };
        }

        public static Func<TElement, bool> Single<TFilter, TElement>(this TFilter filter)
            where TFilter : IFilter
        {
            var keys = filter.Keys;
            var expectedKeyValues = new Dictionary<string, StringOrRegex>();
            foreach (var key in keys)
            {
                var expected = filter[key];
                if (expected != null)
                {
                    expectedKeyValues.Add(key, expected);
                }
            }
            if (expectedKeyValues.Count > 1)
            {
                throw new ArgumentException("you should provide only on property for filtering", nameof(filter));
            }
            var properties = typeof(TElement).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p);
            //TODO: reflect faster
            return element =>
            {
                foreach (var item in expectedKeyValues)
                {
                    if (properties.TryGetValue(item.Key, out var property))
                    {
                        string actual;
                        if (property.PropertyType == typeof(string))
                        {
                            actual = property.GetValue(element) as string;
                        }
                        else
                        {
                            actual = property.GetValue(element)?.ToString();
                        }
                        if (item.Value.IsRegex)
                        {
                            if (!item.Value.Regex.IsMatch(actual))
                            {
                                return false;
                            }
                        }
                        else
                        {
                            if (actual != item.Value.Value)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            };
        }
    }
}
