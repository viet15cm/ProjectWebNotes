using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.User
{

    public class AddRoleModel : UserPageModel
    {
        public AddRoleModel(UserManager<AppUser> userManager, ILogger<UserPageModel> logger, AppDbcontext dbContext, RoleManager<IdentityRole> roleManager) : base(userManager, logger, dbContext, roleManager)
        {
        }
        

        [BindProperty]
        [Display(Name = "Danh sách vai trò")]
        public string[] RoleUsers { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }


        public IdentityRole<string> roleUsers { get; set; }
        public List<IdentityRoleClaim<string>> roleCliamUser { get; set; }

        public List<IdentityUserClaim<string>> userClaims { get; set; }
        public SelectList addRoles { get; set; }
        public class InputModel
        {
            public string Id { get; set; }
        } 
        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            Input = new InputModel()
            {
                Id = id
            };

            RoleUsers =  (await _userManager.GetRolesAsync(user)).ToArray();

            var listRoles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();

            addRoles = new SelectList(listRoles);

            await GetCliam(id);

            return Page();

        }

        async Task GetCliam(string id)
        {
            var roleInUsers =  from r in _dbContext.Roles
                               join ru in _dbContext.UserRoles on r.Id equals ru.RoleId
                               where ru.UserId == id
                               select r;

            var roleInCliams =  from rc in _dbContext.RoleClaims
                                join rcu in roleInUsers on rc.RoleId equals rcu.Id
                                select rc;

            roleCliamUser = await roleInCliams.ToListAsync();

            var userInClaims = from c in _dbContext.UserClaims
                             where c.UserId == id
                             select c;

            userClaims = await userInClaims.ToListAsync();

        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.FindByIdAsync(Input.Id);

            if (user == null)
            {
                return NotFound();
            }

            var OldUserRolers = await _userManager.GetRolesAsync(user);

            var deleteRolers = OldUserRolers.Where(x => !RoleUsers.Contains(x));

            var addRolers = RoleUsers.Where(x => !OldUserRolers.Contains(x));

            var deleteResults = await _userManager.RemoveFromRolesAsync(user, deleteRolers);

            if (!deleteResults.Succeeded)
            {
                foreach (var item in deleteResults.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                var listRoles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
                addRoles = new SelectList(listRoles);
                
                return Page();
            }

            var addRolerResult = await _userManager.AddToRolesAsync(user, addRolers);

            if (!addRolerResult.Succeeded)
            {
                foreach (var item in addRolerResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                var listRoles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();
                addRoles = new SelectList(listRoles);

                return Page();
            }

            StatusMessage = $"Cập nhật vai trò cho người dùng {user.UserName} thành công";

            return RedirectToPage("./index");

        }
    }
}
