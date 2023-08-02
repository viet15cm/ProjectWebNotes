using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paging
{
    public class PostParameters : QueryStringParameters
    {
        public override int PageSize { get => base.PageSize; set => base.PageSize = value; }
    }
}
