using Domain.IdentityModel;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectWebNotes.DbContextLayer;
using Services.Abstractions;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace ProjectWebNotes.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IServiceManager _serviceManager;
        protected readonly UserManager<AppUser> _userManager;
        public static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        protected readonly IMemoryCache _cache;
        protected string[] _KeyMemorys = { "keycategorys", "Keyadmin" ,"keycategory" };

       
        protected const string _administrator = "Administrator";
        protected const string _employee = "Employee";

        protected const string _admin ="Admin";

        protected readonly ILogger<BaseController> _logger;

        protected readonly AppDbcontext _context;
      
       
        public BaseController(IServiceManager serviceManager,                              
                              IMemoryCache memoryCache,
                              ILogger<BaseController> logger,
                              UserManager<AppUser> userManager,
                              AppDbcontext appDbcontext
                              )
        {
           _userManager = userManager;
            _cache = memoryCache;
           _serviceManager = serviceManager;
            _logger = logger;

            _context = appDbcontext;
        }

        public BaseController(IServiceManager serviceManager, IMemoryCache memoryCache)
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
        }

        public class ViewCategory : Category
        {

        }


        public class Manager : AppUser
        {
            public List<string> Roles { get; set; }
        }



        [NonAction]
        public async Task<IEnumerable<Category>> GetAllTreeViewCategories()
        {

            _logger.Log(LogLevel.Information, "Trying to fetch the list of categorys from cache.");

            if (_cache.TryGetValue(_KeyMemorys[0], out IEnumerable<Category> categories))
            {
                _logger.Log(LogLevel.Information, "Categorys list found in cache.");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(_KeyMemorys[0], out categories))
                    {
                        _logger.Log(LogLevel.Information, "Categorys list found in cache.");
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "Categorys list not found in cache. Fetching from database.");
                     

                         //categories =  _context.Categories
                         //               .Select(c => new CategoryFWDetailCountPost
                         //               {
                         //                   Id = c.Id,
                         //                   Title = c.Title,
                         //                   Description = c.Description,
                         //                   Slug = c.Slug,
                         //                   IConFont = c.IConFont,
                         //                   ParentCategoryId = c.ParentCategoryId,
                         //                   CountPost = c.Posts.Count()
                         //               }).ToList();


                        categories = await _serviceManager.CategoryService
                                            .GetAllAsync(ExtendedQuery<Category>
                                    .Set(null, x => x.OrderBy(x => x.Serial), true));


                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                                .SetAbsoluteExpiration(TimeSpan.FromMinutes(300))
                                .SetPriority(CacheItemPriority.Normal);
                        //.SetSize(1024);
                        _cache.Set(_KeyMemorys[0], categories, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return categories;
        }


        [NonAction]
        public async Task<Manager> GetAdministrator()
        {

            _logger.Log(LogLevel.Information, "Trying to fetch the list of Administrator from cache.");

            if (_cache.TryGetValue(_KeyMemorys[1], out Manager administrator))
            {
                _logger.Log(LogLevel.Information, "Administrator  found in cache.");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(_KeyMemorys[1], out administrator))
                    {
                        _logger.Log(LogLevel.Information, "administrator list found in cache.");
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "administrator list not found in cache. Fetching from database.");


                        administrator = await (from r in _context.Roles
                                                   join ur in _context.UserRoles on r.Id equals ur.RoleId
                                                   join u in _context.Users on ur.UserId equals u.Id
                                                   where u.UserName.Equals("Admin")
                                                   group r by new { u.Email, u.NativePlace, u.UrlImage, u.PhoneNumber, u.Describe, u.Id, u.LastName, u.FirstName, u.Address, u.BirthDate, u.Company } into gr
                                                   select new Manager
                                                   {

                                                       Id = gr.Key.Id,
                                                       UrlImage = gr.Key.UrlImage,
                                                       LastName = gr.Key.LastName,
                                                       FirstName = gr.Key.FirstName,
                                                       Describe = gr.Key.Describe,
                                                       BirthDate = gr.Key.BirthDate,
                                                       PhoneNumber = gr.Key.PhoneNumber,
                                                       Address = gr.Key.Address,
                                                       Company = gr.Key.Company,
                                                       Email = gr.Key.Email,
                                                       NativePlace = gr.Key.NativePlace,
                                                       Roles = gr.Select(acc => acc.Name).ToList()
                                                   }).FirstOrDefaultAsync();

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromSeconds(60))
                                .SetAbsoluteExpiration(TimeSpan.FromMinutes(300))
                                .SetPriority(CacheItemPriority.Normal);
                        //.SetSize(1024);
                        _cache.Set(_KeyMemorys[1], administrator, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return administrator;
        }
    }
}
