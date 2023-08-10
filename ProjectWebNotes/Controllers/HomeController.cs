using ATMapper;
using Domain.IdentityModel;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Paging;
using ProjectWebNotes.Areas.Manager.Models;
using ProjectWebNotes.DbContextLayer;
using ProjectWebNotes.FileManager;
using ProjectWebNotes.Models;
using Services.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ProjectWebNotes.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbcontext _context;
        private readonly IServiceManager _serviceManager;

        private readonly IMemoryCache _cache;

        public const string _administrator = "Administrator";
        public const string _admin = "Admin";
        public const string _employee = "Employee";

        private const string _KeyListCategorys = "_listallCategorys";
        public HomeController(IServiceManager serviceManager,
                                AppDbcontext context,
                                IMemoryCache memoryCache
                                )
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
            _context = context;
        
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
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyListCategorys, categories, cacheEntryOptions);

            }

            categories = _cache.Get<IEnumerable<Category>>(_KeyListCategorys);
            
            return categories;
        }


        public class Administrator : AppUser
        {
            [Display(Name = "Role")]
            public string RoleName { get; set; }
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
            if (categories != null)
            {
                categories = TreeViews.GetCategoryChierarchicalTree(categories);
            }

            var administrator = await (from r in _context.Roles
                                join ur in _context.UserRoles on r.Id equals ur.RoleId
                                join u in _context.Users on ur.UserId equals u.Id
                                where r.Name.Equals(_administrator) 
                                select new Administrator
                                {
                                    RoleName = r.Name,
                                    UrlImage = u.UrlImage,
                                    LastName = u.LastName,
                                    FirstName = u.FirstName,
                                }
                                ).FirstOrDefaultAsync();

            ViewData["administrator"] = administrator;
            ViewData["posts"] = postnews;    
            return View(categories);
        }

        public class Manager : Administrator
        {
            public List<string> Roles { get; set; }
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

            var managers = await (from r in _context.Roles
                                  join ur in _context.UserRoles on r.Id equals ur.RoleId
                                  join u in _context.Users on ur.UserId equals u.Id
                                  where r.Name.Equals(_administrator) || r.Name.Equals(_admin) || r.Name.Equals(_employee)
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
                                  }
                               ).ToListAsync();


            ViewData["managers"] = managers;

            ViewData["categorys"] = categories;

            return View(managers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public  IActionResult Error()
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public async Task<PartialViewResult> NewPostPartial( int PageNumber)
        {
            var postParameters = new NewPostParameters() { PageNumber = PageNumber };
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
            return PartialView("_PostNewPartial", postnews);
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