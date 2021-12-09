using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPITemplate.Configuration;
using WebAPITemplate.Models;

namespace WebAPITemplate.Entities
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(Config.GetConnectionString("AuthDBConnection"));
        }
        public override DbSet<IdentityUser> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
    }
}
