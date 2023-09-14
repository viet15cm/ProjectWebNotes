
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
using Microsoft.Extensions.Hosting;
using ModelValidation;
using Paging;
using ProjectWebNotes.Areas.Manager.Models;
using ProjectWebNotes.FileManager;
using ProjectWebNotes.Security.Requirements;
using Services.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace ProjectWebNotes.Areas.Manager.Controllers
{

    [Authorize(Policy = "Admin")]
    public class PostController : BaseMangerController
    {

        private readonly IFileServices _fileServices;

        public PostController(IServiceManager serviceManager,
            IMemoryCache memoryCache,
            UserManager<AppUser> userManager,
            IAuthorizationService authorizationService, 
            ILogger<BaseDocController> logger,
            IFileServices fileServices) : base(serviceManager, memoryCache, userManager, authorizationService, logger)
        {
            _fileServices = fileServices;
        }

        [HttpGet]
        public  PartialViewResult PostsPartial(int PageNumber)
        {
            var postParameters = new PostParameters() { PageNumber = PageNumber };

            var posts = _serviceManager.PostService.Posts(postParameters);

            ViewData["Posts"] = posts;

            return PartialView("_PostPagingPartial", posts);
        }


        public class IFromFileViewModel
        {
            public Post Post  { get; set; }

            [BindProperty]
            [FileImgValidations(new string[] { ".jpg", ".jpeg", ".png", ".jfif" })]
            [Display(Name = "Banner Post")]
            public IFormFile FormFile { get; set; }
        }


        [HttpGet]
        public  async Task<IActionResult> Banner(string id)
        {

            var post = await _serviceManager.PostService.GetByIdAsync(id);

            var rs = await _authorizationService.AuthorizeAsync(User, post,
                                                           new CanOptionPostUserRequirements());

            if (!rs.Succeeded)
            {
                return Forbid();

            }

            var fromFileViewModel = new IFromFileViewModel();

            fromFileViewModel.Post = post;

            return View(fromFileViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Banner([FromForm] IFromFileViewModel fromFileViewModel, [FromRoute] string id)
        {

            if (fromFileViewModel is null)
            {
                return NotFound();
            }


            var postForUpdate = ObjectMapper.Mapper.Map<PostForUpdateBannerDto>(fromFileViewModel.Post);

            if (postForUpdate is null)
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
                string olUrl = postForUpdate.Banner;

                string Url = FileServices.GetUniqueFileName(FormFile.FileName);

                var resultFile = await _fileServices.CreateFileAsync(BannerPost.GetBannerPost(), FormFile, Url);


                if (resultFile)
                {
                    postForUpdate.Banner = Url;

                    var postDto = await _serviceManager.PostService.UpdateAsync(id, postForUpdate);

                    StatusMessage = $"Banner post {postDto.Title} của bạn đã cập nhật";

                    if (olUrl != null)
                    {
                        await _fileServices.DeleteFileAsync(BannerPost.GetBannerPost(), olUrl);
                    }

                    return RedirectToAction("Banner", new { id = id });
                }

                await _fileServices.DeleteFileAsync(BannerPost.GetBannerPost(), Url);

            }

            var post = await _serviceManager.PostService.GetByIdAsync(id);

         
            fromFileViewModel.Post = post;

            return View(fromFileViewModel);

          
        }

        public class BidingPostCategory
        {
            public string IdPost { get; set; }

            [Display(Name ="Danh mục")]
            public string[] PostCategorys { get; set; }

            public SelectList addCategory { get; set; }
        }


        public BidingPostCategory bidingPostCategory;

      
        async Task<Post> RunPostDetail(string id)
        {
            var post = await _serviceManager
              .PostService
              .GetByIdAsync(id, ExtendedQuery<Post>
              .Set(ExtendedInclue
              .Set<Post>(x => x.Include(x => x.Images)
                                  .Include(x => x.PostChilds)
                                  .Include(x => x.PostParent)
                                  .Include(x => x.PostChilds)
                                  .Include(x => x.Category)
                                  )));
            return post;
              
        }

        [HttpGet]
        public async Task<IActionResult> Detail([FromRoute]string id)
        {
            var post = await RunPostDetail(id);

            if (post == null)
            {
                return NotFound();
            }

            if (post.Banner != null)
            {
                post.Banner = _fileServices.HttpContextAccessorPathImgSrcIndex(BannerPost.GetBannerPost(), post.Banner);
            }

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

            var rs = await _authorizationService.AuthorizeAsync(User, post,
                                                        new CanOptionPostUserRequirements());

            if (!rs.Succeeded)
            {
                return Forbid();
            }

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

            var rs = await _authorizationService.AuthorizeAsync(User, post,
                                                           new CanOptionPostUserRequirements());

            if (!rs.Succeeded)
            {
                return Forbid();

            }


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


            var post = await _serviceManager.PostService.UpdateAsync(id, postForUpdateDto);
            StatusMessage = $"Cập nhật thành bài viết #{post.Title}#";
           
            return RedirectToAction("editpost", new { pagenumber = postParameters.PageNumber});
        }

        [HttpGet]
        public async Task<IActionResult> DeletePost([FromRoute] string id , [FromQuery] PostParameters postParameters)
        {
            var posts = _serviceManager.PostService.Posts(postParameters);

            ViewData["Posts"] = posts;

            var post = await _serviceManager.PostService.GetByIdAsync(id);

            var rs = await _authorizationService.AuthorizeAsync(User, post,
                                                           new CanOptionPostUserRequirements());

            if (!rs.Succeeded) 
            {
                return Forbid();
            
            }

            var postDto = ObjectMapper.Mapper.Map<PostDto>(post);

            return View(postDto);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost([FromRoute] string id, bool IsDelte)
        {
           
            var post = await _serviceManager.PostService.DeleteAsync(id);
           
            StatusMessage = $"Xóa thành công bài viết #{post.Title}# ";
         
            return RedirectToAction("index");

        }

        public class ViewAddCategory : PostForUpdateIdCategoryDto
        {
            public string PostId;

            public List<Category> categories;

        }

        [HttpGet]
      
        public async Task<IActionResult> AddCategory([FromRoute] string id, [FromQuery] PostParameters postParameters)
        {

            var post = await _serviceManager.PostService.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            var rs = await _authorizationService.AuthorizeAsync(User, post,
                                                          new CanOptionPostUserRequirements());

            if (!rs.Succeeded)
            {
                return Forbid();

            }

            var cateforys = await GetAllTreeViewCategories();

            var treeViewcategories = TreeViews.GetCategoryChierarchicalTree(cateforys);

            ViewData["categorys"] = treeViewcategories;

            var des = new List<Category>();

            TreeViews.CreateTreeViewCategorySeleteItems(treeViewcategories, des, 0);

            return View(new ViewAddCategory() { PostId = id , categories = des , CategoryId =  post.CategoryId} );
           
        }

        [HttpPost]
        
        public async Task<IActionResult> AddCategory([FromRoute] string id, [FromForm] ViewAddCategory viewAddCategory)
        {

            var post = await _serviceManager.PostService.UpDateAsync(id, viewAddCategory);

            StatusMessage = $"Cập nhật thành công danh mục cho bài viết --{post.Title}--";


            return RedirectToAction("detail", new { id = post.Id });

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
                , ExtendedQuery<Content>
                .Set(ExtendedInclue
                .Set<Content>(x => x.Include(x => x.Post)
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
                ,ExtendedQuery<Content>
                .Set(ExtendedInclue
                .Set<Content>(x => x.Include(x => x.Post)
                .ThenInclude(x => x.Contents))));

                var postFWDImgaesDto = ObjectMapper.Mapper.Map<PostsFWDContentImagesDto>(content.Post);

                var des = new List<ContentSelectDto>();

                TreeViews.CreateTreeViewContentSeleteItems(postFWDImgaesDto.Contents.Where(x => x.ParentContentId == null), des, 0);

                ViewData["TreeViewContentSelete"] = des;

                return View(contentForUpdateDto);
            }

          

            var contentDto = await _serviceManager.ContentService.UpdateAsync(contentid, contentForUpdateDto);

            StatusMessage = $"Cập nhật thành công";

    

            return RedirectToAction("EditContent", new { id = id, contentid = contentid });


        }

        [HttpPost]
        public async Task<IActionResult> DeleteContent([FromRoute]string id , [FromQuery]int contentid)        
        {


            var contentDto = await _serviceManager.ContentService.DeleteAsync(contentid);

            StatusMessage = $"Xóa thành công nội dung ---{contentDto.Title}---";
            return RedirectToAction("detail", new { id = id  });
        }

        [HttpGet]
        public async Task<IActionResult> Images([FromRoute] string id)
        {
            var post = await RunPostDetail(id);

            var rs = await _authorizationService.AuthorizeAsync(User, post,
                                                           new CanOptionPostUserRequirements());

            if (!rs.Succeeded)
            {
                return Forbid();

            }

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

            var rs = await _authorizationService.AuthorizeAsync(User, post,
                                                            new CanOptionPostUserRequirements());

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
