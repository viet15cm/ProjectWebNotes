using System.ComponentModel.DataAnnotations;


namespace Dto
{
       public class PostDto
       {
        public string Id { set; get; }

        public string Title { set; get; }

        public string Description { set; get; }

        public string Slug { set; get; }

        public string Content { set; get; }

        public DateTime? DateUpdated { set; get; }


        [Display(Name = "Ngày tạo")]
        public DateTime? DateCreate { set; get; }

        public string PostParentId { get; set; }

        [Display(Name = "Nổi bậc")]
        public bool Prime { get; set; }

        public int Serial { get; set; }

        public string CategoryId { get; set; }

        [Display(Name = "Tác giả")]
        public string AuthorId { get; set; }

        public string Banner { get; set; }

    }
    

}
