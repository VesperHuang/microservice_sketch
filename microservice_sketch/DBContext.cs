using microservice_sketch.Models;
using Microsoft.EntityFrameworkCore;

namespace microservice_sketch
{
    public class DBContext:DbContext
    {
        public DBContext() { }

        public DBContext(DbContextOptions<DBContext> options) : base(options) { }

        public DbSet<user> user { get; set; }

        public DbSet<user_role> user_role { get; set; }

        public DbSet<syslogs> syslogs{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           // modelBuilder.Entity<user>().Property(b => b.Url).HasColumnName("UrlAddress");
        }
    }
}
