using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.Role
{

    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, ILogger<RolePageModel> logger, AppDbcontext context) : base(roleManager, logger, context)
        {
        }

        public class InputModel
        {
            [Display(Name ="Tên")]
            [Required(ErrorMessage ="{0} không được bỏ trống.")]
            [StringLength(100, MinimumLength = 4 , ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.")]
            public string Name { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IActionResult OnGet()
        {
            Input = new InputModel();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var role = new IdentityRole(Input.Name);

                var result = await _roleManager.CreateAsync(role);

                if (result.Succeeded)
                {
                    StatusMessage = $"Thêm vai trò {role.Name} thành công.";
                    return RedirectToPage("./Index");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

            }
            return Page(); 
        }
    }
}
