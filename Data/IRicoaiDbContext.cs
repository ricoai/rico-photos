using Microsoft.EntityFrameworkCore;
using ricoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ricoai.Data
{
    public interface IRicoaiDbContext: IDisposable
    {
        DbSet<UserImage> UserImages { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync();

    }
}
