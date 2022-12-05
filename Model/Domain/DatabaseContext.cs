using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace YtmoviesApi.Model.Domain
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        public DbSet<TokenInfo> TokenInfos { get; set; }

        //public DbSet<TokenInfo> TokenInfos { get; set; }


        //public DbSet<TokenInfo> TokenInfos { get; set; }



        //public DbSet<TokenInfo> TokenInfos { get; set; }
    }



}

