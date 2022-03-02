using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Entities.Site;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MarketPlace.DataLayer.Context
{
    public class MarketPlaceDbContext : DbContext
    {
        public MarketPlaceDbContext(DbContextOptions<MarketPlaceDbContext> options)
            : base(options)
        {

        }
        #region Account

        public DbSet<User> Users { get; set; }

        #endregion

        #region Site

        public DbSet<SiteSetting> SiteSettings { get; set; }

        #endregion

        #region On Model Creating
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Cascade;
            }

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
