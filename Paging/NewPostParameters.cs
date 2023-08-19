using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paging
{
    public class NewPostParameters : QueryStringParameters
    {
        public override int PageSize { get; set; } = 7;
    }
}
