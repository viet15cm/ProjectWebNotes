using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Post
    {
        public Post()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { set; get; }

        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [Display(Name = "Tiêu đề")]
        [StringLength(160, MinimumLength = 2, ErrorMessage = "{0} có độ dài từ {1} đến {2} kí tự.")]
        public string Title { set; get; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.Text)]
        [StringLength(200, ErrorMessage ="{0} tối đa 200 ký tự ")]
        public string Description { set; get; }

        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} có độ dài từ {1} đến {2} kí tự.")]
        [RegularExpression(@"^[a-z0-9-]*$")]
        public string Slug { set; get; }


        [Display(Name = ("Nội dung"))]
        public string Content { set; get; }


        [Display(Name = "Ngày cập nhật")]
        public DateTime? DateUpdated { set; get; }

        public string PostParentId { get; set; }


        [Display(Name = "Nổi bậc")]
        public bool Prime { get; set; }

        public virtual ICollection<Post> PostChilds { get; set; }

        [ForeignKey("PostParentId")]
        public virtual Post PostParent { get; set; }

        public virtual ICollection<PostCategory> PostCategories { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        [Display(Name ="Nội dung cấp bậc")]
        public virtual ICollection<Content> Contents { get; set; }

        [Display(Name = "Số thứ tự")]
        public int Serial { get; set; }

    }
}
