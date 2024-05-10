using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watsons.Common.TrafficLogHelpers;

namespace Watsons.Common.EmailHelpers.Entities
{
    public partial class EmailContext : DbContext
    {
        public EmailContext()
        {
        }

        public EmailContext(DbContextOptions<EmailContext> options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // connect with connection string
                //optionsBuilder.UseSqlServer("Data Source=10.98.32.248;Initial Catalog=TRV2_UAT;User ID=sa;Password=!QAZ2wsx#EDC;Trust Server Certificate=True"); // add connection string
                //var connectionString = SysCredential.GetConnectionString("Server185", "eStore");
                //optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
