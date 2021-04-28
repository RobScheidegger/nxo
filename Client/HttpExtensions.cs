using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace NXO.Client
{
    public static class HttpExtensions
    {
        public static async Task<S> PostFromJsonAsync<T,S>(this HttpClient client, string uri, T request) where S : class
        {
            var result = await client.PostAsJsonAsync(uri, request);
            //var resultString = result.Content.ReadAsStringAsync();
            var jsonResult = await result.Content.ReadFromJsonAsync(typeof(S));
            return jsonResult as S;
        }
    }
}
