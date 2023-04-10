using Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProjectWebNotes.Areas.Manager.Models;
using Services.Abstractions;


namespace ProjectWebNotes.Areas.Manager.Controllers
{
    public class CategoryController : BaseController 
    {
        private const string _KeyListCatgorys = "_listallcategories";
        private const string _KeyCategory = "_category";

        public CategoryController(IServiceManager serviceManager, 
                                IMemoryCache memoryCache, 
                                IHttpClientServiceImplementation httpClient) 
                                : base(serviceManager, memoryCache, httpClient)
        {
        }

       
        //Hàm phụ
        [NonAction]
        private void CreateTreeViewCategorySeleteItems(IEnumerable<CategoryForWithDetailDto> categoryTreeLayerDtos 
                                              ,List<CategoryForWithDetailDto> des,
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

                    CreateTreeViewCategorySeleteItems(category.CategoryChildren, des, leve + 1);
                    
                }
            }

        }

        //Hàm phụ
        [NonAction]
        private void CreateTreeViewPostSeleteItems(List<PostForWithDetailDto> postTreeLayerDtos
                                              , List<PostForWithDetailDto> des
                                               ,List<PostForWithDetailDto> postTreeView,
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

                postTreeView.Add(post);

                if (post.PostChilds?.Count > 0)
                {

                    CreateTreeViewPostSeleteItems(post.PostChilds.ToList(), des , postTreeView, leve + 1);

                }
            }

        }

        //[NonAction]
        //async Task<CategoryDto> GetCategoyDto(string id)
        //{

        //    CategoryDto category;

        //    // Phục hồi categories từ Memory cache, không có thì truy vấn Db
        //    if (!_cache.TryGetValue(_KeyCategory, out category))
        //    {
        //        category = await _serviceManager.CategoryService.GetByIdAsync(id);

        //        // Thiết lập cache - lưu vào cache             
        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            .SetSlidingExpiration(TimeSpan.FromMinutes(300));
        //        _cache.Set(_KeyCategory, category, cacheEntryOptions);
        //    }

        //     category = _cache.Get(_KeyCategory) as CategoryDto;

        //    if (category.Id != id) 
        //    {
        //        category = await _serviceManager.CategoryService.GetByIdAsync(id);
        //        // Thiết lập cache - lưu vào cache             
        //        var cacheEntryOptions = new MemoryCacheEntryOptions()
        //            .SetSlidingExpiration(TimeSpan.FromMinutes(300));
        //        _cache.Set(_KeyCategory, category, cacheEntryOptions);
        //    }

        //    return category;
        //}


        [NonAction]
        async Task <IEnumerable<CategoryForWithDetailDto>> GetAllTreeViewCategories()
        {

            IEnumerable<CategoryForWithDetailDto> categories;


            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyListCatgorys, out categories))
            {
                 categories = await _serviceManager.CategoryService.GetAllWithDetailAsync();

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyListCatgorys, categories, cacheEntryOptions);
            }
            
            categories = _cache.Get(_KeyListCatgorys) as IEnumerable<CategoryForWithDetailDto>;
            
            return categories;
        }


      
        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var cateforys = await GetAllTreeViewCategories();

            cateforys = TreeViews.GetCategoryChierarchicalTree(cateforys);
            
            ViewData["categorys"] = cateforys;
            var des = new List<CategoryForWithDetailDto>();

            CreateTreeViewCategorySeleteItems(cateforys, des, 0);

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

                var des = new List<CategoryForWithDetailDto>();

                CreateTreeViewCategorySeleteItems(treeViewcategories, des, 0);

                ViewData["TreeViewCategorys"] = des;

                return View("index", categoryForCreationDto);
            }

            var category = await _serviceManager.CategoryService.CreateAsync(categoryForCreationDto);
            
            StatusMessage = $"Thêm thành công danh mục #{category.Title}#";

            _cache.Remove(_KeyListCatgorys);

            return RedirectToAction("index");
            
        }

        public void FindTreeViewCategory(List<CategoryForWithDetailDto> categorys , string id , CategoryForWithDetailDto categoryskip)
        {

            foreach (var item in categorys)
            {
                if (item.Id == id)
                {
                    categoryskip = item;          
                }

                if (item.CategoryChildren?.Count > 0)
                {
                    FindTreeViewCategory(item.CategoryChildren.ToList(), id, categoryskip);
                }
            }
        }

        public void SkipTreeViewCategory(CategoryForWithDetailDto categoryskip)
        {

        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var categories = await GetAllTreeViewCategories();

            var treeViewcategories = TreeViews.GetCategoryChierarchicalTree(categories);
            
            ViewData["categorys"] = treeViewcategories;

            var des = new List<CategoryForWithDetailDto>();

            CreateTreeViewCategorySeleteItems(treeViewcategories, des, 0);

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

               
                var des = new List<CategoryForWithDetailDto>();

                CreateTreeViewCategorySeleteItems(treeViewcategories, des, 0);

                ViewData["TreeViewCategorys"] = des;

                var cgr = categorys.Where(c => c.Id == id).FirstOrDefault();

                return View("Edit", cgr);
            }

            var category = await _serviceManager.CategoryService.UpdateAsync(id, categoryForUpdate);

            StatusMessage = $"Cập nhật thành công danh mục #{category.Title}#";

            _cache.Remove(_KeyListCatgorys);

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
            _cache.Remove(_KeyListCatgorys);
            return RedirectToAction("index");

        }

        //public List<CategoryForWithDetailDto> listcagory()
        //{
        //    var post = new List<CategoryForWithDetailDto>();
        //    for (int i = 0; i <= 2000; i++)
        //    {

        //        var postdata = new CategoryForWithDetailDto()
        //        {
        //            Id = i.ToString(),
        //            Slug = "slug " + i.ToString(),
        //            Title = "Tieu de " + i.ToString(),
        //            ParentCategoryId = random(i)
        //        };

        //        if (i% 2 == 0)
        //        {
        //            postdata.ParentCategoryId = null; 
        //        }

        //        post.Add(postdata);
        //    }

        //    return post;
        //}

        //string random(int i)
        //{
        //    while (true)
        //    {
        //        Random rd = new Random();
        //        var data = rd.Next(0, 2000);

        //        if (data != i)
        //        {
        //            return data.ToString();
        //        }

        //    }        
        //}

        [HttpGet]
        public async Task<IActionResult> Posts([FromRoute] string id)
        {
            var category = await _serviceManager.CategoryService.GetByIdWithDetailAsync(id);

            category.PostCategories = (await _serviceManager.PostCategoryService.GetByIdCategoryWithDetailAsync(category.Id)).ToList();

            var listPostWithDetails = category.PostCategories.Select(c => c.Post).ToList();

            listPostWithDetails = TreeViews.GetPostChierarchicalTree(listPostWithDetails);

            ViewData["category"] = category;

            ViewData["treeViewPost"] = listPostWithDetails;

            var des = new List<PostForWithDetailDto>();

            var treeViewPost = new List<PostForWithDetailDto>();

            CreateTreeViewPostSeleteItems(listPostWithDetails, des, treeViewPost, 0);

            ViewData["SeletePosts"] = des;

            return View(new PostForCreationDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] PostForCreationDto postForCreationDto, [FromRoute] string id)
        {

            if (!ModelState.IsValid)
            {
                var category = await _serviceManager.CategoryService.GetByIdWithDetailAsync(id);

                ViewData["category"] = category;

                var listPostWithDetails = new List<PostForWithDetailDto>();

                foreach (var item in category.PostCategories)
                {

                    listPostWithDetails.Add(item.Post);
                }

                var des = new List<PostForWithDetailDto>();

                 var treeViewPost = new List<PostForWithDetailDto>();

                CreateTreeViewPostSeleteItems(listPostWithDetails, des, treeViewPost, 0);

                return View(postForCreationDto);
            }

            var post = await _serviceManager.PostService.CreateAsync(postForCreationDto, id);


            StatusMessage = $"Thêm thành bài viết #{post.Title}#";

            return RedirectToAction("Posts", new { id = id });
        }


        [HttpGet]
        public async Task<IActionResult> EditPost([FromRoute] string id, [FromQuery] string idpost)
        {
            var category = await _serviceManager.CategoryService.GetByIdWithDetailAsync(id);

            ViewData["category"] = category;

            var listPostWithDetails = new List<PostForWithDetailDto>();

            foreach (var item in category.PostCategories)
            {
                listPostWithDetails.Add(item.Post);
            }

            var des = new List<PostForWithDetailDto>();

            var treeViewPost = new List<PostForWithDetailDto>();
            
            CreateTreeViewPostSeleteItems(listPostWithDetails, des, treeViewPost, 0);

            ViewData["TreeViewPosts"] = des;

            var postEdit = treeViewPost.Where(p => p.Id == idpost).FirstOrDefault();

            ViewData["postid"] = postEdit.Id;

            return View(postEdit);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(
            [FromForm] PostForUpdateDto postForUpdateDto,
            [FromRoute] string id,
            [FromQuery] string idpost)
        {
            if (!ModelState.IsValid)
            {
                var category = await _serviceManager.CategoryService.GetByIdWithDetailAsync(id);

                ViewData["category"] = category;


                var listPostWithDetails = category.PostCategories.Select(pc => pc.Post).ToList();
             
                var des = new List<PostForWithDetailDto>();

                var treeViewPost = new List<PostForWithDetailDto>();
                CreateTreeViewPostSeleteItems(listPostWithDetails, des, treeViewPost , 0);

                ViewData["TreeViewPosts"] = des;

                ViewData["postid"] = idpost;

                return View(postForUpdateDto);
            }

            var result = await _serviceManager.PostService.UpdateAsync(idpost, postForUpdateDto);

            StatusMessage = $"Cập nhật thành bài viết #{result.Title}#";

            return RedirectToAction("Posts", new { id = id });
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
