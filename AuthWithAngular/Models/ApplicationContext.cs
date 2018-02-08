using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApp.Models
{
    public class ApplicationContext: IdentityDbContext<User>
    {
		public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options) {}

		public DbSet<User> AuthAppUsers { get; set; }
	}
}
