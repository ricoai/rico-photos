using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ricoai.Models;

namespace ricoai.Data
{
    public class RicoaiDbContext : DbContext
    {
        public RicoaiDbContext():base()
        {

        }

        public RicoaiDbContext (DbContextOptions<RicoaiDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserImage> UserImage { get; set; }

        //public async Task<int> SaveChangesAsync()
        //{
        //    return await base.SaveChangesAsync();
        //}

    }
}
