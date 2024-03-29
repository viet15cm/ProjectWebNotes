﻿using ATMapper;
using AutoMapper.Internal.Mappers;
using Contracts;
using Dto;
using Entities.Models;
using Paging;
using Exceptions;
using Services.Abstractions;
using System.Linq;
using System.Reflection;

using System.Linq.Expressions;
using ExtentionLinqEntitys;
using Microsoft.EntityFrameworkCore;

namespace Services
{
    public class PostService : ServiceBase, IPostService
    {
      
        public PostService(IRepositoryWrapper repositoryManager) : base(repositoryManager)
        {
            
        }

        public async Task AddToCategorysAsync(string postId, IEnumerable<string> categoryId)
        {
            var datas = await _repositoryManager.PostCategory.GetByIdPostAsync(postId);

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
            //if (postForCreationDto.DateCreate == null)
            //{
            //    postForCreationDto.DateCreate = _httpClient.GetNistTime();
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
               .GetByIdAsync(postId,
                ExtendedQuery<Post>.Set(ExtendedInclue
                .Set<Post>(p => p.Include(p => p.PostCategories)
                .Include(p => p.PostChilds))));


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

        public IEnumerable<Post> GetAll(IExtendedQuery<Post> expLinqEntity = null)
        {
            return _repositoryManager.Post.GetAll(expLinqEntity);
        }

        public async Task<IEnumerable<Post>> GetAllAsync(IExtendedQuery<Post> expLinqEntity = null ,CancellationToken cancellationToken = default)
        {
            return await _repositoryManager.Post.GetAllAsync(expLinqEntity);

        }  

        public Post GetById(string postId, IExtendedQuery<Post> expLinqEntity = null)
        {
            var post =  _repositoryManager.Post.GetById(postId, expLinqEntity);

            return post;
        }

        public async Task<Post> GetByIdAsync(string postId , IExtendedQuery<Post> expLinqEntity, CancellationToken cancellationToken = default)
        {
           return await _repositoryManager.Post.GetByIdAsync(postId, expLinqEntity);
  
        }

        public async Task<Post> GetBySlugAsync(string slug, IExtendedQuery<Post> expLinqEntity = null, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager.Post.GetBySlugAsync(slug, expLinqEntity, cancellationToken);

            if (post is null)
            {
                throw new CategoryNotFoundException(post.Id);
            }

            return post;
        }

        public PagedList<PostDto> Posts(QueryStringParameters postParameters, IExtendedQuery<Post> expLinqEntity = default)
        {
            var posts = _repositoryManager.Post.Posts(postParameters, expLinqEntity);

            var listPostDto = ObjectMapper.Mapper.Map<List<PostDto>>(posts.ToList());

            var postDtos = new PagedList<PostDto>(listPostDto, posts.TotalCount, postParameters.PageNumber, postParameters.PageSize);

            return postDtos;


        }

        public async Task<PagedList<Post>> PostPaging(QueryStringParameters postParameters, IExtendedQuery<Post> expLinqEntity = default)
        {
            
            Task<PagedList<Post>> task = new Task<PagedList<Post>>(() => {

                var posts = _repositoryManager.Post.Posts(postParameters, expLinqEntity);

                return posts;
            });

            task.Start();
            return await task;

        }

        public async Task RemoveFromCategorysAsync(string idPost, IEnumerable<string> idCategory)
        {
                var datas = await _repositoryManager.PostCategory.GetByIdPostAsync(idPost);
                
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
                .GetByIdAsync(postId, 
                 ExtendedQuery<Post>.Set(ExtendedInclue.
                 Set<Post>(p => p.Include(p => p.PostCategories).Include(p => p.PostChilds))));


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
            var post = await _repositoryManager
                .Post
                .GetByIdAsync(postId,null, cancellationToken);

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

        public async Task<PostDto> UpDateAsync(string postId, PostForUpdateIdCategoryDto postForUpdate, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager
              .Post
              .GetByIdAsync(postId, null, cancellationToken);

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

        public async Task<PostDto> UpdateAsync(string postId, PostForUpdateBannerDto postForUpdate, CancellationToken cancellationToken = default)
        {
            var post = await _repositoryManager
               .Post
               .GetByIdAsync(postId, null, cancellationToken);

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
