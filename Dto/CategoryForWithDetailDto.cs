using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace Dto
{
    public class CategoryForWithDetailDto
    {
        public string Id { get; set; }
      
        public string Title { set; get; }
       
        public string Description { set; get; }

        public string Slug { set; get; }

        public string Content { set; get; }
    
        public int Serial { get; set; }

        public string ParentCategoryId { get; set; }
 
        public virtual CategoryForWithDetailDto ParentCategory { set; get; }

        public string IConFont { get; set; }

        public virtual ICollection<CategoryForWithDetailDto> CategoryChildren { get; set; }

        public virtual ICollection<PostCategoryForWithDetailDto> PostCategories { get; set; }
    }
}
