
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dto
{
    public class CategoryDto
    {
    
        public string Id { set; get; }
  
        public string Title { set; get; }

        public string Description { set; get; }

        public string Slug { set; get; }

        public string Content { set; get; }

        public int Serial { get; set; }
     
        public string ParentCategoryId { get; set; }       
  
        public string IConFont { get; set; }

    }
}
