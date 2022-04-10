using MarketPlace.DataLayer.Entities.Account;
using MarketPlace.DataLayer.Entities.Contacts;
using MarketPlace.DataLayer.Entities.Site;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using MarketPlace.DataLayer.Entities.Products;
using MarketPlace.DataLayer.Entities.Store;

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
        public DbSet<Slider> Sliders { get; set; }

        #endregion

        #region Site

        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<SiteBanner> SiteBanners { get; set; }

        #endregion

        #region Contacts

        public DbSet<ContactUs> ContactUses { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<TicketMessage> TicketMessages { get; set; }

        #endregion

        #region Store

        public DbSet<Seller> Sellers { get; set; }

        #endregion

        #region Products

        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSelectedCategory> ProductSelectedCategories { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductGallery> ProductGalleries { get; set; }

        #endregion

        #region On Model Creating

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}
