using Dto;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProjectWebNotes.Views.Shared.Components.BoxListCategory
{
    public class BoxListCategory : ViewComponent
    {

        public const string COMPONENTNAME = "BoxListCategory";

        public IViewComponentResult Invoke(IEnumerable<Category> categorys)
        {

            return View(categorys);

        }
    }
}
