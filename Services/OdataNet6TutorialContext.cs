using Microsoft.EntityFrameworkCore;
using OData_webapi_netcore6.Models;
using System.Security.Claims;

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

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            //new AuditHelper(this, this.httpContextAccessor?.HttpContext?.User).AddAuditLogs();
            this.AddTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            //new AuditHelper(this, this.httpContextAccessor?.HttpContext?.User).AddAuditLogs();
            this.AddTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            this.AddTimestamps();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }


        private void AddTimestamps()
        {
            // We can manage the entries by what states they are in: https://www.entityframeworktutorial.net/efcore/changetracker-in-ef-core.aspx
            var entities = this.ChangeTracker.Entries().Where(x => x.Entity is CoreEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            var currentAuth0ID = this.httpContextAccessor?.HttpContext?.User.Identities.FirstOrDefault().Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            // TODO - SET SECURITY
            //var currentAuth0ID = "test";


            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((CoreEntity)entity.Entity).CreatedBy = currentAuth0ID;
                    ((CoreEntity)entity.Entity).CreatedDate = DateTime.UtcNow;
                }

                ((CoreEntity)entity.Entity).ModifiedBy = currentAuth0ID;
                ((CoreEntity)entity.Entity).ModifiedDate = DateTime.UtcNow;
            }
        }
    }
}
