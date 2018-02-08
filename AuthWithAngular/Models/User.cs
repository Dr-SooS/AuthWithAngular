using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApp.Models
{
    public class User: IdentityUser
    {
		public bool Blocked { get; set; } = false;
    }
}
