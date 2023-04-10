

namespace Dto
{
    public class PostForWithDetailDto
    {
      
        public string Id { set; get; }

        public string Title { set; get; }

        public string Description { set; get; }
   
        public string Slug { set; get; }

        public string Content { set; get; }

        public DateTime? DateUpdated { set; get; }

        public string PostParentId { get; set; }
  
        public bool Prime { get; set; }

        public virtual ICollection<PostForWithDetailDto> PostChilds { get; set; }
     
        public virtual PostForWithDetailDto PostParent { get; set; }

        public virtual ICollection<PostCategoryForWithDetailDto> PostCategories { get; set; }

        public int Serial { get; set; }
    }
}
