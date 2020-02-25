using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySqlLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySqlLogin.Data
{
    public class DataContext : IdentityDbContext<Entidad>
    {
        public DbSet<Entidad> EntidadesNetCore { get; set; }

        #region Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        #endregion

    }
}
