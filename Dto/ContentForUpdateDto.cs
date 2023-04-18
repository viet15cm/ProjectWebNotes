using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dto
{
    public class ContentForUpdateDto
    {

        [Display(Name = "Khóa")]
        [StringLength(20, MinimumLength = 4, ErrorMessage = "Độ dài {0} từ {2} đến {1} kí tự ")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "kí tự là chữ thường không dấu cách số từ 0 tới 9 bao gồm dấu - ")]
        public string KeyTitleId { get; set; }

        [Display(Name = "Tiêu đề")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} dài {1} đến {2}")]
        public string Title { get; set; }

        [Display(Name = "Nội dung")]
        [DataType(DataType.Text)]
        public string TextContents { get; set; }

        [Display(Name = "Nội dung cha")]
        public int? ParentContentId { get; set; }

        [Display(Name = "Bài viết")]
        public string PostId { get; set; }
    }
}
