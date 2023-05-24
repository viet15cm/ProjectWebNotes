using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.Role
{
    public class AddRoleClaimModel : RolePageModel
    {
        public AddRoleClaimModel(RoleManager<IdentityRole> roleManager, ILogger<RolePageModel> logger, AppDbcontext context) : base(roleManager, logger, context)
        {
        }
        

        public IdentityRole role { set; get; }
        public class InputModel
        {
            [Display(Name = "Tên")]
            [Required(ErrorMessage = "{0} không được bỏ trống.")]
            [StringLength(100, MinimumLength = 4, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.")]
            public string Name { get; set; }

            [Display(Name = "Giá trị")]
            [Required(ErrorMessage = "{0} không được bỏ trống.")]
            [StringLength(100, MinimumLength = 4, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.")]
            public string Value { get; set; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null )
            {
                return NotFound();
            }
            role = await _roleManager.FindByIdAsync(id);
            
            if (role == null)
            {
                return NotFound();
            }
            Input = new InputModel();

            return Page();
        }

        public async Task<IActionResult> OnPost(string id)
        {
            if (ModelState.IsValid)
            {
                role = await _roleManager.FindByIdAsync(id);

                if (role == null)
                {
                    return NotFound();
                }

                var isCheckClaim =  (await _roleManager.GetClaimsAsync(role)).Any(c => c.Value == Input.Value && c.Type == Input.Name);

                if (isCheckClaim)
                {
                    ModelState.AddModelError(string.Empty, "Claim đã tồn tại");
                    return Page();
                }

                var claim = new Claim(Input.Name, Input.Value);

                var result = await _roleManager.AddClaimAsync(role, claim);

                if (result.Succeeded)
                {
                    StatusMessage = $"Thêm claim ({claim.Type}) thành công.";

                    return RedirectToPage("./Edit", new { id = role.Id });
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
