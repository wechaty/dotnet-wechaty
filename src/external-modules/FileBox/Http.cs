using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Wechaty
{
    internal static class Http
    {
        private static HttpClient GetHttpClient(string url)
        {
            var uri = new Uri(url);
            //TODO:should use http client factory
            var client = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
            return client;
        }

        public static async Task<HttpContentHeaders> HeadContentHeaders(string url)
        {
            var client = GetHttpClient(url);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Head
            };
            var response = await client.SendAsync(request);
            response = response.EnsureSuccessStatusCode();
            return response.Content.Headers;
        }

        public static async Task<byte[]> GetFile(string url, IDictionary<string, IEnumerable<string>>? headers = default)
        {
            var client = GetHttpClient(url);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get
            };
            if (headers?.Count > 0)
            {
                foreach (var item in headers)
                {
                    request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }
            }
            var response = await client.SendAsync(request);
            response = response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        public static async Task<Stream> GetStream(string url, IDictionary<string, IEnumerable<string>>? headers = default)
        {
            var client = GetHttpClient(url);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get
            };
            if (headers?.Count > 0)
            {
                foreach (var item in headers)
                {
                    request.Headers.TryAddWithoutValidation(item.Key, item.Value);
                }
            }
            var response = await client.SendAsync(request);
            response = response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
