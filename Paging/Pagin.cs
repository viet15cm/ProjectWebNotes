using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paging
{
    public class Pagin
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string UrlAction { get; set; }
        public bool IsPage { get; set; } = false;
    }
}
