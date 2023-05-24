using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProjectWebNotes.Areas.Identity.Pages.Account.Manager
{
    public class SuccessConfirmEmailModel : PageModel
    {

        [TempData]
        public string StatusMessage { get; set; }

        public void OnGet()
        {
        }
    }
}
