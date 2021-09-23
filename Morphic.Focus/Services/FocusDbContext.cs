using Microsoft.EntityFrameworkCore;
using Morphic.Focus.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Services
{
    public class FocusDbContext : DbContext
    {
        public DbSet<GeneralSetting> GeneralSettings { get; set; }
        public DbSet<UnblockItem> UnblockItems { get; set; }
        public DbSet<BlockList> BlockLists { get; set; }
        public DbSet<BlockListAppWebsite> BlockListAppWebsites { get; set; }

        public FocusDbContext(DbContextOptions options) : base(options) { }
    }
}
