using ATMapper;
using Domain.IdentityModel;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Paging;
using ProjectWebNotes.Areas.Manager.Models;
using ProjectWebNotes.DbContextLayer;
using ProjectWebNotes.FileManager;
using ProjectWebNotes.Models;
using Services.Abstractions;
using System.Diagnostics;
using System.Xml.Linq;

namespace ProjectWebNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbcontext _context;
        private readonly IServiceManager _serviceManager;

        private readonly IMemoryCache _cache;

        private readonly UserManager<AppUser> _userManager;
        private const string _KeyListCategorys = "_listallCategorys";
        public HomeController(IServiceManager serviceManager,
                                AppDbcontext context,
                                IMemoryCache memoryCache,
                                UserManager<AppUser> userManager)
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
            _context = context;
            _userManager = userManager;
        }

        [NonAction]
        async Task<IEnumerable<Category>> GetAllTreeViewCategories()
        {

            IEnumerable<Category> categories;

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyListCategorys, out categories))
            {
                categories = await _serviceManager
                    .CategoryService
                    .GetAllAsync(ExpLinqEntity<Category>
                    .ResLinqEntity
                    (ExpExpressions
                    .ExtendInclude<Category>(x => x.Include(x => x.PostCategories))
                    , x => x.OrderBy(x => x.Serial), true));

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(200));
                _cache.Set(_KeyListCategorys, categories, cacheEntryOptions);

            }

            categories = _cache.Get<IEnumerable<Category>>(_KeyListCategorys);
            
            return categories;
        }

       
        [HttpGet]
        public  async Task<IActionResult> Index([FromQuery] NewPostParameters postParameters)
        {
            var categories = await GetAllTreeViewCategories();


            var posts = _serviceManager.PostService.Posts(postParameters);
            var listPosNews = ObjectMapper.Mapper.Map<List<PostSlugCategoryDto>>(posts.ToList());

            var postnews = new PagedList<PostSlugCategoryDto>(listPosNews, posts.TotalCount, postParameters.PageNumber, postParameters.PageSize);


            foreach (var category in categories)
            {
                foreach (var post in postnews)
                {
                    if (post.SlugCategory is null)
                    {
                        if (category.PostCategories.Any(x => x.PostID == post.Id))
                        {
                            post.SlugCategory = category.Slug;
                        }
                    }

                    continue;
                   
                }
            }

            categories = TreeViews.GetCategoryChierarchicalTree(categories);
            
            var admin = await _userManager.FindByNameAsync("admin");

            ViewData["admin"] = admin;
            ViewData["posts"] = postnews;    
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            var categories = await GetAllTreeViewCategories();

            if (categories is null)
            {
                _cache.Remove(_KeyListCategorys);

                categories = await GetAllTreeViewCategories();
            }


            categories = TreeViews.GetCategoryChierarchicalTree(categories);

            var admin = await _userManager.FindByNameAsync("admin");

            ViewData["categorys"] = categories;

            return View(admin);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public  IActionResult Error()
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult SetThemme([FromBody] string data)
        {
            CookieOptions cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Append("theme", data, cookie);
            return Ok();
        }
    }
}