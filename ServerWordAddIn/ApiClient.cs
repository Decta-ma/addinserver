using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServerWordAddIn
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }
    }

    public static class ApiClient
    {
        private static readonly HttpClient Http = Create();

        private static HttpClient Create()
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(Settings.BaseUrl),
                Timeout = TimeSpan.FromSeconds(10)
            };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Settings.ApiToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public static Task<string> GetRibbonXmlAsync()
            => SendAsync(new HttpRequestMessage(HttpMethod.Get, "ribbon"));

        public static async Task<FormSchema> GetFormAsync(string commandId)
        {
            var body = await SendAsync(new HttpRequestMessage(HttpMethod.Get,
                "forms/" + Uri.EscapeDataString(commandId)));
            return JsonConvert.DeserializeObject<FormSchema>(body);
        }

        public static async Task<JArray> RunCommandAsync(string commandId, Dictionary<string, object> values)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "commands/" + Uri.EscapeDataString(commandId))
            {
                Content = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json")
            };
            var body = await SendAsync(request);
            return (JArray)JObject.Parse(body)["actions"];
        }

        private static async Task<string> SendAsync(HttpRequestMessage request)
        {
            HttpResponseMessage response;
            try
            {
                response = await Http.SendAsync(request);
            }
            catch (TaskCanceledException)
            {
                throw new ApiException("The server did not answer in time.");
            }
            catch (HttpRequestException)
            {
                throw new ApiException("Cannot reach the server at " + Settings.BaseUrl +
                                       ". Please check that it is running.");
            }

            var body = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
                throw new ApiException(Describe((int)response.StatusCode, body));
            return body;
        }

        private static string Describe(int status, string body)
        {
            if (status == 401) return "The server rejected the API token.";
            if (status == 422)
            {
                try
                {
                    var errors = JObject.Parse(body)["errors"] as JObject;
                    if (errors != null)
                        return string.Join("\n", errors.Properties()
                            .SelectMany(p => p.Value.Select(m => (string)m)));
                }
                catch (JsonException) { }
            }
            return "The server returned an error (" + status + ").";
        }
    }
}