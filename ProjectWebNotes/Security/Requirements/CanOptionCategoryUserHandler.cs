using Domain.IdentityModel;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebNotes.Security.Requirements
{
    public class CanOptionCategoryUserHandler : AuthorizationHandler<CanOptionCategoryUserRequirements, string>
    {
        //private readonly UserManager<AppUser> _userManager;

        public CanOptionCategoryUserHandler()
        {

        }
        //public CanOptionCategoryUserHandler(UserManager<AppUser> userManager)
        //{
        //    _userManager = userManager;
        //}
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanOptionCategoryUserRequirements requirement, string categroyId)
        {
            //var user = _userManager.GetUserAsync(context.User).Result;

            if (context.User.IsInRole("Admin") || context.User.IsInRole("Administrator"))
            {
                context.Succeed(requirement);
                
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }
       
    }
}
