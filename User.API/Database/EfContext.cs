using Microsoft.EntityFrameworkCore;
using User.API.Database.Entities;

namespace User.API.Database
{
    public class EfContext : DbContext
    {

        public EfContext(DbContextOptions<EfContext> options) : base(options) { }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Data Source=.;Initial Catalog=Microservice;Integrated Security=False;Persist Security Info=False;User ID=sa;Password=Shenyu19901025");
        //}
    }
}
