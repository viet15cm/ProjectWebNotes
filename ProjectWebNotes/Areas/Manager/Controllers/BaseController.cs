using Domain.IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Abstractions;

namespace ProjectWebNotes.Areas.Manager.Controllers
{

    [Area("Manager")]
    public class BaseController : Controller
    {
        protected readonly IServiceManager _serviceManager;

        protected readonly UserManager<AppUser> _userManager;

        protected readonly IAuthorizationService _authorizationService;
        
        protected readonly IMemoryCache _cache;


        public BaseController(IServiceManager serviceManager , IMemoryCache memoryCache, UserManager<AppUser> userManager , IAuthorizationService authorizationService)
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }


        [TempData]
        public string StatusMessage { get; set; }
    }
}
