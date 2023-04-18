using ATMapper;
using AutoMapper.Internal.Mappers;
using Contracts;
using Dto;
using Entities.Models;
using Paging;
using Exceptions;
using Services.Abstractions;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Linq.Expressions;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class PostService : ServiceBase , IPostService
    {
        public PostService(IRepositoryWrapper repositoryManager) : base(repositoryManager)
        {
        }

        public async Task AddToCategorysAsync(string postId, IEnumerable<string> categoryId)
        {
            var datas = await _repositoryManager.PostCategory.GetByIdPostWithDetailAsync(postId);

                foreach (var item in categoryId)
                {
                    if (datas.Any(x => x.CategoryID == item))
                    {
                        continue;
                    }
                    var input = new PostCategory() { CategoryID = item, PostID = postId };

                    _repositoryManager.PostCategory.Insert(input);
                }

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

        }

        public async Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto, CancellationToken cancellationToken = default)
        {
            if (postForCreationDto is null)
            {
                throw new PostNotFoundException("Dữ liệu trống.");
            }
            if (postForCreationDto.Id is null)
            {
                postForCreationDto.Id = Guid.NewGuid().ToString();
            }
            //if (postForCreationDto.DateUpdated == null)
            //{
            //    postForCreationDto.DateUpdated = _httpClient.GetNistTime();
            //}

            var post = ObjectMapper.Mapper.Map<Post>(postForCreationDto);

            _repositoryManager.Post.Create(post);

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return ObjectMapper.Mapper.Map<PostDto>(post);
        }

        public async Task<PostDto> CreateAsync(PostForCreationDto postForCreationDto, string categoryId, CancellationToken cancellationToken = default)
        {

            if (categoryId is null) {

                throw new PostNotFoundException("Dữ liệu Id trống");
            }
            if (postForCreationDto is null)
            {
                throw new PostNotFoundException("Dữ liệu trống.");
            }
            if (postForCreationDto.Id is null)
            {
                postForCreationDto.Id = Guid.NewGuid().ToString();
            }
            //if (postForCreationDto.DateUpdated == null)
            //{
            //    postForCreationDto.DateUpdated = _httpClient.GetNistTime();
            //}


            var post = ObjectMapper.Mapper.Map<Post>(postForCreationDto);

            _repositoryManager.Post.Create(post);


            _repositoryManager.PostCategory.Insert(new PostCategory()
            {
                CategoryID = categoryId,
                PostID = post.Id
            });

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return ObjectMapper.Mapper.Map<PostDto>(post);
        }

        public async Task<PostDto> DeleteAsync(string postId, CancellationToken cancellationToken = default)
        {
            if (postId is null)
            {
                throw new PostNotFoundException("Không tìm thấy dữ liệu nạp Id.");
            }

            var post = await _repositoryManager
               .Post
               .GetByIdWithDetailAsync(postId,
                ExpLinqEntity<Post>.ResLinqEntity(ExpExpressions.ExtendInclude<Post>(p => p.Include(p => p.PostCategories).Include(p => p.PostChilds))));


            if (post.PostChilds?.Count> 0)
            {
                foreach (var item in post.PostChilds)
                {
                    item.PostParentId = post.PostParentId;
                }
            }

            if (post.PostCategories?.Count >0)
            {
                foreach (var item in post.PostCategories)
                {
                    _repositoryManager.PostCategory.Remove(item);
                }
            }

            _repositoryManager.Post.Delete(post);

            await _repositoryManager.UnitOfWork.SaveChangesAsync();

            return ObjectMapper.Mapper.Map<PostDto>(post);
        }

        

        public async Task<IEnumerable<PostDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.Post.GetAllAsync();

            return ObjectMapper.Mapper.Map<IEnumerable<PostDto>>(post);
        }



        public async Task<IEnumerable<Post>> GetAllWithDetailAsync(IExpLinqEntity<Post> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            var dataPosts = await _repositoryManager.Post.GetAllWithDetailAsync();


            return dataPosts;
        }

        public async Task<PostDto> GetByIdAsync(string postId, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.Post.GetByIdAsync(postId);

            return ObjectMapper.Mapper.Map<PostDto>(post);
        }


        public async Task<Post> GetByIdWithDetailAsync(string postId, IExpLinqEntity<Post> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.Post.GetByIdWithDetailAsync(postId , expLinqEntity);

            return post;
        }

        public PagedList<PostDto> Posts(PostParameters postParameters)
        {
            var posts = _repositoryManager.Post.Posts(postParameters);

            var listPostDto = ObjectMapper.Mapper.Map<List<PostDto>>(posts.ToList());

            var postDtos = new PagedList<PostDto>(listPostDto, posts.TotalCount, postParameters.PageNumber, postParameters.PageSize);

            return postDtos;


        }

        public async Task RemoveFromCategorysAsync(string idPost, IEnumerable<string> idCategory)
        {
           
                var datas = await _repositoryManager.PostCategory.GetByIdPostWithDetailAsync(idPost);
                
                foreach (var pc in datas)
                {
                    var isFind = idCategory.Any(x => x == pc.CategoryID);
                    if (isFind)
                    {
                        _repositoryManager.PostCategory.Remove(pc);
                    }
                }

                await _repositoryManager.UnitOfWork.SaveChangesAsync();

        }

        public async Task<PostDto> UpdateAsync(string postId, PostForUpdateDto postForUpdateDto, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager
                .Post
                .GetByIdWithDetailAsync(postId, 
                ExpLinqEntity<Post>.ResLinqEntity(ExpExpressions.ExtendInclude<Post>(p => p.Include(p => p.PostCategories).Include(p => p.PostChilds))));


            if (post == null)
            {
                throw new PostNotFoundException(post.Id);
            }

            if (postForUpdateDto == null)
            {
                throw new PostNotFoundException("Không tìm thấy dữ liệu đầu vào .");
            }

            if (postForUpdateDto.PostParentId != null)
            {
                if (post.Id == postForUpdateDto.PostParentId)
                {
                    throw new CategoryIdDuplicatePatentIdException("Không được lấy Id mục cha trùng lập với Id mục con");
                }
            }

            if (post.PostChilds?.Count > 0)
            {
                if (post.PostParentId != postForUpdateDto.PostParentId)
                {
                    foreach (var item in post.PostChilds)
                    {
                        item.PostParentId = post.PostParentId;
                    }
                }
            }

            Type TypePostForupdateDto = postForUpdateDto.GetType();

            Type TypePost = post.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(TypePostForupdateDto.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                PropertyInfo propertyInfo = TypePost.GetProperty(prop.Name);
                propertyInfo.SetValue(post, prop.GetValue(postForUpdateDto, null));

            }

            _repositoryManager.Post.Update(post);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<PostDto>(post);
        }

        public async Task<PostDto> UpdateAsync(string postId, PostForUpdateContentDto postForUpdate, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.Post.GetByIdWithDetailAsync(postId);

            if (post == null)
            {
                throw new PostNotFoundException(post.Id);
            }

            if (postForUpdate == null)
            {
                throw new PostNotFoundException("Không tìm thấy dữ liệu đầu vào .");
            }

            Type TypePostForupdateDto = postForUpdate.GetType();

            Type TypePost = post.GetType();

            IList<PropertyInfo> props = new List<PropertyInfo>(TypePostForupdateDto.GetProperties());

            foreach (PropertyInfo prop in props)
            {
                PropertyInfo propertyInfo = TypePost.GetProperty(prop.Name);
                propertyInfo.SetValue(post, prop.GetValue(postForUpdate, null));

            }

            _repositoryManager.Post.Update(post);

            await _repositoryManager.UnitOfWork.SaveChangesAsync(cancellationToken);

            return ObjectMapper.Mapper.Map<PostDto>(post);
        }
    }
}
