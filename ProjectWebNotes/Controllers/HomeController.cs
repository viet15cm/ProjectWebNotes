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
using Microsoft.Extensions.Hosting;
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
    public class HomeController : BaseController
    {
        private readonly IFileServices _fileServices;
        
        public HomeController(IServiceManager serviceManager, IMemoryCache memoryCache, ILogger<BaseController> logger, UserManager<AppUser> userManager, AppDbcontext appDbcontext, IFileServices fileServices ) : base(serviceManager, memoryCache, logger, userManager, appDbcontext)
        {
            _fileServices = fileServices;
        }


        [HttpGet]
        public  async Task<IActionResult> Index([FromQuery] NewPostParameters postParameters)
        {
            var categories = await GetAllTreeViewCategories();

            if (categories != null)
            {
                categories = TreeViews.GetCategoryChierarchicalTree(categories);
            }

            var postqery =(from p in _context.Posts
                       join u in _context.Users on p.AuthorId equals u.Id
                       join c in _context.Categories on p.CategoryId equals c.Id
                       where p.CategoryId != null
                       select new PostNewDto
                       {
                           Id = p.Id,
                           Slug = p.Slug,
                           AuthorName = u.UserName,
                           Banner = p.Banner,
                           Title = p.Title,
                           DateCreate = p.DateCreate,
                           DateUpdated = p.DateUpdated,
                           Description = p.Description,
                           CategoryId = p.CategoryId,
                           TitleCategory = c.Title,
                           SlugCategory = c.Slug,

                       }).OrderByDescending(x => x.DateCreate);

            var postnews = PagedList<PostNewDto>.ToPagedList(postqery, postParameters.PageNumber, postParameters.PageSize);

            foreach (var post in postnews)
            {
                post.Banner = post.Banner = _fileServices.HttpContextAccessorPathImgSrcIndex(BannerPost.GetBannerPost(), post.Banner);
            }

            foreach (var post in postnews)
            {
                if (post.Description is null)
                {
                    continue;
                }

                var spaces = post.Description.Split(' ');

                if (spaces.Length <= 30)
                {
                    continue;
                }

                post.Description = "";

                for (int i = 0; i < 27; i++)
                {
                    post.Description += spaces[i] + " ";
                }

                post.DescriptionCollapse = "";

                for (int i = 27; i < spaces.Length; i++)
                {
                    post.DescriptionCollapse += spaces[i] + " ";
                }
                
            }

            var administrator = await GetAdministrator();

            ViewData["administrator"] = administrator;
            ViewData["posts"] = postnews;    
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            var categories = await GetAllTreeViewCategories();

           
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

            foreach (var manager in managers)
            {
                manager.UrlImage = _fileServices.HttpContextAccessorPathImgSrcIndex(Avatar.GetAvartar(), manager.UrlImage);
            }
           

            ViewData["categorys"] = categories;

            return View(managers);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public  IActionResult Error()
        {
            
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public  PartialViewResult NewPostPartial( int PageNumber)
        {
            var postParameters = new NewPostParameters() { PageNumber = PageNumber };

            var posts = (from p in _context.Posts
                         join u in _context.Users on p.AuthorId equals u.Id
                         join c in _context.Categories on p.CategoryId equals c.Id
                         where p.CategoryId != null
                         select new PostNewDto
                         {
                             Id = p.Id,
                             Slug = p.Slug,
                             AuthorName = u.UserName,
                             Banner = p.Banner,
                             Title = p.Title,
                             DateCreate = p.DateCreate,
                             DateUpdated = p.DateUpdated,
                             Description = p.Description,
                             CategoryId = p.CategoryId,
                             TitleCategory = c.Title,
                             SlugCategory = c.Slug

                         }).OrderByDescending(x => x.DateCreate);

            var postnews = PagedList<PostNewDto>.ToPagedList(posts, postParameters.PageNumber, postParameters.PageSize);
            
            return PartialView("_PostNewPartial", postnews);
        }

        [HttpGet]
        public PartialViewResult ShowNewPostPartial(int PageNumber)
        {
           
            var postParameters = new NewPostParameters() { PageNumber = PageNumber };

            var posts = (from p in _context.Posts
                         join u in _context.Users on p.AuthorId equals u.Id
                         join c in _context.Categories on p.CategoryId equals c.Id
                         where p.CategoryId != null
                         select new PostNewDto
                         {
                             Id = p.Id,
                             Slug = p.Slug,
                             AuthorName = u.UserName,
                             Banner = p.Banner,
                             Title = p.Title,
                             DateCreate = p.DateCreate,
                             DateUpdated = p.DateUpdated,
                             Description = p.Description,
                             CategoryId = p.CategoryId,
                             TitleCategory = c.Title,
                             SlugCategory = c.Slug,

                         }).OrderByDescending(x => x.DateCreate);
            
            var postnews = PagedList<PostNewDto>.ToPagedList(posts, postParameters.PageNumber, postParameters.PageSize);


            foreach (var post in postnews)
            {
                if (post.Description is null)
                {
                    continue;
                }

                var spaces = post.Description.Split(' ');

                if (spaces.Length <= 27)
                {
                    continue;
                }

                post.Description = "";

                for (int i = 0; i < 27; i++)
                {
                    post.Description += spaces[i] + " ";
                }

                post.DescriptionCollapse = "";

                for (int i = 27; i < spaces.Length; i++)
                {
                    post.DescriptionCollapse += spaces[i] + " ";
                }
            }

            
            foreach (var post in postnews)
            {
                post.Banner = post.Banner = _fileServices.HttpContextAccessorPathImgSrcIndex(BannerPost.GetBannerPost(), post.Banner);
            }


            return PartialView("_ShowNewPostPartial", postnews);
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