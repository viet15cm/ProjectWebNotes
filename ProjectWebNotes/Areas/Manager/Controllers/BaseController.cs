using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Abstractions;

namespace ProjectWebNotes.Areas.Manager.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IServiceManager _serviceManager;

        private readonly UserManager<AppUser> _userManager;

        protected readonly IMemoryCache _cache;


        public BaseController(IServiceManager serviceManager , IMemoryCache memoryCache, UserManager<AppUser> userManager)
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
            _userManager = userManager;
        }


        [TempData]
        public string StatusMessage { get; set; }
    }
}
