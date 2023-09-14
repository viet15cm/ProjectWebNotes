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
using ModelValidation;
using ProjectWebNotes.Areas.Manager.Models;
using ProjectWebNotes.FileManager;
using Services.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebNotes.Areas.Manager.Controllers
{
    [Authorize(Policy = "Administrator")]
    public class CategoryController : BaseMangerController
    {

       
        private readonly IFileServices _fileServices;

        public CategoryController(IServiceManager serviceManager, 
            IMemoryCache memoryCache, 
            UserManager<AppUser> userManager, 
            IAuthorizationService authorizationService,
            ILogger<BaseDocController> logger,
            IFileServices fileServices
           )
             : base(serviceManager, memoryCache, userManager, authorizationService, logger)
            {
                _fileServices = fileServices;
            }

        [NonAction]
        async Task<Category> GetByIdCategoy(string id)
        {

            return await _serviceManager
                     .CategoryService
                     .GetByIdAsync(id, ExtendedQuery<Category>.Set(ExtendedInclue.Set<Category>(x => x.Include(x => x.Posts)) ,null, true));
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
        public async Task<IActionResult> Edit([FromRoute] string id, [FromForm] CategoryForUpdateDto categoryForUpdate)
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

        public class IFromFileViewModel
        {
            public Category Category { get; set; }

            [BindProperty]
            [FileImgValidations(new string[] { ".jpg", ".jpeg", ".png", ".jfif" })]
            [Display(Name = "Icon")]
            public IFormFile FormFile { get; set; }
        }



        [HttpGet]
        public async Task<IActionResult> Icon(string id)
        {
            var categories = await GetAllTreeViewCategories();

            var treeViewcategories = TreeViews.GetCategoryChierarchicalTree(categories);

            ViewData["categorys"] = treeViewcategories;

            var category = categories.Where(c => c.Id == id).FirstOrDefault();

            if (category is null)
            {
                return NotFound();
            }
            var fromFileViewModel = new IFromFileViewModel();

            fromFileViewModel.Category = category;

            return View(fromFileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Icon([FromForm] IFromFileViewModel fromFileViewModel, [FromRoute] string id)
        {

            if (fromFileViewModel is null)
            {
                return NotFound();
            }

            var category = ObjectMapper.Mapper.Map<CategoryForUpdateIconDto>(fromFileViewModel.Category);

            if (category is null)
            {
                return NotFound(id);
            }

            var FormFile = fromFileViewModel.FormFile;

            if (FormFile == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                string olUrl = category.IConFont;

                string Url = FileServices.GetUniqueFileName(FormFile.FileName);

                var resultFile = await _fileServices.CreateFileAsync(IconCategory.GetIcon(), FormFile, Url);


                if (resultFile)
                {
                    category.IConFont = Url;

                    var categoryDto = await _serviceManager.CategoryService.UpdateAsync(id, category);

                    StatusMessage = $"Biểu tượng danh mục {categoryDto.Title} của bạn đã cập nhật";

                    if (olUrl != null)
                    {
                        await _fileServices.DeleteFileAsync(IconCategory.GetIcon(), olUrl);
                    }


                    return RedirectToAction("Icon", new { id = id });
                }

                await _fileServices.DeleteFileAsync(IconCategory.GetIcon(), Url);

            }

            var categories = await GetAllTreeViewCategories();

            var treeViewcategories = TreeViews.GetCategoryChierarchicalTree(categories);

            ViewData["categorys"] = treeViewcategories;
            var categori = categories.Where(c => c.Id == id).FirstOrDefault();

            if (categori is null)
            {
                return NotFound();
            }

            fromFileViewModel.Category = categori;

            return View(fromFileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] string id , bool IsDelte)
        {
           var category = await _serviceManager.CategoryService.DeleteAsync(id);            
            StatusMessage = $"Xóa thành công danh mục #{category.Title}# ";

            return RedirectToAction("index");

        }


        async Task<Category> RunView(string id)
        {
            var category = await GetByIdCategoy(id);

            category.Posts = TreeViews.GetPostChierarchicalTree(category.Posts);

       
            ViewData["category"] = category;

            var des = new List<PostSelectDto>();

            TreeViews.CreateTreeViewPostSeleteItems(category.Posts.ToList(), des, 0);

            ViewData["SeletePosts"] = des;

            return category;
           
        }

        [HttpGet]
        public async Task<IActionResult> Posts([FromRoute] string id)
        {
            await RunView(id);
            return View(new PostForCreationDto() { CategoryId = id});
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] PostForCreationDto postForCreationDto, [FromRoute] string id)
        {

            if (!ModelState.IsValid)
            {
                await RunView(id);
                return View("Posts", postForCreationDto);
            }

            if (postForCreationDto.DateCreate is null)
            {
                postForCreationDto.DateCreate = _serviceManager.HttpClient.GetNistTime();
            }

            var user = await _userManager.GetUserAsync(User);

            postForCreationDto.AuthorId = user.Id;

            var post = await _serviceManager.PostService.CreateAsync(postForCreationDto);

            StatusMessage = $"Thêm thành bài viết #{post.Title}#";

            return RedirectToAction("Posts", new { id = post.CategoryId });
        }


        [HttpGet]
        public async Task<IActionResult> EditPost([FromRoute] string id)
        {

            var postEdit = await _serviceManager.PostService.GetByIdAsync(id);

            if (postEdit is null)
            {
                return NotFound($"Không tìm thấy id {postEdit.Id}");
            }

            var category = await RunView(postEdit.CategoryId);

            if (category is null)
            {
                return NotFound($"Không tìm thấy id {postEdit.CategoryId}");
            }

            var postDto = ObjectMapper.Mapper.Map<PostDto>(postEdit);

            return View(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(
            [FromForm] PostForUpdateDto postForUpdateDto,
            [FromRoute] string id
           )
        {
            
            if (!ModelState.IsValid)
            {
               
                var postEdit = await _serviceManager.PostService.GetByIdAsync(id);

                if (postEdit is null)
                {
                    return NotFound($"Không tìm thấy id {postEdit.Id}");
                }

                var category = await RunView(postEdit.CategoryId);

                if (category is null)
                {
                    return NotFound($"Không tìm thấy id {category.Id}");
                }

                var postDto = ObjectMapper.Mapper.Map<PostDto>(postEdit);

                return View(postDto);
            }

            var result = await _serviceManager.PostService.UpdateAsync(id, postForUpdateDto);

            StatusMessage = $"Cập nhật thành bài viết #{result.Title}#";

            return RedirectToAction("EditPost", new { id = result.Id });
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost([FromRoute]string id)
        {
            var postEdit = await _serviceManager.PostService.GetByIdAsync(id);

            if (postEdit is null)
            {
                return NotFound($"Không tìm thấy id {postEdit.Id}");
            }

            var category = await RunView(postEdit.CategoryId);

            if (category is null)
            {
                return NotFound($"Không tìm thấy id {category.Id}");
            }

            var postDto = ObjectMapper.Mapper.Map<PostDto>(postEdit);

            return View(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost([FromRoute] string id , bool isDelete)
        {
            var post = await _serviceManager.PostService.DeleteAsync(id);

            StatusMessage = $"Xóa thành công bài viết #{post.Title}# ";

            return RedirectToAction("posts", new {id = post.CategoryId});

        }

        [HttpGet]
        public async Task<IActionResult> Contents([FromRoute] string id)
        {
           var category = await _serviceManager.CategoryService.GetByIdAsync(id);

            if (category is null)
            {
                return NotFound();
            }

            var contentDto = ObjectMapper.Mapper.Map<CategoryDto>(category);

            return View(contentDto);
        }

        [HttpPost]
        public async Task<IActionResult> Contents([FromForm] CategoryForUpdateContentDto categoryForUpdateContentDto, [FromRoute] string id)
        {
            if (categoryForUpdateContentDto is null)
            {
                return NotFound();
            }
            var cateoryUpdate = await _serviceManager.CategoryService.UpdateAsync(id, categoryForUpdateContentDto);

            StatusMessage = $"Cập nhật thành công nội dung --{cateoryUpdate.Title}--";

            return RedirectToAction("Contents", new { id });

            
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
