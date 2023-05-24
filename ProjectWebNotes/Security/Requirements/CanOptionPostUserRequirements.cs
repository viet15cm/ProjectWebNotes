using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebNotes.Security.Requirements
{
    public class CanOptionPostUserRequirements : IAuthorizationRequirement
    {
     
        public bool IsSharedPostUser { get; set; }

        public CanOptionPostUserRequirements( bool _IsSharedPostUser = false)
        {
            IsSharedPostUser = _IsSharedPostUser;
        }
    }
}
