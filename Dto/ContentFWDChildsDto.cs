using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class ContentFWDChildsDto : ContentDto
    {
        public virtual ICollection<ContentFWDChildsDto> ContentChildrens { get; set; }
    }
}
