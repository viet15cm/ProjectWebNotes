
using Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Paging;
using ProjectWebNotes.Areas.Manager.Models;
using ProjectWebNotes.Models;
using Services.Abstractions;
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
        private void CreateTreeViewSeleteItems(IEnumerable<PostForWithDetailDto> postTreeLayerDtos
                                              , List<PostForWithDetailDto> des,
                                               int leve)
        {

            foreach (var post in postTreeLayerDtos)
            {
                string perfix = string.Concat(Enumerable.Repeat("-", leve));

                des.Add(new PostForWithDetailDto()
                {
                    Id = post.Id,
                    Title = perfix + post.Title
                });

                if (post.PostChilds?.Count > 0)
                {

                    CreateTreeViewSeleteItems(post.PostChilds, des, leve + 1);

                }
            }

        }

        [NonAction]
        private void CreateTreeViewSeleteItems(IEnumerable<CategoryForWithDetailDto> categoryTreeLayerDtos
                                              , List<CategoryForWithDetailDto> des,
                                               int leve)
        {

            foreach (var category in categoryTreeLayerDtos)
            {
                string perfix = string.Concat(Enumerable.Repeat("-", leve));

                des.Add(new CategoryForWithDetailDto()
                {
                    Id = category.Id,
                    Title = perfix + category.Title
                });

                if (category.CategoryChildren?.Count > 0)
                {

                    CreateTreeViewSeleteItems(category.CategoryChildren, des, leve + 1);

                }
            }

        }

        [NonAction]
        async Task<IEnumerable<CategoryForWithDetailDto>> GetAllTreeViewCategories()
        {

            IEnumerable<CategoryForWithDetailDto> categories;

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyListCatgorys, out categories))
            {

                var sqlCategory = await _serviceManager.CategoryService.GetAllWithDetailAsync();

                categories = TreeViews.GetCategoryChierarchicalTree(sqlCategory);

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyListCatgorys, categories, cacheEntryOptions);
            }

            categories = _cache.Get(_KeyListCatgorys) as IEnumerable<CategoryForWithDetailDto>;

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
        async Task<IEnumerable<PostForWithDetailDto>> GetAllPostTreeViews()
        {

            IEnumerable<PostForWithDetailDto> posts;


            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyList, out posts))
            {

                posts = await _serviceManager.PostService.GetAllWithDetailAsync();

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyList, posts, cacheEntryOptions);
            }

            posts = _cache.Get(_KeyList) as IEnumerable<PostForWithDetailDto>;

            return posts;
        }

     


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var posts = await GetAllPostTreeViews();

            ViewData["Posts"] = posts;

            var des = new List<PostForWithDetailDto>();

            CreateTreeViewSeleteItems(posts, des, 0);

            ViewData["TreeViewPosts"] = des;
            return View(new PostForCreationDto());
        }
        
        

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PostForCreationDto postForCreationDto)
        {

            if (!ModelState.IsValid)
            {
                var posts = await GetAllPostTreeViews();
                ViewData["Posts"] = posts;

                var des = new List<PostForWithDetailDto>();

                CreateTreeViewSeleteItems(posts, des, 0);

                ViewData["TreeViewPosts"] = des;

                return View("index", postForCreationDto);
            }

            

            var post = await _serviceManager.PostService.CreateAsync(postForCreationDto);

            StatusMessage = $"Thêm thành bài viết #{post.Title}#";

            _cache.Remove(_KeyList);

            return RedirectToAction("index");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var posts = await GetAllPostTreeViews();

            ViewData["Posts"] = posts;

            var cateforyDto = await GetPostDto(id);

            return View(cateforyDto);
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] string id, bool IsDelte)
        {
            var post = await _serviceManager.PostService.DeleteAsync(id);
            _cache.Remove(_KeyList);
            StatusMessage = $"Xóa thành công bài viết #{post.Title}# ";
            return RedirectToAction("index");

        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var posts = await GetAllPostTreeViews();

            ViewData["Posts"] = posts;

            var des = new List<PostForWithDetailDto>();

            CreateTreeViewSeleteItems(posts, des, 0);

            ViewData["TreeViewPosts"] = des;

            var postDto = await GetPostDto(id);

           

            return View(postDto);

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] PostForUpdateDto postForUpdateDto)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var posts = await GetAllPostTreeViews();

                ViewData["posts"] = posts;

                var des = new List<PostForWithDetailDto>();

                CreateTreeViewSeleteItems(posts, des, 0);

                ViewData["TreeViewPosts"] = des;

                return View(postForUpdateDto);
            }

            var post = await _serviceManager.PostService.UpdateAsync(id, postForUpdateDto);

            StatusMessage = $"Cập nhật thành bài viết #{post.Title}#";

            _cache.Remove(_KeyList);

            _cache.Remove(_KeyObj);

            return RedirectToAction("Edit");

        }

        public class BidingPostCategory
        {

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

            var des = new List<CategoryForWithDetailDto>();

            CreateTreeViewSeleteItems(cateforys, des, 0);

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

        [HttpGet]
        public async Task<IActionResult> Contents(string id)
        {
            var post = await _serviceManager.PostService.GetByIdAsync(id);

            
            
            return View(post);
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
         public IActionResult Posts([FromQuery] PostParameters postParameters)
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
            
            return RedirectToAction("Posts");

        }

        [HttpGet]
        public async Task<IActionResult> AddItemCategory([FromRoute] string id, [FromQuery] PostParameters postParameters)
        {
            var posts = _serviceManager.PostService.Posts(postParameters);

            ViewData["Posts"] = posts;

            var cateforys = await GetAllTreeViewCategories();

            var des = new List<CategoryForWithDetailDto>();

            CreateTreeViewSeleteItems(cateforys, des, 0);

            var post = await _serviceManager.PostService.GetByIdWithDetailAsync(id);

            bidingPostCategory = new BidingPostCategory();

            bidingPostCategory.PostCategorys = post.PostCategories.Select(c => c.CategoryID).ToArray();

            bidingPostCategory.addCategory = new SelectList(des, "Id", "Title");

            return View(bidingPostCategory);
        }

        [HttpPost]
        public async Task<IActionResult> AddItemCategory([FromForm] BidingPostCategory bidingPostCategory, 
                                                            [FromRoute] string id , 
                                                            [FromQuery] PostParameters postParameters)
        {
            var post = await _serviceManager.PostService.GetByIdWithDetailAsync(id);

            if (bidingPostCategory.PostCategorys == null)
            {
                bidingPostCategory.PostCategorys = new string[] { };
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

            return RedirectToAction("AddItemCategory", new { id = post.Id , pageNumber = postParameters.PageNumber });


        }

    }
}
