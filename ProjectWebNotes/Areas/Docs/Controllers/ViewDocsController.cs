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
using ProjectWebNotes.FileManager;
using Services.Abstractions;
using System.Threading;

namespace ProjectWebNotes.Areas.Docs.Controllers
{
    [Area("Docs")]
    public class ViewDocsController : BaseDocController
    {
        
       

        private readonly IFileServices _fileServices;
        public ViewDocsController(IServiceManager serviceManager, 
            IMemoryCache memoryCache, UserManager<AppUser> userManager,
            IAuthorizationService authorizationService,
            ILogger<BaseDocController> logger ,
            IFileServices fileServices,
            IHttpContextAccessor httpContextAccessor) : base(serviceManager, memoryCache, userManager, authorizationService, logger , httpContextAccessor)
        {
            _fileServices = fileServices;
         
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

            category.Posts = TreeViews.GetPostChierarchicalTree(category.Posts);

            category.IConFont = _fileServices.HttpContextAccessorPathImgSrcIndex(IconCategory.GetIcon(), category.IConFont);

            if (category.IConFont is null)
            {
                category.IConFont = HttpContextAccessorPathDomain() + UrlBannerCategoryDefault;
            }

            if (category.CategoryChildren?.Count > 0)
            {
                foreach (var cate in category.CategoryChildren)
                {
                    cate.IConFont = _fileServices.HttpContextAccessorPathImgSrcIndex(IconCategory.GetIcon(), cate.IConFont);

                    if (cate.IConFont is null)
                    {
                        cate.IConFont = HttpContextAccessorPathDomain() + UrlBannerCategoryDefault;
                    }
                }
            }

            if (category == null)
            {
                return NotFound("Category is null");
            }

            var listPostInCategory = category.Posts.ToList();


            var listSerialUrl = new List<string>();


            var data = FindPostBySlug(listPostInCategory, post, listSerialUrl); // chỉnh lại code
          
            ViewData["slugPost"] = curentPost.Slug;
            ViewData["listSerialUrl"] = listSerialUrl;
            ViewData["currentCategory"] = category;
            ViewData["categorys"] = categorys;

            ViewData["curenturl"] = HttpContextAccessorPathDomain() + UrlPost + curentPost.Slug;

            curentPost.Contents = TreeViews.GetContentChierarchicalTree(curentPost.Contents);

            curentPost.Banner = _fileServices.HttpContextAccessorPathImgSrcIndex(BannerPost.GetBannerPost(), curentPost.Banner);

            if (curentPost.Banner is null)
            {
                curentPost.Banner = HttpContextAccessorPathDomain() + UrlBannerPostDefault;
            }

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


            category.Posts = TreeViews.GetPostChierarchicalTree(category.Posts);

            ViewData["slugPost"] = null;
            ViewData["listSerialUrl"] = new List<string>();
            ViewData["currentCategory"] = category;
            ViewData["categorys"] = categorys;
            ViewData["curenturl"] = HttpContextAccessorPathDomain() + UrlCategory + category.Slug;
            
            category.IConFont = _fileServices.HttpContextAccessorPathImgSrcIndex(IconCategory.GetIcon(), category.IConFont);

            if (category.IConFont is null)
            {
                category.IConFont = HttpContextAccessorPathDomain() + UrlBannerCategoryDefault;
            }

            if (category.CategoryChildren?.Count> 0)
            {
                foreach (var cate in category.CategoryChildren)
                {
                    cate.IConFont = _fileServices.HttpContextAccessorPathImgSrcIndex(IconCategory.GetIcon(), cate.IConFont);

                    if (cate.IConFont is null)
                    {
                        cate.IConFont = HttpContextAccessorPathDomain() + UrlBannerCategoryDefault;
                    }
                }
            }
            return View(category);
        }


        [NonAction]
        Post FindPostBySlug(List<Post> Posts, string Slug, List<string> Slugs)
        {

            try
            {
                foreach (var p in Posts)
                {
                    // xử lý cộng nối tiếp các url có trong node

                    Slugs.Add(p.Slug);

                    if (p.Slug == Slug)
                    {
                        return p;
                    }

                    var p1 = FindPostBySlug(p.PostChilds?.ToList() ?? new List<Post>(), Slug, Slugs);

                    if (p1 != null)
                        return p1;
                }
                Slugs.RemoveAt(Slugs.Count() - 1);

                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }

        
    }


}
