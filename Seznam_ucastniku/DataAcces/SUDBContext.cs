using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Seznam_ucastniku.Entities;

namespace Seznam_ucastniku.DataAcces
{
    public class SUDBContext : DbContext
    {
        public DbSet<Record> Records { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SU_EfCore;Encrypt=False;");
        }
    }

}
