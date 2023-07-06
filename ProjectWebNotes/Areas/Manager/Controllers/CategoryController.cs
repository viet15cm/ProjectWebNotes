using ATMapper;
using Domain.IdentityModel;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ProjectWebNotes.Areas.Manager.Models;
using Services.Abstractions;


namespace ProjectWebNotes.Areas.Manager.Controllers
{
    [Authorize(Policy = "Admin")]
    public class CategoryController : BaseController 
    {

        private const string _KeyListCategorys = "_listallCategorys";
        private const string _KeyCategory = "_category";

        public CategoryController(IServiceManager serviceManager, 
                                IMemoryCache memoryCache, 
                                UserManager<AppUser> userManager,
                                IAuthorizationService authorizationService) 
                                : base(serviceManager, memoryCache, userManager , authorizationService)
        {
        }


        [NonAction]
        async Task<Category> GetByIdCategoy(string id)
        {

            Category category;

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyCategory, out category))
            {

                category = await _serviceManager
                    .CategoryService
                    .GetByIdAsync(id, ExpLinqEntity<Category>.ResLinqEntity(null, null, true));
                category.PostCategories = (await _serviceManager
                                        .PostCategoryService
                                        .GetByIdCategoryAsync(id,
                                        ExpLinqEntity<PostCategory>.ResLinqEntity(ExpExpressions.ExtendInclude<PostCategory>(x => x.Include(x => x.Post)), null, true))).ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyCategory, category, cacheEntryOptions);
            }

            category = _cache.Get(_KeyCategory) as Category;

            if (category.Id != id)
            {
                category = await _serviceManager
                    .CategoryService
                    .GetByIdAsync(id, ExpLinqEntity<Category>.ResLinqEntity(null, null, true));
                category.PostCategories = (await _serviceManager
                                        .PostCategoryService
                                        .GetByIdCategoryAsync(id,
                                        ExpLinqEntity<PostCategory>.ResLinqEntity(ExpExpressions.ExtendInclude<PostCategory>(x => x.Include(x => x.Post)), null, true))).ToList();


                // Thiết lập cache - lưu vào cache             
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyCategory, category, cacheEntryOptions);
            }

            return category;
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
                    .GetAllAsync(ExpLinqEntity<Category>.ResLinqEntity(null, x => x.OrderBy(x => x.Serial), true));

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(500));
                _cache.Set(_KeyListCategorys, categories, cacheEntryOptions);
            }

            else
            {
                categories = _cache.Get(_KeyListCategorys) as IEnumerable<Category>;
            }

            return categories;
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var cateforys = await GetAllTreeViewCategories();

            cateforys = TreeViews.GetCategoryChierarchicalTree(cateforys);
            
            ViewData["categorys"] = cateforys;
            var des = new List<Category>();

            TreeViews.CreateTreeViewCategorySeleteItems(cateforys, des, 0);

            ViewData["TreeViewCategorys"] = des;
            return View(new CategoryForCreationDto());
        }

       
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CategoryForCreationDto categoryForCreationDto)
        {
            
            if (!ModelState.IsValid)
            {
                var cateforys = await GetAllTreeViewCategories();

                var treeViewcategories = TreeViews.GetCategoryChierarchicalTree(cateforys);

                ViewData["categorys"] = treeViewcategories;

                var des = new List<Category>();

                TreeViews.CreateTreeViewCategorySeleteItems(treeViewcategories, des, 0);

                ViewData["TreeViewCategorys"] = des;

                return View("index", categoryForCreationDto);
            }

            var category = await _serviceManager.CategoryService.CreateAsync(categoryForCreationDto);
            
            StatusMessage = $"Thêm thành công danh mục #{category.Title}#";

            _cache.Remove(_KeyListCategorys);

            return RedirectToAction("index");
            
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var categories = await GetAllTreeViewCategories();

            var treeViewcategories = TreeViews.GetCategoryChierarchicalTree(categories);
            
            ViewData["categorys"] = treeViewcategories;

            var des = new List<Category>();

            TreeViews.CreateTreeViewCategorySeleteItems(treeViewcategories, des, 0);

            ViewData["TreeViewCategorys"] = des;

            var category = categories.Where(c => c.Id == id).FirstOrDefault();

            return View(category);

        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute]string id, [FromForm]CategoryForUpdateDto categoryForUpdate)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var categorys = await GetAllTreeViewCategories();

                var treeViewcategories = TreeViews.GetCategoryChierarchicalTree(categorys);

                ViewData["categorys"] = treeViewcategories;

               
                var des = new List<Category>();

                TreeViews.CreateTreeViewCategorySeleteItems(treeViewcategories, des, 0);

                ViewData["TreeViewCategorys"] = des;

                var cgr = categorys.Where(c => c.Id == id).FirstOrDefault();

                return View("Edit", cgr);
            }

            var category = await _serviceManager.CategoryService.UpdateAsync(id, categoryForUpdate);

            StatusMessage = $"Cập nhật thành công danh mục #{category.Title}#";

            _cache.Remove(_KeyListCategorys);

            return RedirectToAction("Edit");

        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id) 
        {
            var categories = await GetAllTreeViewCategories();

            var treeViewcategories = TreeViews.GetCategoryChierarchicalTree(categories);

            ViewData["categorys"] = treeViewcategories;

            var category = categories.Where(c => c.Id == id).FirstOrDefault();    

            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] string id , bool IsDelte)
        {
           var category = await _serviceManager.CategoryService.DeleteAsync(id);            
            StatusMessage = $"Xóa thành công danh mục #{category.Title}# ";
            _cache.Remove(_KeyListCategorys);
            return RedirectToAction("index");

        }


        async Task<List<Post>> RunView(string id)
        {
            var category = await GetByIdCategoy(id);

            var listPostWithDetails = category.PostCategories.Select(c => c.Post).OrderBy(x => x.Serial).ToList();

            var outPut = listPostWithDetails;

            listPostWithDetails = TreeViews.GetPostChierarchicalTree(listPostWithDetails);

            var categoryBrpost = new CategoryBranchPosts(ObjectMapper.Mapper.Map<CategoryDto>(category), ObjectMapper.Mapper.Map<List<PostFWDetailChilds>>(listPostWithDetails));

            ViewData["category"] = categoryBrpost;

            var des = new List<PostSelectDto>();

            TreeViews.CreateTreeViewPostSeleteItems(listPostWithDetails, des, 0);

            ViewData["SeletePosts"] = des;

            return outPut;
        }

        [HttpGet]
        public async Task<IActionResult> Posts([FromRoute] string id)
        {
            await RunView(id);
            return View(new PostForCreationDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] PostForCreationDto postForCreationDto, [FromRoute] string id)
        {

            if (!ModelState.IsValid)
            {
                await RunView(id);
                return View("Posts", postForCreationDto);
            }

            var post = await _serviceManager.PostService.CreateAsync(postForCreationDto, id);

            StatusMessage = $"Thêm thành bài viết #{post.Title}#";

            _cache.Remove(_KeyCategory);

            return RedirectToAction("Posts", new { id = id });
        }


        [HttpGet]
        public async Task<IActionResult> EditPost([FromRoute] string id, [FromQuery] string postid)
        {

            var listPostbra =  await RunView(id);

            var postEdit = listPostbra.Where(p => p.Id == postid).FirstOrDefault();

            if (postEdit is null)
            {
                return RedirectToAction("Posts", new {id= id});
            }

            var postDto = ObjectMapper.Mapper.Map<PostDto>(postEdit);

            return View(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(
            [FromForm] PostForUpdateDto postForUpdateDto,
            [FromRoute] string id,
            [FromQuery] string postid)
        {
            if (!ModelState.IsValid)
            {
                var listPostbra = await RunView(id);

                var postEdit = listPostbra.Where(p => p.Id == postid).FirstOrDefault();

                if (postEdit is null)
                {
                    return RedirectToAction("Posts", new { id = id });
                }

                var postDto = ObjectMapper.Mapper.Map<PostDto>(postEdit);

                return View(postDto);
            }

            var result = await _serviceManager.PostService.UpdateAsync(postid, postForUpdateDto);

            StatusMessage = $"Cập nhật thành bài viết #{result.Title}#";
           
            _cache.Remove(_KeyCategory);

            return RedirectToAction("EditPost", new { id = id , postid = postid});
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost([FromRoute]string id ,[FromQuery] string postid)
        {
            var listPostbra = await RunView(id);

            var postEdit = listPostbra.Where(p => p.Id == postid).FirstOrDefault();

            if (postEdit is null)
            {
                return RedirectToAction("Posts", new { id = id });
            }

            var postDto = ObjectMapper.Mapper.Map<PostDto>(postEdit);

            return View(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost([FromRoute] string id , [FromQuery] string postid, bool íDelete)
        {
            var post = await _serviceManager.PostService.DeleteAsync(postid);

            StatusMessage = $"Xóa thành công bài viết #{post.Title}# ";

            _cache.Remove(_KeyCategory);
            return RedirectToAction("posts", new {id = id});

        }

        //[HttpPost]
        //public async Task<JsonResult> Create([FromBody] CategoryForCreationDto categoryForCreationDto)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        string messages = string.Join("; ", ModelState.Values
        //                                .SelectMany(x => x.Errors)
        //                                .Select(x => x.ErrorMessage));

        //        //Response.StatusCode = (int)HttpStatusCode.BadRequest;

        //        return Json(messages);

        //    }
        //    var category = await _serviceManager.CategoryService.CreateAsync(categoryForCreationDto);

        //    //Response.StatusCode = (int)HttpStatusCode.OK;

        //    return Json("ok");
        //}

        //public async Task<ActionResult> SetCategory()
        //{
        //    try
        //    {
        //        var cateforys = await _serviceManager.CategoryService.GetAllChildAsync();
        //        return PartialView("_GetAllCategoryPartial", cateforys);
        //    }
        //    catch (Exception ex)
        //    {
        //        return View("Error");
        //    }
        //}



    }
}
