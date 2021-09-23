using Microsoft.EntityFrameworkCore;
using Morphic.Focus.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Morphic.Focus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FocusDbContextFactory _contextFactory = new FocusDbContextFactory();
            using (FocusDbContext context = _contextFactory.CreateDbContext())
            {
                context.Database.Migrate();
            }
        }
    }
}
