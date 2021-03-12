using System;
using System.Collections.Generic;
using System.Text;

namespace Wechaty.Module.Plugin.Abstractions
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PluginAttribute : Attribute
    {
        /// <summary>
        /// 插件版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 插件描述
        /// </summary>
        public string Description { get; set; }
    }
}
