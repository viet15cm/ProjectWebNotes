using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProjectWebNotes.Areas.Docs.Views.Shared.Components.CategorySidebar
{
    public class CategorySidebar : ViewComponent
    {
        public class CategorySidebarData
        {
            public string slugCategory { set; get; }

            public string slugPost { get; set; }

            public List<string> listSerialUrl { get; set; }

            public Category currentCategory { get; set; }

            public List<Post> listPostInCategory { get; set; }

        }

        public const string COMPONENTNAME = "CategorySidebar";
        public CategorySidebar() { }
        public IViewComponentResult Invoke(CategorySidebarData data)
        {
            return View(data);
        }

    }
}