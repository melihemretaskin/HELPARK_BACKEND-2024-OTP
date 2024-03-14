using Microsoft.EntityFrameworkCore;
using Modals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class HelparkDbContext : DbContext
    {
        public HelparkDbContext(DbContextOptions<HelparkDbContext> options)
           : base(options)
        {
        }

        public DbSet<User> User { get; set; }
    }
}
