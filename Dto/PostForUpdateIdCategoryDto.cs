using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class PostForUpdateIdCategoryDto
    {

        [Display(Name = "Danh Mục")]
        public string CategoryId { get; set; }
    }
}
