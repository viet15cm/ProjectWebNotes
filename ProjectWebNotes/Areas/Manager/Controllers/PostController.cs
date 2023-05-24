
using ATMapper;
using Domain.IdentityModel;
using Dto;
using Entities.Models;
using ExtentionLinqEntitys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Paging;
using ProjectWebNotes.Areas.Manager.Models;
using ProjectWebNotes.FileManager;
using ProjectWebNotes.Security.Requirements;
using Services.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebNotes.Areas.Manager.Controllers
{

    [Authorize(Policy = "Employee")]
    public class PostController : BaseController
    {
        private const string _KeyList = "_listallPosts";
        private const string _KeyObj = "_post";

        private const string _KeyListCatgorys = "_listallPosts";

        private readonly IFileServices _fileServices;
        public PostController(IServiceManager serviceManager, 
                                IMemoryCache memoryCache,
                                UserManager<AppUser> userManager,
                                 IAuthorizationService authorizationService,
                                IFileServices fileServices
                               
                                )
            : base(serviceManager, memoryCache, userManager , authorizationService)
        {
            _fileServices = fileServices;
        }

        [NonAction]
        async Task<IEnumerable<Category>> GetAllTreeViewCategories()
        {

            IEnumerable<Category> categories;


            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyListCatgorys, out categories))
            {
                categories = await _serviceManager.
                    CategoryService
                    .GetAllAsync(ExpLinqEntity<Category>.ResLinqEntity(null, null, true));

                // Thiết lập cache - lưu vào cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(300));
                _cache.Set(_KeyListCatgorys, categories, cacheEntryOptions);
            }

            categories = _cache.Get(_KeyListCatgorys) as IEnumerable<Category>;

            return categories;
        }


        [NonAction]
        async Task<IEnumerable<Post>> GetAllPostTreeViews()
        {

            IEnumerable<Post> posts;

            // Phục hồi categories từ Memory cache, không có thì truy vấn Db
            if (!_cache.TryGetValue(_KeyList, out posts))
            {

                posts = await _serviceManager.PostService.GetAllAsync();

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


        public BidingPostCategory bidingPostCategory;

        [HttpGet]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> AddCategory([FromRoute] string id)
        {
            var posts = await GetAllPostTreeViews();

            ViewData["Posts"] = posts;

            var cateforys = await GetAllTreeViewCategories();

            var des = new List<Category>();

            TreeViews.CreateTreeViewCategorySeleteItems(cateforys, des, 0);

            var post = await _serviceManager.PostService.GetByIdAsync(id);

            bidingPostCategory = new BidingPostCategory();

            bidingPostCategory.PostCategorys = post.PostCategories.Select(c => c.CategoryID).ToArray();
            
            bidingPostCategory.addCategory = new SelectList(des, "Id", "Title");

            return View(bidingPostCategory);
        }

        [Authorize(Policy = "Admin")]

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromForm] BidingPostCategory bidingPostCategory, [FromRoute] string id)
        {
            var post = await _serviceManager.PostService.GetByIdAsync(id);

            if (bidingPostCategory.PostCategorys == null)
            {
                bidingPostCategory.PostCategorys = new string[] {};
            }

            var oldPostCategorys = (await _serviceManager
                .PostService
                .GetByIdAsync(id))
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
              .GetByIdAsync(id, ExpLinqEntity<Post>
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

            if (post.Images?.Count > 0)
            {
                foreach (var item in post.Images)
                {
                    item.Url = _fileServices.HttpContextAccessorPathImgSrcIndex(ImagePost.GetImagePost(), item.Url);
                }
            }
            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Contents([FromRoute] string id)
        {
            var post = await RunPostDetail(id);

            var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDImagesDto>(post);

            if (postFWDImgaesDto.Images?.Count >0)
            {
                foreach (var item in postFWDImgaesDto.Images)
                {
                    item.Url = _fileServices.HttpContextAccessorPathImgSrcIndex(ImagePost.GetImagePost(), item.Url);
                }
            }


            return View(postFWDImgaesDto);
        }


        [HttpPost]
        public async Task<IActionResult> Contents([FromForm] PostForUpdateContentDto postForUpdateContentDto,
                                                    [FromRoute]string id)

        {
            if (!ModelState.IsValid)
            {
                var post = await RunPostDetail(id);

                var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDImagesDto>(post);


                if (postFWDImgaesDto.Images?.Count > 0)
                {
                    foreach (var item in postFWDImgaesDto.Images)
                    {
                        item.Url = _fileServices.HttpContextAccessorPathImgSrcIndex(ImagePost.GetImagePost(), item.Url);
                    }
                }

                return View(postFWDImgaesDto);
            }

            var rs = await _authorizationService.AuthorizeAsync(User, id,
                                                         new CanOptionPostUserRequirements(true));

            if (!rs.Succeeded)
            {
                return Forbid();
            }

            if (postForUpdateContentDto.DateUpdated is null)
            {
                postForUpdateContentDto.DateUpdated = _serviceManager.HttpClient.GetNistTime();
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

                return View("index", postForCreationDto);
            }

            if (postForCreationDto.DateCreate is null)
            {
                postForCreationDto.DateCreate = _serviceManager.HttpClient.GetNistTime();
            }

            var user = await _userManager.GetUserAsync(User);

            postForCreationDto.AuthorId = user.Id;

            var post = await _serviceManager.PostService.CreateAsync(postForCreationDto);

            StatusMessage = $"Thêm thành công bài viết #{post.Title}#";

            return RedirectToAction("index");
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

            var post = await _serviceManager.PostService.GetByIdAsync(id);


            var postDto = ObjectMapper.Mapper.Map<PostDto>(post);

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

                var itemPost = await _serviceManager.PostService.GetByIdAsync(id);

                var postDto = ObjectMapper.Mapper.Map<PostDto>(itemPost);


                ViewData["Posts"] = posts;

                return View(postDto);

            }

            var rs = await _authorizationService.AuthorizeAsync(User, id,
                                                          new CanOptionPostUserRequirements(true));

            if (!rs.Succeeded)
            {
                return Forbid();
            }

            var post = await _serviceManager.PostService.UpdateAsync(id, postForUpdateDto);
            StatusMessage = $"Cập nhật thành bài viết #{post.Title}#";
            return RedirectToAction("editpost", new { pageNumber = postParameters.PageNumber});
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost([FromRoute] string id , [FromQuery] PostParameters postParameters)
        {
            var posts = _serviceManager.PostService.Posts(postParameters);

            ViewData["Posts"] = posts;

            var post = await _serviceManager.PostService.GetByIdAsync(id);

            var postDto = ObjectMapper.Mapper.Map<PostDto>(post);

            return View(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost([FromRoute] string id, bool IsDelte)
        {
            var rs = await _authorizationService.AuthorizeAsync(User, id,
                                                          new CanOptionPostUserRequirements(true));

            if (!rs.Succeeded)
            {
                return Forbid();
            }

            var post = await _serviceManager.PostService.DeleteAsync(id);
           
            StatusMessage = $"Xóa thành công bài viết #{post.Title}# ";
            
            return RedirectToAction("index");

        }

        [HttpGet]
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "Admin")]
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


        [HttpGet]
        public async Task<IActionResult> CreateContent([FromRoute] string id)
        {
            var post = await RunPostDetail(id);


            var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDContentImagesDto>(post);

            if (postFWDImgaesDto.Images?.Count > 0)
            {
                foreach (var item in postFWDImgaesDto.Images)
                {
                    item.Url = _fileServices.HttpContextAccessorPathImgSrcIndex( ImagePost.GetImagePost(), item.Url);
                }
            }

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
            if (!ModelState.IsValid)
            {
                var post = await RunPostDetail(id);

                var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDContentImagesDto>(post);

                if (postFWDImgaesDto.Images?.Count > 0)
                {
                    foreach (var item in postFWDImgaesDto.Images)
                    {
                        item.Url = _fileServices.HttpContextAccessorPathImgSrcIndex(ImagePost.GetImagePost(), item.Url);
                    }
                }


                var des = new List<ContentSelectDto>();

                TreeViews.CreateTreeViewContentSeleteItems(postFWDImgaesDto.Contents.Where(x => x.ParentContentId == null), des, 0);

                ViewData["postFWDImgaesDto"] = postFWDImgaesDto;

                ViewData["TreeViewContentSelete"] = des;

                return View(contentForCreateDto);
            }

            var rs = await _authorizationService.AuthorizeAsync(User, id,
                                                          new CanOptionPostUserRequirements(true));

            if (!rs.Succeeded)
            {
                return Forbid();
            }

            if (contentForCreateDto?.PostId is null)
            {
                contentForCreateDto.PostId = id;
            }

            var contentDto = await _serviceManager.ContentService.CreateAsync(contentForCreateDto);

            StatusMessage = $"Thêm thành công nội dung ---{contentDto.Title}---";

            return RedirectToAction("detail", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> EditContent([FromRoute]string id , [FromQuery]int contentid)
        {
            var content = await _serviceManager
                .ContentService
                .GetByIdAsync(contentid
                , ExpLinqEntity<Content>
                .ResLinqEntity(ExpExpressions
                .ExtendInclude<Content>(x => x.Include(x => x.Post)
                .ThenInclude(x => x.Contents)
                .Include(x => x.Post)
                .ThenInclude(x => x.Images))));

            var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDContentImagesDto>(content.Post);

            var des = new List<ContentSelectDto>();

            TreeViews.CreateTreeViewContentSeleteItems(postFWDImgaesDto.Contents.Where(x => x.ParentContentId == null), des, 0);

            if (content.Post.Images?.Count > 0)
            {
                foreach (var item in content.Post.Images)
                {
                    item.Url = _fileServices.HttpContextAccessorPathImgSrcIndex(ImagePost.GetImagePost(), item.Url);
                }
            }

            ViewData["TreeViewContentSelete"] = des;

            return View(content);
        }

        [HttpPost]
        public async Task<IActionResult> EditContent([FromForm] ContentForUpdateDto contentForUpdateDto, [FromRoute] string id , [FromQuery] int contentid)
        {
            if (!ModelState.IsValid)
            {
                var content = await _serviceManager
                .ContentService
                .GetByIdAsync(contentid
                , ExpLinqEntity<Content>
                .ResLinqEntity(ExpExpressions
                .ExtendInclude<Content>(x => x.Include(x => x.Post)
                .ThenInclude(x => x.Contents))));

                var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDContentImagesDto>(content.Post);

                var des = new List<ContentSelectDto>();

                TreeViews.CreateTreeViewContentSeleteItems(postFWDImgaesDto.Contents.Where(x => x.ParentContentId == null), des, 0);

                ViewData["TreeViewContentSelete"] = des;

                return View(contentForUpdateDto);
            }

            var rs = await _authorizationService.AuthorizeAsync(User, id,
                                                          new CanOptionPostUserRequirements(true));

            if (!rs.Succeeded)
            {
                return Forbid();
            }

            var contentDto = await _serviceManager.ContentService.UpdateAsync(contentid, contentForUpdateDto);

            StatusMessage = $"Chỉnh sửa thành công nội dung ---{contentDto.Title}---";

            return RedirectToAction("detail", new { id = id });


        }

        [HttpPost]
        public async Task<IActionResult> DeleteContent([FromRoute]string id , [FromQuery]int contentid)        
        {

            var rs = await _authorizationService.AuthorizeAsync(User, id,
                                                          new CanOptionPostUserRequirements(true));

            if (!rs.Succeeded)
            {
                return Forbid();
            }

            var contentDto = await _serviceManager.ContentService.DeleteAsync(contentid);

            StatusMessage = $"Xóa thành công nội dung ---{contentDto.Title}---";

            return RedirectToAction("detail", new { id = id });
        }

        [HttpGet]
        public async Task<IActionResult> Images([FromRoute] string id)
        {
            var post = await RunPostDetail(id);

            var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDImagesDto>(post);

            ViewData["postFWDImgaesDto"] = postFWDImgaesDto;

            var image = new ImageView();
            
            image.SelectImages = post.Images.Select(x => x.Url).ToList();

            image.AvailableImages = post.Images.Select(x => new ImageSelectList() { Text = x.Url, Value = x.Url }).ToList();
            
            return View(image);

        }

        [HttpPost]
        public async Task<IActionResult> EditImages(ImageView image , [FromRoute]string id)
        {
            if (image == null)
            {
                return NotFound();
            }

            var post =  await RunPostDetail(id);

            var rs = await _authorizationService.AuthorizeAsync(User, id,
                                                          new CanOptionPostUserRequirements(true));

            if (!rs.Succeeded)
            {
                return Forbid();
            }

            var messgae = 0;
            var deleteImge = 0;

            if (ModelState.IsValid)
            {
                if (image.FormFiles != null)
                {
                    foreach (var fronfile in image.FormFiles)
                    {
                        var data = new ImageForCreateDto()
                        {
                            Url = FileServices.GetUniqueFileName(fronfile.FileName),

                            PostId = post.Id
                        };

                        var resultFile = await _fileServices.CreateFileAsync(ImagePost.GetImagePost(), fronfile, data.Url);

                        if (resultFile)
                        {
                            var resultImage = await _serviceManager.ImageService.CreateAsync(data);
                            messgae += 1;
                        }

                    }

                }

                if (image.SelectImages.Count > 0)
                {
                    foreach (var item in image.SelectImages)
                    {

                        var img = post.Images.FirstOrDefault(x => x.Url.Equals(item));

                        var deletedataImgeResult = await _serviceManager.ImageService.DeleteAsync(img.Id);

                        var resultFileimge = await _fileServices.DeleteFileAsync( ImagePost.GetImagePost(), item);
       
                         deleteImge += 1;                        
                    }
                }
                StatusMessage = $"Thêm thành công {messgae} ảnh , đả xóa {deleteImge} ảnh ";

                return RedirectToAction("Detail", new { id = post.Id });
            }


            var images =  post.Images;

            image.SelectImages = post.Images.Select(x => x.Url).ToList();

            image.AvailableImages = post.Images.Select(x => new ImageSelectList() { Text = x.Url, Value = x.Url }).ToList();


            return View("Images", image);
        }

    }
}
