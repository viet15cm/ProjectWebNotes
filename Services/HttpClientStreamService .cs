using Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services
{
    public class HttpClientStreamService : IHttpClientServiceImplementation
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly JsonSerializerOptions _options;
        public HttpClientStreamService()
        {
            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri("http://www.microsoft.com");
            }
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _httpClient.DefaultRequestHeaders.Clear();
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public static DateTime GetNistTime()
        {
            string url = "http://www.microsoft.com";

            using (HttpResponseMessage response = _httpClient.GetAsync(url).Result)
            {
                return DateTime.ParseExact(response.Headers.GetValues("date").FirstOrDefault(),
                         "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                         CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal);
            };
        }
    }
}
