using System;
using System.Reflection.Metadata.Ecma335;

namespace ProjectWebNotes.Models
{
    public class Paging
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string UrlAction { get; set; }

        public bool IsPage { get; set; } = false;
    }

}
