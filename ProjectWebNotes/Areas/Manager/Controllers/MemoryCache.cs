using Domain.IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Services.Abstractions;

namespace ProjectWebNotes.Areas.Manager.Controllers
{
    public class MemoryCache : BaseController
    {
        public MemoryCache(IServiceManager serviceManager, IMemoryCache memoryCache, UserManager<AppUser> userManager, IAuthorizationService authorizationService, ILogger<BaseController> logger) : base(serviceManager, memoryCache, userManager, authorizationService, logger)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Delete() 
        {
            _cache.Remove(_KeyMemorys[0]);
            _cache.Remove(_KeyMemorys[1]);
            _cache.Remove(_KeyMemorys[2]);

            StatusMessage = $"Bộ nhớ cache đã bị xóa.";

            return RedirectToAction("index");

        }
    }
}
