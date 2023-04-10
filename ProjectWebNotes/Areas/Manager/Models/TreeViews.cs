using Dto;

namespace ProjectWebNotes.Areas.Manager.Models
{
    public static class TreeViews
    {

        public static List<PostForWithDetailDto> GetPostChierarchicalTree(IEnumerable<PostForWithDetailDto> allCats, string parentId = null)
        {

            return allCats.Where(c => c.PostParentId == parentId)
                            .Select(c => new PostForWithDetailDto()
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

        public static List<PostForWithDetailDto> GetPostChildren(IEnumerable<PostForWithDetailDto> cats, string parentId)
        {
            return cats.Where(c => c.PostParentId == parentId)
                    .Select(c => new PostForWithDetailDto
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

        public static List<CategoryForWithDetailDto> GetCategoryChierarchicalTree(IEnumerable<CategoryForWithDetailDto> allCats, string parentId = null)
        {

            return allCats.Where(c => c.ParentCategoryId == parentId)
                            .Select(c => new CategoryForWithDetailDto()
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

        public static List<CategoryForWithDetailDto> GetCateogryChildren(IEnumerable<CategoryForWithDetailDto> cats, string parentId)
        {
            return cats.Where(c => c.ParentCategoryId == parentId)
                    .Select(c => new CategoryForWithDetailDto
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




    }
}
