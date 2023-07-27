using Dto;
using Entities.Models;
using Microsoft.AspNetCore.Mvc;

namespace ProjectWebNotes.Areas.Manager.Views.Shared.Components.ImgManager
{
    public class ImgManager : ViewComponent
    {

        public const string COMPONENTNAME = "ImgManager";

        public IViewComponentResult Invoke(ICollection<ImageDto> Images)
        {

            return View(Images);

        }
    }
}
