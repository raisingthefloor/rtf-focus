using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Services
{
    public class FocusDbContextFactory : IDesignTimeDbContextFactory<FocusDbContext>
    {
        public FocusDbContext CreateDbContext(string[] args = null)
        {
            var options = new DbContextOptionsBuilder<FocusDbContext>();
            
            options.UseSqlite("Data Source=winfocus.db");
            options.UseLazyLoadingProxies();
            return new FocusDbContext(options.Options);
        }
    }
}
