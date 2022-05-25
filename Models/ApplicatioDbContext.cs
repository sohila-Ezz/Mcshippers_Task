
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Mcshippers_Task.Models
{
  
        public class ApplicatioDbContext : IdentityDbContext<ApplicationUser>
        {
            public ApplicatioDbContext() { }
            public ApplicatioDbContext(DbContextOptions options) : base(options)
            {

            }
          
            public DbSet<Product> Products { get; set; }

        }
    }
