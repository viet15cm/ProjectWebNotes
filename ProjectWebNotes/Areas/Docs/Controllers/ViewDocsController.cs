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
using ProjectWebNotes.Areas.Manager.Controllers;
using ProjectWebNotes.Areas.Manager.Models;
using Services.Abstractions;
using System.Threading;

namespace ProjectWebNotes.Areas.Docs.Controllers
{
    [Area("Docs")]
    public class ViewDocsController : BaseController
    {
        public ViewDocsController(IServiceManager serviceManager, IMemoryCache memoryCache, UserManager<AppUser> userManager, IAuthorizationService authorizationService, ILogger<BaseController> logger) : base(serviceManager, memoryCache, userManager, authorizationService, logger)
        {
        }

      
        [HttpGet]
        [Route("post/{post?}")]
        public async Task<IActionResult> Post([FromRoute] string post)
        {

            //Memory cache
            var categorys = await GetAllTreeViewCategories();

            if (post is null)
            {
                return NotFound();
            }

            categorys = TreeViews.GetCategoryChierarchicalTree(categorys);

            if (categorys is null)
            {
                return NotFound("Categorys is null ");
            }

            var curentPost = await _serviceManager.PostService.GetBySlugAsync(post, ExtendedQuery<Post>.Set(ExtendedInclue
                                    .Set<Post>(x => x.Include(x => x.Category).Include(x => x.Contents)),
                                                                    x => x.OrderBy(x => x.Serial), true));

            if (curentPost.Category is null)
            {
                return NotFound();
            }

            var category = await _serviceManager
                                    .CategoryService
                                    .GetBySlugAsync(curentPost.Category.Slug, ExtendedQuery<Category>
                                    .Set(ExtendedInclue
                                    .Set<Category>(x => x.Include(x => x.CategoryChildren)
                                                                   .Include(x => x.Posts)),
                                                                    x => x.OrderBy(x => x.Serial), true));

            if (category == null)
            {
                return NotFound("Category is null");
            }

            var listPostInCategory = category.Posts.ToList();

            listPostInCategory = TreeViews.GetPostChierarchicalTree(listPostInCategory);

            var listSerialPosts = new List<Post>();


            var data = FindPostBySlug(listPostInCategory, post, listSerialPosts); // chỉnh lại code

            var listSerialUrl = listSerialPosts.Select(p => p.Slug).ToList();
            
            ViewData["slugPost"] = curentPost.Slug;
            ViewData["listSerialUrl"] = listSerialUrl;
            ViewData["currentCategory"] = category;
            ViewData["categorys"] = categorys;

            curentPost.Contents = TreeViews.GetContentChierarchicalTree(curentPost.Contents);

            return View(curentPost);

        }

        [Route("category/{category?}")]
        [HttpGet]
        public async Task<IActionResult> Index([FromRoute(Name = "category")] string slugCategory)
        {

            //Memory cache
            var categorys = await GetAllTreeViewCategories();

            if (slugCategory is null)
            {
                return NotFound();
            }

            categorys = TreeViews.GetCategoryChierarchicalTree(categorys);

            if (categorys is null)
            {
                return NotFound("Categorys is null ");
            }
            

            var category = await _serviceManager
                                    .CategoryService
                                    .GetBySlugAsync(slugCategory, ExtendedQuery<Category>
                                    .Set(ExtendedInclue
                                    .Set<Category>(x => x.Include(x => x.CategoryChildren)
                                                                    .Include(x => x.Posts)),                                                                  
                                                                    x => x.OrderBy(x => x.Serial), true));

            if (category == null)
            {
                return NotFound("Category is null");
            }


            category.Posts = TreeViews.GetPostChierarchicalTree(category.Posts.ToList());

            ViewData["slugPost"] = null;
            ViewData["listSerialUrl"] = new List<string>();
            ViewData["currentCategory"] = category;
            ViewData["categorys"] = categorys;


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
