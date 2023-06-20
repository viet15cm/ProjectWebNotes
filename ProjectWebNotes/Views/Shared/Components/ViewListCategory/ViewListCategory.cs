using Entities.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ProjectWebNotes.Views.Shared.Components.ViewListCategory
{
    public class ViewListCategory : ViewComponent
    {


        public const string COMPONENTNAME = "ViewListCategory";

        public IViewComponentResult Invoke(List<Category> categorys)
        {
            return  View(categorys);
        }
    }
}
