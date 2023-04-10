using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Abstractions;

namespace ProjectWebNotes.Areas.Manager.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IServiceManager _serviceManager;

        protected readonly IMemoryCache _cache;

        protected readonly IHttpClientServiceImplementation _httpClient;

        public BaseController()
        {

        }

        public BaseController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;

        }

        public BaseController(IServiceManager serviceManager, IMemoryCache memoryCache)
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
          
        }
        public BaseController(IServiceManager serviceManager , IMemoryCache memoryCache, IHttpClientServiceImplementation httpClient)
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
            _httpClient = httpClient;
        }


        [TempData]
        public string StatusMessage { get; set; }
    }
}
