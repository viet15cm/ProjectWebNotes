using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class PostsFWDContentImagesDto : PostsFWDImagesDto
    {
        public ICollection<ContentFWDChildsDto> Contents { get; set; }


    }
}
