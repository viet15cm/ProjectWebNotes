using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{
    public class Category
    {
        public Category()
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
        [StringLength(200, ErrorMessage = "{0} tối đa 200 ký tự ")]
        public string Description { set; get; }

        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} có độ dài từ {1} đến {2} kí tự.")]
        [RegularExpression(@"^[a-z0-9-]*$")]
       
        public string Slug { set; get; }

        [Display(Name = ("Nội dung"))]
        public string Content { set; get; }


        [Display(Name = "Thứ tự")]
        public int Serial { get; set; }
        public virtual ICollection<Category> CategoryChildren { get; set; }

        public string ParentCategoryId { get; set; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Danh mục cha")]
        public virtual Category ParentCategory { set; get; }

        [StringLength(50, ErrorMessage = "{0} có độ dài dưới 50 kí tự")]
        [Display(Name = "Icon")]
        public string IConFont { get; set; }

        public virtual ICollection<PostCategory> PostCategories { get; set; }

    }
}
