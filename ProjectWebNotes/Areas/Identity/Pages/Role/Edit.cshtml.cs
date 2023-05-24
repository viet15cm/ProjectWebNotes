using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.Role
{

    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, ILogger<RolePageModel> logger, AppDbcontext context) : base(roleManager, logger, context)
        {
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public List<IdentityRoleClaim<string>> Clamis { get; set; }
        public class InputModel
        {
          
            public string Id { get; set; }

            [Required(ErrorMessage ="{0} nhập tên.")]
            [Display(Name="Tên")]
            [StringLength(100, MinimumLength = 5 , ErrorMessage ="{0} có độ dài từ {2} đén {1}")]
            public string Name { get; set; }

            
        }

        public async Task<IActionResult> OnGet(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            Input = new InputModel()
            {
                Id = role.Id,
                Name = role.Name
            };

            Clamis = await _context.RoleClaims.Where(c => c.RoleId == role.Id).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(Input.Id);

                if (role == null)
                {
                    return NotFound();
                }

                role.Name = Input.Name;

                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    StatusMessage = $"Cập nhật vai trò {role.Name} thành công.";

                    return RedirectToPage("./index");
                }

                Clamis = await _context.RoleClaims.Where(c => c.RoleId == role.Id).ToListAsync();

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }

            return Page();
        }
    }
}


