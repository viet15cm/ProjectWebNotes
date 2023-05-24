using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paging
{
    public class UserParameters
    {
        const int maxPageSize = 50;
        public virtual int PageNumber { get; set; } = 1;
        private int _pageSize = 5;
        public virtual int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }

    }
}
