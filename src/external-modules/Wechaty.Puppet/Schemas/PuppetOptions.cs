using System.Collections.Generic;

namespace Wechaty.Schemas
{
    public class PuppetOptions : Dictionary<string, object?>
    {
        public string? Name { get => TryGetValue("name", out var value) ? value as string : null; set => this["name"] = value; }
        public string? Endpoint { get => TryGetValue("endpoint", out var value) ? value as string : null; set => this["endpoint"] = value; }
        public long? Timeout { get => TryGetValue("timeout", out var value) ? value as long? : null; set => this["timeout"] = value; }
        public string? Token { get => TryGetValue("token", out var value) ? value as string : null; set => this["token"] = value; }
        public string? PuppetProvider { get => TryGetValue("puppetprovider", out var value) ? value as string : null; set => this["puppetprovider"] = value; }

    }
}
