using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dto
{
    public class PostForUpdateContentDto
    {

        [Display(Name = ("Nội dung"))]
        public string Content { set; get; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime? DateUpdated { set; get; }
    }
}
