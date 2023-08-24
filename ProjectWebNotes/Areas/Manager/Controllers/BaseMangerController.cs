using Domain.IdentityModel;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectWebNotes.DbContextLayer;
using ProjectWebNotes.FileManager;
using ProjectWebNotes.Models;
using Services.Abstractions;

namespace ProjectWebNotes.Areas.Manager.Controllers
{

    [Area("Manager")]
    public class BaseMangerController : Controller
    {
        protected readonly IServiceManager _serviceManager;
        public static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        protected readonly IMemoryCache _cache;     
        protected readonly ILogger<BaseDocController> _logger;
        protected UserManager<AppUser> _userManager;
        
        protected readonly IAuthorizationService _authorizationService;

        protected const string UrlPost = "/post/";
        
        

        public BaseMangerController(IServiceManager serviceManager,
                                IMemoryCache memoryCache,
                                UserManager<AppUser> userManager ,
                                IAuthorizationService authorizationService,
                                ILogger<BaseDocController> logger                            
                               )
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _logger= logger;         
           
        }


        [TempData]
        public string StatusMessage { get; set; }



        [NonAction]
        public async Task<IEnumerable<Category>> GetAllTreeViewCategories()
        {

            _logger.Log(LogLevel.Information, "Trying to fetch the list of categorys from cache.");

            if (_cache.TryGetValue(MemoryCaheKey.KeyMemorys[0], out IEnumerable<Category> categories))
            {
                _logger.Log(LogLevel.Information, "Categorys list found in cache.");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(MemoryCaheKey.KeyMemorys[0], out categories))
                    {
                        _logger.Log(LogLevel.Information, "Categorys list found in cache.");
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "Categorys list not found in cache. Fetching from database.");


                        categories = await _serviceManager.CategoryService
                                            .GetAllAsync(ExtendedQuery<Category>
                                    .Set(null, x => x.OrderBy(x => x.Serial), true));

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                                .SetAbsoluteExpiration(TimeSpan.FromMinutes(300))
                                .SetPriority(CacheItemPriority.Normal);
                        //.SetSize(1024);
                        _cache.Set(MemoryCaheKey.KeyMemorys[0], categories, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return categories;
        }

    }
}
