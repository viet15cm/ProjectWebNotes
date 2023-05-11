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
     
        //private readonly JsonSerializerOptions _options;
        private readonly HttpClient _httpClient;

        public HttpClientStreamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public DateTime GetNistTime()
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
