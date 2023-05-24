using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProjectWebNotes.DbContextLayer;
using Domain.IdentityModel;
using Entities.Models;

namespace ProjectWebNotes.Security.Requirements
{
    public class CanOptionPostUserHandler : AuthorizationHandler<CanOptionPostUserRequirements, string>
    {
        
        private readonly UserManager<AppUser> _userManager;

        private readonly AppDbcontext _context;

        public CanOptionPostUserHandler(UserManager<AppUser> userManager, AppDbcontext appDbcontext)
        {         
            _userManager = userManager;
            _context = appDbcontext;
        }

        protected override async  Task HandleRequirementAsync(AuthorizationHandlerContext context, CanOptionPostUserRequirements requirement, string PostId)
        {
            var user = _userManager.GetUserAsync(context.User).Result;

            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == PostId);

            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                await Task.CompletedTask;
            }


            if (user.Id == post.AuthorId)
            {
                if (requirement.IsSharedPostUser)
                {
                    var postcategoy = await _context.PostCategories.Where(x => x.PostID == PostId).FirstOrDefaultAsync();

                    if (postcategoy == null)
                    {
                        context.Succeed(requirement);
                        await Task.CompletedTask;
                    }
                }
                else
                {
                    context.Succeed(requirement);
                    await Task.CompletedTask;
                }
             
            }



            await Task.CompletedTask;
        }
    }
}
