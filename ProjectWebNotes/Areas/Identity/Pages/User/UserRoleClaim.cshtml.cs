using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectWebNotes.DbContextLayer;

namespace ProjectWebNotes.Areas.Identity.Pages.User
{
    public class UserRoleClaimModel : UserPageModel
    {
        public UserRoleClaimModel(UserManager<AppUser> userManager, ILogger<UserPageModel> logger, AppDbcontext dbContext, RoleManager<IdentityRole> roleManager) : base(userManager, logger, dbContext, roleManager)
        {
            
        }

        [BindProperty]
        public InputModel input { get; set; }

        public IdentityUser<string> user { get; set; }

        [BindProperty]
        public string handler { get; set; }

        public class InputModel
        {
            [Display(Name = "Tên")]
            [Required(ErrorMessage = "{0} không được bỏ trống.")]
            [StringLength(100, MinimumLength = 4, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.")]
            public string TypeName { get; set; }

            [Display(Name = "Giá trị")]
            [Required(ErrorMessage = "{0} không được bỏ trống.")]
            [StringLength(100, MinimumLength = 4, ErrorMessage = "{0} phải dài từ {2} đến {1} kí tự.")]
            public string Value { get; set; }
        }

        public NotFoundObjectResult OnGet() => NotFound("Không được truy cập.");
        public async Task<IActionResult> OnGetAddUserClaim(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            handler = "AddUserClaim";
            return Page();
        }

        public async Task<IActionResult> OnPostAddUserClaim(string id)
        {
            handler = "AddUserClaim";

            if (id == null)
            {
                return NotFound();
            }

            user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
        
            if (ModelState.IsValid)
            {
                var claim = new Claim(input.TypeName, input.Value);

                var cliamUser = await _userManager.GetClaimsAsync((AppUser)user);

                var isDuplicate = cliamUser.Any(x => x.Type == claim.Type && x.Value == claim.Value);

                if (isDuplicate)
                {
                   
                    ModelState.AddModelError(string.Empty, $"Claim {claim.Type} đã tồn tại.");
                    return Page();
                }

                var result = await _userManager.AddClaimAsync((AppUser)user, claim);

                if (result.Succeeded)
                {
                    StatusMessage = $"Thêm thành công claim {claim.Type} cho User {user.UserName}";

                    return RedirectToPage("./AddRole" ,new { id = user.Id });
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }
            }
           
            return Page();
        }

        public async Task<IActionResult> OnGetEditUserClaim(int id)
        {
            handler = "EditUserClaim";

            var claim = await _dbContext.UserClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }
            user = await _userManager.FindByIdAsync(claim.UserId);

            if (user == null)
            {
                return NotFound();
            }

            input = new InputModel()
            {
                TypeName = claim.ClaimType,
                Value = claim.ClaimValue
            };

            return Page();
        }

        public async Task<IActionResult> OnPostEditUserClaim(int id)
        {
            handler = "EditUserClaim";

            if (ModelState.IsValid)
            {
                var claim = await _dbContext.UserClaims.FindAsync(id);

                if (claim == null)
                {
                    return NotFound();
                }
                user = await _userManager.FindByIdAsync(claim.UserId);

                if (user == null)
                {
                    return NotFound();
                }

                if (claim.ClaimType == input.TypeName && claim.ClaimValue == input.Value)
                {
                    return Page();
                }

                var cliamUser = await _userManager.GetClaimsAsync((AppUser)user);

                var isDuplicate = cliamUser.Any(x => x.Type == input.TypeName && x.Value == input.Value);

                if (isDuplicate)
                {

                    ModelState.AddModelError(string.Empty, $"Claim {input.TypeName} đã tồn tại.");
                    return Page();
                }

                claim.ClaimType = input.TypeName;
                claim.ClaimValue = input.Value;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    StatusMessage = $"Cập nhật claim {claim.ClaimType} thành công";
                    return RedirectToPage("./AddRole", new { id = user.Id });
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, $"Lỗi ngoại lệ liên hệ admin.");
                    return Page();
                }

            }
          
            return Page();
        }

        public async Task<IActionResult> OnGetDeleteUserClaim(int id)
        {
            handler = "DeleteUserClaim";

            var claim = await _dbContext.UserClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }
            user = await _userManager.FindByIdAsync(claim.UserId);

            if (user == null)
            {
                return NotFound();
            }

            input = new InputModel()
            {
                TypeName = claim.ClaimType,
                Value = claim.ClaimValue
            };
        
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteUserClaim(int id)
        {
            handler = "DeleteUserClaim";

            var claim = await _dbContext.UserClaims.FindAsync(id);

            if (claim == null)
            {
                return NotFound();
            }
            user = await _userManager.FindByIdAsync(claim.UserId);

            if (user == null)
            {
                return NotFound();
            }

            var deleteclaim = new Claim(claim.ClaimType, claim.ClaimValue);

            var result = await _userManager.RemoveClaimAsync((AppUser)user, deleteclaim);

            if (result.Succeeded)
            {
                StatusMessage = $"Xóa thành công";
                return RedirectToPage("./AddRole", new { id = user.Id });
            }

            foreach (var item in result.Errors)
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
           
            return Page();
        }
    }
}

