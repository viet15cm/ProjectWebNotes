using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebNotes.Security.Requirements
{
    public class NewUpdatePostHandler : AuthorizationHandler<AppAuthorizationRequirement, Post>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AppAuthorizationRequirement requirement, Post resource)
        {
            if (IsNewDateTimePost(resource))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private bool IsNewDateTimePost(Post post)
        {
            var createDateTimePost = post.DateCreate;

            if (createDateTimePost == null)
            {
                return false;
            }

            DateTime dateCreate = Convert.ToDateTime(createDateTimePost);
            DateTime Now = Convert.ToDateTime(DateTime.Now);

            TimeSpan Day = Now - dateCreate;
            int sumDay = Day.Days;

            return sumDay <= 1;
        }
    }
}
