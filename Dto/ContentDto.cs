using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dto
{
    public class ContentDto
    {
     
        public int Id { get; set; }

        [Display(Name = "Khóa")]
        public string KeyTitleId { get; set; }

        [Display(Name = "Tiêu đề")]
        public string Title { get; set; }

        [Display(Name = "Nội dung")]
        public string TextContents { get; set; }

        [Display(Name = "Nội dung cha")]
        public int? ParentContentId { get; set; }



    }
}
