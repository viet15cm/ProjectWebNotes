using ATMapper;
using Domain.IdentityModel;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using ProjectWebNotes.Areas.Manager.Models;
using Services.Abstractions;

namespace ProjectWebNotes.Areas.Docs.Controllers
{
    [Area("Docs")]
    public class ViewDocsController : Controller
    {
        protected readonly IServiceManager _serviceManager;

        protected readonly IMemoryCache _cache;

        private const string _KeyCategory = "_KeyCategory";

        private const string _KeyListCategorys = "_listallCategorys";
        public ViewDocsController(IServiceManager serviceManager , IMemoryCache memoryCache)
        {
            _serviceManager = serviceManager;
            _cache = memoryCache;
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
                    .ResLinqEntity(null, x => x.OrderBy(x => x.Serial), true));

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(200));
                _cache.Set(_KeyListCategorys, categories, cacheEntryOptions);
            }
            
            categories = _cache.Get(_KeyListCategorys) as IEnumerable<Category>;
            

            return categories;
        }

        [NonAction]
        async Task<Category> GetBySlugCategoy(string slug)
        {

            Category category;

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyCategory, out category))
            {

                category = await _serviceManager
                    .CategoryService
                    .GetBySlugAsync(slug, ExpLinqEntity<Category>
                    .ResLinqEntity(ExpExpressions
                    .ExtendInclude<Category>(x => x.Include(x => x.CategoryChildren)
                                                    .Include(x => x.PostCategories)
                                                    .ThenInclude(x => x.Post)
                                                    .ThenInclude(x => x.Contents)),
                                                    x => x.OrderBy(x => x.Serial), true));
             
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(200));
                _cache.Set(_KeyCategory, category, cacheEntryOptions);
            }

            category = _cache.Get(_KeyCategory) as Category;
        

            if (category.Slug != slug)
            {
                category = await _serviceManager
                    .CategoryService
                    .GetBySlugAsync(slug, ExpLinqEntity<Category>
                    .ResLinqEntity(ExpExpressions
                    .ExtendInclude<Category>(x => x.Include(x => x.CategoryChildren)
                                                    .Include(x => x.PostCategories)
                                                    .ThenInclude(x => x.Post)
                                                    .ThenInclude(x => x.Contents)), x => x.OrderBy(x => x.Serial), true));
                // Thiết lập cache - lưu vào cache             
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(200));
                _cache.Set(_KeyCategory, category, cacheEntryOptions);

                category = _cache.Get(_KeyCategory) as Category;
            }

            return category;
        }


        [Route("Docs/{category?}")]
        public async Task<IActionResult> Index([FromRoute(Name = "category")] string slugCategory, [FromQuery] string post)
        {

            //Memory cache
            var categorys = await GetAllTreeViewCategories();

            if (slugCategory == null)
            {
                return NotFound();
            }

            categorys = TreeViews.GetCategoryChierarchicalTree(categorys);
            
            //Memory cache
            var category = await GetBySlugCategoy(slugCategory);


            if (category == null)
            {
                return NotFound();
            }

            var listPostInCategory = new List<Post>();

            foreach (var item in category.PostCategories)
            {
                listPostInCategory.Add(item.Post);
            }

            listPostInCategory = TreeViews.GetPostChierarchicalTree(listPostInCategory);

            Post postCurent = null;

            var listSerialPosts = new List<Post>();

            if (!string.IsNullOrEmpty(post))
            {
                postCurent = FindPostBySlug(listPostInCategory, post, listSerialPosts);

            }

            var listSerialUrl = listSerialPosts.Select(p => p.Slug).ToList();
            
           
            ViewData["slugCategory"] = slugCategory;
            ViewData["slugPost"] = post;
            ViewData["listSerialUrl"] = listSerialUrl;
            ViewData["listPostInCategory"] = listPostInCategory;
            ViewData["currentCategory"] = category;
            ViewData["categorys"] = categorys;
           
            //ViewData["relateToPost"] = relateToPost;
            //ViewData["currentParentCategory"] = currentParentCategory;

            if (postCurent != null)
            {
                postCurent.Contents = TreeViews.GetContentChierarchicalTree(postCurent.Contents);
                
                return View("post", postCurent);

            }
            return View(category);
        }

        [NonAction]
        Category FindCategoryBySlug(List<Category> categories, string Slug, List<Category> listCategory)
        {

            try
            {
                foreach (var c in categories)
                {
                    // xử lý cộng nối tiếp các url có trong node

                    listCategory.Add(c);

                    if (c.Slug == Slug)
                    {
                        return c;
                    }
                    var c1 = FindCategoryBySlug(c.CategoryChildren?.ToList(), Slug, listCategory);

                    if (c1 != null)
                        return c1;
                }
                listCategory.RemoveAt(listCategory.Count() - 1);

                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        [NonAction]
        Category FindCategoryBySlug(ICollection<Category> categories, string Slug, List<Category> listCategory)
        {

            try
            {
                foreach (var c in categories)
                {
                    // xử lý cộng nối tiếp các url có trong node

                    listCategory.Add(c);

                    if (c.Slug == Slug)
                    {
                        return c;
                    }
                    var c1 = FindCategoryBySlug(c.CategoryChildren?.ToList(), Slug, listCategory);

                    if (c1 != null)
                        return c1;
                }
                listCategory.RemoveAt(listCategory.Count() - 1);

                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        [NonAction]
        Post FindPostBySlug(List<Post> Posts, string Slug, List<Post> listPost)
        {

            try
            {
                foreach (var p in Posts)
                {
                    // xử lý cộng nối tiếp các url có trong node

                    listPost.Add(p);

                    if (p.Slug == Slug)
                    {
                        return p;
                    }

                    var p1 = FindPostBySlug(p.PostChilds?.ToList() ?? new List<Post>(), Slug, listPost);

                    if (p1 != null)
                        return p1;
                }
                listPost.RemoveAt(listPost.Count() - 1);

                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        
    }


}
