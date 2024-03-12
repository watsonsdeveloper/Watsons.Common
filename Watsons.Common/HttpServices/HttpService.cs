using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Watsons.Common.HttpServices
{
    public class HttpService : IHttpService
    { 
        private readonly IHttpClientFactory _httpClientFactory;
        public HttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<Tout?> PostAsnyc<Tin, Tout>(string url, Tin? body, Dictionary<string, dynamic>? headers = null, Dictionary<string, dynamic>? query = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                var json = JsonSerializer.Serialize(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await httpClient.PostAsync(url, content);

                response.EnsureSuccessStatusCode();  // Throws an exception if the HTTP response status is an error code.

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Tout>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }

        public async Task<Tout?> GetAsync<Tout>(string url, Dictionary<string, dynamic>? query, Dictionary<string, dynamic>? headers = null)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                url = url + "?";
                foreach (var item in query)
                {
                    url = url + item.Key + "=" + item.Value + "&";
                }
                HttpResponseMessage response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();  // Throws an exception if the HTTP response status is an error code.

                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Tout>(responseContent);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
        }
    }
}
