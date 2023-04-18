using Dto;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ProjectWebNotes.Areas.Manager.Models
{
    public static class TreeViews
    {

        public static List<Post> GetPostChierarchicalTree(IEnumerable<Post> allCats, string parentId = null)
        {

            return allCats.Where(c => c.PostParentId == parentId)
                            .Select(c => new Post()
                            {
                                Id = c.Id,
                                Title = c.Title,
                                Serial = c.Serial,
                                Content = c.Content,
                                Prime = c.Prime,
                                Description = c.Description,
                                Slug = c.Slug,
                                PostParentId = c.PostParentId,
                                PostCategories = c.PostCategories,
                                PostChilds = GetPostChildren(allCats.ToList(), c.Id)
                            })
                            .ToList();
        }

        public static List<Post> GetPostChildren(IEnumerable<Post> cats, string parentId)
        {
            return cats.Where(c => c.PostParentId == parentId)
                    .Select(c => new Post
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Serial = c.Serial,
                        Content = c.Content,
                        Description = c.Description,
                        Slug = c.Slug,          
                        Prime = c.Prime,
                        PostParentId = c.PostParentId,
                        PostCategories = c.PostCategories,
                        PostChilds = GetPostChildren(cats, c.Id)
                    })
                    .ToList();
        }



        /// <summary>
        /// </summary>
        /// <param name="allCats"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>

        public static List<Category> GetCategoryChierarchicalTree(IEnumerable<Category> allCats, string parentId = null)
        {

            return allCats.Where(c => c.ParentCategoryId == parentId)
                            .Select(c => new Category()
                            {
                                Id = c.Id,
                                Title = c.Title,
                                Serial = c.Serial,
                                Content = c.Content,
                                IConFont = c.IConFont,
                                Description = c.Description,
                                Slug = c.Slug,
                                ParentCategoryId = c.ParentCategoryId,
                                PostCategories = c.PostCategories,
                                CategoryChildren = GetCateogryChildren(allCats.ToList(), c.Id)
                            })
                            .ToList();
        }

        public static List<Category> GetCateogryChildren(IEnumerable<Category> cats, string parentId)
        {
            return cats.Where(c => c.ParentCategoryId == parentId)
                    .Select(c => new Category
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Serial = c.Serial,
                        Content = c.Content,
                        IConFont = c.IConFont,
                        Description = c.Description,
                        Slug = c.Slug,
                        ParentCategoryId = c.ParentCategoryId,
                        PostCategories = c.PostCategories,
                        CategoryChildren = GetCateogryChildren(cats, c.Id)
                    })
                    .ToList();
        }

  
        public static void CreateTreeViewPostSeleteItems(List<Post> postTreeLayerDtos
                                             , List<PostSelectDto> des,
                                              int leve)
        {

            foreach (var post in postTreeLayerDtos)
            {
                string perfix = string.Concat(Enumerable.Repeat("-", leve));

                des.Add(new PostSelectDto()
                {
                    Id = post.Id,
                    Title = perfix + post.Title
                });


                if (post.PostChilds?.Count > 0)
                {

                    CreateTreeViewPostSeleteItems(post.PostChilds.ToList(), des, leve + 1);

                }
            }

        }


        public static void CreateTreeViewCategorySeleteItems(IEnumerable<Category> categoryTreeLayerDtos
                                              , List<Category> des,
                                               int leve)
        {

            foreach (var category in categoryTreeLayerDtos)
            {
                string perfix = string.Concat(Enumerable.Repeat("-", leve));

                des.Add(new Category()
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


        public static void CreateTreeViewContentSeleteItems(IEnumerable<ContentFWDChildsDto> contentFWDChilds
                                             , List<ContentSelectDto> des,
                                              int leve)
        {

            foreach (var content in contentFWDChilds)
            {
                string perfix = string.Concat(Enumerable.Repeat("-", leve));

                des.Add(new ContentSelectDto()
                {
                    Id = content.Id,
                    Title = perfix + content.Title
                });

                if (content.ContentChildrens?.Count > 0)
                {

                    CreateTreeViewContentSeleteItems(content.ContentChildrens, des, leve + 1);

                }
            }

        }


    }
}
