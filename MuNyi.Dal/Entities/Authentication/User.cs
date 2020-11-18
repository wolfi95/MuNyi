using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MuNyi.Dal.Entities.Authentication
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
    }
}
