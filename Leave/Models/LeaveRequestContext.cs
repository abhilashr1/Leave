using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Leave.Models
{
    public class LeaveRequestContext : DbContext
    {
        public DbSet<LeaveRequest> LeaveRequest { get; set; }
        public IEnumerable<object> AdminModel { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Leaves.db");
        }
    }

}
