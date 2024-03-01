using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PetCenter9.Models;

namespace PetCenter9.Data
{
    public class PetCenter9Context : IdentityDbContext
    {
        public PetCenter9Context (DbContextOptions<PetCenter9Context> options)
            : base(options)
        {

        }
        public DbSet<PetCenter9.Models.Owners>? Owners { get; set; }
        public DbSet<PetCenter9.Models.Pets>? Pets { get; set; }
        public DbSet<PetCenter9.Models.Vaccines>? Vaccines { get; set; }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }

    }
}
