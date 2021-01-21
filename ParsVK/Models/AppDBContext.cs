using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParsVK.Models
{
    public class AppDBContext: DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> opt):base(opt)
        {
            Database.EnsureCreated();
        }

        public DbSet<Profile> Profiles { get; set; }
    }
}
