using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthWithAngular.Models
{
    public class UserFrontDto
    {
		public string Email { get; set; }
		public string Id { get; set; }
		public bool Blocked { get; set; }
		public string Role { get; set; }
    }
}
