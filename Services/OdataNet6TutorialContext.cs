using Microsoft.EntityFrameworkCore;
using OData_webapi_netcore6.Models;

namespace OData_webapi_netcore6.Services
{
    public class OdataNet6TutorialContext : DbContext
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        protected readonly IConfiguration configuration;

        public OdataNet6TutorialContext(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Authors> Authors { get; set; }
        public DbSet<Books> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(this.configuration.GetConnectionString("SQLiteConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Books>().OwnsOne(c => c.Authors);
        }
    }
}
