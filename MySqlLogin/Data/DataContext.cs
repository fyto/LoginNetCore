using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MySqlLogin.Models;

namespace MySqlLogin.Data
{
    public class DataContext : IdentityDbContext<Entidad>
    {
        public DbSet<Ciudad> CIUDAD { get; set; }
        public DbSet<EntidadArmadillo> ENTIDAD { get; set; }

        #region Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        #endregion

    }
}
