using Dto;
using Entities.Models;

namespace ProjectWebNotes.Areas.Manager.Models
{
    public class CategoryBranchPosts
    {

        public CategoryDto Category { get; set; }

        public List<PostFWDetailChilds> Posts { get; set; }


        public CategoryBranchPosts(CategoryDto category, List<PostFWDetailChilds> posts)
        {
            Category = category;
            Posts = posts;
        }
    }
}
