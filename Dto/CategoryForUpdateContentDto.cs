using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dto
{
    public class CategoryForUpdateContentDto
    {

        [Display(Name = ("Nội dung"))]
        public string Content { set; get; }
    }
}
