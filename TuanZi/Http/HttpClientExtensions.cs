using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace TuanZi.Http
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PatchAsync(this HttpClient client, string url, HttpContent content)
        {
            return client.PatchAsync(new Uri(url), content);
        }

        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, Uri uri, HttpContent content)
        {
            HttpMethod method = new HttpMethod("PATCH");
            HttpRequestMessage request = new HttpRequestMessage(method, uri) { Content = content };
            HttpResponseMessage response = await client.SendAsync(request);
            return response;
        }
    }
}