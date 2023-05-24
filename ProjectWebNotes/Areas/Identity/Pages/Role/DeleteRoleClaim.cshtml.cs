using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.Role
{
    public class DeleteRoleClaimModel : RolePageModel
    {
        public DeleteRoleClaimModel(RoleManager<IdentityRole> roleManager, ILogger<RolePageModel> logger, AppDbcontext context) : base(roleManager, logger, context)
        {
        }

        [BindProperty]
        public InputModel input { get; set; }

        public IdentityRole role {get; set;}
        
        public class InputModel
        {
            public string Name { get; set; }

            public string Value { get; set; }
        }
        public async Task<IActionResult> OnGet(int id)
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

            input = new InputModel()
            {
                Name = claim.ClaimType,
                Value = claim.ClaimValue
            };

            return Page();

        }

        public async Task<IActionResult> OnPost(int id)
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

            var deleteClaim = new Claim(claim.ClaimType, claim.ClaimValue);


            var result = await _roleManager.RemoveClaimAsync(role, deleteClaim);

            if (result.Succeeded)
            {
                StatusMessage = $"Xáo claim ({claim.ClaimType}) thành công";
                return RedirectToPage("./Edit" ,new { id = role.Id});
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            return Page();

        }
    }
}
