﻿using System.ComponentModel.DataAnnotations;
namespace Dto
{
    public class PostForCreationDto
    {

        public PostForCreationDto()
        { 
           Id = Guid.NewGuid().ToString();
        }
            
        public string Id { set; get; }

        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [Display(Name = "Tiêu đề")]
        [StringLength(160, MinimumLength = 2, ErrorMessage = "{0} có độ dài từ {1} đến {2} kí tự.")]
        public string Title { set; get; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.Text)]
        [StringLength(1000, ErrorMessage = "{0} tối đa 1000 ký tự ")]
        public string Description { set; get; }

        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} có độ dài từ {1} đến {2} kí tự.")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "{0} chỉ gồm các kí tự thường, số, dấu (-) ")]
        public string Slug { set; get; }

        [Display(Name = ("Nội dung"))]
        public string Content { set; get; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? DateCreate { set; get; }
        public string PostParentId { get; set; }

        [Display(Name = "Nổi bậc")]
        public bool Prime { get; set; }

        [Display(Name = "Số thứ tự")]
        public int Serial { get; set; }

        [StringLength(50, ErrorMessage = "{0} có độ dài dưới 50 kí tự")]
        [Display(Name = "Icon")]
        public string IConFont { get; set; }

        public string CategoryId { get; set; }

        [Display(Name = "Tác giả")]
        public string AuthorId { get; set; }
    }
}
