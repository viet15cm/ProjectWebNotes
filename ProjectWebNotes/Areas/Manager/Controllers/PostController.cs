
using ATMapper;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Paging;
using ProjectWebNotes.Areas.Manager.Models;
using ProjectWebNotes.Models;
using Services.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;
using static ProjectWebNotes.Areas.Manager.Controllers.PostController;

namespace ProjectWebNotes.Areas.Manager.Controllers
{
    public class PostController : BaseController
    {
        private const string _KeyList = "_listallPosts";
        private const string _KeyObj = "_post";

        private const string _KeyListCatgorys = "_listallPosts";

        public PostController(IServiceManager serviceManager, 
                                IMemoryCache memoryCache,
                                IHttpClientServiceImplementation httpClient)
            : base(serviceManager, memoryCache, httpClient)
        {
        }

        [NonAction]
        async Task<IEnumerable<Category>> GetAllTreeViewCategories()
        {

            IEnumerable<Category> categories;


            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyListCatgorys, out categories))
            {
                categories = await _serviceManager.CategoryService.GetAllWithDetailAsync();

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyListCatgorys, categories, cacheEntryOptions);
            }

            categories = _cache.Get(_KeyListCatgorys) as IEnumerable<Category>;

            return categories;
        }


        [NonAction]
        async Task<PostDto> GetPostDto(string id)
        {

            PostDto post;

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyObj, out post))
            {

                post = await _serviceManager.PostService.GetByIdAsync(id);

                // Thiết lập cache - lưu vào cache             
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyObj, post, cacheEntryOptions);
            }

            post = _cache.Get(_KeyObj) as PostDto;

            if (post.Id != id)
            {
                post = await _serviceManager.PostService.GetByIdAsync(id);
                // Thiết lập cache - lưu vào cache             
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyObj, post, cacheEntryOptions);
            }

            return post;
        }


        [NonAction]
        async Task<IEnumerable<Post>> GetAllPostTreeViews()
        {

            IEnumerable<Post> posts;

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyList, out posts))
            {

                posts = await _serviceManager.PostService.GetAllWithDetailAsync();

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyList, posts, cacheEntryOptions);
            }

            posts = _cache.Get(_KeyList) as IEnumerable<Post>;

            return posts;
        }


        public class BidingPostCategory
        {
            public string IdPost { get; set; }

            [Display(Name ="Danh mục")]
            public string[] PostCategorys { get; set; }

           
            public SelectList addCategory { get; set; }
        }


        public BidingPostCategory bidingPostCategory = new BidingPostCategory();

        [HttpGet]
        public async Task<IActionResult> AddCategory([FromRoute] string id)
        {
            var posts = await GetAllPostTreeViews();

            ViewData["Posts"] = posts;

            var cateforys = await GetAllTreeViewCategories();

            var des = new List<Category>();

            TreeViews.CreateTreeViewCategorySeleteItems(cateforys, des, 0);

            var post = await _serviceManager.PostService.GetByIdWithDetailAsync(id);

            bidingPostCategory = new BidingPostCategory();

            bidingPostCategory.PostCategorys = post.PostCategories.Select(c => c.CategoryID).ToArray();
            
            bidingPostCategory.addCategory = new SelectList(des, "Id", "Title");

            return View(bidingPostCategory);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] BidingPostCategory bidingPostCategory, [FromRoute] string id)
        {
            var post = await _serviceManager.PostService.GetByIdWithDetailAsync(id);

            if (bidingPostCategory.PostCategorys == null)
            {
                bidingPostCategory.PostCategorys = new string[] {};
            }

            var oldPostCategorys = (await _serviceManager
                .PostService
                .GetByIdWithDetailAsync(id))
                ?.PostCategories.Select(c => c.CategoryID);


            var deleteCategorys = oldPostCategorys.Where(x => !bidingPostCategory.PostCategorys.Contains(x));

            var addCategory = bidingPostCategory.PostCategorys.Where(x => !oldPostCategorys.Contains(x));

            await _serviceManager.PostService.RemoveFromCategorysAsync(post.Id, deleteCategorys);

            await _serviceManager.PostService.AddToCategorysAsync(post.Id, addCategory);

            StatusMessage = $"Cập nhật thành công danh mục cho bài viết --{post.Title}--";

            return RedirectToAction("AddCategory", new {id = post.Id });


        }

        async Task<Post> RunPostDetail(string id)
        {
            var post = await _serviceManager
              .PostService
              .GetByIdWithDetailAsync(id, ExpLinqEntity<Post>
              .ResLinqEntity(ExpExpressions
              .ExtendInclude<Post>(x => x.Include(x => x.Images)
                                  .Include(x => x.PostChilds)
                                  .Include(x => x.PostParent)
                                  .Include(x => x.PostChilds)
                                  .Include(x => x.Contents)
                                  .Include(x => x.PostCategories)
                                  .ThenInclude(x => x.Category)
                                  )));

            return post;
        }

        [HttpGet]
        public async Task<IActionResult> Detail([FromRoute]string id)
        {
            var post = await RunPostDetail(id);
            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Contents([FromRoute] string id)
        {
            var post = await RunPostDetail(id);

            var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDImagesDto>(post);
            return View(postFWDImgaesDto);
        }

        [HttpGet]
        public async Task<IActionResult> CreateContent([FromRoute] string id)
        {
            var post = await RunPostDetail(id);

            var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDContentImagesDto>(post);

            var des = new List<ContentSelectDto>();

            TreeViews.CreateTreeViewContentSeleteItems(postFWDImgaesDto.Contents.Where(x => x.ParentContentId == null), des, 0);
            
            ViewData["postFWDImgaesDto"] = postFWDImgaesDto;

            ViewData["TreeViewContentSelete"] = des;

            return View(new ContentForCreateDto());
            
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent([FromForm] ContentForCreateDto contentForCreateDto, [FromRoute] string id
                                                      )
        {
            if (contentForCreateDto?.PostId is null)
            {
                contentForCreateDto.PostId = id;
            }
             var contentDto = await _serviceManager.ContentService.CreateAsync(contentForCreateDto);

            StatusMessage = $"Thêm thành công nội dung ---{contentDto.Title}---";

            return RedirectToAction("detail", new {id = id });
        }


        public async Task<IActionResult> Contents([FromForm] PostForUpdateContentDto postForUpdateContentDto,
                                                    [FromRoute]string id)

        {
            if (!ModelState.IsValid)
            {
                var post = await _serviceManager
                .PostService
                .GetByIdWithDetailAsync(id, ExpLinqEntity<Post>.ResLinqEntity(ExpExpressions.ExtendInclude<Post>(x => x.Include(x => x.Images))));

                var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDImagesDto>(post);
                
                return View(postFWDImgaesDto);
            }

            var postUpdate = await _serviceManager.PostService.UpdateAsync(id, postForUpdateContentDto);

            StatusMessage = $"Cập nhật thành công nội dung --{postUpdate.Title}--";

            return RedirectToAction("Contents", new { id});
        }
        

        [HttpPost]

        public async Task<IActionResult> CreatePost([FromForm] PostForCreationDto postForCreationDto, 
                                                    [FromQuery] PostParameters postParameters)
        {
            if (!ModelState.IsValid)
            {
                var posts = _serviceManager.PostService.Posts(postParameters);
                ViewData["Posts"] = posts;

                return View("Posts", postForCreationDto);
            }

            var post = await _serviceManager.PostService.CreateAsync(postForCreationDto);

            StatusMessage = $"Thêm thành công bài viết #{post.Title}#";

            return RedirectToAction("Posts");
        }

      
        [HttpGet]
         public IActionResult Index([FromQuery] PostParameters postParameters)
         {
            var posts = _serviceManager.PostService.Posts(postParameters);

            ViewData["Posts"] = posts;

            return View(new PostForCreationDto());
         }

        [HttpGet]
        public async Task<IActionResult> EditPost([FromQuery] PostParameters postParameters ,[FromRoute] string id)
        {
            var posts = _serviceManager.PostService.Posts(postParameters);

            ViewData["Posts"] = posts;

            var postDto = await _serviceManager.PostService.GetByIdAsync(id);

            return View(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost([FromForm] PostForUpdateDto postForUpdateDto , 
                                        [FromRoute] string id , 
                                        [FromQuery] PostParameters postParameters)
        {
            if (!ModelState.IsValid)
            {
                var posts = _serviceManager.PostService.Posts(postParameters);

                var postDto = await _serviceManager.PostService.GetByIdAsync(id);

                ViewData["Posts"] = posts;

                return View(postDto);

            }

            var post = await _serviceManager.PostService.UpdateAsync(id, postForUpdateDto);
            StatusMessage = $"Cập nhật thành bài viết #{post.Title}#";
            return RedirectToAction("Posts", new { pageNumber = postParameters.PageNumber});
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost([FromRoute] string id , [FromQuery] PostParameters postParameters)
        {
            var posts = _serviceManager.PostService.Posts(postParameters);

            ViewData["Posts"] = posts;

            var postDto = await _serviceManager.PostService.GetByIdAsync(id);

            return View(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost([FromRoute] string id, bool IsDelte)
        {
            var post = await _serviceManager.PostService.DeleteAsync(id);
           
            StatusMessage = $"Xóa thành công bài viết #{post.Title}# ";
            
            return RedirectToAction("index");

        }

        [HttpGet]
        public async Task<IActionResult> AddItemCategory([FromRoute] string id, [FromQuery] PostParameters postParameters)
        {
            
            var cateforys = await GetAllTreeViewCategories();

            cateforys = TreeViews.GetCategoryChierarchicalTree(cateforys);

            var des = new List<Category>();

            TreeViews.CreateTreeViewCategorySeleteItems(cateforys, des, 0);

            var post = await RunPostDetail(id);

            bidingPostCategory = new BidingPostCategory();

            bidingPostCategory.PostCategorys = post.PostCategories.Select(c => c.CategoryID).ToArray();

            bidingPostCategory.addCategory = new SelectList(des, "Id", "Title");

            bidingPostCategory.IdPost = post.Id;

            return View(bidingPostCategory);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemCategory([FromForm] BidingPostCategory bidingPostCategory, 
                                                            [FromRoute] string id
                                                            )
        {
            var post = await RunPostDetail(id);

            if (bidingPostCategory.PostCategorys == null)
            {
                bidingPostCategory.PostCategorys = new string[] { };
            }

            var oldPostCategorys = post
                .PostCategories.Select(c => c.CategoryID);


            var deleteCategorys = oldPostCategorys.Where(x => !bidingPostCategory.PostCategorys.Contains(x));

            var addCategory = bidingPostCategory.PostCategorys.Where(x => !oldPostCategorys.Contains(x));

            await _serviceManager.PostService.RemoveFromCategorysAsync(post.Id, deleteCategorys);

            await _serviceManager.PostService.AddToCategorysAsync(post.Id, addCategory);

            StatusMessage = $"Cập nhật thành công danh mục cho bài viết --{post.Title}--";

            return RedirectToAction("detail", new { id = post.Id});


        }

    }
}
