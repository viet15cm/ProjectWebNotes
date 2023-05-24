using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.Role
{
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, ILogger<RolePageModel> logger, AppDbcontext context) : base(roleManager, logger, context)
        {
        }


        public IdentityRole role { set; get; }

        public IdentityRoleClaim<string> Claim {get; set;}
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

        public async Task<IActionResult> OnGet(int id)
        {

            var claim = await _context.RoleClaims.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (claim == null)
            {
                return NotFound();

            }
            role = await _roleManager.FindByIdAsync(claim.RoleId);

            if (role == null)
            {
                return NotFound();
            }
            Input = new InputModel()
            {
                Name = claim.ClaimType,
                Value = claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPost(int id)
        {
            if (ModelState.IsValid)
            {
                var claim = await _context.RoleClaims.FindAsync(id);

                if (claim == null)
                {
                    return NotFound();
                }

                role = await _roleManager.FindByIdAsync(claim.RoleId);

                if (role == null)
                {
                    return NotFound();
                }

                if (claim.ClaimType == Input.Name && claim.ClaimValue == Input.Value)
                {
                    return Page();
                }

                var isCheckClaim = (await _roleManager.GetClaimsAsync(role)).Any(c => c.Value == Input.Value && c.Type == Input.Name);

                if (isCheckClaim)
                {
                    ModelState.AddModelError(string.Empty, "Claim đã tồn tại");

                    return Page();
                }

                claim.ClaimType = Input.Name;
                claim.ClaimValue = Input.Value;

                var result = await _context.SaveChangesAsync();

               
               StatusMessage = $"Cập nhât claim ({claim.ClaimType}) thành công.";

               return RedirectToPage("./Edit", new { id = role.Id });
                
            }

            ModelState.AddModelError(string.Empty, "Loi gi do");

            return Page();
        }
    }
}
